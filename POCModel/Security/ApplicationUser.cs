using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace POCModel.Security
{
    public class ApplicationUser : IdentityUser
    {

        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }



        public bool? IsDeleted { get; set; }
        public DateTime CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string CreatedByIP { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string UpdatedByIP { get; set; }
        //no need created by

       
    }
}