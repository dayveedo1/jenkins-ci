using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ViewModels
{
    public class ViewLoginUser
    {
        [Required]
        public String AccountNo { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Password must be 6-15 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}
