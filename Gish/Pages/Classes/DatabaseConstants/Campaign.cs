using SQLite;
using System;

namespace Gish.Pages.Classes;

[Table("Campaigns")]
public class Campaign
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    [Indexed]
    public int RPGSystemID { get; set; } // Foreign Key tying this campaign back to a specific system

    [MaxLength(150)]
    public string Title { get; set; } = string.Empty; // e.g., "Curse of Strahd"
}