FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

# Copy just the csproj first so `dotnet restore`'s layer is cached by Kaniko
# and only re-runs when a package reference actually changes.
COPY atp.ApiAutomation.Framework/atp.ApiAutomation.Framework.csproj atp.ApiAutomation.Framework/
RUN dotnet restore atp.ApiAutomation.Framework/atp.ApiAutomation.Framework.csproj

COPY atp.ApiAutomation.Framework/ atp.ApiAutomation.Framework/
WORKDIR /app/atp.ApiAutomation.Framework

ENTRYPOINT ["dotnet", "test"]
