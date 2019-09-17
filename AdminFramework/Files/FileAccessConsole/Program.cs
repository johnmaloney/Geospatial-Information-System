using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace FileAccessConsole
{
    class Program
    {
        private static string uploadStorageAcct;
        private static string storageAccountKey;

        static void Main(string[] args)
        {
            BuildConfiguration();

            Console.WriteLine("Enter commands:");

            string line;
            Console.WriteLine("Enter list, file:[name], dir:[name] (press CTRL+Z to exit):");
            Console.WriteLine();
            do
            {
                line = Console.ReadLine();
                if (line != null)
                {
                    switch (line.ToLower().Substring(line.IndexOf(':')))
                    {
                        case "list":
                            {
                               
                                break;
                            }
                        case "file":
                            {
                                
                                break;
                            }
                        case "dir":
                            {

                                break;
                            }
                        default:
                            break;
                    }
                }
            } while (line != null);
        }

        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
               .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();
            uploadStorageAcct = config["UploadStorageAcctName"];
            storageAccountKey = config["StorageAccountKey"];
        }
    }
}
