
namespace BookApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            this.Sellers = new HashSet<Seller>();
            this.Rentals = new HashSet<Rental>();
        }
    
        public int Id { get; set; }

        [Required(ErrorMessage ="Book Title is required!")]
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int InstitutionId { get; set; }

        [Required(ErrorMessage = "Book ISBN is required!")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Book Author is required!")]
        public string Author { get; set; }
        public Nullable<int> Edition { get; set; }

        [Required(ErrorMessage = "Book Publisher is required!")]
        public string Publisher { get; set; }

        [Display(Name = "Module Code")]
        [Required(ErrorMessage = "Module Code is required!")]
        public string ModuleCode { get; set; }
        public Nullable<int> ConditionId { get; set; }
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Book Description is required!")]
        public string Description { get; set; }
        public string Image { get; set; }

        [Required(ErrorMessage = "Book Owner is required!")]
        public string Owner { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public virtual Category Category { get; set; }
        public virtual Condition Condition { get; set; }
        public virtual Institution Institution { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Seller> Sellers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
