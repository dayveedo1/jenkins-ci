using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;



namespace EazyMobile.DAL.Data.Models
{
    public class Account
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public string AccountNo { get; set; }
        public bool Show { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public short AccountStatus { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
