using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewOtp
    {
        public long SeqNo { get; set; }
        public string AccountNo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpTimeCreated { get; set; }
        public string Status { get; set; }

        public static implicit operator ViewOtp(Otp otp)
        {
            return new ViewOtp
            {
                SeqNo = otp.SeqNo,
                AccountNo = otp.AccountNo,
                Email = otp.Email,
                PhoneNumber = otp.PhoneNumber,
                OtpCode = otp.OtpCode,
                OtpTimeCreated = otp.OtpTimeCreated,
                Status = otp.Status
            };
        }

        public static implicit operator Otp(ViewOtp otp)
        {
            return new Otp
            {
                SeqNo = otp.SeqNo,
                AccountNo = otp.AccountNo,
                Email = otp.Email,
                PhoneNumber = otp.PhoneNumber,
                OtpCode = otp.OtpCode,
                OtpTimeCreated = otp.OtpTimeCreated,
                Status = otp.Status
            };
        }
    }
}
