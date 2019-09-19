using Files;
using Files.CloudFileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileAccessConsole
{
    class Program
    {
        private static string uploadStorageAcct;
        private static string storageAccountKey;

        static void Main(string[] args)
        {
            BuildConfiguration();
            var repository = new AzureFileRepository(
                new AzureFileReader(uploadStorageAcct, storageAccountKey, "gis-uploads"));
            var home = Guid.NewGuid().ToString();
            Console.WriteLine("Enter commands:");

            string line;
            Console.WriteLine("Enter list, file:[name], dir:[name] (press CTRL+Z to exit):");
            Console.WriteLine();
            do
            {
                line = Console.ReadLine();
                if (line != null)
                {
                    // Get the command to the left of the : //
                    var command = Regex.Match(line, @"^.*?(?=:)").Value;

                    // GETS fileNameArg FROM file:fileNameArg //
                    var argument = Regex.IsMatch(line, @"(?<=:)(.*)(?=/)") 
                        ? Regex.Match(line, @"(?<=:)(.*)(?=/)").Value
                        : string.Empty;

                    // GETS fileName FROM file:directory/fileName //
                    var secondArg = Regex.IsMatch(line, @"[^/]+$")
                        ? Regex.Match(line, @"[^/]+$").Value
                        : string.Empty;

                    switch (command.ToLower())
                    {
                        case "get":
                            {
                                Task.Run(() => repository.Get(argument ?? home, secondArg));
                                break;
                            }
                        case "list": 
                            {
                                Task.Run(() => repository.GetDirectory(home));
                                break;
                            }
                        case "file":
                            {
                                var f = new File()
                                {
                                    Name = secondArg ?? "data.geojson",
                                    Directory = argument ?? home,
                                    TextContents = FileSampleContent()
                                };
                                Task.Run(()=> repository.Add(f));
                                break;
                            }
                        case "dir":
                            {
                                var f = new File()
                                {
                                    Directory = argument ?? home
                                };
                                Task.Run(() => repository.Add(f));
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

        private static string FileSampleContent()
        {
            return "{\r\n\t\"type\": \"FeatureCollection\",\r\n  \"features\": [\r\n    {\r\n      \"type\": \"Feature\",\r\n      \"properties\": {\r\n        \"SCALERANK\": 1,\r\n        \"NATSCALE\": 300,\r\n        \"LABELRANK\": 1,\r\n        \"FEATURECLA\": \"Admin-1 capital\",\r\n        \"NAME\": \"Denver\",\r\n        \"NAMEPAR\": null,\r\n        \"NAMEALT\": \"Denver-Aurora\",\r\n        \"DIFFASCII\": 0,\r\n        \"NAMEASCII\": \"Denver\",\r\n        \"ADM0CAP\": 0,\r\n        \"CAPALT\": 0,\r\n        \"CAPIN\": null,\r\n        \"WORLDCITY\": 0,\r\n        \"MEGACITY\": 1,\r\n        \"SOV0NAME\": \"United States\",\r\n        \"SOV_A3\": \"USA\",\r\n        \"ADM0NAME\": \"United States of America\",\r\n        \"ADM0_A3\": \"USA\",\r\n        \"ADM1NAME\": \"Colorado\",\r\n        \"ISO_A2\": \"US\",\r\n        \"NOTE\": null,\r\n        \"LATITUDE\": 39.7391880484,\r\n        \"LONGITUDE\": -104.984015952,\r\n        \"CHANGED\": 5,\r\n        \"NAMEDIFF\": 0,\r\n        \"DIFFNOTE\": \"Changed scale rank.\",\r\n        \"POP_MAX\": 2313000,\r\n        \"POP_MIN\": 1548599,\r\n        \"POP_OTHER\": 1521278,\r\n        \"RANK_MAX\": 12,\r\n        \"RANK_MIN\": 12,\r\n        \"GEONAMEID\": 5419384,\r\n        \"MEGANAME\": \"Denver-Aurora\",\r\n        \"LS_NAME\": \"Denver\",\r\n        \"LS_MATCH\": 1,\r\n        \"CHECKME\": 0,\r\n        \"MAX_POP10\": 1548599,\r\n        \"MAX_POP20\": 2100407,\r\n        \"MAX_POP50\": 2174327,\r\n        \"MAX_POP300\": 2174327,\r\n        \"MAX_POP310\": 0,\r\n        \"MAX_NATSCA\": 100,\r\n        \"MIN_AREAKM\": 909,\r\n        \"MAX_AREAKM\": 1345,\r\n        \"MIN_AREAMI\": 351,\r\n        \"MAX_AREAMI\": 519,\r\n        \"MIN_PERKM\": 371,\r\n        \"MAX_PERKM\": 606,\r\n        \"MIN_PERMI\": 231,\r\n        \"MAX_PERMI\": 376,\r\n        \"MIN_BBXMIN\": -105.24166667,\r\n        \"MAX_BBXMIN\": -105.24166667,\r\n        \"MIN_BBXMAX\": -104.86666667,\r\n        \"MAX_BBXMAX\": -104.70833333,\r\n        \"MIN_BBYMIN\": 39.5,\r\n        \"MAX_BBYMIN\": 39.5,\r\n        \"MIN_BBYMAX\": 39.95833333,\r\n        \"MAX_BBYMAX\": 40.025,\r\n        \"MEAN_BBXC\": -104.993967228,\r\n        \"MEAN_BBYC\": 39.72985042,\r\n        \"COMPARE\": 0,\r\n        \"GN_ASCII\": \"Denver\",\r\n        \"FEATURE_CL\": null,\r\n        \"FEATURE_CO\": null,\r\n        \"ADMIN1_COD\": 0,\r\n        \"GN_POP\": 0,\r\n        \"ELEVATION\": 0,\r\n        \"GTOPO30\": 0,\r\n        \"TIMEZONE\": null,\r\n        \"GEONAMESNO\": \"GeoNames match general + researched.\",\r\n        \"UN_FID\": 537,\r\n        \"UN_ADM0\": \"United States of America\",\r\n        \"UN_LAT\": 39.57,\r\n        \"UN_LONG\": -105.07,\r\n        \"POP1950\": 505,\r\n        \"POP1955\": 641,\r\n        \"POP1960\": 809,\r\n        \"POP1965\": 923,\r\n        \"POP1970\": 1054,\r\n        \"POP1975\": 1198,\r\n        \"POP1980\": 1356,\r\n        \"POP1985\": 1437,\r\n        \"POP1990\": 1528,\r\n        \"POP1995\": 1747,\r\n        \"POP2000\": 1998,\r\n        \"POP2005\": 2241,\r\n        \"POP2010\": 2313,\r\n        \"POP2015\": 2396,\r\n        \"POP2020\": 2502,\r\n        \"POP2025\": 2590,\r\n        \"POP2050\": 2661,\r\n        \"CITYALT\": \"Denver\"\r\n      },\r\n      \"geometry\": {\r\n        \"type\": \"Point\",\r\n        \"coordinates\": [ -104.9859618109682, 39.7411339069655 ]\r\n      }\r\n    },\r\n    {\r\n      \"type\": \"Feature\",\r\n      \"properties\": {\r\n        \"SCALERANK\": 1,\r\n        \"NATSCALE\": 300,\r\n        \"LABELRANK\": 1,\r\n        \"FEATURECLA\": \"Populated place\",\r\n        \"NAME\": \"Chicago\",\r\n        \"NAMEPAR\": null,\r\n        \"NAMEALT\": null,\r\n        \"DIFFASCII\": 0,\r\n        \"NAMEASCII\": \"Chicago\",\r\n        \"ADM0CAP\": 0,\r\n        \"CAPALT\": 0,\r\n        \"CAPIN\": null,\r\n        \"WORLDCITY\": 1,\r\n        \"MEGACITY\": 1,\r\n        \"SOV0NAME\": \"United States\",\r\n        \"SOV_A3\": \"USA\",\r\n        \"ADM0NAME\": \"United States of America\",\r\n        \"ADM0_A3\": \"USA\",\r\n        \"ADM1NAME\": \"Illinois\",\r\n        \"ISO_A2\": \"US\",\r\n        \"NOTE\": null,\r\n        \"LATITUDE\": 41.8299906607,\r\n        \"LONGITUDE\": -87.7500549741,\r\n        \"CHANGED\": 0,\r\n        \"NAMEDIFF\": 0,\r\n        \"DIFFNOTE\": null,\r\n        \"POP_MAX\": 8990000,\r\n        \"POP_MIN\": 2841952,\r\n        \"POP_OTHER\": 3635101,\r\n        \"RANK_MAX\": 13,\r\n        \"RANK_MIN\": 12,\r\n        \"GEONAMEID\": 4887398,\r\n        \"MEGANAME\": \"Chicago\",\r\n        \"LS_NAME\": \"Chicago\",\r\n        \"LS_MATCH\": 1,\r\n        \"CHECKME\": 0,\r\n        \"MAX_POP10\": 3747798,\r\n        \"MAX_POP20\": 5069998,\r\n        \"MAX_POP50\": 8416660,\r\n        \"MAX_POP300\": 8416660,\r\n        \"MAX_POP310\": 8450289,\r\n        \"MAX_NATSCA\": 300,\r\n        \"MIN_AREAKM\": 1345,\r\n        \"MAX_AREAKM\": 4804,\r\n        \"MIN_AREAMI\": 519,\r\n        \"MAX_AREAMI\": 1855,\r\n        \"MIN_PERKM\": 471,\r\n        \"MAX_PERKM\": 2946,\r\n        \"MIN_PERMI\": 293,\r\n        \"MAX_PERMI\": 1830,\r\n        \"MIN_BBXMIN\": -88.40833333,\r\n        \"MAX_BBXMIN\": -88.03629002,\r\n        \"MIN_BBXMAX\": -87.52813766,\r\n        \"MAX_BBXMAX\": -87.125,\r\n        \"MIN_BBYMIN\": 41.39166667,\r\n        \"MAX_BBYMIN\": 41.45833333,\r\n        \"MIN_BBYMAX\": 42.00097164,\r\n        \"MAX_BBYMAX\": 42.49166667,\r\n        \"MEAN_BBXC\": -87.858739566,\r\n        \"MEAN_BBYC\": 41.83271891,\r\n        \"COMPARE\": 0,\r\n        \"GN_ASCII\": \"Chicago\",\r\n        \"FEATURE_CL\": \"P\",\r\n        \"FEATURE_CO\": \"PPL\",\r\n        \"ADMIN1_COD\": 0,\r\n        \"GN_POP\": 2841952,\r\n        \"ELEVATION\": 179,\r\n        \"GTOPO30\": 181,\r\n        \"TIMEZONE\": \"America/Chicago\",\r\n        \"GEONAMESNO\": \"GeoNames match with ascii name + lat + long whole numbers.\",\r\n        \"UN_FID\": 531,\r\n        \"UN_ADM0\": \"United States of America\",\r\n        \"UN_LAT\": 41.82,\r\n        \"UN_LONG\": -87.64,\r\n        \"POP1950\": 4999,\r\n        \"POP1955\": 5565,\r\n        \"POP1960\": 6183,\r\n        \"POP1965\": 6639,\r\n        \"POP1970\": 7106,\r\n        \"POP1975\": 7160,\r\n        \"POP1980\": 7216,\r\n        \"POP1985\": 7285,\r\n        \"POP1990\": 7374,\r\n        \"POP1995\": 7839,\r\n        \"POP2000\": 8333,\r\n        \"POP2005\": 8820,\r\n        \"POP2010\": 8990,\r\n        \"POP2015\": 9211,\r\n        \"POP2020\": 9516,\r\n        \"POP2025\": 9756,\r\n        \"POP2050\": 9932,\r\n        \"CITYALT\": null\r\n      },\r\n      \"geometry\": {\r\n        \"type\": \"Point\",\r\n        \"coordinates\": [ -87.75200083270931, 41.83193651927843 ]\r\n      }\r\n    }\r\n  \r\n  ]\r\n}\r\n";
        }
    }
}
