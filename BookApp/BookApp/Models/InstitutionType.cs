
namespace BookApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class InstitutionType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InstitutionType()
        {
            this.Institutions = new HashSet<Institution>();
        }
    
        public int id { get; set; }

        [Required(ErrorMessage ="University Type is required!")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Institutions { get; set; }
    }
}
