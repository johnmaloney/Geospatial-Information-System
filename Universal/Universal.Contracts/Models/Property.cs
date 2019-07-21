using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Models
{
    public struct Property
    {
        #region Fields



        #endregion

        #region Properties

        public string Name { get; set; }
        public Type ValueType { get; set; }
        public object Value { get; set; }

        #endregion

        #region Methods



        #endregion
    }
}
