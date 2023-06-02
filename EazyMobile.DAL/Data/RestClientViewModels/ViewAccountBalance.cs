using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.RestClientViewModels
{
    public class ViewAccountBalance
    {
        public decimal AvailableBalance { get; set; }
        public decimal LienBalance { get; set; }
        public decimal LedgerBalance { get; set; }
    }
}
