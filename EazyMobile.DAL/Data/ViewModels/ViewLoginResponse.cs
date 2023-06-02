using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewLoginResponse
    {

        public ViewUser user { get; set; }
        //public ViewUser user { get; set; }
        public string token { get; set; }
    }
}
