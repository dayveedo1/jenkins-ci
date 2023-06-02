using System;
using System.Collections.Generic;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public partial class PrmType
    {
        public PrmType()
        {
            PrmTypesDetails = new HashSet<PrmTypesDetail>();
        }

        public short TypeCode { get; set; }
        public string TypeDesc { get; set; }
        public bool UserDefined { get; set; }
        public string LabelCode { get; set; }
        public string LabelDesc { get; set; }

        public virtual ICollection<PrmTypesDetail> PrmTypesDetails { get; set; }
    }
}
