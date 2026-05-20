using SQLite;
using Gish.Pages.Classes;

using System.Diagnostics;
using Debug = System.Diagnostics.Debug;

namespace Gish.Pages.MainPages;

public partial class Scheduler : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public Scheduler()
    {
        InitializeComponent();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        

        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);
    }
}