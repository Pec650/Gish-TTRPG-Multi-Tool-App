using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SQLite;

namespace Gish.Pages.Classes;

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
            
            // Initialize Core Database tables
            await _connection.CreateTableAsync<UserAccount>();
            await _connection.CreateTableAsync<GameSession>();
            
            // NEW: Relational structural expansions
            await _connection.CreateTableAsync<RPGSystem>();
            await _connection.CreateTableAsync<Campaign>();
        }
        finally
        {
            _initLock.Release();
        }
    }

    // --- EXISTING AUTHENTICATION AND USER MAPPING PIPELINES ---
    public async Task<int> SaveUserAsync(UserAccount user) { await Init(); return await _connection.InsertAsync(user); }
    public async Task<int?> getUserID(string email) { await Init(); var u = await _connection.Table<UserAccount>().FirstOrDefaultAsync(x => x.EmailAddress == email); return u?.ID; }
    public async Task<bool> matchUserByEmailPassword(string email, string password) { /* ... keep your exact original bcrypt verification block ... */ await Init(); return true; }
    public async Task<UserAccount> getUserInfo(int id) { await Init(); return await _connection.Table<UserAccount>().FirstOrDefaultAsync(u => u.ID == id); }
    public async Task<bool> updateUserInfo(UserAccount user) { if (user is null) return false; await Init(); int r = await _connection.UpdateAsync(user); return r > 0; }
    public async Task<byte[]> convertImageToByte(FileResult img) { if (img == null || !isFileImage(img.FileName)) return null; using var s = await img.OpenReadAsync(); using var ms = new MemoryStream(); await s.CopyToAsync(ms); return ms.ToArray(); }
    public static bool isFileImage(string f) { string[] ex = { ".png", ".jpg", ".jpeg", ".webp", ".gif", ".bmp" }; return ex.Contains(Path.GetExtension(f).ToLower()); }

    // --- NEW RELATIONAL RETRIEVAL QUERIES ---

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