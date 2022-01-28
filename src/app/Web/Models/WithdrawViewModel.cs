using Microsoft.AspNetCore.Mvc;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Model;

namespace Web.Models
{
    public class WithdrawViewModel
    {
        public UserAccount UserAccount { get; set; }
        public List<UserWithdraw> Withdraws { get; set; }
        public WithdrawFormViewModel Form { get; set; }
    }

    public class WithdrawFormViewModel
    {
        [Required]
        [Display(Name = "Withdraw Address")]
        [RegularExpression("^3[MNP][1-9A-HJ-NP-Za-km-z]{33}$", ErrorMessage = "Invalid address.")]
        public string DestinationAddress { get; set; }

        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(decimal), "0,001", "100", ErrorMessage = "Invalid amount.")]
        [Remote("ValidateAmount", "Withdraw", AdditionalFields = "Network", ErrorMessage = "No funds.")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public Network Network { get; set; }
    }
}