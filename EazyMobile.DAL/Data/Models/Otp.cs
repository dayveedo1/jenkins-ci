using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.Models
{
    public class Otp
    {
        public long SeqNo { get; set; }
        public string AccountNo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpTimeCreated { get; set; }
        public string Status { get; set; }
    }
}
