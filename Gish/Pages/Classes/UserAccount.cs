namespace MauiApp1.Classes;

using SQLite;

public class UserAccount
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string Username { get; set; }
    
    [Unique]
    public string EmailAddress { get; set; }
    
    public string PasswordHashed { get; set; }
}