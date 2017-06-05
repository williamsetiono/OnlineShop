using System;
using System.Linq;
using System.Security.Principal;

namespace OnlineShop.App.Securities
{
    public class MyPrincipal : IPrincipal
    {
        private MyAccount ma = new MyAccount();

        public MyAccount Ma
        {
            get { return ma; }
            set { ma = value; }
        }

        public IIdentity Identity
        {
            get;
            set;
        }

        public MyPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return this.ma.Roles.Equals(role);
        }
    }
}