namespace POCModel.Security
{
    public class VMRegister
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// 0 means free user
        /// </summary>
        public decimal PaymentAmount { get; set; }


        //dummy data in separate db or shared user db
        public string FavoriteProgrammingLanguages { get; set; }
    
        public string FavoriteIDEs { get; set; }
    }
}
