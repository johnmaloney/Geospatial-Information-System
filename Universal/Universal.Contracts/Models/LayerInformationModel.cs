using System;
using System.Collections.Generic;
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



        #endregion
    }
}
