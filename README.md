Trainline.PromocodeService.sln

### TODO

- Generate configuration
- Copy a teamcity pipeline from another component e.g. FulfilmentService
- Create a service in Environment Manager, reserving ports and continue with the [new component creation steps](https://wiki.thetrainline.com/display/FUS/New+Component+Creation)

## Runbooks
[Runbook](RUNBOOK.md), [deployments](DEPLOYMENT.md) and [monitoring](MONITORING.md)

### Urls

Cluster: https://c22-promocode.service.ttlnonprod.local/

Integration: https://c99-promocode.service.ttlnonprod.local/

Staging: https://st1-promocode.service.ttlnonprod.local/

Prod: https://promocode.service.ttlprod.local/

### Docs

Just like our other APIs our documentation tool of choice is swagger, to access the API docs you should be able to navigate to one of the Urls above followed by '/docs'.

## Local Development

Run the following powershell commands to reserve the url locally, and create a database if necessary:

```powershell
.\setup.ps1 urlacl

# If you have a database
.\setup.ps1 localdb
```

This project is setup to use Piper during it's build, test and package flow in TeamCity, this means that the same process can be replicated locally as it is based upon the pipespec.yml file.

For further information see [https://github.thetrainline.com/DietCode/piper](https://github.thetrainline.com/DietCode/piper)

To install piper locally issue the following:

```
dotnet tool install -g Trainline.Piper --add-source https://artifactory.corp.local/artifactory/api/nuget/nuget-virtual
```

To build, test or package your code:

```
tl-piper build
tl-piper package
```

## Health Checks

We currently support two health checks

- /diagnostics/healthcheck
- /ping

Both endpoints provide a formatted output in the following format:

```json
{
  "ApplicationId": "42480",
  "ServiceName": "PromocodeService",
  "Version": "1.0.0",
  "Slice": "blue",
  "IsHealthy": true,
  "Checks": []
}
```

## Exceptions

We utilise the Trainline.NetCore.Exception library in order to present a consistent exceptional response contract; that format is as follows:

```json
{
  "errors": [
    {
      "code": "42480.10",
      "detail": "InvalidProperty: The property field is required.",
      "meta": {
      	"severity": "correctable",
	"service": "PromocodeService"
      }
    }
  ]
}
```

# New Relic

This service can be found on new relic with the name: `$env_name$ Trainline.Service_LOWERCASE ($slice$)`.

# Logs

This service reports logs to Kibana using a combination of Serilog and filebeat.
Serilog writes the logs to a file/syslog and filebeat.
