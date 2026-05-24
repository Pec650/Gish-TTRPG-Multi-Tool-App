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
        await _connection.CreateTableAsync<Initiative>();
        await _connection.CreateTableAsync<GameSession>();
        await _connection.CreateTableAsync<RPGSystem>();
        await _connection.CreateTableAsync<Campaign>();
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
    
    public async Task<Creations> GetCreationInfo(int id)
    {
        await Init();
        Creations creation = await _connection.Table<Creations>().FirstOrDefaultAsync(c => c.ID == id);
        return creation;
    }
    
    public async Task<List<Creations>> GetAllCreations(string searchString, bool hasSubclass, bool hasLineage, bool hasMonster, bool hasSpell, bool hasFeat)
    {
        await Init();
        var query = _connection.Table<Creations>();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(c => c.Title.ToLower().Contains(searchString.ToLower()));
        }

        bool isAnyFilterActive = hasSubclass || hasLineage || hasMonster || hasSpell || hasFeat;

        if (isAnyFilterActive)
        {
            query = query.Where(c => 
                (hasSubclass && c.CreationType == Creations.CreationTypeEnum.Subclass) ||
                (hasLineage  && c.CreationType == Creations.CreationTypeEnum.Lineage)  ||
                (hasMonster  && c.CreationType == Creations.CreationTypeEnum.Monster)  ||
                (hasSpell    && c.CreationType == Creations.CreationTypeEnum.Spell)    ||
                (hasFeat     && c.CreationType == Creations.CreationTypeEnum.Feat)
            );
        }
        
        int userID = App.getUserID();
        List<Creations> creations = await query.Where(c => c.UserID == userID)
                                               .OrderByDescending(c => c.ModifyDate)
                                               .ToListAsync();
        
        return creations;
    }

    public async Task<List<Creations>> GetRecentCreations()
    {
        await Init();
        int userID = App.getUserID();
        List<Creations> creations = await _connection.Table<Creations>()
                                                     .Where(c => c.UserID == userID)
                                                     .OrderByDescending(c => c.ModifyDate)
                                                     .Take(3)
                                                     .ToListAsync();
        return creations;
    }
    
    public async Task<bool> updatedCreationInfo(Creations creation)
    {
        await Init();
        
        if (creation is null)
        {
            return false;
        }

        int rowsAffected = await _connection.UpdateAsync(creation);
            
        return rowsAffected > 0;
    }
    
    public async Task<bool> DeleteCreationAsync(int id)
    {
        await Init();
        return await _connection.DeleteAsync<Creations>(id) != 0;
    }

    public async Task<int> AddInitiative(bool isPlayer)
    {
        await Init();
        Initiative newInitiative = new Initiative()
        {
            UserID = App.getUserID(),
            isPlayer = isPlayer
        };
        return await _connection.InsertAsync(newInitiative);
    }
    
    public async Task<int> UpdateInitiative(Initiative item)
    {
        await Init();
        return await _connection.UpdateAsync(item);
    }

    public async Task<List<Initiative>> GetAllInitiativeInOrder()
    {
        await Init();
        int userID = App.getUserID();
        List<Initiative> orderedList = await _connection.Table<Initiative>()
            .Where(i => i.UserID == userID)
            .OrderByDescending(i => i.InitiativeNum)
            .ToListAsync();
        return orderedList;
    }

    public async Task<bool> DeleteAllInitiative()
    {
        await Init();
        int userID = App.getUserID();
        int rowsAffected = await _connection.ExecuteAsync("DELETE FROM Initiative WHERE UserID = ?", userID);
        return rowsAffected > 0;
    }
    
    public async Task<bool> DeleteInitiativeAsync(int id)
    {
        await Init();
        int rowsAffected = await _connection.Table<Initiative>()
            .DeleteAsync(x => x.ID == id);
        return rowsAffected > 0;
    }
    
    public async Task<List<RPGSystem>> GetAllSystemsAsync()
    {
        await Init();
        return await _connection.Table<RPGSystem>().ToListAsync();
    }

    public async Task<List<Campaign>> GetCampaignsBySystemAsync(int systemId)
    {
        await Init();
        return await _connection.Table<Campaign>().Where(c => c.RPGSystemID == systemId).ToListAsync();
    }

    public async Task<List<GameSession>> GetSessionsByCampaignAsync(int campaignId)
    {
        await Init();
        return await _connection.Table<GameSession>().Where(s => s.CampaignID == campaignId).ToListAsync();
    }

    // --- GAME SESSION CORE ENGINES WITH 3-HOUR VALIDATION ANCHORS ---

    public async Task<List<GameSession>> GetAllSessionsAsync()
    {
        await Init();
        return await _connection.Table<GameSession>().OrderBy(s => s.Date).ToListAsync();
    }

    public async Task<List<GameSession>> GetSessionsForDayAsync(DateTime targetDate)
    {
        await Init();
        return await _connection.Table<GameSession>()
                                 .Where(s => s.Date == targetDate.Date)
                                 .ToListAsync();
    }

    public async Task<bool> IsSessionTimeValidAsync(DateTime targetDate, TimeSpan proposedStart, int currentSessionId = 0)
    {
        await Init();
        
        // Pull down all booked sessions occurring on that specific target day
        var daysSessions = await GetSessionsForDayAsync(targetDate);

        foreach (var bookedSession in daysSessions)
        {
            // If editing an existing session, ignore self-evaluation check triggers
            if (bookedSession.SessionID == currentSessionId)
                continue;

            // Calculate the absolute distance delta separating the two times
            TimeSpan delta = (proposedStart - bookedSession.StartTime).Duration();

            if (delta.TotalHours < 3.0)
            {
                return false; // Collision detected! Violates the 3-hour buffer constraint
            }
        }

        return true; // Safe to book! No scheduling overlap conflicts found
    }

    public async Task<int> SaveSessionWithValidationAsync(GameSession session)
    {
        await Init();

        // Enforce the 3-hour calendar rule check
        bool isValid = await IsSessionTimeValidAsync(session.Date, session.StartTime, session.SessionID);
        if (!isValid)
        {
            throw new InvalidOperationException("Scheduling conflict: Sessions must be spaced at least 3 hours apart.");
        }

        if (session.SessionID != 0)
        {
            return await _connection.UpdateAsync(session);
        }
        else
        {
            return await _connection.InsertAsync(session);
        }
    }

    public async Task<int> DeleteSessionAsync(GameSession session)
    {
        await Init();
        return await _connection.DeleteAsync(session);
    }
}