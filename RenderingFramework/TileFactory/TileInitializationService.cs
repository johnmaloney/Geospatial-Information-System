using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Universal.Contracts.Serial;

namespace TileFactory
{
    /// <summary>
    /// This class acts as a broker into the various different ways a tile
    /// might be initialized into memory. The tiles in this scenario can be
    /// of the ITile type, which is the baseline for the building of rendered
    /// tiles
    /// </summary>
    public class TileInitializationService
    {
        #region Fields

        private readonly IFileProvider fileProvider;
        private readonly Dictionary<string, string> files = new Dictionary<string, string>();
        private object fileLock = new object();

        #endregion

        #region Properties



        #endregion

        #region Methods

        public TileInitializationService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
            foreach(var item in this.fileProvider.GetDirectoryContents("/"))
            {
                files.Add(item.Name, item.PhysicalPath);
            }
        }

        /// <summary>
        /// Look into the file system and see what tiles with the given identifier
        /// can be found.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public IEnumerable<Feature> InitializeTile(string identifier)
        {
            if (files.ContainsKey(identifier))
            {
                var regex = new Regex(@"\.[A-Za-z0-9]+$");
                var match = regex.Match(identifier);
                switch (match.Value)
                {
                    case ".json":
                        {
                            var json = GetText(files[identifier]);
                            return json.FromJsonInto<List<Feature>>();
                        }
                    default:
                        break;
                }
            }

            return null;
        }

        private string GetText(string filePath)
        {
            string fileText = string.Empty;
            lock(fileLock)
            {
                fileText = File.ReadAllText(filePath);
            }
            return fileText;
        }

        #endregion
    }
}
