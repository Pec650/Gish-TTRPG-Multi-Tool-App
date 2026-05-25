namespace Gish.Pages.Classes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using SQLite;

public class Initiative : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public int UserID { get; set;  }
    
    public int InitiativeNum { get; set; }
    
    public string Name { get; set; }
    
    public bool isPlayer { get; set; }
    
    private bool _showDeleteIcon;
    public bool ShowDeleteIcon
    {
        get => _showDeleteIcon;
        set
        {
            if (_showDeleteIcon != value)
            {
                _showDeleteIcon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowTypeLabel));
            }
        }
    }

    public bool ShowTypeLabel => !ShowDeleteIcon;

    public Initiative()
    {
        UserID = App.GetUserId();
        InitiativeNum = 1;
        Name = "";
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}