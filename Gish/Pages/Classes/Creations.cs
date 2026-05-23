using System.Runtime.InteropServices.JavaScript;

namespace Gish.Pages.Classes;

using SQLite;

public class Creations
{
    public enum CreationTypeEnum
    {
        Subclass,
        Lineage,
        Monster,
        Spell,
        Feat
    }
    
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public int UserID { get; set;  }
    
    [Unique]
    public string Title { get; set; }
    
    public CreationTypeEnum CreationType { get; set; }
    
    public string TypeIconIMGSource { get; set; }
    
    public string CreationSubtype { get; set; }
    
    public string Description { get; set; }
    
    public string RPGSystem { get; set; }
    
    public DateTime ModifyDate { get; set; }

    public Creations()
    {
        UserID = App.getUserID();
        ModifyDate = DateTime.UtcNow;
    }
}