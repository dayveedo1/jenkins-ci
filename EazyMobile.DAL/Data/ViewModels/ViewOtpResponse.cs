using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewOtpResponse
    {
        public string Email { get; set; }
        public string OtpValue { get; set; }
        public double OtpExpirationTime { get; set; }
    }
}
