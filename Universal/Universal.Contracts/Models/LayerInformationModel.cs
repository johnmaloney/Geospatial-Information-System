using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Universal.Contracts.Models
{
    public class LayerInformationModel
    {
        #region Fields



        #endregion

        #region Properties

        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Property[] Properties { get; set; }

        #endregion

        #region Methods

        public T GetPropertyValueAs<T>(string name)
        {
            // First check the properties to ensure that one of them meet the criteria //
            if (this.Properties != null && Properties.Any(p => p.Name.ToLowerInvariant() == name.ToLowerInvariant()))
            {
                var property = Properties.First(p => p.Name.ToLowerInvariant() == name.ToLowerInvariant());
                return property.GetValueAs<T>();
            }

            return default(T);
        }

        #endregion
    }
}
