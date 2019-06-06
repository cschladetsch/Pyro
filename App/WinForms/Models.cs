using Pyro;

namespace WinForms
{
    class UserModel
    {
        public string Email { get; set; }
        private IConstRef<Organisation> Organisation { get; set; }
    }

    class Organisation
    {
        public string Name { get; set; }
    }
}

