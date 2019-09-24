### Logging

Logging is hosted on a Docker VM in Windows Azure(MyDockerVM)
[Url for Kibana Logs](http://40.118.239.120:5601/app/kibana)

Logging using the ILogger instance is only done in the Host applications, this is to reduce the required
libraries in other libraries. For instance no logging is done in the Messaging or File libraries. 

These libraries still use the interface type ILogEntry and can return a list of log entries from their
methods/classes. This list would then have to executed upon by the host application. Here is an example
of a logging hook from the Messenging library: 
```csharp
    public void RegisterForLogNotifications(Func<IEnumerable<ILogEntry>, Task> logHandler)
    {
        logRegistrations.Add(logHandler);
    }

......

        // Notify any registrations of the log events. //
        if (logRegistrations != null && logRegistrations.Count > 0)
        {
            foreach (var handler in logRegistrations)
            {
                await handler(logging);
            }
        }
```


### Files
This library uses Azure File storage to do file operations.  
The main use for this is to allow a central repository for files that will be processed by  
a service instance of the tile processing application.

This library can be used for any account on Azure File Storage  
You will need a [actual file store](https://docs.microsoft.com/en-us/azure/storage/files/), a shared key for access, and this library. 


To use the file library first get the Nuget from the [GIS repository](https://aetos.pkgs.visualstudio.com/Geospatial-Information-System/_packaging/GIS-Packages-Public/nuget/v3/index.json)  

Configure a location for your storage account name and storage account access key, like this:
```json
{
  "UploadStorageAcctName": "gisfilestore" // This the container you will put files in ,
  "StorageAccountKey": "gbToQuzqaK3h664blBF78qUljig=="
}
```

Then add the following code to the initializer in your application:  
```csharp
   var repository = new AzureFileRepository(
      new AzureFileReader(
        [YOUR STORAGE ACCT NAME], 
        [YOUR STORAGE ACCT SHARE KEY], 
        "[YOUR ACTUAL FILESTORE NAME]"));
```

Once you have the above code setup and working you are mostly there, this library takes care of a bunch 
of things for you like Access Headers, reading the information from directories, encoding/decoding the file 
for storage, etc.