# Geospatial Information System
This repository hosts all of the documentation and source code for the GIS constructed as a capstone
 project. The GIS is architecturally designed to be hosted in a disconnected environment with 
a publisher/subscriber pipeline that distributes events.

#### Applications
The Admin site can communicate with the backend services, this application is
available here:  https://gis-admin.azurewebsites.net/

The backend services consist of [Logging](http://40.118.239.120:5601/app/kibana#/discover?_g=()), 
[File Storage](https://docs.microsoft.com/en-us/rest/api/storageservices/file-service-rest-api), 
[Messenging](https://docs.microsoft.com/en-us/azure/service-bus-messaging/) and 
[Tile Rendering](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/RenderingFramework)

These services make up the foundation of this GIS, and allow it to operate in a disconnected manner.

The code that makes each part of the architecture work is found under this repository:  
1. [Logging](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/AdminFramework)
2. [Cloud File Access](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/AdminFramework/Files)
3. [Messenging](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/EventFramework)
4. [Tile Rendering](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/RenderingFramework)

#### Example Workflow
Here is an example worlflow with each stage described:
1. After accessing the [Admin](https://gis-admin.azurewebsites.net/) site select a look to the upper 
right corner for the Job request form. Select "Project Data" and pick a valid GeoJSON file from your file system.

2. Once the application uploads the request for projecting the data the server will do two things, it will
store the [file in the Cloud and generate a message](https://github.com/johnmaloney/Geospatial-Information-System/blob/master/Applications/AdminManagementApp/Controllers/MessageController.cs)
(Post method line 44) for the 
[Messaging framework](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/EventFramework). 
This message goes into a queue 
and waits for a tile processor to take the work on. 

3. An instance of the [tile factory](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/Applications/TileProcessingApp) 
will accept the queued message pull the file from the file storage and process the file into projected data.

4. When the data is projected the tile processor will store the features as a new layer on the server. Its important 
to note that this is not stored in the file system on the server, only in memory. While this will pose a challenge to
the memory pressure on the server it allows the original file to be stored in the cloud away from the rendering server.
This allows the file to remain accessible even if the rendering server goes out of commission.

5. The tile processor will send a message that it has completed the job and the new layer will be available from the 
layers dropdown. An important feature here is that the processing server has set the layer url, this means that when
the client application selects this layer to load it will be rendered by a server in the Cloud, allowing for different
servers to take on different workloads for clients. This is a key distinction from having a single server to render all the tile
requests.


#### References
#Tile Factory
This is the Mapbox Vector Tile specification 2.1
