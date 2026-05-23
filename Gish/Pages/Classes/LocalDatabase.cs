namespace Gish.Pages.Classes;
using SQLite;

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
        await _connection.CreateTableAsync<Creations>();
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
    
    public async Task<UserAccount> getUserInfo(int id)
    {
        await Init();
        var user = await _connection.Table<UserAccount>()
            .FirstOrDefaultAsync(u => u.ID == id);
        return user;
    }

    public async Task<bool> updateUserInfo(UserAccount user)
    {
        if (user is null)
        {
            return false;
        }

        int rowsAffected = await _connection.UpdateAsync(user);
            
        return rowsAffected > 0;
    }
    
    public async Task<byte[]> convertImageToByte(FileResult img)
    {
        if (isFileImage(img.FileName))
        {
            try
            {
                var stream = await img.OpenReadAsync();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch
            {
                return null;
            }
        }
        return null;
    }
    
    public static bool isFileImage(string fileName)
    {
        string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".webp", ".gif", ".bmp" };
        string extension = Path.GetExtension(fileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
    
    public async Task<int> SaveCreationAsync(Creations creation)
    {
        await Init();
        return await _connection.InsertAsync(creation);
    }
    
    public async Task<List<Creations>> GetAllCreations()
    {
        await Init();
        List<Creations> creations = await _connection.Table<Creations>().ToListAsync();
        return creations;
    }
}