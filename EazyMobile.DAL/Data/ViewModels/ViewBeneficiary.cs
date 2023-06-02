using EazyMobile.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewBeneficiary
    {
        public string UserId { get; set; }
        public string BeneficiaryAccountNo { get; set; }
        public string BeneficiaryAccountName { get; set; }
        public string BeneficiaryBank { get; set; }

        public static implicit operator ViewBeneficiary(Beneficiary beneficiary)
        {
            return new ViewBeneficiary
            {
                UserId = beneficiary.UserId,
                BeneficiaryAccountNo = beneficiary.BeneficiaryAccountNo,
                BeneficiaryAccountName = beneficiary.BeneficiaryAccountName,
                BeneficiaryBank = beneficiary.BeneficiaryBank
            };
        }

        public static implicit operator Beneficiary(ViewBeneficiary beneficiary)
        {
            return new Beneficiary
            {
                UserId = beneficiary.UserId,
                BeneficiaryAccountNo = beneficiary.BeneficiaryAccountNo,
                BeneficiaryAccountName = beneficiary.BeneficiaryAccountName,
                BeneficiaryBank = beneficiary.BeneficiaryBank
            };
        }
    }
}
