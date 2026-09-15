namespace _7_Builder.Models
{
    public class UserBuilder
    {
        private readonly User _user = new();

        public UserBuilder SetName(string name)
        {
            _user.Name = name;
            return this;
        }

        public UserBuilder SetEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder MakeAdmin()
        {
            _user.IsAdmin = true;
            return this;
        }

        public User Build()
        {
            return _user;
        }
    }
}
