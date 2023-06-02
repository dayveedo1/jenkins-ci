using System;
using System.Collections.Generic;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public class LoginHist
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public string DeviceId { get; set; }
        public string Location { get; set; }

        public virtual Device Device { get; set; }
    }
}
