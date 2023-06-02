using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewForgotPasswordRequest
    {
        public string AccountNo { get; set; }
        public string Pin { get; set; }
    }
}
