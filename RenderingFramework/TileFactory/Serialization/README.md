
##Serialization into  Protobuf

C# documentation : https://developers.google.com/protocol-buffers/docs/csharptutorial

The command line for producing the VectorTile.cs file from the VectorTile.proto file follows this format:

protoc -I=$SRC_DIR --csharp_out=$DST_DIR $SRC_DIR/addressbook.proto

example:
C:\Users\maloney1\Source\Repos\PSU\RenderingFramework\TileFactory\Serialization>
protoc -I=C:\Users\maloney1\Source\Repos\PSU\RenderingFramework\TileFactory\Serialization\  
--csharp_out=C:\Users\maloney1\Source\Repos\PSU\RenderingFramework\TileFactory\Serialization\ VectorTile.proto

Note that the $DST_DIR does not specify a file name, just a directory