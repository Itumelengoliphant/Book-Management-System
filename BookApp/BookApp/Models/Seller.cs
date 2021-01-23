
namespace BookApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Seller
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Enquiry { get; set; }
        public string Address { get; set; }
        public int BookId { get; set; }
    
        public virtual Book Book { get; set; }
    }
}
