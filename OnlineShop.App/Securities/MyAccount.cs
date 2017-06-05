namespace OnlineShop.App.Securities
{
    public class MyAccount
    {
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string fullname;

        public string Fullname
        {
            get { return fullname; }
            set { fullname = value; }
        }
        
        private string roles;

        public string Roles
        {
            get { return roles; }
            set { roles = value; }
        }

        public MyAccount()
        { }

        public MyAccount(string username, string fullname, string roles)
        {
            this.username = username;
            this.fullname = fullname;           
            this.roles = roles;
        }

    }
}