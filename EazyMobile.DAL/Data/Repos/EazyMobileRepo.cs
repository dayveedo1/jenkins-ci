using EazyMobile.DAL.Data.Interfaces;
using EazyMobile.DAL.Data.Models;
using EazyMobile.DAL.Data.RestClientViewModels;
using EazyMobile.DAL.Data.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace EazyMobile.DAL.Data.Repos
{
    public class EazyMobileRepo : IEazyMobileRepo
    {

        //private readonly int BAD_REQUEST = 400;
        //private readonly int NOT_FOUND = 404;
        //private readonly int OK = 200;
        //private readonly int CREATED = 201;

        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly EazyMobileContext context;

        private User user;

        private readonly ISendGridEmailService emailService;

        //public IHttpClientFactory httpClient;

        public ViewAccountDetails ViewAccountDetails { get; set; }
        public ViewResponseCode code { get; } = new ViewResponseCode();


        public EazyMobileRepo(Microsoft.AspNetCore.Identity.UserManager<User> userManager,
                                Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager,
                                IConfiguration configuration,
                                EazyMobileContext context,
                                ISendGridEmailService emailService)//, 
                                                                   //IHttpClientFactory httpClient)

        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.context = context;
            this.emailService = emailService;
            //this.httpClient = httpClient;
        }

        #region User
        public Task<ViewApiResponse> UserAdd(ViewUser view)
        {
            throw new NotImplementedException();
        }

        //public async Task<ViewApiResponse> RegisterUser(ViewRegisterUser view)
        //{

        //    try
        //    {
        //        var response = await GetAccountDetailsByAcctNumber(view.AccountNo);
        //        if (response.Status == code.STATUS_UNKNOWN_CODE)
        //            return response;

        //        var data = (ViewAccountDetails)response.Data;
        //        if (data == null)
        //            return new ViewApiResponse
        //            {
        //                Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
        //                Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
        //                Data = $"Enter valid account"
        //            };

        //        bool isAccountActivatedForMobile = IsAccountActivatedForMobile(data.AccountNumber);
        //        if (isAccountActivatedForMobile)
        //            return new ViewApiResponse
        //            {
        //                Status = code.DUPLICATE_RECORD_CODE,
        //                Message = code.DUPLICATE_RECORD_MSG,
        //                Data = $"{data.AccountNumber} is registered for Mobile Banking"
        //            };

        //        //var getAccount = await AccountGetSingle(view.AccountNo);
        //        //if (getAccount.Status == BAD_REQUEST)
        //        //    return new ViewApiResponse
        //        //    {
        //        //        Status = BAD_REQUEST,
        //        //        Message = "Not Found",
        //        //        Data = $"Account No: {view.AccountNo} Not Found"
        //        //    };

        //        //var account = (Account)getAccount.Data;
        //        var account = new Account
        //        {
        //            AccountNo = data.AccountNumber,
        //            Show = true
        //        };

        //        await context.Accounts.AddAsync(account);
        //        //await context.SaveChangesAsync();

        //        var user = new User()
        //        {
        //            UserName = data.Email,
        //            Email =  data.Email, //view.Email,
        //            Pin = view.Pin,
        //            Password = view.Password,
        //            Block = false,
        //            Suspend = false,
        //            Active = false,
        //            DateCreated = DateTime.Now,
        //            OnBoardingComplete = false,
        //            EmailConfirmed = true,
        //            TwoFactorEnabled = true,
        //            Bvn = data.BVN,
        //            PhoneNumber = data.MainPhone
        //        };

        //        var result = await userManager.CreateAsync(user, view.Password);
        //        if (result.Succeeded)
        //        {
        //            //var generatedOtp = GenerateOtp();
        //            //user.Otp = generatedOtp;
        //            //user.OtpExpiration = DateTime.Now.AddMinutes(2);
        //            //await context.SaveChangesAsync();

        //            //await emailService.SendEmailAsync(user.Email, "OTP", $"One Time Password: {generatedOtp}");

        //            var roles = roleManager.FindByNameAsync("ROLE_USER").Result;
        //            await userManager.AddToRoleAsync(user, roles.Name);

        //            account.UserId = user.Id;
        //            await context.SaveChangesAsync();

        //            var getUser =  await UserGetSingle(user.Id);

        //            return new ViewApiResponse
        //            {
        //                Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
        //                Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
        //                Data = getUser //user
        //            };
        //        }
        //        else
        //        {
        //            return new ViewApiResponse
        //            {
        //                Status = code.STATUS_UNKNOWN_CODE,
        //                Message = code.STATUS_UNKNOWN_MSG,
        //                Data = result.Errors.FirstOrDefault()
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ViewApiResponse
        //        {
        //            Status = code.STATUS_UNKNOWN_CODE,
        //            Message = code.STATUS_UNKNOWN_MSG,
        //            Data = ex.Message
        //        };
        //    }



        //}

        public async Task<ViewApiResponse> RegisterUser(ViewRegisterUser view)
        {
            try
            {
                bool isAccountRegistered = IsAccountActivatedForMobile(view.AccountNo);
                if (isAccountRegistered)
                    return new ViewApiResponse
                    {
                        Status = code.DUPLICATE_RECORD_CODE,
                        Message = code.DUPLICATE_RECORD_MSG,
                        Data = $"Account Registered"
                    };

                //var details = await context.Otps.Where(x => x.AccountNo == view.AccountNo).SingleOrDefaultAsync();
                var fetchAccountRecordFromOtp = await OtpGetByAccountNo(view.AccountNo);
                if (fetchAccountRecordFromOtp.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_OTP_CODE,
                        Message = code.INVALID_OTP_MSG,
                        Data = $"Invalid OTP"
                    };

                var details = (Otp)fetchAccountRecordFromOtp.Data;



                var viewVerifyOtp = new ViewVerifyOtp
                {
                    AccountNo = view.AccountNo,
                    Otp = view.Otp
                };

                var verifyOtp = await VerifyOtp(viewVerifyOtp);
                if (verifyOtp.Status == code.EXPIRED_OTP_CODE)
                    return verifyOtp;

                if (verifyOtp.Status == code.INVALID_OTP_CODE)
                    return verifyOtp;

                if (verifyOtp.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
                {
                    var retrievedDetails = await RetrieveDetails(view.AccountNo);

                    var account = new Account
                    {
                        AccountNo = details.AccountNo,
                        Show = true,
                        AccountName = retrievedDetails.AccountName,
                        AccountType = retrievedDetails.AccountType,
                        AccountStatus = (short)retrievedDetails.AccountStatus,
                        BranchCode = retrievedDetails.BranchCode,
                        BranchName = retrievedDetails.BranchName
                    };

                    await context.Accounts.AddAsync(account);
                    //await context.SaveChangesAsync();

                    //var bvn = await RetrieveBvn(view.AccountNo);
                    var hashPassword = HashPassword(view.Password);
                    var hashPin = HashPin(view.Pin);

                    var filterUsername = retrievedDetails.AccountName.Split(' ');
                    var capitalizeUserName = CapitalizeFirstLetter(filterUsername[0].ToLower());

                    var user = new User()
                    {
                        UserName = capitalizeUserName,     //filterUsername[0].ToLower(),details.Email,
                        Email = details.Email,
                        Pin = hashPin,      //view.Pin, 
                        Password = hashPassword,
                        UserStatus = code.STATUS_ACTIVE,
                        DateCreated = DateTime.Now,
                        OnBoardingComplete = false,
                        EmailConfirmed = true,
                        TwoFactorEnabled = true,
                        Bvn = retrievedDetails.Bvn,//bvn,
                        PhoneNumber = details.PhoneNumber,
                        CustomerId = retrievedDetails.CustomerId,
                        FullName = retrievedDetails.AccountName
                    };

                    var result = await userManager.CreateAsync(user, view.Password);
                    if (result.Succeeded)
                    {

                        var roles = roleManager.FindByNameAsync("ROLE_USER").Result;
                        await userManager.AddToRoleAsync(user, roles.Name);

                        account.UserId = user.Id;

                        var transferLimit = new TransferLimit
                        {
                            UserId = user.Id,
                            PreviousLimit = 0,
                            CurrentLimit = 100000,
                            DateTimeModified = DateTime.Now
                        };

                        context.TransferLimits.Add(transferLimit);

                        //New Addition update to be tested
                        details.Status = "Used OTP";
                        await context.SaveChangesAsync();

                        var getUser = await UserGetSingle(user.Id);
                        return getUser;

                    }
                    else
                    {
                        return new ViewApiResponse
                        {
                            Status = code.STATUS_UNKNOWN_CODE,
                            Message = code.STATUS_UNKNOWN_MSG,
                            Data = result.Errors.FirstOrDefault()
                        };
                    }
                }
                else
                {
                    return new ViewApiResponse
                    {
                        Status = code.STATUS_UNKNOWN_CODE,
                        Message = code.STATUS_UNKNOWN_MSG,
                        Data = $"Invalid OTP. Unsuccessful"
                    };
                }

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> RegisterUser2(ViewRegisterUser view)
        {
            try
            {
                AccountStatusCheck accountStatusCheck = await IsAccountPresentOnMobileDB(view.AccountNo);
                if (accountStatusCheck == AccountStatusCheck.PresentActivated)
                    return new ViewApiResponse
                    {
                        Status = code.DUPLICATE_RECORD_CODE,
                        Message = code.DUPLICATE_RECORD_MSG,
                        Data = $"Account Registered"
                    };

                else if (accountStatusCheck == AccountStatusCheck.PresentNotActivated)
                {
                    var isPasswordVerified = await VerifyPassword(view.AccountNo, view.Password);
                    var isPinVerified = await VerifyPinByAccountNo(view.AccountNo, view.Pin);

                    if (isPasswordVerified.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
                    {
                        if (isPinVerified.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
                        {
                            var fetchAccountNo = await AccountGetSingle(view.AccountNo);

                            var result = (Account)fetchAccountNo.Data;
                            result.AccountStatus = 1;

                            await context.SaveChangesAsync();
                            var getUser = await UserByAccountNoGet(view.AccountNo);
                            return getUser;
                        }
                        else
                        {
                            return isPinVerified;
                        }
                    }
                    else
                    {
                        return isPasswordVerified;

                    }
                }
                else
                {
                    var fetchAccountRecordFromOtp = await OtpGetByAccountNo(view.AccountNo);
                    if (fetchAccountRecordFromOtp.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                        return new ViewApiResponse
                        {
                            Status = code.INVALID_OTP_CODE,
                            Message = code.INVALID_OTP_MSG,
                            Data = $"Invalid OTP"
                        };

                    var details = (Otp)fetchAccountRecordFromOtp.Data;

                    var viewVerifyOtp = new ViewVerifyOtp
                    {
                        AccountNo = view.AccountNo,
                        Otp = view.Otp
                    };

                    var verifyOtp = await VerifyOtp(viewVerifyOtp);
                    if (verifyOtp.Status == code.EXPIRED_OTP_CODE)
                        return verifyOtp;

                    if (verifyOtp.Status == code.INVALID_OTP_CODE)
                        return verifyOtp;

                    if (verifyOtp.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
                    {
                        var retrievedAccountDetails = await RetrieveDetails(view.AccountNo);
                        var retrievedAccountsList = await GetAccountDetailsByCustomerId(retrievedAccountDetails.CustomerId);

                        if (retrievedAccountsList.Status != code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
                            return new ViewApiResponse
                            {
                                Status = code.STATUS_UNKNOWN_CODE,
                                Message = code.STATUS_UNKNOWN_MSG,
                                Data = "Error fetching the account list"
                            };


                        var accountsList = (List<ViewAccountDetails>)retrievedAccountsList.Data;


                        List<Account> newAccountList = new();

                        foreach (var item in accountsList)
                        {
                            var account = new Account
                            {
                                AccountNo = item.AccountNumber,
                                Show = true,
                                AccountName = item.AccountName,
                                AccountType = item.BankAccountType,
                                AccountStatus = (short)(item.AccountNumber == view.AccountNo ? 1 : 0),
                                BranchCode = item.BranchSolid,
                                BranchName = item.BranchName
                            };

                            newAccountList.Add(account);
                        }

                        //await context.Accounts.AddAsync(account);
                        await context.Accounts.AddRangeAsync(newAccountList);
                        //await context.SaveChangesAsync();

                        //var bvn = await RetrieveBvn(view.AccountNo);
                        var hashPassword = HashPassword(view.Password);
                        var hashPin = HashPin(view.Pin);

                        var filterUsername = retrievedAccountDetails.AccountName.Split(' ');
                        var capitalizeUserName = CapitalizeFirstLetter(filterUsername[0].ToLower());

                        var user = new User()
                        {
                            UserName = capitalizeUserName,     //filterUsername[0].ToLower(),details.Email,
                            Email = details.Email,
                            Pin = hashPin,      //view.Pin, 
                            Password = hashPassword,
                            UserStatus = code.STATUS_ACTIVE,
                            DateCreated = DateTime.Now,
                            OnBoardingComplete = false,
                            EmailConfirmed = true,
                            TwoFactorEnabled = true,
                            Bvn = retrievedAccountDetails.Bvn,//bvn,
                            PhoneNumber = details.PhoneNumber,
                            CustomerId = retrievedAccountDetails.CustomerId,
                            FullName = retrievedAccountDetails.AccountName
                        };

                        var result = await userManager.CreateAsync(user, view.Password);
                        if (result.Succeeded)
                        {

                            var roles = roleManager.FindByNameAsync("ROLE_USER").Result;
                            await userManager.AddToRoleAsync(user, roles.Name);

                            foreach (var item in newAccountList)
                            {
                                item.UserId = user.Id;
                            }

                            var transferLimit = new TransferLimit
                            {
                                UserId = user.Id,
                                PreviousLimit = 0,
                                CurrentLimit = 100000,
                                DateTimeModified = DateTime.Now
                            };

                            context.TransferLimits.Add(transferLimit);

                            //New Addition update to be tested
                            details.Status = "Used OTP";
                            await context.SaveChangesAsync();

                            var getUser = await UserGetSingle(user.Id);
                            return getUser;

                        }
                        else
                        {
                            return new ViewApiResponse
                            {
                                Status = code.STATUS_UNKNOWN_CODE,
                                Message = code.STATUS_UNKNOWN_MSG,
                                Data = result.Errors.FirstOrDefault()
                            };
                        }
                    }
                    else
                    {
                        return new ViewApiResponse
                        {
                            Status = code.STATUS_UNKNOWN_CODE,
                            Message = code.STATUS_UNKNOWN_MSG,
                            Data = $"Invalid OTP. Unsuccessful"
                        };

                    }
                }

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        //public async Task<ViewApiResponse> RegisterUser(ViewRegisterUser view)
        //{

        //    try
        //    {
        //        var getAccount = await AccountGetSingle(view.AccountNo);
        //        if (getAccount.Status == BAD_REQUEST)
        //            return new ViewApiResponse
        //            {
        //                Status = BAD_REQUEST,
        //                Message = "Not Found",
        //                Data = $"Account No: {view.AccountNo} Not Found"
        //            };

        //        var account = (Account)getAccount.Data;

        //        var user = new User()
        //        {
        //            UserName = view.Email,
        //            Email = view.Email,
        //            Pin = view.Pin,
        //            Password = view.Password,
        //            Block = false,
        //            Suspend = false,
        //            Active = false,
        //            DateCreated = DateTime.Now.Date,
        //            OnBoardingComplete = false,
        //            EmailConfirmed = true,
        //            TwoFactorEnabled = true
        //        };

        //        var result = await userManager.CreateAsync(user, view.Password);
        //        if (result.Succeeded)
        //        {
        //            //var generatedOtp = GenerateOtp();
        //            //user.Otp = generatedOtp;
        //            //user.OtpExpiration = DateTime.Now.AddMinutes(2);
        //            //await context.SaveChangesAsync();

        //            //await emailService.SendEmailAsync(user.Email, "OTP", $"One Time Password: {generatedOtp}");

        //            var roles = roleManager.FindByNameAsync("ROLE_USER").Result;
        //            await userManager.AddToRoleAsync(user, roles.Name);

        //            account.UserId = user.Id;
        //            await context.SaveChangesAsync();

        //            return new ViewApiResponse
        //            {
        //                Status = CREATED,
        //                Message = "Success",
        //                Data = user
        //            };
        //        }
        //        else
        //        {
        //            return new ViewApiResponse
        //            {
        //                Status = BAD_REQUEST,
        //                Message = "Failed",
        //                Data = result.Errors.FirstOrDefault()
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ViewApiResponse
        //        {
        //            Status = 400,
        //            Message = "Failed",
        //            Data = ex.Message
        //        };
        //    }



        //}

        //public async Task<ViewApiResponse> RegisterAdmin(ViewRegisterUser view)
        //{

        //    try
        //    {
        //        var getAccount = await AccountGetSingle(view.AccountNo);
        //        if (getAccount.Status == BAD_REQUEST)
        //            return new ViewApiResponse
        //            {
        //                Status = BAD_REQUEST,
        //                Message = "Not Found",
        //                Data = $"Account No: {view.AccountNo} Not Found"
        //            };

        //        var account = (Account)getAccount.Data;

        //        var user = new User()
        //        {
        //            UserName = view.Email,
        //            Email = view.Email,
        //            Pin = view.Pin,
        //            Password = view.Password,
        //            Block = false,
        //            Suspend = false,
        //            Active = true,
        //            DateCreated = DateTime.Now.Date,
        //            OnBoardingComplete = false,

        //        };

        //        var result = await userManager.CreateAsync(user, view.Password);
        //        if (result.Succeeded)
        //        {
        //            var roles = roleManager.FindByNameAsync("ROLE_USER").Result;
        //            await userManager.AddToRoleAsync(user, roles.Name);
        //            var roleAdmin = roleManager.FindByNameAsync("ROLE_ADMIN").Result;
        //            await userManager.AddToRoleAsync(user, roleAdmin.Name);

        //            account.UserId = user.Id;
        //            await context.SaveChangesAsync();

        //            return new ViewApiResponse
        //            {
        //                Status = CREATED,
        //                Message = "Success",
        //                Data = user
        //            };
        //        }
        //        else
        //        {
        //            return new ViewApiResponse
        //            {
        //                Status = BAD_REQUEST,
        //                Message = "Failed",
        //                Data = result.Errors.FirstOrDefault()
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ViewApiResponse
        //        {
        //            Status = 400,
        //            Message = "Failed",
        //            Data = ex.Message
        //        };
        //    }



        //}

        public async Task<ViewApiResponse> Login(ViewLoginUser view)
        {
            if (view.AccountNo == null || view.Password == null)
            {
                return new ViewApiResponse
                {
                    Status = code.INVALID_ACCOUNT_CODE,
                    Message = code.INVALID_ACCOUNT_MSG,
                    Data = $"Invalid account number or password"
                };
            }

            try
            {
                var validate = await ValidateUser(view);

                if (validate.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"Invalid account number or password 01"
                    };
                }


                else if (validate.Status == code.INVALID_PASSWORD_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_ACCOUNT_CODE,
                        Message = code.INVALID_ACCOUNT_MSG,
                        Data = $"Invalid account number or password 02"
                    };

                }

                else if (validate.Status == code.BLACKLISTED_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.BLACKLISTED_CODE,
                        Message = code.BLACKLISTED_MSG,
                        Data = $"Account blacklisted"
                    };
                }

                else if (validate.Status == code.BLOCKED_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.BLOCKED_CODE,
                        Message = code.BLOCKED_MSG,
                        Data = $"Account blocked"
                    };
                }

                else if (validate.Status == code.LOCKED_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.LOCKED_CODE,
                        Message = code.LOCKED_MSG,
                        Data = "Please change password"
                    };
                }

                else if (validate.Status == code.STATUS_UNKNOWN_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.STATUS_UNKNOWN_CODE,
                        Message = code.STATUS_UNKNOWN_MSG,
                        Data = $"Internal Server Error"
                    };
                }

                var token = await CreateToken();

                //var viewUser = new ViewUser
                //{
                //    UserId = user.UserId,
                //    //Password = user.Password,
                //    DateCreated = user.DateCreated,
                //    Bvn = user.Bvn,
                //    UserStatus = user.UserStatus,
                //    BlockDate = user.BlockDate,
                //    SuspendDate = user.SuspendDate,
                //    BlockedBy = user.BlockedBy,
                //    //PasswordRetryCount = user.PasswordRetryCount,
                //    //Pin = user.Pin,
                //    OnBoardingComplete = user.OnBoardingComplete,
                //    Email = user.Email,
                //    PhoneNumber = user.PhoneNumber,
                //    FullName = user.FullName,

                //    Accounts = user.Accounts.Select(a => new ViewAccount
                //    {
                //        AccountNo = a.AccountNo,
                //        SeqNo = a.SeqNo,
                //        Show = a.Show,
                //        UserId = a.UserId,
                //        AccountName = a.AccountName,
                //        AccountType = a.AccountType,
                //        BranchCode = a.BranchCode,
                //        BranchName = a.BranchName
                //    }).ToList(),

                //    UserMaintHists = user.UserMaintHists.Select(x => new ViewUserMaintHist
                //    {
                //        SeqNo = x.SeqNo,
                //        UserId = x.UserId,
                //        BlockOldValue = x.BlockOldValue,
                //        BlockNewValue = x.BlockNewValue,
                //        SuspendOldValue = x.SuspendOldValue,
                //        SuspendNewValue = x.SuspendNewValue,
                //        ActivityDate = x.ActivityDate,
                //        MaintFlagCode = x.MaintFlagCode,
                //        BlockedBy = x.BlockedBy,
                //    }).ToList(),

                //    Devices = user.Devices.Select(x => new ViewDevice
                //    {
                //        UserId = x.UserId,
                //        DeviceId = x.DeviceId,
                //    }).ToList(),

                //    Beneficiaries = user.Beneficiaries.Select(x => new ViewBeneficiary
                //    {
                //        UserId = x.UserId,
                //        BeneficiaryAccountNo = x.BeneficiaryAccountNo,
                //        BeneficiaryAccountName = x.BeneficiaryAccountName,
                //        BeneficiaryBank = x.BeneficiaryBank,
                //    }).ToList(),

                //    TransferLimit = user.TransferLimit,

                //    EmailConfirmed = user.EmailConfirmed,
                //    Id = user.Id,
                //    //LockoutEnd = user.LockoutEnd,
                //    TwoFactorEnabled = user.TwoFactorEnabled,
                //    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                //    //ConcurrencyStamp = user.ConcurrencyStamp,
                //    //SecurityStamp = user.SecurityStamp,
                //    //PasswordHash = user.PasswordHash,
                //    //NormalizedEmail = user.NormalizedEmail,
                //    //NormalizedUserName = user.NormalizedUserName,
                //    UserName = user.UserName,
                //    //LockoutEnabled = user.LockoutEnabled,
                //    //AccessFailedCount = user.AccessFailedCount
                //};

                var viewUser = UserByAccountNoGet(view.AccountNo).Result.Data;

                var response = new ViewLoginResponse
                {
                    user = (ViewUser)viewUser,
                    token = token
                };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> Login2(ViewLoginUser view)
        {
            if (view.AccountNo == null || view.Password == null)
            {
                return new ViewApiResponse
                {
                    Status = code.INVALID_ACCOUNT_CODE,
                    Message = code.INVALID_ACCOUNT_MSG,
                    Data = $"Invalid account number or password"
                };
            }

            try
            {
                var validate = await ValidateUser2(view);

                if (validate.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"Invalid account number or password 01"
                    };
                }


                else if (validate.Status == code.INVALID_PASSWORD_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_ACCOUNT_CODE,
                        Message = code.INVALID_ACCOUNT_MSG,
                        Data = $"Invalid account number or password 02"
                    };

                }

                else if (validate.Status == code.INVALID_ACCOUNT_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_ACCOUNT_CODE,
                        Message = code.INVALID_ACCOUNT_MSG,
                        Data = $"Invalid account number or password 02"
                    };

                }

                else if (validate.Status == code.INACTIVE_ACCOUNT_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.INACTIVE_ACCOUNT_CODE,
                        Message = code.INACTIVE_ACCOUNT_MSG,
                        Data = $"Inactive account number or invalid password"
                    };

                }

                else if (validate.Status == code.BLACKLISTED_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.BLACKLISTED_CODE,
                        Message = code.BLACKLISTED_MSG,
                        Data = $"Account blacklisted"
                    };
                }

                else if (validate.Status == code.BLOCKED_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.BLOCKED_CODE,
                        Message = code.BLOCKED_MSG,
                        Data = $"Account blocked"
                    };
                }

                else if (validate.Status == code.LOCKED_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.LOCKED_CODE,
                        Message = code.LOCKED_MSG,
                        Data = "Please change password"
                    };
                }

                else if (validate.Status == code.STATUS_UNKNOWN_CODE)
                {
                    return new ViewApiResponse
                    {
                        Status = code.STATUS_UNKNOWN_CODE,
                        Message = code.STATUS_UNKNOWN_MSG,
                        Data = $"Internal Server Error"
                    };
                }

                var token = await CreateToken();

                //var viewUser = new ViewUser
                //{
                //    UserId = user.UserId,
                //    //Password = user.Password,
                //    DateCreated = user.DateCreated,
                //    Bvn = user.Bvn,
                //    UserStatus = user.UserStatus,
                //    BlockDate = user.BlockDate,
                //    SuspendDate = user.SuspendDate,
                //    BlockedBy = user.BlockedBy,
                //    //PasswordRetryCount = user.PasswordRetryCount,
                //    //Pin = user.Pin,
                //    OnBoardingComplete = user.OnBoardingComplete,
                //    Email = user.Email,
                //    PhoneNumber = user.PhoneNumber,
                //    FullName = user.FullName,

                //    Accounts = user.Accounts.Select(a => new ViewAccount
                //    {
                //        AccountNo = a.AccountNo,
                //        SeqNo = a.SeqNo,
                //        Show = a.Show,
                //        UserId = a.UserId,
                //        AccountName = a.AccountName,
                //        AccountType = a.AccountType,
                //        BranchCode = a.BranchCode,
                //        BranchName = a.BranchName
                //    }).ToList(),

                //    UserMaintHists = user.UserMaintHists.Select(x => new ViewUserMaintHist
                //    {
                //        SeqNo = x.SeqNo,
                //        UserId = x.UserId,
                //        BlockOldValue = x.BlockOldValue,
                //        BlockNewValue = x.BlockNewValue,
                //        SuspendOldValue = x.SuspendOldValue,
                //        SuspendNewValue = x.SuspendNewValue,
                //        ActivityDate = x.ActivityDate,
                //        MaintFlagCode = x.MaintFlagCode,
                //        BlockedBy = x.BlockedBy,
                //    }).ToList(),

                //    Devices = user.Devices.Select(x => new ViewDevice
                //    {
                //        UserId = x.UserId,
                //        DeviceId = x.DeviceId,
                //    }).ToList(),

                //    Beneficiaries = user.Beneficiaries.Select(x => new ViewBeneficiary
                //    {
                //        UserId = x.UserId,
                //        BeneficiaryAccountNo = x.BeneficiaryAccountNo,
                //        BeneficiaryAccountName = x.BeneficiaryAccountName,
                //        BeneficiaryBank = x.BeneficiaryBank,
                //    }).ToList(),

                //    TransferLimit = user.TransferLimit,

                //    EmailConfirmed = user.EmailConfirmed,
                //    Id = user.Id,
                //    //LockoutEnd = user.LockoutEnd,
                //    TwoFactorEnabled = user.TwoFactorEnabled,
                //    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                //    //ConcurrencyStamp = user.ConcurrencyStamp,
                //    //SecurityStamp = user.SecurityStamp,
                //    //PasswordHash = user.PasswordHash,
                //    //NormalizedEmail = user.NormalizedEmail,
                //    //NormalizedUserName = user.NormalizedUserName,
                //    UserName = user.UserName,
                //    //LockoutEnabled = user.LockoutEnabled,
                //    //AccessFailedCount = user.AccessFailedCount
                //};

                var viewUser = UserByAccountNoGet(view.AccountNo).Result.Data;

                var response = new ViewLoginResponse
                {
                    user = (ViewUser)viewUser,
                    token = token
                };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserGet()
        {
            try
            {
                var response = await context.Users
                                      .Include(x => x.Accounts)
                                      .Include(b => b.Beneficiaries)
                                      .Include(t => t.TransferLimit)
                                      .Include(u => u.UserMaintHists)
                                      .Include(d => d.Devices)
                                      .ToListAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserGetSingleExt(string userId)
        {
            try
            {
                var response = await context.Users.
                                        Where(x => x.Id == userId)
                                        .Include(x => x.Accounts)
                                        .Include(b => b.Beneficiaries)
                                        .Include(t => t.TransferLimit)
                                        .Include(u => u.UserMaintHists)
                                        .Include(d => d.Devices)
                                        .FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{userId} Not Found"
                    };

                //var user = (User)response;
                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response //user
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        //to be tested
        public async Task<ViewApiResponse> UserGetSingle(string userId)
        {
            try
            {
                var response = await context.Users.Where(x => x.Id == userId)
                                .Include(x => x.Accounts)
                                .Include(b => b.Beneficiaries)
                                .Include(t => t.TransferLimit)
                                .Include(u => u.UserMaintHists)
                                .Include(d => d.Devices)
                                .SingleOrDefaultAsync();

                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{userId} Not Found"
                    };

                var viewUser = new ViewUser
                {
                    UserId = response.UserId,
                    //Password = response.Password,
                    DateCreated = response.DateCreated,
                    Bvn = response.Bvn,
                    UserStatus = response.UserStatus,
                    BlockDate = response.BlockDate,
                    SuspendDate = response.SuspendDate,
                    BlockedBy = response.BlockedBy,
                    //PasswordRetryCount = response.PasswordRetryCount,
                    //Pin = response.Pin,
                    OnBoardingComplete = response.OnBoardingComplete,
                    Email = response.Email,
                    PhoneNumber = response.PhoneNumber,
                    FullName = response.FullName,
                    CustomerId = response.CustomerId,

                    Accounts = response.Accounts.Select(a => new ViewAccount
                    {
                        AccountNo = a.AccountNo,
                        SeqNo = a.SeqNo,
                        Show = a.Show,
                        UserId = a.UserId,
                        AccountName = a.AccountName,
                        AccountType = a.AccountType,
                        BranchCode = a.BranchCode,
                        BranchName = a.BranchName,
                        AccountStatus = a.AccountStatus == 1 ? "Active" : "Inactive",
                        AccountBalance = GetAccountBal(a.AccountNo).Result
                    }).ToList(),

                    UserMaintHists = response.UserMaintHists.Select(x => new ViewUserMaintHist
                    {
                        SeqNo = x.SeqNo,
                        UserId = x.UserId,
                        BlockOldValue = x.BlockOldValue,
                        BlockNewValue = x.BlockNewValue,
                        SuspendOldValue = x.SuspendOldValue,
                        SuspendNewValue = x.SuspendNewValue,
                        ActivityDate = x.ActivityDate,
                        MaintFlagCode = x.MaintFlagCode,
                        BlockedBy = x.BlockedBy,
                    }).ToList(),

                    Devices = response.Devices.Select(x => new ViewDevice
                    {
                        UserId = x.UserId,
                        DeviceId = x.DeviceId,
                    }).ToList(),

                    Beneficiaries = response.Beneficiaries.Select(x => new ViewBeneficiary
                    {
                        UserId = x.UserId,
                        BeneficiaryAccountNo = x.BeneficiaryAccountNo,
                        BeneficiaryAccountName = x.BeneficiaryAccountName,
                        BeneficiaryBank = x.BeneficiaryBank,
                    }).ToList(),

                    TransferLimit = response.TransferLimit,

                    EmailConfirmed = response.EmailConfirmed,
                    Id = response.Id,
                    //LockoutEnd = response.LockoutEnd,
                    TwoFactorEnabled = response.TwoFactorEnabled,
                    PhoneNumberConfirmed = response.PhoneNumberConfirmed,
                    //ConcurrencyStamp = response.ConcurrencyStamp,
                    //SecurityStamp = response.SecurityStamp,
                    //PasswordHash = response.PasswordHash,
                    //NormalizedEmail = response.NormalizedEmail,
                    //NormalizedUserName = response.NormalizedUserName,
                    UserName = response.UserName,
                    //LockoutEnabled = response.LockoutEnabled,
                    //AccessFailedCount = response.AccessFailedCount
                };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = viewUser
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserGetSingleV2(string userId)
        {
            try
            {
                var response = await context.Users.Where(x => x.Id == userId)
                                .Include(x => x.Accounts)
                                .Include(b => b.Beneficiaries)
                                .Include(t => t.TransferLimit)
                                .Include(u => u.UserMaintHists)
                                .Include(d => d.Devices)
                                .SingleOrDefaultAsync();

                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{userId} Not Found"
                    };

                var viewUser = new ViewUser
                {
                    UserId = response.UserId,
                    //Password = response.Password,
                    DateCreated = response.DateCreated,
                    Bvn = response.Bvn,
                    UserStatus = response.UserStatus,
                    BlockDate = response.BlockDate,
                    SuspendDate = response.SuspendDate,
                    BlockedBy = response.BlockedBy,
                    //PasswordRetryCount = response.PasswordRetryCount,
                    //Pin = response.Pin,
                    OnBoardingComplete = response.OnBoardingComplete,
                    Email = response.Email,
                    PhoneNumber = response.PhoneNumber,
                    FullName = response.FullName,

                    Accounts = response.Accounts.Select(a => new ViewAccount
                    {
                        AccountNo = a.AccountNo,
                        SeqNo = a.SeqNo,
                        Show = a.Show,
                        UserId = a.UserId,
                        AccountName = a.AccountName,
                        AccountType = a.AccountType,
                        BranchCode = a.BranchCode,
                        BranchName = a.BranchName,
                        // AccountStatus =
                        AccountBalance = GetAccountBal(a.AccountNo).Result
                    }).ToList(),

                    UserMaintHists = response.UserMaintHists.Select(x => new ViewUserMaintHist
                    {
                        SeqNo = x.SeqNo,
                        UserId = x.UserId,
                        BlockOldValue = x.BlockOldValue,
                        BlockNewValue = x.BlockNewValue,
                        SuspendOldValue = x.SuspendOldValue,
                        SuspendNewValue = x.SuspendNewValue,
                        ActivityDate = x.ActivityDate,
                        MaintFlagCode = x.MaintFlagCode,
                        BlockedBy = x.BlockedBy,
                    }).ToList(),

                    Devices = response.Devices.Select(x => new ViewDevice
                    {
                        UserId = x.UserId,
                        DeviceId = x.DeviceId,
                    }).ToList(),

                    Beneficiaries = response.Beneficiaries.Select(x => new ViewBeneficiary
                    {
                        UserId = x.UserId,
                        BeneficiaryAccountNo = x.BeneficiaryAccountNo,
                        BeneficiaryAccountName = x.BeneficiaryAccountName,
                        BeneficiaryBank = x.BeneficiaryBank,
                    }).ToList(),

                    TransferLimit = response.TransferLimit,

                    EmailConfirmed = response.EmailConfirmed,
                    Id = response.Id,
                    //LockoutEnd = response.LockoutEnd,
                    TwoFactorEnabled = response.TwoFactorEnabled,
                    PhoneNumberConfirmed = response.PhoneNumberConfirmed,
                    //ConcurrencyStamp = response.ConcurrencyStamp,
                    //SecurityStamp = response.SecurityStamp,
                    //PasswordHash = response.PasswordHash,
                    //NormalizedEmail = response.NormalizedEmail,
                    //NormalizedUserName = response.NormalizedUserName,
                    UserName = response.UserName,
                    //LockoutEnabled = response.LockoutEnabled,
                    //AccessFailedCount = response.AccessFailedCount
                };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = viewUser
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserByAccountNoGet(string accountNo)
        {
            try
            {
                var getAccount = await AccountGetSingle(accountNo);
                if (getAccount.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return getAccount;

                var account = (Account)getAccount.Data;

                var userId = account.UserId;
                var getUser = await UserGetSingle(userId);
                return getUser;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserDelete(string userId)
        {
            try
            {
                //var getUser = await UserGetSingle(userId);
                var getUser = await context.Users.SingleOrDefaultAsync(x => x.Id == userId);
                //if (getUser.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                //    return getUser;

                if (getUser == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = "Not Found"
                    };

                //var userToBeDeleted = (User)getUser.Data;
                var userToBeDeleted = getUser;
                context.Remove(userToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{userToBeDeleted.UserName} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public Task<ViewApiResponse> UserUpdate(ViewUser view)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewApiResponse> SetOnboardingToTrue(string accountNo)
        {
            if (accountNo is null)
            {
                return new ViewApiResponse
                {
                    Status = code.INVALID_ACCOUNT_CODE,
                    Message = code.INVALID_ACCOUNT_MSG,
                    Data = $"Enter a valid account"
                };
            }

            try
            {
                var getUserAccount = await UserByAccountNoGet(accountNo);
                if (getUserAccount.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return getUserAccount;

                var getUserDetails = (User)getUserAccount.Data;

                if (getUserDetails.OnBoardingComplete.Equals(false))
                    getUserDetails.OnBoardingComplete = true;

                await context.SaveChangesAsync();
                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"User onboarded successfully "
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }

        }

        public async Task<ViewApiResponse> ForgotPassword(ViewForgotPasswordRequest view)
        {
            if (view == null)
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = $"Invalid Details"
                };

            try
            {
                var fetchUser = UserByAccountNoGet(view.AccountNo).Result;
                if (fetchUser.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return fetchUser;

                var user = (User)fetchUser.Data;
                var pin = user.Pin;
                bool isPinValid = BC.Verify(view.Pin, pin);

                if (isPinValid)
                {
                    var response = await SendOtpForPasswordReset(view.AccountNo);
                    if (response.Status == code.STATUS_UNKNOWN_CODE)
                        return response;
                    //if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    //    return response;

                    return response;

                }
                else
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_PIN_CODE,
                        Message = code.INVALID_PIN_MSG,
                        Data = $"Invalid Pin"
                    };
                }


            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> ResetPassword(ViewResetPasswordRequest view)
        {
            if (view.AccountNo == null || view.Password == null || view.Otp == null)
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = $"Invalid details"
                };

            try
            {
                var fetchUser = UserByAccountNoGet(view.AccountNo).Result;
                if (fetchUser.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return fetchUser;

                var user = (User)fetchUser.Data;

                var viewVerifyOtp = new ViewVerifyOtp
                {
                    AccountNo = view.AccountNo,
                    Otp = view.Otp
                };

                var verifyOtp = VerifyOtp(viewVerifyOtp).Result;
                //if (verifyOtp.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                //    return verifyOtp;
                if (verifyOtp.Status == code.EXPIRED_OTP_CODE)
                    return verifyOtp;
                else if (verifyOtp.Status == code.INVALID_OTP_CODE || verifyOtp.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_OTP_CODE,
                        Message = code.INVALID_OTP_MSG,
                        Data = $"Invalid OTP"
                    };
                //return verifyOtp;

                if (user.LockoutEnd != null && user.AccessFailedCount >= 3)
                {

                    var hasher = new PasswordHasher();
                    var hashedPassword = hasher.HashPassword(view.Password);

                    //to be inspected
                    //var verifyPasswordHash = hasher.VerifyHashedPassword(user.PasswordHash, view.Password);
                    //if (verifyPasswordHash.Equals("Success"))
                    //    return new ViewApiResponse
                    //    {
                    //        Status = code.INVALID_PASSWORD_CODE,
                    //        Message = code.INVALID_PASSWORD_MSG,
                    //        Data = $"Please use a new password"
                    //    };

                    user.PasswordHash = hashedPassword;
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;

                    //var resetPassword = userManager.ChangePasswordAsync(user, user.PasswordHash, view.Password);
                    var resetPassword = await userManager.UpdateAsync(user);

                    //set OTP status to Used
                    var fetchOtpRecord = context.Otps.Where(x => x.AccountNo.Equals(view.AccountNo) && x.Status.Equals("Unused OTP")).SingleOrDefaultAsync();
                    if (fetchOtpRecord != null)
                    {
                        var otpRecord = (Otp)fetchOtpRecord.Result;
                        otpRecord.Status = "Expired OTP";

                        context.Entry(otpRecord).State = EntityState.Detached;
                        context.Entry(otpRecord).State = EntityState.Modified;

                        context.Otps.Update(otpRecord);
                        await context.SaveChangesAsync();
                    }

                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = $"{resetPassword}  Password reset successfully"
                    };
                }
                else
                {
                    var hasher = new PasswordHasher();
                    var hashedPassword = hasher.HashPassword(view.Password);

                    //to be inspected
                    //var verifyPasswordHash = hasher.VerifyHashedPassword(user.PasswordHash, hashedPassword);
                    //if (verifyPasswordHash.Equals("Success"))
                    //    return new ViewApiResponse
                    //    {
                    //        Status = code.INVALID_PASSWORD_CODE,
                    //        Message = code.INVALID_PASSWORD_MSG,
                    //        Data = $"Please use a new password"
                    //    };

                    user.PasswordHash = hashedPassword;

                    var resetPassword = await userManager.UpdateAsync(user);

                    //set OTP status to Used
                    var fetchOtpRecord = context.Otps.Where(x => x.AccountNo.Equals(view.AccountNo) && x.Status.Equals("Unused OTP")).SingleOrDefaultAsync();
                    if (fetchOtpRecord != null)
                    {
                        var otpRecord = (Otp)fetchOtpRecord.Result;
                        otpRecord.Status = "Expired OTP";

                        context.Entry(otpRecord).State = EntityState.Detached;
                        context.Entry(otpRecord).State = EntityState.Modified;

                        context.Otps.Update(otpRecord);
                        await context.SaveChangesAsync();
                    }

                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = $"{resetPassword}. Password reset successfully"
                    };
                }

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region Account
        public Task<ViewApiResponse> AccountAdd(ViewAccount view)
        {
            throw new NotImplementedException();
        }

        public Task<ViewApiResponse> AccountDelete(string accountNo)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewApiResponse> AccountGet()
        {

            try
            {
                var response = await context.Accounts.ToListAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> AccountGetSingle(string accountNo)
        {
            try
            {
                var response = await context.Accounts.Where(x => x.AccountNo == accountNo).SingleOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{accountNo} Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

                //return new ViewApiResponse
                //{
                //    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                //    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                //    Data = new ViewAccount
                //    {
                //        SeqNo = response.SeqNo,
                //        UserId = response.UserId,
                //        AccountNo = response.AccountNo,
                //        Show = response.Show
                //    }
                //};
            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public Task<ViewApiResponse> AccountUpdate(ViewAccount view)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region PrmType

        public async Task<ViewApiResponse> PrmTypeAdd(ViewPrmType view)
        {
            try
            {
                PrmType entity = view;
                var response = context.PrmTypes.AddAsync(entity);
                await context.SaveChangesAsync();

                var result = await PrmTypeGetSingle(entity.TypeCode);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypeGet()
        {
            try
            {
                var response = await context.PrmTypes.ToListAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypeGetSingle(short typeCode)
        {
            try
            {
                var response = await context.PrmTypes.Where(x => x.TypeCode == typeCode).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{typeCode} Not Found"
                    };

                return new ViewApiResponse
                {

                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypeUpdate(ViewPrmType view)
        {
            try
            {
                var response = await PrmTypeGetSingle(view.TypeCode);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var entityToBeModified = (PrmType)response.Data;
                entityToBeModified.TypeDesc = view.TypeDesc;
                entityToBeModified.UserDefined = view.UserDefined;
                entityToBeModified.LabelCode = view.LabelCode;
                entityToBeModified.LabelDesc = view.LabelDesc;

                context.Entry(entityToBeModified).State = EntityState.Detached;
                context.Entry(entityToBeModified).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var result = await PrmTypeGetSingle(entityToBeModified.TypeCode);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypeDelete(short typeCode)
        {
            try
            {
                var result = await PrmTypeGetSingle(typeCode);
                if (result.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return result;

                var prmTypeToBeDeleted = (PrmType)result.Data;
                context.Remove(prmTypeToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{prmTypeToBeDeleted.TypeCode} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region PrmTypesDetail

        public async Task<ViewApiResponse> PrmTypesDetailAdd(ViewPrmTypesDetail view)
        {
            try
            {
                PrmTypesDetail prmTypesDetail = view;
                var result = context.PrmTypesDetails.AddAsync(prmTypesDetail);
                await context.SaveChangesAsync();

                var response = await PrmTypesDetailGetSingle(prmTypesDetail.TypeCode, prmTypesDetail.Code);
                return response;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypesDetailGet()
        {
            try
            {
                var response = await context.PrmTypesDetails.ToListAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypesDetailGetSingle(short typeCode, string tCode)
        {
            try
            {
                var response = await context.PrmTypesDetails.Where(x => x.TypeCode == typeCode && x.Code == tCode).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{typeCode} Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypesDetailUpdate(ViewPrmTypesDetail view)
        {
            try
            {
                var response = await PrmTypesDetailGetSingle(view.TypeCode, view.Code);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var entityToBeModified = (PrmTypesDetail)response.Data;
                //entityToBeModified.Code = view.Code;
                entityToBeModified.Description = view.Description;
                entityToBeModified.Display = view.Display;

                context.Entry(entityToBeModified).State = EntityState.Detached;
                context.Entry(entityToBeModified).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var result = await PrmTypesDetailGetSingle(entityToBeModified.TypeCode, entityToBeModified.Code);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> PrmTypesDetailDelete(short typeCode, string tCode)
        {
            try
            {
                var response = await PrmTypesDetailGetSingle(typeCode, tCode);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var prmTypeDetailToBeDeleted = (PrmTypesDetail)response.Data;
                context.Remove(prmTypeDetailToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{prmTypeDetailToBeDeleted.TypeCode} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }


        #endregion


        #region Device

        public async Task<ViewApiResponse> DeviceAdd(ViewDevice view)
        {
            try
            {
                Device device = view;
                var result = context.Devices.AddAsync(device);
                await context.SaveChangesAsync();

                var response = await DeviceGetSingle(device.DeviceId);
                return response;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> DeviceGet()
        {
            try
            {
                var response = await context.Devices.ToListAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> DeviceGetSingle(string deviceId)
        {
            try
            {
                var response = await context.Devices.Where(x => x.DeviceId == deviceId).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {

                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{deviceId} Not Found"
                    };

                return new ViewApiResponse
                {

                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public Task<ViewApiResponse> DeviceUpdate(ViewDevice view)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewApiResponse> DeviceDelete(string deviceId)
        {
            try
            {
                var response = await DeviceGetSingle(deviceId);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var deviceToBeDeleted = (Device)response.Data;
                context.Remove(deviceToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{deviceToBeDeleted.DeviceId} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region Beneficiary

        public async Task<ViewApiResponse> BeneficiaryAdd(ViewBeneficiary view)
        {
            try
            {
                Beneficiary beneficiary = view;
                var result = context.Beneficiaries.AddAsync(beneficiary);
                await context.SaveChangesAsync();

                var response = await BeneficiaryGetSingle(beneficiary.UserId, beneficiary.BeneficiaryAccountNo);
                return response;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> BeneficiaryGet()
        {
            try
            {
                List<ViewBeneficiary> beneficiaryList = new();

                var response = await context.Beneficiaries.ToListAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };
                foreach (var record in response)
                {
                    beneficiaryList.Add(record);
                }

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = beneficiaryList
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> BeneficiaryGetSingle(string userId, string beneficiaryAccountNo)
        {
            try
            {
                var response = await context.Beneficiaries.Where(x => x.UserId == userId && x.BeneficiaryAccountNo == beneficiaryAccountNo).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{beneficiaryAccountNo} Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = new ViewBeneficiary
                    {
                        UserId = response.UserId,
                        BeneficiaryAccountNo = response.BeneficiaryAccountNo,
                        BeneficiaryAccountName = response.BeneficiaryAccountName,
                        BeneficiaryBank = response.BeneficiaryBank
                    }
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> BeneficiaryUpdate(ViewBeneficiary view)
        {
            try
            {
                var response = await BeneficiaryGetSingle(view.UserId, view.BeneficiaryAccountNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var entityToBeModified = (Beneficiary)response.Data;
                entityToBeModified.BeneficiaryAccountNo = view.BeneficiaryAccountNo;
                entityToBeModified.BeneficiaryAccountName = view.BeneficiaryAccountName;
                entityToBeModified.BeneficiaryBank = view.BeneficiaryBank;

                context.Entry(entityToBeModified).State = EntityState.Detached;
                context.Entry(entityToBeModified).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var result = await BeneficiaryGetSingle(entityToBeModified.UserId, entityToBeModified.BeneficiaryAccountNo);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> BeneficiaryDelete(string userId, string beneficiaryAccountNo)
        {
            try
            {
                var response = await BeneficiaryGetSingle(userId, beneficiaryAccountNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var beneficiaryToBeDeleted = (Beneficiary)response.Data;
                context.Remove(beneficiaryToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{beneficiaryToBeDeleted.BeneficiaryAccountNo} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region LoginHist

        public async Task<ViewApiResponse> LoginHistAdd(ViewLoginHist view)
        {
            try
            {
                LoginHist loginHist = view;
                var result = await context.LoginHists.AddAsync(loginHist);
                await context.SaveChangesAsync();

                var response = await LoginHistGetSingle(loginHist.SeqNo);
                return response;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> LoginHistGet()
        {
            try
            {
                //List<ViewLoginHist> loginHistList = new();

                var response = await context.LoginHists.ToListAsync();
                if (response.Count == 0)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                //foreach (var record in response)
                //{
                //    loginHistList.Add(record);
                //}

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response   //loginHistList
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> LoginHistGetSingle(long seqNo)
        {
            try
            {
                var response = await context.LoginHists.Where(x => x.SeqNo == seqNo).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.STATUS_UNKNOWN_CODE,
                        Message = code.STATUS_UNKNOWN_MSG,
                        Data = $"{seqNo} Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                    //Data = new ViewLoginHist
                    //{
                    //    SeqNo = response.SeqNo,
                    //    UserId = response.UserId,
                    //    LoginDate = response.LoginDate,
                    //    DeviceId = response.DeviceId,
                    //    Location = response.Location
                    //}
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> LoginHistUpdate(ViewLoginHist view)
        {
            try
            {
                var response = await LoginHistGetSingle(view.SeqNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var entityToBeModified = (LoginHist)response.Data;
                entityToBeModified.DeviceId = view.DeviceId;
                entityToBeModified.Location = view.Location;

                context.Entry(entityToBeModified).State = EntityState.Detached;
                context.Entry(entityToBeModified).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var result = await LoginHistGetSingle(entityToBeModified.SeqNo);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> LoginHistDelete(long seqNo)
        {
            try
            {
                var response = await LoginHistGetSingle(seqNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var loginHistToBeDeleted = (LoginHist)response.Data;
                context.Remove(loginHistToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{loginHistToBeDeleted.SeqNo} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region UserMaintHist

        public async Task<ViewApiResponse> UserMaintHistAdd(ViewUserMaintHist view)
        {
            try
            {
                UserMaintHist userMaintHist = view;
                var result = await context.UserMaintHists.AddAsync(userMaintHist);
                await context.SaveChangesAsync();

                var response = await UserMaintHistGetSingle(userMaintHist.SeqNo);
                return response;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserMaintHistGet()
        {
            try
            {
                var response = await context.UserMaintHists.ToListAsync();
                if (response.Count == 0)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserMaintHistGetSingle(long seqNo)
        {
            try
            {
                var response = await context.UserMaintHists.Where(x => x.SeqNo == seqNo).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.STATUS_UNKNOWN_CODE,
                        Message = code.STATUS_UNKNOWN_MSG,
                        Data = $"{seqNo} Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserMaintHistUpdate(ViewUserMaintHist view)
        {
            try
            {
                var response = await UserMaintHistGetSingle(view.SeqNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var entityToBeModified = (UserMaintHist)response.Data;
                entityToBeModified.MaintFlagCode = view.MaintFlagCode;
                entityToBeModified.SuspendNewValue = view.SuspendNewValue;
                entityToBeModified.SuspendOldValue = view.SuspendOldValue;
                entityToBeModified.BlockedBy = view.BlockedBy;
                entityToBeModified.BlockNewValue = view.BlockNewValue;
                entityToBeModified.BlockOldValue = view.BlockOldValue;


                context.Entry(entityToBeModified).State = EntityState.Detached;
                context.Entry(entityToBeModified).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var result = await UserMaintHistGetSingle(entityToBeModified.SeqNo);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> UserMaintHistDelete(long seqNo)
        {
            try
            {
                var response = await UserMaintHistGetSingle(seqNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var userMaintHistToBeDeleted = (UserMaintHist)response.Data;
                context.Remove(userMaintHistToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{userMaintHistToBeDeleted.SeqNo} Deleted"
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {

                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }


        #endregion


        #region Otp

        public async Task<ViewApiResponse> OtpAdd(ViewOtp view)
        {
            try
            {
                Otp otp = view;
                var result = context.Otps.AddAsync(otp);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = result
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> OtpGetByAccountNo(string accountNo)
        {
            try
            {
                var response = await context.Otps.Where(x => x.AccountNo == accountNo).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region TransferLimit
        public async Task<ViewApiResponse> TransferLimitAdd(ViewTransferLimit view)
        {
            if (view == null)
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = $"Invalid Details"
                };

            try
            {
                TransferLimit transferLimit = view;
                var result = context.TransferLimits.AddAsync(transferLimit);
                await context.SaveChangesAsync();

                var response = await TransferLimitGetSingle(transferLimit.SeqNo);
                return response;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> TransferLimitGet()
        {
            try
            {
                //List<ViewTransferLimit> transferLimits = new();

                var response = await context.TransferLimits.ToListAsync();
                if (response.Count == 0)
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = response
                    };

                //foreach (var record in response)
                //{
                //    transferLimits.Add(record);
                //}

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response //transferLimits
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> TransferLimitGetSingle(long seqNo)
        {
            try
            {
                var response = await context.TransferLimits.Where(x => x.SeqNo == seqNo).FirstOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"{seqNo} Not Found"
                    };

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = response
                    //Data = new ViewTransferLimit
                    //{
                    //    SeqNo = response.SeqNo,
                    //    UserId = response.UserId,
                    //    PreviousLimit = response.PreviousLimit,
                    //    CurrentLimit = response.CurrentLimit,
                    //    DateTimeModified = response.DateTimeModified
                    //}
                };

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> TransferLimitUpdate(ViewTransferLimit view)
        {
            if (view == null)
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = $"Invalid Details"
                };

            try
            {
                var response = await TransferLimitGetSingle(view.SeqNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var entityToBeModified = (TransferLimit)response.Data;
                entityToBeModified.PreviousLimit = view.PreviousLimit;
                entityToBeModified.CurrentLimit = view.CurrentLimit;
                entityToBeModified.DateTimeModified = DateTime.Now;

                context.Entry(entityToBeModified).State = EntityState.Detached;
                context.Entry(entityToBeModified).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var result = await UserMaintHistGetSingle(entityToBeModified.SeqNo);
                return result;

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> TransferLimitDelete(long seqNo)
        {
            try
            {
                var response = await TransferLimitGetSingle(seqNo);
                if (response.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return response;

                var transferLimitToBeDeleted = (ViewTransferLimit)response.Data;
                context.Remove(transferLimitToBeDeleted);
                await context.SaveChangesAsync();

                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"{transferLimitToBeDeleted.SeqNo} Deleted"
                };
            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region RestClient

        public async Task<ViewApiResponse> GetAccountDetailsByAcctNumber(string accountNo)
        {
            if (String.IsNullOrEmpty(accountNo))
                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = $"Enter a valid account"
                };

            ViewAccountDetails data = new ViewAccountDetails();

            try
            {
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string baseUrl = $"http://63.250.52.14:5007/api/Datalinks/";

                string url = QueryHelpers.AddQueryString(baseUrl + "GetAccountDetailsByAcctNumber", "accountNo", accountNo);

                using (var response = await client.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        data = await response.Content.ReadFromJsonAsync<ViewAccountDetails>();  //JsonSerializer.Deserialize<ViewAccountDetails>(response.ReasonPhrase);

                        if (data != null)
                        {
                            var result = await context.Accounts.Where(x => x.AccountNo.Equals(data.AccountNumber)).SingleOrDefaultAsync();

                            if (result == null)
                            {
                                return new ViewApiResponse
                                {
                                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                                    Data = new ViewVerifyAccountDetails
                                    {
                                        AccountNumber = data.AccountNumber,
                                        AccountName = data.AccountName,
                                        AccountType = data.BankAccountType,
                                        AccountStatus = data.AccountStatus,
                                        BranchCode = data.BranchSolid,
                                        BranchName = data.BranchName,
                                        CustomerId = data.CustomerId,
                                        MainPhone = data.MainPhone,
                                        Email = data.Email,
                                        BVN = data.BVN
                                    }
                                };
                            }
                            else
                            {
                                return new ViewApiResponse
                                {
                                    Status = code.DUPLICATE_RECORD_CODE,
                                    Message = code.DUPLICATE_RECORD_MSG,
                                    Data = new ViewVerifyAccountDetails
                                    {
                                        AccountNumber = data.AccountNumber,
                                        AccountName = data.AccountName,
                                        AccountType = data.BankAccountType,
                                        AccountStatus = data.AccountStatus,
                                        BranchCode = data.BranchSolid,
                                        BranchName = data.BranchName,
                                        CustomerId = data.CustomerId,
                                        MainPhone = data.MainPhone,
                                        Email = data.Email,
                                        BVN = data.BVN
                                    }
                                };
                            }

                        }
                        else
                        {
                            return new ViewApiResponse
                            {
                                Status = code.INVALID_ACCOUNT_CODE,
                                Message = code.INVALID_ACCOUNT_MSG,
                                Data = new ViewVerifyAccountDetails
                                {
                                    AccountNumber = "",
                                    AccountName = "",
                                    AccountType = "",
                                    AccountStatus = 0,
                                    BranchCode = "",
                                    BranchName = "",
                                    CustomerId = "",
                                    MainPhone = "",
                                    Email = "",
                                    BVN = ""
                                }
                            };

                        }

                    }
                    else
                    {
                        return new ViewApiResponse
                        {
                            Status = code.STATUS_UNKNOWN_CODE,
                            Message = code.STATUS_UNKNOWN_MSG,
                            Data = new ViewVerifyAccountDetails
                            {
                                AccountNumber = "",
                                AccountName = "",
                                AccountType = "",
                                AccountStatus = 0,
                                BranchCode = "",
                                BranchName = "",
                                CustomerId = "",
                                MainPhone = "",
                                Email = "",
                                BVN = ""
                            }
                        };


                    }

                }

            }
            catch (HttpRequestException ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> AccountCheckByAcctNumber(string accountNo)
        {
            if (String.IsNullOrEmpty(accountNo))
                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = $"Enter a valid account"
                };

            ViewAccountDetails data = new ViewAccountDetails();

            try
            {
                AccountStatusCheck accountStatusCheck = await IsAccountPresentOnMobileDB(accountNo);
                if (accountStatusCheck == AccountStatusCheck.PresentActivated)
                    return new ViewApiResponse
                    {
                        Status = code.DUPLICATE_RECORD_CODE,
                        Message = code.DUPLICATE_RECORD_MSG,
                        Data = "Account Registered"
                    };
                else if (accountStatusCheck == AccountStatusCheck.PresentNotActivated)
                {
                    return new ViewApiResponse
                    {
                        Status = code.PRESENT_NOT_ACTIVATED_RECORD_CODE,
                        Message = code.PRESENT_NOT_ACTIVATED_RECORD_MSG,
                        Data = "Present, Not Activated"
                    };
                } else
                {
                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string baseUrl = $"http://63.250.52.14:5007/api/Datalinks/";

                    string url = QueryHelpers.AddQueryString(baseUrl + "GetAccountDetailsByAcctNumber", "accountNo", accountNo);

                    using (var response = await client.GetAsync(url))
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            data = await response.Content.ReadFromJsonAsync<ViewAccountDetails>();  //JsonSerializer.Deserialize<ViewAccountDetails>(response.ReasonPhrase);

                            if (data != null)
                            {
                                //var result = await context.Accounts.Where(x => x.AccountNo.Equals(data.AccountNumber)).SingleOrDefaultAsync();

                                return new ViewApiResponse
                                {
                                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                                    Data = new ViewVerifyAccountDetails
                                    {
                                        AccountNumber = data.AccountNumber,
                                        AccountName = data.AccountName,
                                        AccountType = data.BankAccountType,
                                        AccountStatus = data.AccountStatus,
                                        BranchCode = data.BranchSolid,
                                        BranchName = data.BranchName,
                                        CustomerId = data.CustomerId,
                                        MainPhone = data.MainPhone,
                                        Email = data.Email,
                                        BVN = data.BVN
                                    }
                                };

                            }
                            else
                            {
                                return new ViewApiResponse
                                {
                                    Status = code.INVALID_ACCOUNT_CODE,
                                    Message = code.INVALID_ACCOUNT_MSG,
                                    Data = new ViewVerifyAccountDetails
                                    {
                                        AccountNumber = "",
                                        AccountName = "",
                                        AccountType = "",
                                        AccountStatus = 0,
                                        BranchCode = "",
                                        BranchName = "",
                                        CustomerId = "",
                                        MainPhone = "",
                                        Email = "",
                                        BVN = ""
                                    }
                                };

                            }

                        }
                        else
                        {
                            return new ViewApiResponse
                            {
                                Status = code.STATUS_UNKNOWN_CODE,
                                Message = code.STATUS_UNKNOWN_MSG,
                                Data = new ViewVerifyAccountDetails
                                {
                                    AccountNumber = "",
                                    AccountName = "",
                                    AccountType = "",
                                    AccountStatus = 0,
                                    BranchCode = "",
                                    BranchName = "",
                                    CustomerId = "",
                                    MainPhone = "",
                                    Email = "",
                                    BVN = ""
                                }
                            };


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> GetAccountDetailsByCustomerId(string cusId)
        {

            List<ViewAccountDetails> data = new();

            try
            {
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string baseUrl = $"http://63.250.52.14:5007/api/Datalinks/";

                string url = QueryHelpers.AddQueryString(baseUrl + "GetAccountDetailsByCustomerId", "cusId", cusId);

                using (var response = await client.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        data = await response.Content.ReadFromJsonAsync<List<ViewAccountDetails>>();  //JsonSerializer.Deserialize<ViewAccountDetails>(response.ReasonPhrase);

                        return new ViewApiResponse
                        {
                            Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                            Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                            Data = data
                        };

                    }
                    else
                    {
                        return new ViewApiResponse
                        {
                            Status = code.STATUS_UNKNOWN_CODE,
                            Message = code.STATUS_UNKNOWN_MSG,
                            Data = null
                        };


                    }

                }

            }
            catch (HttpRequestException ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> GetAccountBalance(string accountNo)
        {

            if (String.IsNullOrEmpty(accountNo))
                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = $"Enter a valid account"
                };

            try
            {
                var client = new HttpClient();


                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string baseUrl = $"http://63.250.52.14:5007/api/Datalinks/";

                string url = QueryHelpers.AddQueryString(baseUrl + "GetAccountBalance", "accountNo", accountNo);


                //var response = await client.SendAsync(uri, HttpCompletionOption.ResponseHeadersRead);

                //var responseString = await response.Content.ReadAsStringAsync();
                //response.EnsureSuccessStatusCode();

                //var options = new JsonSerializerOptions
                //{
                //    PropertyNameCaseInsensitive = true
                //};
                //ViewAccountBalance data = JsonSerializer.Deserialize<ViewAccountBalance>(responseString, options);

                //return new ViewApiResponse
                //{
                //    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                //    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                //    Data = data
                //};

                using (var response = await client.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        //var data = JsonSerializer.Deserialize<ViewAccountBalance>(response.);
                        var data = await response.Content.ReadFromJsonAsync<ViewAccountBalance>();

                        if (data != null)
                        {
                            return new ViewApiResponse
                            {
                                Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                                Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                                Data = data
                            };
                        }
                        else
                        {
                            return new ViewApiResponse
                            {
                                Status = code.INVALID_ACCOUNT_CODE,
                                Message = code.INVALID_ACCOUNT_MSG,
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new ViewApiResponse
                        {
                            Status = code.STATUS_UNKNOWN_CODE,
                            Message = code.STATUS_UNKNOWN_MSG,
                            Data = null
                        };
                    }
                }

            }
            catch (HttpRequestException ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        #endregion


        #region Jwt Service

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<ViewApiResponse> ValidateUser(ViewLoginUser view)
        {
            var account = await AccountGetSingle(view.AccountNo);
            if (account.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return account;

            var getAccount = (Account)account.Data;
            //var user = getAccount.UserId;
            var selectedUser = getAccount.UserId;

            var getUser = await UserGetSingleExt(selectedUser);
            //var getUser = await context.Users.SingleOrDefaultAsync(x => x.Id == selectedUser);
            var userFound = (User)getUser.Data;
            //var selectedUser = (User)getUser.Data;
            user = userFound;

            if (user.UserStatus.Equals(code.BLACKLISTED_CODE))
                return new ViewApiResponse
                {
                    Status = code.BLACKLISTED_CODE,
                    Message = code.BLACKLISTED_MSG,
                    Data = $"Account blacklisted"
                };

            else if (user.UserStatus.Equals(code.BLOCKED_CODE))
                return new ViewApiResponse
                {
                    Status = code.BLOCKED_CODE,
                    Message = code.BLOCKED_MSG,
                    Data = $"Account blocked"
                };

            else if (user.LockoutEnd != null)
                return new ViewApiResponse
                {
                    Status = code.LOCKED_CODE,
                    Message = code.LOCKED_MSG,
                    Data = "Please change password"
                };


            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, view.Password);
            if (isPasswordCorrect.Equals(false))
            {
                user.AccessFailedCount++;
                context.SaveChanges();

                if (user.AccessFailedCount == 3)
                {
                    var lockedAccount = LockAccount(user);
                    return lockedAccount;
                }


                //return isPassowrdCorrect;
                return new ViewApiResponse
                {
                    Status = code.INVALID_PASSWORD_CODE,
                    Message = code.INVALID_PASSWORD_MSG,
                    Data = $"Invalid password"
                };
            }

            //return (user != null && isPassowrdCorrect /*&& user.Status.Equals(code.STATUS_ACTIVE)*/);  //&& isActive.Equals(true)
            return new ViewApiResponse
            {
                Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                Data = $"Valid Password"
            };

        }

        public async Task<ViewApiResponse> ValidateUser2(ViewLoginUser view)
        {
            var account = await AccountGetSingle(view.AccountNo);
            if (account.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                return account;

            var getAccount = (Account)account.Data;
            //var user = getAccount.UserId;

            if (getAccount.AccountStatus == 1)
            {
                var selectedUser = getAccount.UserId;

                var getUser = await UserGetSingleExt(selectedUser);
                //var getUser = await context.Users.SingleOrDefaultAsync(x => x.Id == selectedUser);
                var userFound = (User)getUser.Data;
                //var selectedUser = (User)getUser.Data;
                user = userFound;

                if (user.UserStatus.Equals(code.BLACKLISTED_CODE))
                    return new ViewApiResponse
                    {
                        Status = code.BLACKLISTED_CODE,
                        Message = code.BLACKLISTED_MSG,
                        Data = $"Account blacklisted"
                    };

                else if (user.UserStatus.Equals(code.BLOCKED_CODE))
                    return new ViewApiResponse
                    {
                        Status = code.BLOCKED_CODE,
                        Message = code.BLOCKED_MSG,
                        Data = $"Account blocked"
                    };

                else if (user.LockoutEnd != null)
                    return new ViewApiResponse
                    {
                        Status = code.LOCKED_CODE,
                        Message = code.LOCKED_MSG,
                        Data = "Please change password"
                    };


                var isPasswordCorrect = await userManager.CheckPasswordAsync(user, view.Password);
                if (isPasswordCorrect.Equals(false))
                {
                    user.AccessFailedCount++;
                    context.SaveChanges();

                    if (user.AccessFailedCount == 3)
                    {
                        var lockedAccount = LockAccount(user);
                        return lockedAccount;
                    }

                    //return isPassowrdCorrect;
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_PASSWORD_CODE,
                        Message = code.INVALID_PASSWORD_MSG,
                        Data = $"Invalid password"
                    };
                }

                //return (user != null && isPassowrdCorrect /*&& user.Status.Equals(code.STATUS_ACTIVE)*/);  //&& isActive.Equals(true)
                return new ViewApiResponse
                {
                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                    Data = $"Valid Password"
                };
            }
            else
            {
                return new ViewApiResponse
                {
                    Status = code.INACTIVE_ACCOUNT_CODE,
                    Message = code.INACTIVE_ACCOUNT_MSG,
                    Data = "Inactive Account"
                };
            }
           

        }

        private SigningCredentials GetSigningCredentials()
        {
            try
            {
                //fetch the key we created and saved as a local variable
                var jwtSettings = configuration.GetSection("Jwt");
                var key = jwtSettings.GetSection("SKEY").Value;
                //encrypting the key we just fetched
                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Name, user.UserName)
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
             );

            return token;
        }

        #endregion


        #region Util

        //public async Task<ViewApiResponse> SendOtp(string accountNo) 
        //{

        //    try
        //    {
        //        var getUser = await UserByAccountNoGet(accountNo);

        //        if (getUser.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
        //            return getUser;

        //        if (getUser.Status == code.STATUS_UNKNOWN_CODE)
        //            return getUser;

        //        var user = (User)getUser.Data;

        //        var generatedOtp = GenerateOtp();
        //        //user.Otp = generatedOtp;
        //        user.Otp = HashOtp(generatedOtp);
        //        user.OtpExpiration = DateTime.Now;
        //        await context.SaveChangesAsync();

        //        await emailService.SendEmailAsync(user.Email, "OTP", $"One Time Password: {generatedOtp}");

        //        DateTime dateTime = DateTime.Now.AddMinutes(5);
        //        TimeSpan span = dateTime.Subtract((DateTime)user.OtpExpiration);


        //        return new ViewApiResponse
        //        {
        //            Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
        //            Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
        //            Data = $"OTP sent to {user.Email}, Valid for {span.TotalSeconds.ToString()} seconds"
        //        };

        //    }catch (Exception ex)
        //    {
        //        return new ViewApiResponse
        //        {
        //            Status = code.STATUS_UNKNOWN_CODE,
        //            Message = code.STATUS_UNKNOWN_MSG,
        //            Data = ex.Message
        //        };
        //    }

        //}

        public bool IsAccountActivatedForMobile(string accountNo)
        {
            return context.Accounts.Where(x => x.AccountNo == accountNo).Any();
        }

        public async Task<AccountStatusCheck> IsAccountPresentOnMobileDB(string accountNo)
        {
            var account = await context.Accounts.Where(x => x.AccountNo == accountNo).SingleOrDefaultAsync();

            if (account == null)
            {
                return AccountStatusCheck.NotPresent;
            }
            else if (account.AccountStatus == 0)
            {
                return AccountStatusCheck.PresentNotActivated;
            }
            else
            {
                return AccountStatusCheck.PresentActivated;
            }

        }

        public async Task<ViewApiResponse> SendOtp(string accountNo)
        {
            if (String.IsNullOrEmpty(accountNo))
                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = $"Enter a valid account"
                };

            try
            {
                var fetchAccountDetails = await GetAccountDetailsByAcctNumber(accountNo);
                if (fetchAccountDetails.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE || fetchAccountDetails.Status == code.DUPLICATE_RECORD_CODE)
                {
                    var accountDetails = (ViewVerifyAccountDetails)fetchAccountDetails.Data;

                    var fetchOtpRecord = context.Otps.Where(x => x.AccountNo.Equals(accountDetails.AccountNumber) && x.Status.Equals("Unused OTP")).SingleOrDefaultAsync();
                    if (fetchOtpRecord.Result == null)
                    {
                        var generateOtp = GenerateOtp();

                        var viewOtp = new Otp
                        {
                            AccountNo = accountDetails.AccountNumber,
                            Email = accountDetails.Email,
                            PhoneNumber = accountDetails.MainPhone,
                            OtpCode = HashOtp(generateOtp),
                            OtpTimeCreated = DateTime.Now,
                            Status = "Unused OTP"
                        };

                        await context.Otps.AddAsync(viewOtp);
                        await context.SaveChangesAsync();

                        DateTime dateTime = DateTime.Now.AddMinutes(5);
                        TimeSpan span = dateTime.Subtract((DateTime)viewOtp.OtpTimeCreated);
                        double minutes = span.TotalMinutes;
                        double totalMinutes = Math.Round(minutes);

                        await emailService.SendEmailAsync(viewOtp.Email, "OTP", $"One Time Password: {generateOtp}.\n Valid for {totalMinutes} minutes");

                        return new ViewApiResponse
                        {
                            Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                            Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                            Data = new ViewOtpResponse
                            {
                                Email = HideEmailValue(viewOtp.Email),
                                OtpValue = generateOtp,
                                OtpExpirationTime = totalMinutes
                            }
                        };
                    }
                    else
                    {

                        var otpRecord = (Otp)fetchOtpRecord.Result;

                        otpRecord.Status = "Expired OTP";

                        var local = context.Set<Otp>()
                                .Local
                                .FirstOrDefault(entry => entry.AccountNo.Equals(otpRecord.AccountNo) && entry.Status.Equals("Unused OTP"));

                        if (local != null)
                        {
                            context.Entry(local).State = EntityState.Detached;
                        }

                        context.Otps.Update(otpRecord);
                        await context.SaveChangesAsync();

                        var generateOtp = GenerateOtp();

                        var viewOtp = new Otp
                        {
                            AccountNo = accountDetails.AccountNumber,
                            Email = accountDetails.Email,
                            PhoneNumber = accountDetails.MainPhone,
                            OtpCode = HashOtp(generateOtp),
                            OtpTimeCreated = DateTime.Now,
                            Status = "Unused OTP"
                        };

                        await context.Otps.AddAsync(viewOtp);
                        await context.SaveChangesAsync();

                        DateTime dateTime = DateTime.Now.AddMinutes(5);
                        TimeSpan span = dateTime.Subtract((DateTime)viewOtp.OtpTimeCreated);
                        double minutes = span.TotalMinutes;
                        double totalMinutes = Math.Round(minutes);

                        await emailService.SendEmailAsync(viewOtp.Email, "OTP", $"One Time Password: {generateOtp}.\n Valid for {totalMinutes} minutes");

                        return new ViewApiResponse
                        {
                            Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                            Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                            Data = new ViewOtpResponse
                            {
                                Email = HideEmailValue(viewOtp.Email),    //viewOtp.Email,
                                OtpValue = generateOtp,
                                OtpExpirationTime = totalMinutes
                            }
                        };
                    }
                }
                else
                {
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"Failed"
                    };
                }

            }
            catch (Exception ex)
            {

                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };

            }
        }

        //public async Task<ViewApiResponse> VerifyOtp (ViewVerifyOtp view)
        //{
        //    try
        //    {
        //        var getUser = await UserByAccountNoGet(view.AccountNo);
        //        if (getUser.Status == code.STATUS_UNKNOWN_CODE)
        //            return getUser;

        //        var user = (User)getUser.Data;
        //        var otp = user.Otp;
        //        var otpExpiration = user.OtpExpiration;
        //        bool isVerified = BC.Verify(view.Otp, otp);

        //        if (isVerified)
        //        {
        //            if (!IsOtpExpired((DateTime)user.OtpExpiration))
        //            {
        //                user.Active = true;
        //                user.Otp = null;
        //                user.OtpExpiration = null;

        //                await context.SaveChangesAsync();

        //                return new ViewApiResponse
        //                {
        //                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
        //                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
        //                    Data = $"{view.AccountNo} Verification Success"
        //                };
        //            }
        //            else
        //            {

        //                user.Active = false;
        //                //context.Users.Remove(user);
        //                // reset the otp & otp expiration fields to null
        //                user.Otp = null;
        //                user.OtpExpiration = null;
        //                await context.SaveChangesAsync();

        //                return new ViewApiResponse
        //                {
        //                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
        //                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
        //                    Data = $"Expired or Invalid OTP. Please enter a valid OTP"
        //                };
        //            }

        //        }
        //        else
        //        {

        //            user.Active = false;
        //            //context.Users.Remove(user);
        //            // reset the otp & otp expiration fields to null
        //            await context.SaveChangesAsync();

        //            return new ViewApiResponse
        //            {
        //                Status = code.STATUS_UNKNOWN_CODE,
        //                Message = code.STATUS_UNKNOWN_MSG,
        //                Data = $"Invalid OTP"
        //            };
        //        }

        //    } catch (Exception ex)
        //    {
        //        return new ViewApiResponse
        //        {

        //            Status = code.STATUS_UNKNOWN_CODE,
        //            Message = code.STATUS_UNKNOWN_MSG,
        //            Data = ex.Message
        //        };
        //    }
        //}

        public async Task<ViewApiResponse> VerifyOtp(ViewVerifyOtp view)
        {
            try
            {
                var fetchAccountRecordFromOtp = await context.Otps.Where(x => x.AccountNo.Equals(view.AccountNo) && x.Status.Equals("Unused OTP")).SingleOrDefaultAsync(); //.Result;
                if (fetchAccountRecordFromOtp == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = $"No OTP value generated for {view.AccountNo}"
                    };

                var response = (Otp)fetchAccountRecordFromOtp;

                var otpCode = response.OtpCode;
                var otpTimeCreated = response.OtpTimeCreated;
                bool isOtpVerified = BC.Verify(view.Otp, otpCode);

                if (isOtpVerified)
                {
                    if (!IsOtpExpired((DateTime)otpTimeCreated))
                    {
                        return new ViewApiResponse
                        {
                            Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                            Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                            Data = $"Valid OTP"
                        };
                    }
                    else
                    {
                        // to be implemented => set otp  status field to expired
                        return new ViewApiResponse
                        {
                            Status = code.EXPIRED_OTP_CODE,
                            Message = code.EXPIRED_OTP_MSG,
                            Data = $"OTP Expired"
                        };
                    }
                }
                else
                {
                    // to be implemeted => set otp status field to used
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_OTP_CODE,
                        Message = code.INVALID_OTP_MSG,
                        Data = $"Invalid OTP"
                    };
                }

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> VerifyPin(string userId, string pin)
        {
            try
            {
                var findUserPin = await context.Users.Where(x => x.Id == userId).Select(x => x.Pin).SingleOrDefaultAsync();
                if (findUserPin is null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = null
                    };

                bool isPinVerified = BC.Verify(pin, findUserPin);
                if (isPinVerified)
                {
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = "Pin verified"
                    };
                }
                else
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_PIN_CODE,
                        Message = code.INVALID_PIN_MSG,
                        Data = "Invalid Pin"
                    };
                }


            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> VerifyPinByAccountNo(string accountNo, string pin)
        {
            try
            {

                var findUserId = await context.Accounts.Where(x => x.AccountNo == accountNo).Select(x => x.UserId).SingleOrDefaultAsync();
                if (findUserId is null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = null
                    };

                var findUserPin = await context.Users.Where(x => x.Id == findUserId).Select(x => x.Pin).SingleOrDefaultAsync();
                if (findUserPin is null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = null
                    };

                bool isPinVerified = BC.Verify(pin, findUserPin);
                if (isPinVerified)
                {
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = "Pin verified"
                    };
                }
                else
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_PIN_CODE,
                        Message = code.INVALID_PIN_MSG,
                        Data = "Invalid Pin"
                    };
                }


            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> VerifyPassword(string accountNo, string password)
        {
            try
            {
                var response = await context.Accounts.Where(x => x.AccountNo == accountNo).Select(x => x.UserId).SingleOrDefaultAsync();
                if (response == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = null
                    };

                var getUser = await context.Users.Where(x => x.Id == response).SingleOrDefaultAsync();
                if (getUser == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                        Data = null
                    };

                user = getUser;

                bool isPasswordVerified = await userManager.CheckPasswordAsync(user, password);
                if (isPasswordVerified)
                {
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = "Password verified"
                    };
                }
                else
                {
                    return new ViewApiResponse
                    {
                        Status = code.INVALID_PASSWORD_CODE,
                        Message = code.INVALID_PASSWORD_MSG,
                        Data = "Invalid password"
                    };
                }

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> CheckTransferLimit(string accountNo, decimal amount)
        {
            try
            {
                //var getAccountUser = await UserByAccountNoGet(accountNo);
                var getAccountUser = await context.Accounts.Where(x => x.AccountNo == accountNo).Select(x => x.UserId).SingleOrDefaultAsync();
                if (getAccountUser == null)
                    return new ViewApiResponse
                    {
                        Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Message = code.UNABLE_TO_LOCATE_RECORD_CODE,
                        Data = null
                    };

                var userId = getAccountUser;

                var transferLimit = await context.TransferLimits.Where(x => x.UserId == userId).Select(x => x.CurrentLimit).SingleOrDefaultAsync();
                if (amount > transferLimit)
                {
                    //return false;
                    return new ViewApiResponse
                    {
                        Status = code.TRANSFER_LIMIT_EXCEEDED_CODE,
                        Message = code.TRANSFER_LIMIT_EXCEEDED_MSG,
                        Data = false
                    };
                }
                else
                {
                    //return true;
                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = true
                    };
                }

            } catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> VerifyPinAndTransferLimit(string accountNo, string pin, decimal amount)
        {
            try
            {
                var verifyPin = await VerifyPinByAccountNo(accountNo, pin);

                if (verifyPin.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
                {
                    var verifyTransferLimit = await CheckTransferLimit(accountNo, amount);
                    if (verifyTransferLimit.Status == code.TRANSFER_LIMIT_EXCEEDED_CODE)
                        return verifyTransferLimit;

                    else if (verifyTransferLimit.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                        return verifyTransferLimit;

                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = "Pin verified & limit not exceeded"
                    };
                }
                else
                {
                    return verifyPin;
                }
                

            } catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }
        }

        public async Task<ViewApiResponse> VerifyUserIsRegistered(string accountNo)
        {

            try
            {
                bool isUserAlreadyExisting = IsAccountActivatedForMobile(accountNo);
                var fetchAccount = await AccountGetSingle(accountNo);
                if (fetchAccount.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return new ViewApiResponse
                    {
                        Status = code.STATUS_UNKNOWN_CODE,
                        Message = code.STATUS_UNKNOWN_MSG,
                        Data = $"{accountNo} Not Found"
                    };

                var account = (Account)fetchAccount.Data;
                var fetchAccountUser = account.UserId;

                var returnedUser = await UserGetSingle(fetchAccountUser);
                var fetchUser = (User)returnedUser.Data;

                //var getUser = (User)fetchUser.Data;
                string status = fetchUser.UserStatus;

                if (isUserAlreadyExisting && status.Equals(code.STATUS_ACTIVE))
                    return new ViewApiResponse
                    {
                        Status = code.DUPLICATE_RECORD_CODE,
                        Message = code.DUPLICATE_RECORD_MSG,
                        Data = $"{accountNo} Registered & Active"
                    };

                else if (isUserAlreadyExisting && status.Equals(code.BLOCKED_CODE))
                    return new ViewApiResponse
                    {
                        Status = code.DUPLICATE_RECORD_CODE,
                        Message = code.DUPLICATE_RECORD_MSG,
                        Data = $"{accountNo} Registered & Blocked"
                    };

                else if (isUserAlreadyExisting && status.Equals(code.BLACKLISTED_CODE))
                    return new ViewApiResponse
                    {
                        Status = code.DUPLICATE_RECORD_CODE,
                        Message = code.DUPLICATE_RECORD_MSG,
                        Data = $"{accountNo} Registered & Blacklisted"
                    };

                return new ViewApiResponse
                {
                    Status = code.UNABLE_TO_LOCATE_RECORD_CODE,
                    Message = code.UNABLE_TO_LOCATE_RECORD_MSG,
                    Data = $"{accountNo} Not Found"
                };
            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }

        }

        public async Task<ViewApiResponse> SendOtpForPasswordReset(string accountNo)
        {
            if (String.IsNullOrEmpty(accountNo))
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = $"Enter a valid account no"
                };
            try
            {
                var fetcUserRecord = UserByAccountNoGet(accountNo).Result;
                if (fetcUserRecord.Status == code.UNABLE_TO_LOCATE_RECORD_CODE)
                    return fetcUserRecord;

                var userRecord = (User)fetcUserRecord.Data;

                var fetchOtpRecord = context.Otps.Where(x => x.AccountNo.Equals(accountNo) && x.Status.Equals("Unused OTP")).SingleOrDefaultAsync().Result;
                if (fetchOtpRecord == null)
                {
                    var generateOtpForResetPassword = GenerateOtp();

                    var viewOtp = new Otp
                    {
                        AccountNo = accountNo,
                        Email = userRecord.Email,
                        PhoneNumber = userRecord.PhoneNumber,
                        OtpCode = HashOtp(generateOtpForResetPassword),
                        OtpTimeCreated = DateTime.Now,
                        Status = "Unused OTP"
                    };

                    await context.Otps.AddAsync(viewOtp);
                    await context.SaveChangesAsync();

                    DateTime dateTime = DateTime.Now.AddMinutes(5);
                    TimeSpan span = dateTime.Subtract((DateTime)viewOtp.OtpTimeCreated);
                    double minutes = span.TotalMinutes;
                    double totalMinutes = Math.Round(minutes);

                    await emailService.SendEmailAsync(viewOtp.Email, "OTP", $"One Time Password: {generateOtpForResetPassword}.\n Valid for {totalMinutes} minutes");

                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = new ViewOtpResponse
                        {
                            Email = HideEmailValue(viewOtp.Email),
                            OtpValue = generateOtpForResetPassword,
                            OtpExpirationTime = totalMinutes
                        }
                    };
                }
                else
                {

                    var otpRecord = (Otp)fetchOtpRecord;

                    otpRecord.Status = "Expired OTP";

                    var local = context.Set<Otp>()
                            .Local
                            .FirstOrDefault(entry => entry.AccountNo.Equals(otpRecord.AccountNo) && entry.Status.Equals("Unused OTP"));

                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }

                    context.Otps.Update(otpRecord);
                    await context.SaveChangesAsync();

                    var generateOtpForResetPassword = GenerateOtp();

                    var viewOtp = new Otp
                    {
                        AccountNo = otpRecord.AccountNo,
                        Email = otpRecord.Email,
                        PhoneNumber = otpRecord.PhoneNumber,
                        OtpCode = HashOtp(generateOtpForResetPassword),
                        OtpTimeCreated = DateTime.Now,
                        Status = "Unused OTP"
                    };

                    await context.Otps.AddAsync(viewOtp);
                    await context.SaveChangesAsync();

                    DateTime dateTime = DateTime.Now.AddMinutes(5);
                    TimeSpan span = dateTime.Subtract((DateTime)viewOtp.OtpTimeCreated);
                    double minutes = span.TotalMinutes;
                    double totalMinutes = Math.Round(minutes);

                    await emailService.SendEmailAsync(viewOtp.Email, "OTP", $"One Time Password: {generateOtpForResetPassword}.\n Valid for {totalMinutes} minutes");

                    return new ViewApiResponse
                    {
                        Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                        Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                        Data = new ViewOtpResponse
                        {
                            Email = HideEmailValue(viewOtp.Email),    //viewOtp.Email,
                            OtpValue = generateOtpForResetPassword,
                            OtpExpirationTime = totalMinutes
                        }
                    };
                }

            }
            catch (Exception ex)
            {
                return new ViewApiResponse
                {
                    Status = code.STATUS_UNKNOWN_CODE,
                    Message = code.STATUS_UNKNOWN_MSG,
                    Data = ex.Message
                };
            }

        }

        public static string GenerateOtp()
        {
            Random r = new Random();
            var x = r.Next(0, 1000000);
            var generatedOtp = x.ToString("000000");
            return generatedOtp;
        }

        public static bool IsOtpExpired(DateTime time1)
        {
            if (DateTime.Now.AddMinutes(-5) > time1)
                return true;

            return false;
        }

        public static string HashOtp(string otp)
        {
            return BC.HashPassword(otp);
        }

        public static string HashPassword(string password)
        {
            return BC.HashPassword(password);
        }

        public static string HashPin(string pin)
        {
            return BC.HashPassword(pin);
        }

        public static string HideEmailValue(string emailValue)
        {
            string result = "";
            int startIndexOfEmail = 1;
            int endIndexOfEmail = emailValue.IndexOf('@');
            int actualIndexOfEmail = endIndexOfEmail - 2;

            for (int i = startIndexOfEmail; i < actualIndexOfEmail; i++)
            {
                string charAt = emailValue.Substring(startIndexOfEmail, actualIndexOfEmail);
                result = emailValue.Replace(charAt, "*****");
            }
            return result;
        }

        public static string CapitalizeFirstLetter(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        //public async Task<string> RetrieveBvn(string accountNo)
        //{
        //    var result = await GetAccountDetailsByAcctNumber(accountNo);

        //    var response = (ViewVerifyAccountDetails)result.Data;
        //    var bvn = response.BVN;

        //    return bvn;

        //}

        public async Task<ViewDetails> RetrieveDetails(string accountNo)
        {
            var result = await GetAccountDetailsByAcctNumber(accountNo);

            var response = (ViewVerifyAccountDetails)result.Data;

            var details = new ViewDetails
            {
                AccountName = response.AccountName,
                AccountStatus = response.AccountStatus,
                BranchCode = response.BranchCode,
                BranchName = response.BranchName,
                CustomerId = response.CustomerId,
                AccountType = response.AccountType,
                Bvn = response.BVN
            };

            return details;

        }

        public async Task<decimal> GetAccountBal(string accountNo)
        {
            var record = await GetAccountBalance(accountNo);
            if (record.Status == code.APPROVED_COMPLETED_SUCCESSFULLY_CODE)
            {
                var balance = (ViewAccountBalance)record.Data;

                return balance.AvailableBalance;
            }
            else
            {
                throw new Exception("Error fetching account balance");
            }
        }

        public ViewApiResponse LockAccount(User user)
        {

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddMinutes(10);
            context.SaveChanges();
            return new ViewApiResponse
            {
                Status = code.LOCKED_CODE,
                Message = code.LOCKED_MSG,
                Data = "Please change password"
            };

        }

        public ViewApiResponse UnlockAccount(User user)
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = null;
            context.SaveChanges();
            return new ViewApiResponse
            {
                Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
                Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
                Data = "Account Successfully Unlocked"
            };
        }


        #endregion



        #region To Delete

        // to be deleted
        //          if (!(user.AccessFailedCount >= 3))
        //            {
        //                user.LockoutEnabled = true;
        //                user.LockoutEnd = null;
        //                context.SaveChanges();
        //                return new ViewApiResponse
        //                {
        //                    Status = code.APPROVED_COMPLETED_SUCCESSFULLY_CODE,
        //                    Message = code.APPROVED_COMPLETED_SUCCESSFULLY_MSG,
        //                    Data = "Account Not Locked"
        //                };

        //}
        //            else
        //{
        //    user.LockoutEnabled = true;
        //    user.LockoutEnd = DateTime.Now.AddMinutes(10);
        //    context.SaveChanges();
        //    return new ViewApiResponse
        //    {
        //        Status = code.LOCKED_CODE,
        //        Message = code.LOCKED_MSG,
        //        Data = "Please change password"
        //    };
        //}

        // to be deleted

        #endregion




    }

    public enum AccountStatusCheck
    {
        NotPresent,
        PresentNotActivated,
        PresentActivated
    }
}
