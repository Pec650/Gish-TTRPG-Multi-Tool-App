using SQLite;
using System;

namespace Gish.Pages.Classes;


[Table("RPGSystems")]
public class RPGSystem
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    
    [Unique, MaxLength(100)]
    public string Name { get; set; } = string.Empty; // e.g., "Dungeons & Dragons 5e", "Pathfinder 2e"
}