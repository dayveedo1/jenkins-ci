using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewPrmTypesDetail
    {
        public short TypeCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? Display { get; set; }

        public static implicit operator ViewPrmTypesDetail(PrmTypesDetail prmType)
        {
            return new ViewPrmTypesDetail
            {
                TypeCode = prmType.TypeCode,
                Code = prmType.Code,
                Description = prmType.Description,
                Display = prmType.Display
            };
        }

        public static implicit operator PrmTypesDetail(ViewPrmTypesDetail prmType)
        {
            return new PrmTypesDetail
            {
                TypeCode = prmType.TypeCode,
                Code = prmType.Code,
                Description = prmType.Description,
                Display = prmType.Display
            };
        }
    }
}
