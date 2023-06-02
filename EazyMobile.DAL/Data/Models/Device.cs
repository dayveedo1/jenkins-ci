using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class Device
    {
        public Device()
        {
            LoginHists = new HashSet<LoginHist>();
        }

        public string UserId { get; set; }
        public string DeviceId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<LoginHist> LoginHists { get; set; }
    }
}
