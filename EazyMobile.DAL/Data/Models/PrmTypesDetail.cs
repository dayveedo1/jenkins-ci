using System;
using System.Collections.Generic;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class PrmTypesDetail
    {
        public short TypeCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? Display { get; set; }

        public virtual PrmType TypeCodeNavigation { get; set; }
    }
}
