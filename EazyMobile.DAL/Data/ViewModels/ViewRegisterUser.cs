using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewRegisterUser
    {
        public string AccountNo { get; set; }
        public string Password { get; set; }
        //public string Email { get; set; }
        public string Pin { get; set; }
        public string Otp { get; set; }

    }
}
