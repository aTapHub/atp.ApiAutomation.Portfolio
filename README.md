# atp.ApiAutomation.Portfolio
ApiAutomation Framework Portfolio

This is an API Test Automation Framework.


Here is the core schematic on how the Test and Api Services layers interact:


<img width="691" height="761" alt="Api Framework drawio" src="https://github.com/user-attachments/assets/6e33412e-f464-4a1d-ab78-93bd372515c6" />


----------------------------------------


Libraries used:

<img width="20" height="20" alt="image" src="https://github.com/user-attachments/assets/b838c372-611f-41f6-b7ff-7367d0f407a4" /> RestSharp

<img width="20" height="20" src="https://api.nuget.org/v3-flatcontainer/nunit/3.13.3/icon" alt="NUnit Icon"> NUnit

<img width="20" height="20" src="https://api.nuget.org/v3-flatcontainer/newtonsoft.json/13.0.1/icon" alt="Newtonsoft.Json Icon"> Newtonsoft.Json

<img width="20" height="20" src="https://api.nuget.org/v3-flatcontainer/fluentassertions/8.6.0/icon" alt="FluentAssertions Icon"> FluentAssertions

<img width="20" height="20" alt="icon" src="https://github.com/user-attachments/assets/31acd669-45e2-44ea-91be-4cd2e4b5a596" /> Serilog

<img width="20" height="20" src="https://api.nuget.org/v3-flatcontainer/microsoft.extensions.configuration/6.0.0/icon" alt="Microsoft.Extensions.Configuration Icon"> Microsoft.Extensions.Configuration

----------------------------------------
Framework Directory structure

/atp.ApiAutomation.Framework
|
├── /.github
|   └── /workflows
|       └── dotnet-test.yml
|
├── /Configurations
|   └── ApiSettings.cs
|
├── /Models
|   └── CreateEmployeeModel.cs
|
├── /Services
|   ├── /Employees
|   │   ├── EmployeeEndpoints.cs
|   │   └── EmployeesService.cs
|   ├── /Simulate
|   │   ├── SimulateEndpoints.cs
|   │   └── SimulateService.cs
|   └── BaseService.cs
|
├── /Tests
|   ├── BaseTest.cs
|   ├── EmployeesEndpointTests.cs
|   ├── SimulateBaseTests.cs
|   └── SimulateTests.cs
|
├── appsettings.json
├── atp.ApiAutomation.Framework.csproj
├── atp.ApiAutomation.Framework.sln
├── .gitignore
└── README.md
