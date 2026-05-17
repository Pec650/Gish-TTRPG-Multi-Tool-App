namespace Gish.Pages.Classes;
using SQLite;

public class LocalDatabase
{
    public SQLiteAsyncConnection _connection;

    private SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
    
    private async Task Init()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_connection is not null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "UserAccountData.db3");
            _connection = new SQLiteAsyncConnection(dbPath);
            await _connection.CreateTableAsync<UserAccount>();
        }
        finally
        {
            _initLock.Release();
        }
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
            return false;

        // ✅ Guard against null/invalid hash
        if (string.IsNullOrWhiteSpace(user.PasswordHashed))
        {
            System.Diagnostics.Debug.WriteLine(">> PasswordHashed is null or empty for user: " + email);
            return false;
        }

        try
        {
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHashed);
            return isPasswordValid;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">> BCrypt.Verify failed: {ex.Message}");
            return false;
        }
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
            return false;

        await Init();  // add this
        int rowsAffected = await _connection.UpdateAsync(user);
        return rowsAffected > 0;
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


    //DEBUGGING PURPOSES ONLY
    public async Task DebugPrintUser(string email)
    {
        await Init();
        var user = await _connection.Table<UserAccount>()
                                    .FirstOrDefaultAsync(u => u.EmailAddress == email);

        if (user is null)
        {
            System.Diagnostics.Debug.WriteLine(">> No user found for: " + email);
            return;
        }

        System.Diagnostics.Debug.WriteLine($">> User ID: {user.ID}");
        System.Diagnostics.Debug.WriteLine($">> Email: {user.EmailAddress}");
        System.Diagnostics.Debug.WriteLine($">> PasswordHashed: '{user.PasswordHashed}'");
    }
}