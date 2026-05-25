using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages.Creations_Pages;

public partial class ViewCreationPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();

    private Creations SelectedCreation = null;
    
    public ViewCreationPage(Creations creation)
    {
        InitializeComponent();
        
        if (creation is not null) UpdateUI(creation);
    }

    private void UpdateUI(Creations creation)
    {
        SelectedCreation = creation;
        
        CreationTitle.Text = creation.Title;
        CreationType.Text = "Type: " + creation.CreationType.ToString();
        CreationSubType.Text = "Subtype: " + creation.CreationSubtype;
        CreationDesc.Text = (!IsEmptyInput(creation.Description)) ? creation.Description : "[Empty]";
        CreationFeature.Text = (!IsEmptyInput(creation.RPGSystem)) ? creation.RPGSystem : "[Empty]";
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        setAllButtonState(true);

        if (SelectedCreation is not null)
        {
            SelectedCreation = await _database.GetCreationInfo(SelectedCreation.ID);
            UpdateUI(SelectedCreation);
        }
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        cachedButtons = App.GetAllButtons(this);
        cachedImgButtons = App.GetAllImageButtons(this);

        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.SetButtonState(cachedButtons, enable);
        App.SetImageButtonState(cachedImgButtons, enable);
    }


    private void ReturnPage(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
    
    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }

    private async void OnClickDeleteCreation(object? sender, EventArgs e)
    {
        if (SelectedCreation is not null)
        {
            try
            {
                await _database.DeleteCreationAsync(SelectedCreation.ID);
            }
            catch {}
        }
        Navigation.PopModalAsync();
    }

    private async void OnClickEditCreation(object? sender, EventArgs e)
    {
        try
        {
            setAllButtonState(false);

            if (SelectedCreation is not null)
            {
                await Navigation.PushModalAsync(new EditCreationPage(SelectedCreation));
            }

        }
        catch
        {
            setAllButtonState(true);
        }
    }
}