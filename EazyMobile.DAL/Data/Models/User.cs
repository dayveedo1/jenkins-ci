using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class User : IdentityUser     
    {
        public User()
        {
            Accounts = new HashSet<Account>();
            Beneficiaries = new HashSet<Beneficiary>();
            Devices = new HashSet<Device>();
            //TransferLimits = new HashSet<TransferLimit>();
            UserMaintHists = new HashSet<UserMaintHist>();
        }

        public string UserId { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        public string Bvn { get; set; }
        [Column("Status")]
        public string UserStatus { get; set; }
        //public bool Active { get; set; }
        //public bool Block { get; set; }
        //public bool Suspend { get; set; }
        public DateTime BlockDate { get; set; }
        public DateTime SuspendDate { get; set; }
        public string BlockedBy { get; set; }
        public int PasswordRetryCount { get; set; }
        public string Pin { get; set; }
        public bool OnBoardingComplete { get; set; }
        public string FullName { get; set; }
        public string CustomerId { get; set; }

        //public string Otp { get; set; }
        //public DateTime? OtpExpiration { get; set; }

        //[JsonIgnore]
        //[JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore, IsReference = true)]
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        //public virtual ICollection<TransferLimit> TransferLimits { get; set; }
        public virtual ICollection<UserMaintHist> UserMaintHists { get; set; }
        public virtual TransferLimit TransferLimit { get; set; }
    }
}
