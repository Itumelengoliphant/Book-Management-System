
namespace BookApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string StudentNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int GenderId { get; set; }
        public string Address { get; set; }
        public int InstitutionId { get; set; }
        public string IdentityNumber { get; set; }
        public string Password { get; set; }
        public System.DateTime DOB { get; set; }
    
        public virtual Gender Gender { get; set; }
        public virtual Institution Institution { get; set; }
    }
}
