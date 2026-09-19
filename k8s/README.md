# Kubernetes manifests for CI on the Talos cluster

Application-specific manifests for running this repo's build/test pipeline
on the homelab Talos cluster via ARC (`talos-runners`). Platform-level infra
(Harbor, ARC itself) is source-controlled separately in `ATap.Datacenter`.

## One-time setup (per cluster)

```powershell
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/rbac.yaml

# Harbor's CA, so Kaniko can verify the registry's TLS certificate when pushing.
kubectl -n api-automation create configmap harbor-ca `
  --from-file=ca.crt=D:\Homelab\credentials\talos-optiplex\harbor\ca.crt

# A Harbor robot account scoped to push/pull on the "learning" project
# (Harbor UI: Projects -> learning -> Robot Accounts -> New Robot Account).
kubectl -n api-automation create secret docker-registry harbor-push-creds `
  --docker-server=192.168.0.139:30003 `
  --docker-username='<robot account name>' `
  --docker-password='<robot account secret>'
```

## Build

```powershell
kubectl apply -f k8s/kaniko-build-job.yaml
kubectl wait --for=condition=complete job/kaniko-build -n api-automation --timeout=300s
kubectl logs job/kaniko-build -n api-automation
```

Pushes to `192.168.0.139:30003/learning/api-automation-framework:latest` — one
fixed tag, rebuilt and overwritten on every run.

## Application Under Test

```powershell
kubectl apply -f k8s/aut-deployment.yaml
kubectl apply -f k8s/aut-service.yaml
kubectl wait --for=condition=available deploy/qa-practice-api -n api-automation --timeout=180s
```

Deployed fresh per run, reachable in-cluster at `qa-practice-api:8081` (e.g. by
the test Job). Startup takes ~40-90s (Spring Boot + Hibernate), hence the
generous readiness probe tolerance and the 1Gi memory limit - 512Mi reliably
OOMKilled it during Hibernate init. Readiness checks `/swagger-ui.html`, not
`/swagger/index.html` (that's the .NET Swashbuckle convention; this is a
Java/springfox app - `/swagger/index.html` 404s here).

Tear down with `kubectl delete -f k8s/aut-deployment.yaml -f k8s/aut-service.yaml`
when done, rather than leaving it running permanently.

Note: `namespace.yaml` and `rbac.yaml` are one-time admin setup, not something
the pipeline itself re-applies - see above. If `rbac.yaml` changes (e.g. to add
permissions for the AUT's Deployment/Service), re-run its `kubectl apply` here
with an admin kubeconfig.

## Run tests

Requires `api-creds` (one-time admin setup, alongside the Secrets above):

```powershell
kubectl -n api-automation create secret generic api-creds `
  --from-literal=API_USERNAME='<value>' `
  --from-literal=API_PASSWORD='<value>'
```

```powershell
kubectl apply -f k8s/test-workspace-pvc.yaml
kubectl apply -f k8s/test-job.yaml
kubectl wait --for=condition=complete job/run-tests -n api-automation --timeout=15m
```

The test Job clones the repo fresh (an initContainer, into `test-workspace-pvc`)
and mounts it into the environment-only image built above - code changes never
need an image rebuild, only Dockerfile/`.csproj` changes do (see the
Dockerfile's own comment).

**`test-workspace-pvc.yaml` is applied (not deleted) every run, reused across
runs rather than recreated.** The initContainer wipes its contents before
cloning fresh, so this is safe. This isn't just an optimization: deleting the
PVC right after its Job's pod exits reliably raced Kubernetes' own
`pvc-protection` controller — it tries to patch the PVC's `Unused` condition at
almost the same moment, hits a resourceVersion conflict
(`the object has been modified; please apply your changes to the latest
version and try again`, visible in `kube-controller-manager`'s logs), and that
conflict isn't retried quickly - the finalizer only actually clears on the
controller's next periodic resync, which stalled cleanup for 4-5 minutes every
single run. Reusing the PVC sidesteps the deletion path (and its race)
entirely during normal operation.

**Retrieving the report**: a Job's pod is gone the instant its container exits,
so `kubectl cp`/`exec` have nothing left to talk to directly by the time the
Job shows complete. Mount the same PVC into a short-lived Pod instead:

```powershell
kubectl apply -f k8s/report-retrieval-pod.yaml
kubectl wait --for=condition=ready pod/report-retrieval -n api-automation --timeout=60s
kubectl cp api-automation/report-retrieval:/app/atp.ApiAutomation.Framework/bin/Debug/net8.0/TestReport ./TestReport-latest -c retrieval
kubectl delete -f k8s/report-retrieval-pod.yaml
```

(Deleting just the retrieval Pod is fast - ~30s, its own default grace period.
Only the PVC deletion path is the slow one described above.)

On Windows, run `kubectl cp` from PowerShell, not Git Bash - MSYS rewrites the
`namespace/pod:path` argument as if it were a filesystem path and mangles it.
