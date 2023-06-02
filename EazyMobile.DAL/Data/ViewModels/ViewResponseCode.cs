using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewResponseCode
    {
        #region Response Code
        public readonly string APPROVED_COMPLETED_SUCCESSFULLY_CODE = "00";
        public readonly string STATUS_UNKNOWN_CODE = "01";
        public readonly string INVALID_ACCOUNT_CODE = "07";
        public readonly string INACTIVE_ACCOUNT_CODE = "11";
        public readonly string INVALID_PASSWORD_CODE = "20";
        public readonly string BLOCKED_CODE = "22";
        public readonly string BLACKLISTED_CODE = "23";
        public readonly string LOCKED_CODE = "24";
        public readonly string UNABLE_TO_LOCATE_RECORD_CODE= "25";
        public readonly string DUPLICATE_RECORD_CODE= "26";
        public readonly string PRESENT_NOT_ACTIVATED_RECORD_CODE= "27";
        //public readonly string INVALID_OTP_CODE= "27";
        //public readonly string EXPIRED_OTP_CODE= "28";
        public readonly string INVALID_OTP_CODE = "28";
        public readonly string EXPIRED_OTP_CODE = "29";


        public readonly string TRANSFER_LIMIT_EXCEEDED_CODE = "61";

        public readonly string INVALID_PIN_CODE = "19";

        #endregion

        #region Response Code Description

        public readonly string APPROVED_COMPLETED_SUCCESSFULLY_MSG = "Approved or Completed Successfully";
        public readonly string STATUS_UNKNOWN_MSG = "Status unknown, please wait for settlement report";
        public readonly string INVALID_ACCOUNT_MSG = "Invalid Account";
        public readonly string INACTIVE_ACCOUNT_MSG = "Inactive Account";
        public readonly string INVALID_PASSWORD_MSG = "Invalid Password";
        public readonly string BLOCKED_MSG = "Blocked";
        public readonly string BLACKLISTED_MSG = "Blacklisted";
        public readonly string LOCKED_MSG = "Locked";
        public readonly string UNABLE_TO_LOCATE_RECORD_MSG = "Unable to locate record";
        public readonly string DUPLICATE_RECORD_MSG = "Duplicate record";
        public readonly string INVALID_OTP_MSG = "Invalid OTP";
        public readonly string EXPIRED_OTP_MSG = "Expired OTP";
        public readonly string PRESENT_NOT_ACTIVATED_RECORD_MSG = "Present, Not Activated";

        public readonly string INVALID_PIN_MSG = "Invalid Pin";

        public readonly string TRANSFER_LIMIT_EXCEEDED_MSG = "Transfer limit exceeded";
        #endregion

        #region PrmTypesDetails Parameters

        public readonly string STATUS_ACTIVE = "00";
        //public readonly string STATUS_BLOCKED = "002";
        //public readonly string STATUS_BLACKLISTED = "003";

        #endregion
    }
}
