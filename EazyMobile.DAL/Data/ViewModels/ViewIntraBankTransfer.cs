using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewIntraBankTransfer : IntraBankTransfer
    {
        public string CustNo { get; set; }
        //public string SourceAccount { get; set; }
        //public string DestinationAccountNo { get; set; }
        //public string DestinationAccountName { get; set; }
        //public decimal Amount { get; set; }
        //public string Narration { get; set; }
        public string ReferenceNo { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class IntraBankTransfer
    {
        public string SourceAccount { get; set; }
        public string DestinationAccountNo { get; set; }
        public string DestinationAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
    }
}
