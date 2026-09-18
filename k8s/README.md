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
