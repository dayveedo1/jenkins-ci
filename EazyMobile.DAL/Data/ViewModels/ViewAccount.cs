using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewAccount
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public string AccountNo { get; set; }
        public bool Show { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string AccountStatus { get; set; }

        public decimal AccountBalance { get; set; }


        public static implicit operator ViewAccount(Account account)
        {
            return new ViewAccount
            {
                SeqNo = account.SeqNo,
                UserId = account.UserId,
                AccountNo = account.AccountNo,
                Show = account.Show,
                AccountName = account.AccountName,
                AccountType = account.AccountType,
                //AccountStatus = account.AccountStatus,
                BranchCode = account.BranchCode,
                BranchName = account.BranchName
            };
        }

        public static implicit operator Account(ViewAccount account)
        {
            return new Account
            {
                SeqNo = account.SeqNo,
                UserId = account.UserId,
                AccountNo = account.AccountNo,
                Show = account.Show,
                AccountName = account.AccountName,
                AccountType = account.AccountType,
                //AccountStatus = account.AccountStatus,
                BranchCode = account.BranchCode,
                BranchName = account.BranchName
            };
        }
    }
}
