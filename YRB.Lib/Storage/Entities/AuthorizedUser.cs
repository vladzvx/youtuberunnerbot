namespace YRB.Lib.Storage.Entities
{
    public class AuthorizedUser
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? Username { get; set; }
    }
}
