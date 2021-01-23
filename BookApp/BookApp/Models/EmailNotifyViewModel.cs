using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookApp.Models
{
    public class EmailNotifyViewModel
    {
        [Required]
        [Display(Name ="To(Email Address)")]
        public string ToMail { get; set; }
    }
}