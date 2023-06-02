using EazyMobile.DAL.Data.Interfaces;
using EazyMobile.DAL.Data.RestClientViewModels;
using EazyMobile.DAL.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EazyMobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EazyMobileController : Controller
    {

        private readonly IEazyMobileRepo repo;
        private readonly ISendGridEmailService emailService;
        private readonly IOTP otp;

        private readonly int BAD_REQUEST = 400;
        private readonly int NOT_FOUND = 404;
        private readonly int OK = 200;
        private readonly int CREATED = 201;
        //private readonly int UNAUTHORIZED = 401;
        private readonly int FORBIDDEN = 403;
        private readonly int INTERNAL_SERVER_ERROR = 500;

        public ViewResponseCode code { get; } = new ViewResponseCode();

        public EazyMobileController(IEazyMobileRepo repo, ISendGridEmailService emailService, IOTP otp)
        {
            this.repo = repo;
            this.emailService = emailService;
            this.otp = otp;
        }


        #region User

        /// <summary>
        /// Endpoint to register a user
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("RegisterUser")]
        public async Task<ActionResult<ViewUser>> RegisterUser(ViewRegisterUser view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.RegisterUser(view);
                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                if (response.Status == code.EXPIRED_OTP_CODE)
                    return StatusCode(BAD_REQUEST, response);

                if (response.Status == code.INVALID_OTP_CODE)
                    return StatusCode(BAD_REQUEST, response);

                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(CREATED, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
               
            }
        }

        [HttpPost("RegisterUser2")]
        public async Task<ActionResult<ViewUser>> RegisterUser2(ViewRegisterUser view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.RegisterUser2(view);
                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                else if (response.Status == code.DUPLICATE_RECORD_CODE)
                    return StatusCode(FORBIDDEN, response);

                else if (response.Status == code.EXPIRED_OTP_CODE)
                    return StatusCode(BAD_REQUEST, response);

                else if (response.Status == code.INVALID_OTP_CODE)
                    return StatusCode(BAD_REQUEST, response);

                else if (response.Status == code.INVALID_PASSWORD_CODE)
                    return StatusCode(BAD_REQUEST, response);

                else if (response.Status == code.INVALID_PIN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                else if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(CREATED, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState.ToString());
                
            }
        }

        //[HttpPost("RegisterAdmin")]
        //public async Task<ActionResult<ViewApiResponse>> RegisterAdmin(ViewRegisterUser view)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await repo.RegisterAdmin(view);
        //        if (response.Status == BAD_REQUEST)
        //            return StatusCode(BAD_REQUEST, response);

        //        return StatusCode(CREATED, response);
        //    }
        //    else
        //    {
        //        return StatusCode(BAD_REQUEST, "Unable to save User");
        //    }
        //}

        /// <summary>
        /// To Login a user
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ActionResult<ViewUser>> Login(ViewLoginUser view)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }

            var response = await repo.Login(view);
            if (response.Status == code.INVALID_ACCOUNT_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }

            //else if (response.Status == code.LOCKED_CODE)
            //{
            //    return StatusCode(BAD_REQUEST, response);
            //}
            else if (response.Status == code.BLOCKED_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.LOCKED_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.BLACKLISTED_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
            {
                return StatusCode(INTERNAL_SERVER_ERROR, response);
            }
            else
            {
                return StatusCode(OK, response);
            }
        }

        /// <summary>
        /// To Login a user
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("Login2")]
        public async Task<ActionResult<ViewUser>> Login2(ViewLoginUser view)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(BAD_REQUEST, ModelState.ToString());
            }

            var response = await repo.Login2(view);
            if (response.Status == code.INVALID_ACCOUNT_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.INACTIVE_ACCOUNT_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.BLOCKED_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.LOCKED_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.BLACKLISTED_CODE)
            {
                return StatusCode(BAD_REQUEST, response);
            }
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
            {
                return StatusCode(INTERNAL_SERVER_ERROR, response);
            }
            else
            {
                return StatusCode(OK, response);
            }
        }

        /// <summary>
        /// Endpoint to return list of all users
        /// </summary>
        /// <returns></returns>
        [HttpGet("UserGet")]
        public async Task<ActionResult<ViewUser>> UserGet()
        {
            var response = await repo.UserGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to return single user record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("UserGetSingle/{userId}")]
        public async Task<ActionResult<ViewUser>> UserGetSingle(string userId)
        {
            var response = await repo.UserGetSingle(userId);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        //[HttpGet("UserGetSingleExt/{userId}")]
        //public async Task<ActionResult<ViewUser>> UserGetSingleExt(string userId)
        //{
        //    var response = await repo.UserGetSingleExt(userId);
        //    if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
        //        return StatusCode(NOT_FOUND, response);
        //    else if (response.Status == code.STATUS_UNKNOWN_CODE)
        //        return StatusCode(BAD_REQUEST, response);

        //    return StatusCode(OK, response);
        //}

        /// <summary>
        /// Endpoint to return use by account no
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        [HttpGet("UserByAccountNoGet/{accountNo}")]
        public async Task<ActionResult<ViewUser>> UserByAccountNoGet(string accountNo)
        {
            var response = await repo.UserByAccountNoGet(accountNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to delete a user record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("UserDelete/{userId}")]
        public async Task<ActionResult<ViewApiResponse>> UserDelete(string userId)
        {
            var response = await repo.UserDelete(userId);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// To set user onboarding to completed
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        [HttpGet("SetOnboardingToTrue/{accountNo}")]
        public async Task<ActionResult<ViewApiResponse>> SetOnboardingToTrue(string accountNo)
        {
            var response = await repo.SetOnboardingToTrue(accountNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ViewOtpResponse>> ForgotPassword(ViewForgotPasswordRequest view)
        {
            if (!ModelState.IsValid)
                return StatusCode(BAD_REQUEST, ModelState);

            var response = await repo.ForgotPassword(view);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status == code.INVALID_PIN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ViewApiResponse>> ResetPassword(ViewResetPasswordRequest view)
        {
            if (!ModelState.IsValid)
                return StatusCode(BAD_REQUEST, ModelState);

            var response = await repo.ResetPassword(view);
            if (response.Status ==  code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status ==  code.EXPIRED_OTP_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status ==  code.INVALID_OTP_CODE)
                return StatusCode(BAD_REQUEST, response);

            return  StatusCode(OK, response);
        }

        #endregion

        #region Account

        /// <summary>
        /// Endpoint to return list of all accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet("AccountGet")]
        public async Task<ActionResult<ViewApiResponse>> AccountGet()
        {
            var response = await repo.AccountGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to return an account record
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        [HttpGet("AccountGetSingle/{accountNo}")]
        public async Task<ActionResult<ViewApiResponse>> AccountGetSingle(string accountNo)
        {
            var response = await repo.AccountGetSingle(accountNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        #endregion

        #region PrmType

        /// <summary>
        /// Endpoint to add a PrmType record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("PrmTypeAdd")]
        public async Task<ActionResult<ViewPrmType>> PrmTypeAdd(ViewPrmType view)
        {

            if (ModelState.IsValid)
            {
                var response = await repo.PrmTypeAdd(view);
                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(CREATED, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }

        }

        /// <summary>
        /// Endpoint to return all PrmType record
        /// </summary>
        /// <returns></returns>
        [HttpGet("PrmTypeGet")]
        public async Task<ActionResult<ViewPrmType>> PrmTypeGet()
        {
            var response = await repo.PrmTypeGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to return single PrmType record
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        [HttpGet("PrmTypeGetSingle/{typeCode}")]
        public async Task<ActionResult<ViewPrmType>> PrmTypeGetSingle(short typeCode)
        {
            var response = await repo.PrmTypeGetSingle(typeCode);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);

        }

        /// <summary>
        /// Endpoint to delete PrmType record
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        [HttpDelete("PrmTypeDelete/{typeCode}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewApiResponse>> PrmTypeDelete(short typeCode)
        {
            var response = await repo.PrmTypeDelete(typeCode);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to update PrmType record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPut("PrmTypeUpdate")]
        public async Task<ActionResult<ViewPrmType>> PrmTypeUpdate(ViewPrmType view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.PrmTypeUpdate(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(NOT_FOUND, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }


        #endregion

        #region PrmTypesDetail

        /// <summary>
        /// Endpoint to save a PrmTypeDetail record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("PrmTypesDetailAdd")]
        public async Task<ActionResult<ViewPrmTypesDetail>> PrmTypesDetailAdd(ViewPrmTypesDetail view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.PrmTypesDetailAdd(view);
                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }

        /// <summary>
        /// Endpoint to return all PrmTypeDetail record
        /// </summary>
        /// <returns></returns>
        [HttpGet("PrmTypesDetailGet")]
        public async Task<ActionResult<ViewPrmTypesDetail>> PrmTypesDetailGet()
        {
            var response = await repo.PrmTypesDetailGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to return single PrmTypesDetail record
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("PrmTypesDetailGetSingle/{typeCode}/{code}")]
        public async Task<ActionResult<ViewPrmTypesDetail>> PrmTypesDetailGetSingle(short typeCode, string tCode)
        {
            var response = await repo.PrmTypesDetailGetSingle(typeCode, tCode);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to delete PrmTypesDetail record
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpDelete("PrmTypesDetailDelete/{typeCode}/{code}")]
        public async Task<ActionResult<ViewApiResponse>> PrmTypesDetailDelete(short typeCode, string tCode)
        {
            var response = await repo.PrmTypesDetailDelete(typeCode, tCode);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to update PrmTypesDetail record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPut("PrmTypesDetailUpdate")]
        public async Task<ActionResult<ViewPrmTypesDetail>> PrmTypesDetailUpdate(ViewPrmTypesDetail view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.PrmTypesDetailUpdate(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(NOT_FOUND, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, $"Unable to update record");
            }
        }


        #endregion

        #region Device

        /// <summary>
        /// Endpoint to add a device record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("DeviceAdd")]
        public async Task<ActionResult<ViewDevice>> DeviceAdd(ViewDevice view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.DeviceAdd(view);
                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }

        /// <summary>
        /// Endpoint to return all device record
        /// </summary>
        /// <returns></returns>
        [HttpGet("DeviceGet")]
        public async Task<ActionResult<ViewDevice>> DeviceGet()
        {
            var response = await repo.DeviceGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to return single Device record
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("DeviceGetSingle/{deviceId}")]
        public async Task<ActionResult<ViewDevice>> DeviceGetSingle(string deviceId)
        {
            var response = await repo.DeviceGetSingle(deviceId);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to delete a device record
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpDelete("DeviceDelete/{deviceId}")]
        public async Task<ActionResult<ViewApiResponse>> DeviceDelete(string deviceId)
        {
            var response = await repo.DeviceDelete(deviceId);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }


        #endregion

        #region Beneficiary

        /// <summary>
        /// Endpoint to add a beneficiary record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("BeneficiaryAdd")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewBeneficiary>> BeneficiaryAdd(ViewBeneficiary view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.BeneficiaryAdd(view);
                if (response.Status == code.STATUS_UNKNOWN_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(CREATED, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }

        /// <summary>
        /// Endpoint to return all beneficiary record
        /// </summary>
        /// <returns></returns>
        [HttpGet("BeneficiaryGet")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewBeneficiary>> BeneficiaryGet()
        {
            var response = await repo.BeneficiaryGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to return single beneficiary record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="beneficiaryAccountNo"></param>
        /// <returns></returns>
        [HttpGet("BeneficiaryGetSingle/{userId}/{beneficiaryAccountNo}")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewBeneficiary>> BeneficiaryGetSingle(string userId, string beneficiaryAccountNo)
        {
            var response = await repo.BeneficiaryGetSingle(userId, beneficiaryAccountNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }


        /// <summary>
        /// Endpoint to delete a beneficiary record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="beneficiaryAccountNo"></param>
        /// <returns></returns>
        [HttpDelete("BeneficiaryDelete/{userId}/{beneficiaryAccountNo}")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewApiResponse>> BeneficiaryDelete(string userId, string beneficiaryAccountNo)
        {
            var response = await repo.BeneficiaryDelete(userId, beneficiaryAccountNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to update Beneficiary record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPut("BeneficiaryUpdate")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewBeneficiary>> BeneficiaryUpdate(ViewBeneficiary view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.BeneficiaryUpdate(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(NOT_FOUND, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }


        #endregion

        #region LoginHist

        /// <summary>
        /// Endpoint to add LoginHist record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("LoginHistAdd")]
        public async Task<ActionResult<ViewLoginHist>> LoginHistAdd(ViewLoginHist view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.LoginHistAdd(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(CREATED, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }

        /// <summary>
        /// Endpoint to return all LoginHist record
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoginHistGet")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewLoginHist>> LoginHistGet()
        {
            var response = await repo.LoginHistGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        ///  Endpoint to return single LoginHist record
        /// </summary>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        [HttpGet("LoginHistGetSingle/{seqNo}")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewLoginHist>> LoginHistGetSingle(long seqNo)
        {
            var response = await repo.LoginHistGetSingle(seqNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }


        /// <summary>
        /// Endpoint to delete a LoginHist record
        /// </summary>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        [HttpDelete("LoginHistDelete/{seqNo}")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewApiResponse>> LoginHistDelete(long seqNo)
        {
            var response = await repo.LoginHistDelete(seqNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to update LoginHist record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPut("LoginHistUpdate")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewLoginHist>> LoginHistUpdate(ViewLoginHist view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.LoginHistUpdate(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(NOT_FOUND, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }


        #endregion

        #region UserMaintHist

        /// <summary>
        /// Endpoint to add UserMaintHist record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("UserMaintHistAdd")]
        public async Task<ActionResult<ViewUserMaintHist>> UserMaintHistAdd(ViewUserMaintHist view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.UserMaintHistAdd(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(BAD_REQUEST, response);

                return StatusCode(CREATED, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }

        /// <summary>
        /// Endpoint to return all UserMaintHist record
        /// </summary>
        /// <returns></returns>
        [HttpGet("UserMaintHistGet")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewUserMaintHist>> UserMaintHistGet()
        {
            var response = await repo.UserMaintHistGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        ///  Endpoint to return single UserMaintHist record
        /// </summary>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        [HttpGet("UserMaintHistGetSingle/{seqNo}")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewUserMaintHist>> UserMaintHistGetSingle(long seqNo)
        {
            var response = await repo.UserMaintHistGetSingle(seqNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }


        /// <summary>
        /// Endpoint to delete a UserMaintHist record
        /// </summary>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        [HttpDelete("UserMaintHistDelete/{seqNo}")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewApiResponse>> UserMaintHistDelete(long seqNo)
        {
            var response = await repo.UserMaintHistDelete(seqNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to update UserMaintHist record
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPut("UserMaintHistUpdate")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Role_User")]
        public async Task<ActionResult<ViewUserMaintHist>> UserMaintHistUpdate(ViewUserMaintHist view)
        {
            if (ModelState.IsValid)
            {
                var response = await repo.UserMaintHistUpdate(view);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return StatusCode(NOT_FOUND, response);

                return StatusCode(OK, response);
            }
            else
            {
                return StatusCode(BAD_REQUEST, ModelState);
            }
        }

        #endregion

        #region TransferLimit

        [HttpPost("TransferLimitAdd")]
        public async Task<ActionResult<ViewTransferLimit>> TransferLimitAdd(ViewTransferLimit view)
        {
            if (!ModelState.IsValid)
                return StatusCode(BAD_REQUEST, ModelState);

            var response = await repo.TransferLimitAdd(view);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("TransferLimitGet")]
        public async Task<ActionResult<ViewTransferLimit>> TransferLimitGet()
        {
            var response = await repo.TransferLimitGet();
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("TransferLimitGetSingle/{seqNo}")]
        public async Task<ActionResult<ViewTransferLimit>> TransferLimitGetSingle(long seqNo)
        {
            var response = await repo.TransferLimitGetSingle(seqNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpPut("TransferLimitUpdate")]
        public async Task<ActionResult<ViewTransferLimit>> TransferLimitUpdate(ViewTransferLimit view)
        {
            if (!ModelState.IsValid)
                return StatusCode(BAD_REQUEST, ModelState);

            var response = await repo.TransferLimitUpdate(view);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpDelete("TransferLimitDelete/{seqNo}")]
        public async Task<ActionResult<ViewApiResponse>> TransferLimitDelete(long seqNo)
        {
            var response = await repo.TransferLimitDelete(seqNo);
            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);

            else if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        #endregion

        #region Util

        /// <summary>
        /// Endpoint to send OTP 
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        [HttpGet("SendOtp")]
        public async Task<ActionResult<ViewApiResponse>> SendOtp(string accountNo)
        {
            var response = await repo.SendOtp(accountNo);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);
            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(400, response);

            return StatusCode(OK, response);
        }

        /// <summary>
        /// Endpoint to verify OTP
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost("VerifyOtp")]
        public async Task<ActionResult<ViewVerifyOtp>> VerifyOtp(ViewVerifyOtp view)
        {
            var response = await repo.VerifyOtp(view);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("VerifyPin/{userId}/{pin}")]
        public async Task<ActionResult<ViewApiResponse>> VerifyPin(string userId, string pin)
        {
            var response = await repo.VerifyPin(userId, pin);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);
            else if (response.Status == code.INVALID_PIN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("VerifyPinByAccountNo/{accountNo}/{pin}")]
        public async Task<ActionResult<ViewApiResponse>> VerifyPinByAccountNo(string accountNo, string pin)
        {
            var response = await repo.VerifyPinByAccountNo(accountNo, pin);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);
            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(NOT_FOUND, response);
            else if (response.Status == code.INVALID_PIN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("VerifyPassword/{account}/{password}")]
        public async Task<ActionResult<ViewApiResponse>> VerifyPassword(string account, string password)
        {
            var response = await repo.VerifyPassword(account, password);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return StatusCode(BAD_REQUEST, response);
            else if (response.Status == code.INVALID_PASSWORD_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("VerifyUserIsRegistered/{accountNo}")]
        public async Task<ActionResult<ViewApiResponse>> VerifyUserIsRegistered(string accountNo)
        {
            var response = await repo.VerifyUserIsRegistered(accountNo);
            if (response.Status == code.DUPLICATE_RECORD_CODE)
                return Ok(response);

            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return NotFound(response);

            return BadRequest(response);
        }

        [HttpGet("CheckTransferLimit/{accountNo}/{amount}")]
        public async Task<ActionResult<ViewApiResponse>> CheckTransferLimit(string accountNo, decimal amount)
        {
            var response = await repo.CheckTransferLimit(accountNo, amount);
            if (response.Status == code.TRANSFER_LIMIT_EXCEEDED_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }

        [HttpGet("VerifyPinAndTransferLimit/{accountNo}/{pin}/{amount}")]
        public async Task<ActionResult<ViewApiResponse>> VerifyPinAndTransferLimit(string accountNo, string pin, decimal amount)
        {
            var response = await repo.VerifyPinAndTransferLimit(accountNo, pin, amount);
            if (response.Status == code.TRANSFER_LIMIT_EXCEEDED_CODE)
                return StatusCode(BAD_REQUEST, response);
            else if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return StatusCode(INTERNAL_SERVER_ERROR, response);
            else if (response.Status == code.INVALID_PIN_CODE)
                return StatusCode(BAD_REQUEST, response);

            return StatusCode(OK, response);
        }


        //to be tested
        //[HttpGet("RetrieveDetails/{accountNo}")]
        //public async Task<ActionResult<ViewDetails>> RetrieveDetails(string accountNo)
        //{
        //    var response = await repo.RetrieveDetails(accountNo);
        //    return StatusCode(200, response);
        //}

        #endregion

        #region Rest Client

        [HttpGet("GetAccountDetailsByAcctNumber")]
        public async Task<ActionResult<ViewVerifyAccountDetails>> GetAccountDetailsByAcctNumber(string accountNo)
        {
            var response = await repo.GetAccountDetailsByAcctNumber(accountNo);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return BadRequest(response);

            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("AccountCheckByAcctNumber")]
        public async Task<ActionResult<ViewVerifyAccountDetails>> AccountCheckByAcctNumber(string accountNo)
        {
            var response = await repo.AccountCheckByAcctNumber(accountNo);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return BadRequest(response);

            else if (response.Status == code.PRESENT_NOT_ACTIVATED_RECORD_CODE)
                return StatusCode(BAD_REQUEST, response);

            else if (response.Status == code.DUPLICATE_RECORD_CODE)
                return StatusCode(FORBIDDEN, response);

            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetAccountBalance")]
        public async Task<ActionResult<ViewAccountBalance>> GetAccountBalance(string accountNo)
        {
            var response = await repo.GetAccountBalance(accountNo);
            if (response.Status == code.STATUS_UNKNOWN_CODE)
                return BadRequest(response);

            if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return BadRequest(response);

            return Ok(response);
        }

        #endregion

    }
}
