using POCModel.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POCModel
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public bool? IsDeleted { get; set; }

        /// Not possible to set ForeignKey because user table is in other database
        ////ref to ApplicationUser
        //[ForeignKey("ApplicationUser")]

        public string? CreatedById { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        //ref to ApplicationUser

        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// Not possible to set ForeignKey because user table is in other database
        /// </summary>
        //[ForeignKey("ApplicationUser")]
        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateTime { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string CreatedByIP { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string UpdatedByIP { get; set; }
    }
}
