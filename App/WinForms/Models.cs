namespace WinForms
{
    using Pyro;

    internal class UserModel
    {
        public string Email { get; set; }
        private IConstRef<Organisation> Organisation { get; set; }
    }

    internal class Organisation
    {
        public string Name { get; set; }
    }
}

