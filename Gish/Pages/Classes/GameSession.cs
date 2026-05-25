using SQLite;
using System;

namespace Gish.Pages.Classes;

[Table("GameSessions")]
public class GameSession
{
    [PrimaryKey, AutoIncrement]
    public int SessionID { get; set; }

    [Indexed]
    public int CampaignID { get; set; } // Foreign Key linking this session back to its parent campaign

    public int UserID { get; set; }
    
    public string Title { get; set; } = string.Empty; // e.g., "Session 12: The Death House"
    
    public DateTime Date { get; set; } // The calendar day date component
    
    public TimeSpan StartTime { get; set; } // e.g., 13:30:00 (1:30 PM)
    
    public string FormattedStartTime => DateTime.Today.Add(StartTime).ToString("h:mm tt");
    
    public TimeSpan EndTime { get; set; } // Calculated or explicitly stored to enforce duration padding
    
    public GameSession() 
    {
        UserID = App.GetUserId();
    }
}