using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewLoginHist
    {
        public long SeqNo { get; set; }
        public string UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public string DeviceId { get; set; }
        public string Location { get; set; }

        public static implicit operator ViewLoginHist(LoginHist loginHist)
        {
            return new ViewLoginHist
            {
                SeqNo = loginHist.SeqNo,
                UserId = loginHist.UserId,
                LoginDate = loginHist.LoginDate,
                DeviceId = loginHist.DeviceId,
                Location = loginHist.Location
            };

        }

        public static implicit operator LoginHist(ViewLoginHist loginHist)
        {
            return new LoginHist
            {
                SeqNo = loginHist.SeqNo,
                UserId = loginHist.UserId,
                LoginDate = DateTime.Now,
                DeviceId = loginHist.DeviceId,
                Location = loginHist.Location
            };
        }
    }
}
