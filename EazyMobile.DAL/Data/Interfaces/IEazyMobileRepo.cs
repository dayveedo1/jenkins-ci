using EazyMobile.DAL.Data.RestClientViewModels;
using EazyMobile.DAL.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.Interfaces
{
    public interface IEazyMobileRepo
    {
        #region JwtService

        Task<ViewApiResponse> ValidateUser(ViewLoginUser view);
        Task<ViewApiResponse> ValidateUser2(ViewLoginUser view);
        Task<String> CreateToken();

        #endregion

        #region User
        Task<ViewApiResponse> UserAdd(ViewUser view);
        Task<ViewApiResponse> UserGet();
        Task<ViewApiResponse> UserGetSingle(string userId);
        //to be tested
        //Task<ViewApiResponse> UserGetSingleExt(string userId);
        Task<ViewApiResponse> UserDelete(string userId);
        Task<ViewApiResponse> UserUpdate(ViewUser view);
        Task<ViewApiResponse> UserByAccountNoGet(string accountNo);

        Task<ViewApiResponse> RegisterUser(ViewRegisterUser view);
        Task<ViewApiResponse> RegisterUser2(ViewRegisterUser view);
        //Task<ViewApiResponse> RegisterAdmin(ViewRegisterUser view);
        Task<ViewApiResponse> Login(ViewLoginUser view);
        Task<ViewApiResponse> Login2(ViewLoginUser view);
        Task<ViewApiResponse> SetOnboardingToTrue(string accountNo);
        Task<ViewApiResponse> ForgotPassword(ViewForgotPasswordRequest view);
        Task<ViewApiResponse> ResetPassword(ViewResetPasswordRequest view);

        #endregion

        #region Account

        Task<ViewApiResponse> AccountAdd(ViewAccount view);
        Task<ViewApiResponse> AccountGet();
        Task<ViewApiResponse> AccountGetSingle(string accountNo);
        Task<ViewApiResponse> AccountUpdate(ViewAccount view);
        Task<ViewApiResponse> AccountDelete(string accountNo);

        #endregion

        #region PrmType

        Task<ViewApiResponse> PrmTypeAdd(ViewPrmType view);
        Task<ViewApiResponse> PrmTypeGet();
        Task<ViewApiResponse> PrmTypeGetSingle(short typeCode);
        Task<ViewApiResponse> PrmTypeUpdate(ViewPrmType view);
        Task<ViewApiResponse> PrmTypeDelete(short typeCode);

        #endregion

        #region PrmTypesDetail

        Task<ViewApiResponse> PrmTypesDetailAdd(ViewPrmTypesDetail view);
        Task<ViewApiResponse> PrmTypesDetailGet();
        Task<ViewApiResponse> PrmTypesDetailGetSingle(short typeCode, string code);
        Task<ViewApiResponse> PrmTypesDetailUpdate(ViewPrmTypesDetail view);
        Task<ViewApiResponse> PrmTypesDetailDelete(short typeCode, string code);

        #endregion

        #region Device

        Task<ViewApiResponse> DeviceAdd(ViewDevice view);
        Task<ViewApiResponse> DeviceGet();
        Task<ViewApiResponse> DeviceGetSingle(string deviceId);
        Task<ViewApiResponse> DeviceUpdate(ViewDevice view);
        Task<ViewApiResponse> DeviceDelete(string deviceId);

        #endregion

        #region Beneficiary

        Task<ViewApiResponse> BeneficiaryAdd(ViewBeneficiary view);
        Task<ViewApiResponse> BeneficiaryGet();
        Task<ViewApiResponse> BeneficiaryGetSingle(string userId, string beneficiaryAccountNo);
        Task<ViewApiResponse> BeneficiaryUpdate(ViewBeneficiary view);
        Task<ViewApiResponse> BeneficiaryDelete(string userId, string beneficiaryAccountNo);

        #endregion

        #region  LoginHist

        Task<ViewApiResponse> LoginHistAdd(ViewLoginHist view);
        Task<ViewApiResponse> LoginHistGet();
        Task<ViewApiResponse> LoginHistGetSingle(long seqNo);
        Task<ViewApiResponse> LoginHistUpdate(ViewLoginHist view);
        Task<ViewApiResponse> LoginHistDelete(long seqNo);

        #endregion

        #region UserMaintHist

        Task<ViewApiResponse> UserMaintHistAdd(ViewUserMaintHist view);
        Task<ViewApiResponse> UserMaintHistGet();
        Task<ViewApiResponse> UserMaintHistGetSingle(long seqNo);
        Task<ViewApiResponse> UserMaintHistUpdate(ViewUserMaintHist view);
        Task<ViewApiResponse> UserMaintHistDelete(long seqNo);

        #endregion

        #region Otp

        Task<ViewApiResponse> OtpAdd(ViewOtp view);
        Task<ViewApiResponse> OtpGetByAccountNo(string accountNo);
        #endregion

        #region TransferLimit
        Task<ViewApiResponse> TransferLimitAdd(ViewTransferLimit view);
        Task<ViewApiResponse> TransferLimitGet();
        Task<ViewApiResponse> TransferLimitGetSingle(long seqNo);
        Task<ViewApiResponse> TransferLimitUpdate(ViewTransferLimit view);
        Task<ViewApiResponse> TransferLimitDelete(long seqNo);

        #endregion

        #region Util
        Task<ViewApiResponse> SendOtp(string accountNo);
        Task<ViewApiResponse> VerifyOtp(ViewVerifyOtp view);
        Task<ViewApiResponse> VerifyPin(string userId, string pin);
        Task<ViewApiResponse> VerifyPinByAccountNo(string accountNo, string pin);
        Task<ViewApiResponse> VerifyPassword(string account, string password);
        Task<ViewApiResponse> CheckTransferLimit(string accountNo, decimal amount);
        Task<ViewApiResponse> VerifyUserIsRegistered(string accountNo);
        Task<ViewApiResponse> VerifyPinAndTransferLimit(string accountNo, string pin, decimal amount);



        //Task<ViewDetails> RetrieveDetails(string accountNo);
        #endregion

        #region Rest Client
        Task<ViewApiResponse> GetAccountDetailsByAcctNumber(string accountNo);
        Task<ViewApiResponse> AccountCheckByAcctNumber(string accountNo);
        Task<ViewApiResponse> GetAccountBalance(string accountNo);

        #endregion

    }
}
