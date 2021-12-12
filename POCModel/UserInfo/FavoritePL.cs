using System.ComponentModel.DataAnnotations;

namespace POCModel.UserInfo
{
    public class FavoritePL: BaseEntity
    {
        [Required(AllowEmptyStrings = false)]
        public string FavoriteProgrammingLanguages { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string FavoriteIDEs { get; set; }
    }

    
}
