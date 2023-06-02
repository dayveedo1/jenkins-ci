using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.RestClientViewModels
{
    public class ViewVerifyAccountDetails
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public short AccountStatus { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string MainPhone { get; set; }
        public string BVN { get; set; }

    }
}
