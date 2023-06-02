using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewUserMaintHist
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

        public static implicit operator ViewUserMaintHist(UserMaintHist userMaintHist)
        {
            return new ViewUserMaintHist
            {
                SeqNo = userMaintHist.SeqNo,
                UserId = userMaintHist.UserId,
                BlockOldValue = userMaintHist.BlockOldValue,
                BlockNewValue = userMaintHist.BlockNewValue,
                SuspendOldValue = userMaintHist.SuspendOldValue,
                SuspendNewValue = userMaintHist.SuspendNewValue,
                ActivityDate = userMaintHist.ActivityDate,
                MaintFlagCode = userMaintHist.MaintFlagCode,
                BlockedBy = userMaintHist.BlockedBy
            };
        }

        public static implicit operator UserMaintHist(ViewUserMaintHist userMaintHist)
        {
            return new UserMaintHist
            {
                SeqNo = userMaintHist.SeqNo,
                UserId = userMaintHist.UserId,
                BlockOldValue = userMaintHist.BlockOldValue,
                BlockNewValue = userMaintHist.BlockNewValue,
                SuspendOldValue = userMaintHist.SuspendOldValue,
                SuspendNewValue = userMaintHist.SuspendNewValue,
                ActivityDate = userMaintHist.ActivityDate,
                MaintFlagCode = userMaintHist.MaintFlagCode,
                BlockedBy = userMaintHist.BlockedBy
            };
        }
    }
}
