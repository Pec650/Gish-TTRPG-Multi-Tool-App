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
        if (user is not null) user.PasswordHashed = null;
        return user;
    }
    
    // public async Task<bool> setProfileImage(int id, FileResult img)
    // {
    //     try
    //     {
    //         byte[] newProfilePic = await UserAccount.convertImageToByte(img);
    //
    //         if (newProfilePic is null)
    //         {
    //             return false;
    //         }
    //         
    //         await Init();
    //         var user = await _connection.Table<UserAccount>()
    //             .FirstOrDefaultAsync(u => u.ID == id);
    //
    //         if (user is null)
    //         {
    //             return false;
    //         }
    //
    //         user.ProfileImage = newProfilePic;
    //         await this.UpdateAsync(user);
    //     }
    //     catch
    //     {
    //         return false;
    //     }
    //     return false;
    // }
    
    public static async Task<byte[]> convertImageToByte(FileResult img)
    {
        if (isFileImage(img.FileName))
        {
            try
            {
                using var stream = await img.OpenReadAsync();
                using var memoryStream = new MemoryStream();
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
    
    private static bool isFileImage(string fileName)
    {
        string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".webp", ".gif", ".bmp" };
        string extension = Path.GetExtension(fileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}