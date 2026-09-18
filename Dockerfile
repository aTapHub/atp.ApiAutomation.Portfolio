# Environment-only image: SDK + restored NuGet packages, no test source.
# The test Job mounts the actual framework source fresh at run time (via an
# initContainer git clone into a shared emptyDir) rather than baking it in
# here, so code changes never require rebuilding this image - only a
# Dockerfile or .csproj change does.
FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
COPY atp.ApiAutomation.Framework/atp.ApiAutomation.Framework.csproj atp.ApiAutomation.Framework/
RUN dotnet restore atp.ApiAutomation.Framework/atp.ApiAutomation.Framework.csproj

WORKDIR /app/atp.ApiAutomation.Framework
ENTRYPOINT ["dotnet", "test"]
