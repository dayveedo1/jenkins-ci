using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class Beneficiary
    {
        public string UserId { get; set; }
        public string BeneficiaryAccountNo { get; set; }
        public string BeneficiaryAccountName { get; set; }
        public string BeneficiaryBank { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
