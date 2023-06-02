using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.RestClientViewModels
{
    public class ViewAccountDetails
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BranchSolid { get; set; }
        public string BranchName { get; set; }
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string MainAddress { get; set; }
        public string MainCity { get; set; }
        public string MainState { get; set; }
        public string MainCountry { get; set; }
        public string AccountOfficerCode { get; set; }
        public string AccountOfficerName { get; set; }
        public string MainPhone { get; set; }
        public string AlternatePhone { get; set; }
        public string BankAccountType { get; set; }
        public DateTime OpeningDate { get; set; }
        public string BVN { get; set; }
        public DateTime DOB { get; set; }
        public short AccountStatus { get; set; }
        public string SchmCode { get; set; }
        public string Gender { get; set; }

    }
}
