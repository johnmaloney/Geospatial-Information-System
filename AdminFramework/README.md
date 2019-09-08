### Logging

Logging is hosted on a Docker VM in Windows Azure(MyDockerVM)
[Url for Kibana Logs](http://40.118.239.120:5601/app/kibana)


#### Versioning
The versioning of this library is currently managed by the build parameter named 'constantVersionNumber'.
The purpose of this is to allow a new Nuget package to be deployed with changes and builds that occur will
download the latest Nuget and use it in the deployment. This may not be sustainable in the long run but it
ensures that every deployment has the latest code. It acts as if the project is directly referenced in the 
projects that reference it.