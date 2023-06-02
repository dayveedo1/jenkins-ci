using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class TransferLimit
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public decimal PreviousLimit { get; set; }
        public decimal CurrentLimit { get; set; }
        public DateTime DateTimeModified { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
       
    }
}
