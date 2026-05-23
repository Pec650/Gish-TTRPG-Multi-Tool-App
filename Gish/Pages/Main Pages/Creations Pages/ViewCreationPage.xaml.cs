using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages.Creations_Pages;

public partial class ViewCreationPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public ViewCreationPage(Creations creation)
    {
        InitializeComponent();
        
        if (creation is not null) UpdateUI(creation);
    }

    private void UpdateUI(Creations creation)
    {
        CreationTitle.Text = creation.Title;
        CreationType.Text = "Type: " + creation.CreationType.ToString();
        CreationSubType.Text = "Subtype: " + creation.CreationSubtype;
        CreationDesc.Text = (!IsEmptyInput(creation.Description)) ? creation.Description : "[Empty]";
        CreationFeature.Text = (!IsEmptyInput(creation.RPGSystem)) ? creation.RPGSystem : "[Empty]";
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        setAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);

        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }


    private void ReturnPage(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
    
    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }
}