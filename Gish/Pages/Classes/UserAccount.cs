namespace Gish.Pages.Classes;

using SQLite;

public class UserAccount
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string Username { get; set; } = string.Empty;
    
    [Unique]
    public string EmailAddress { get; set; } = string.Empty;
    
    public string PasswordHashed { get; set; } = string.Empty;
    
    // Made nullable to support default fallback placeholder icons cleanly
    public byte[]? ProfileImage { get; set; }
    
    public UserAccount()
    {
        ProfileImage = null;
    }
}