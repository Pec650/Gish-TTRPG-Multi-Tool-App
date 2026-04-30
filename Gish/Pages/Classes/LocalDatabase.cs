namespace ToDoList.Pages.Classes;
using SQLite;
using MauiApp1.Classes;

public class LocalDatabase
{
    public SQLiteAsyncConnection _connection;
    
    private async Task Init()
    {
        if (_connection is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "UserAccountData.db3");
        
        _connection = new SQLiteAsyncConnection(dbPath);
        await _connection.CreateTableAsync<UserAccount>();
    }
    
    public async Task<int> SaveUserAsync(UserAccount user)
    {
        await Init();
        return await _connection.InsertAsync(user);
    }

    public async Task<int?> getUserID(string email)
    {
        await Init();
        var user = await _connection.Table<UserAccount>()
                                    .FirstOrDefaultAsync(u => u.EmailAddress == email);
        return (user is not null) ?  user.ID : null;
    }

    public async Task<bool> matchUserByEmailPassword(string email, string password)
    {
        await Init();
        var user = await _connection.Table<UserAccount>()
                                    .Where(u => u.EmailAddress == email)
                                    .FirstOrDefaultAsync();
        if (user is null)
        {
            return false;
        }
        
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHashed);

        return isPasswordValid;
    }
}