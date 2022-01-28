using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace WebApi.Models
{
    public class WithdrawFormModel
    {
        [Required]
        [Display(Name = "Withdraw Address")]
        [RegularExpression("^3[MNP][1-9A-HJ-NP-Za-km-z]{33}$", ErrorMessage = "Invalid address.")]
        public string DestinationAddress { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public long Amount { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Required]
        [Display(Name = "2FA Code")]
        public int TwoFactorAuthCode { get; set; }

        [Required]
        [Range(typeof(Network), "1","10")]
        public Network Network { get; set; }
    }
}