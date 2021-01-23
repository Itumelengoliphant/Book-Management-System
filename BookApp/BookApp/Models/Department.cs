
namespace BookApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Department
    {
        public int Id { get; set; }

        [Display(Name="Department Name")]
        public string Name { get; set; }

        [Display(Name = "Department Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int CampusId { get; set; }
    
        public virtual Campus Campu { get; set; }
    }
}
