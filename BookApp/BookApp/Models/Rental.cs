
namespace BookApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Rental
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }

        [Required(ErrorMessage ="Error: Guardian Name Required!")]
        [Display(Name="Full Name")]
        public string GuardianName { get; set; }

        [Required(ErrorMessage = "Error: Guardian Surname Required!")]
        [Display(Name = "Last Name")]
        public string GuardianSurname { get; set; }

        [Required(ErrorMessage = "Error: Guardian ID Number Required!")]
        [Display(Name = "ID Number")]
        public string GuardianID { get; set; }

        [Required(ErrorMessage = "Error: Guardian Cell Number Required!")]
        [Display(Name = "Cell Number")]
        public string GuardianCell { get; set; }

        [Required(ErrorMessage = "Error: Residential Address Required!")]
        [Display(Name = "Residential Address")]
        public string GuardianAddress { get; set; }

        [Display(Name = "Penalty?")]
        public Nullable<bool> HasPenalty { get; set; }

        [Display(Name = "Rental Date")]
        [DataType(DataType.Date)]
        public System.DateTime DateOfRent { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public System.DateTime DateOfReturn { get; set; }

        [Display(Name = "Date Expected")]
        [DataType(DataType.Date)]
        public System.DateTime ExpectedDate { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
