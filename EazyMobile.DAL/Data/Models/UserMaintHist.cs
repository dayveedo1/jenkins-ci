using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class UserMaintHist
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public bool BlockOldValue { get; set; }
        public bool BlockNewValue { get; set; }
        public bool SuspendOldValue { get; set; }
        public bool SuspendNewValue { get; set; }
        public DateTime ActivityDate { get; set; }
        public string MaintFlagCode { get; set; }
        public string BlockedBy { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
