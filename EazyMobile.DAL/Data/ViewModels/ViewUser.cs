using EazyMobile.DAL.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewUser
    {
        //public string Id { get; set; }
        public string UserId { get; set; }
        //public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Bvn { get; set; }
        public string UserStatus { get; set; }
        //public bool Active { get; set; }
        //public bool Block { get; set; }
        //public bool Suspend { get; set; }
        public DateTime BlockDate { get; set; }
        public DateTime SuspendDate { get; set; }
        public string BlockedBy { get; set; }
        //public int PasswordRetryCount { get; set; }
        //public string Pin { get; set; }
        public bool OnBoardingComplete { get; set; }
        //public string Otp { get; set; }
        //public DateTime? OtpExpiration { get; set; }
        public string FullName { get; set; }
        public string CustomerId { get; set; }

        //[JsonIgnore]
        public virtual ICollection<ViewAccount> Accounts { get; set; }
        //[JsonIgnore]
        public virtual ICollection<ViewBeneficiary> Beneficiaries { get; set; }
        //[JsonIgnore]
        public virtual ICollection<ViewDevice> Devices { get; set; }
        //[JsonIgnore]
        //public virtual TransferLimit TransferLimits { get; set; }
        public ViewTransferLimit TransferLimit { get; set; }
        //[JsonIgnore]
        public virtual ICollection<ViewUserMaintHist> UserMaintHists { get; set; }


        public bool EmailConfirmed { get; set; }
        //public DateTimeOffset? LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        //public string ConcurrencyStamp { get; set; }
        //public string SecurityStamp { get; set; }
        //public string PasswordHash { get; set; }
        //public string NormalizedEmail { get; set; }
        //public string NormalizedUserName { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        //public bool LockoutEnabled { get; set; }
        //public int AccessFailedCount { get; set; }


        public static implicit operator ViewUser(User user)
        {
            return new ViewUser
            {

                UserId = user.UserId,
                //Password = user.Password,
                DateCreated = user.DateCreated,
                Bvn = user.Bvn,
                //Active = user.Active,
                //Block = user.Block,
                //Suspend = user.Suspend,
                UserStatus = user.UserStatus,
                BlockDate = user.BlockDate,
                SuspendDate = user.SuspendDate,
                BlockedBy = user.BlockedBy,
                //PasswordRetryCount = user.PasswordRetryCount,
                //Pin = user.Pin,
                OnBoardingComplete = user.OnBoardingComplete,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                //Otp = user.Otp,
                //OtpExpiration = user.OtpExpiration,
                FullName = user.FullName,
                CustomerId = user.CustomerId,

                Accounts = user.Accounts.Select(a => new ViewAccount
                {
                    AccountNo = a.AccountNo,
                    SeqNo = a.SeqNo,
                    Show = a.Show,
                    UserId = a.UserId
                }).ToList(),


                UserMaintHists = user.UserMaintHists.Select(x => new ViewUserMaintHist
                {
                    SeqNo = x.SeqNo,
                    UserId = x.UserId,
                    BlockOldValue = x.BlockOldValue,
                    BlockNewValue = x.BlockNewValue,
                    SuspendOldValue = x.SuspendOldValue,
                    SuspendNewValue = x.SuspendNewValue,
                    ActivityDate = x.ActivityDate,
                    MaintFlagCode = x.MaintFlagCode,
                    BlockedBy = x.BlockedBy,
                }).ToList(),
                Devices = user.Devices.Select(x => new ViewDevice
                {
                    UserId = x.UserId,
                    DeviceId = x.DeviceId,
                }).ToList(),
                Beneficiaries = user.Beneficiaries.Select(x => new ViewBeneficiary
                {
                    UserId = x.UserId,
                    BeneficiaryAccountNo = x.BeneficiaryAccountNo,
                    BeneficiaryAccountName = x.BeneficiaryAccountName,
                    BeneficiaryBank = x.BeneficiaryBank,
                }).ToList(),

                TransferLimit = user.TransferLimit,

                EmailConfirmed = user.EmailConfirmed,
                Id = user.Id,
                //LockoutEnd = user.LockoutEnd,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                //ConcurrencyStamp = user.ConcurrencyStamp,
                //SecurityStamp = user.SecurityStamp,
                //PasswordHash = user.PasswordHash,
                //NormalizedEmail = user.NormalizedEmail,
                //NormalizedUserName = user.NormalizedUserName,
                UserName = user.UserName,
                //LockoutEnabled = user.LockoutEnabled,
                //AccessFailedCount = user.AccessFailedCount
            };

        }

        //public static implicit operator User(ViewUser user)
        //{
        //    return new User
        //    {

        //        UserId = user.UserId,
        //        //Password = user.Password,
        //        DateCreated = user.DateCreated,
        //        Bvn = user.Bvn,
        //        //Active = user.Active,
        //        //Block = user.Block,
        //        //Suspend = user.Suspend,
        //        UserStatus = user.UserStatus,
        //        BlockDate = user.BlockDate,
        //        SuspendDate = user.SuspendDate,
        //        BlockedBy = user.BlockedBy,
        //        PasswordRetryCount = user.PasswordRetryCount,
        //        //Pin = user.Pin,
        //        OnBoardingComplete = user.OnBoardingComplete,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        //Otp = user.Otp,
        //        //OtpExpiration = user.OtpExpiration,

        //        //Accounts = ,
        //        //UserMaintHists = user.UserMaintHists.ToList(),
        //        //Devices = user.Devices.ToList(),
        //        //Beneficiaries = user.Beneficiaries.ToList(),
        //        //TransferLimit = user.TransferLimit,

        //        EmailConfirmed = user.EmailConfirmed,
        //        Id = user.Id,
        //        LockoutEnd = user.LockoutEnd,
        //        TwoFactorEnabled = user.TwoFactorEnabled,
        //        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
        //        //ConcurrencyStamp = user.ConcurrencyStamp,
        //        //SecurityStamp = user.SecurityStamp,
        //        //PasswordHash = user.PasswordHash,
        //        NormalizedEmail = user.NormalizedEmail,
        //        NormalizedUserName = user.NormalizedUserName,
        //        UserName = user.UserName,
        //        LockoutEnabled = user.LockoutEnabled,
        //        AccessFailedCount = user.AccessFailedCount
        //    };


        //}
    }

}
