using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewPrmType
    {
        public short TypeCode { get; set; }
        public string TypeDesc { get; set; }
        public bool UserDefined { get; set; }
        public string LabelCode { get; set; }
        public string LabelDesc { get; set; }

        public static implicit operator ViewPrmType(PrmType prmType)
        {
            return new ViewPrmType
            {
                TypeCode = prmType.TypeCode,
                TypeDesc = prmType.TypeDesc,
                UserDefined = prmType.UserDefined,
                LabelCode = prmType.LabelCode,
                LabelDesc = prmType.LabelDesc
            };
        }

        public static implicit operator PrmType(ViewPrmType prmType)
        {
            return new PrmType
            {
                TypeCode = prmType.TypeCode,
                TypeDesc = prmType.TypeDesc,
                UserDefined = prmType.UserDefined,
                LabelCode = prmType.LabelCode,
                LabelDesc = prmType.LabelDesc
            };
        }
    }
}
