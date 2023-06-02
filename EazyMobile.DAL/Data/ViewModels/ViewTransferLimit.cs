using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewTransferLimit
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public decimal PreviousLimit { get; set; }
        public decimal CurrentLimit { get; set; }
        public DateTime DateTimeModified { get; set; }

        public static implicit operator ViewTransferLimit(TransferLimit transferLimit)
        {
            return new ViewTransferLimit
            {
                SeqNo = transferLimit.SeqNo,
                UserId = transferLimit.UserId,
                PreviousLimit = transferLimit.PreviousLimit,
                CurrentLimit = transferLimit.CurrentLimit,
                DateTimeModified = transferLimit.DateTimeModified
            };
        }

        public static implicit operator TransferLimit(ViewTransferLimit transferLimit)
        {
            return new TransferLimit
            {
                SeqNo = transferLimit.SeqNo,
                UserId = transferLimit.UserId,
                PreviousLimit = transferLimit.PreviousLimit,
                CurrentLimit = transferLimit.CurrentLimit,
                DateTimeModified = transferLimit.DateTimeModified
            };
        }
    }
}
