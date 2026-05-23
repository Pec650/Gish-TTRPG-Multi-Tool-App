using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages.Creations_Pages;

public partial class EditCreationPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();

    private int CreationID = -1;
    
    public EditCreationPage(Creations creation)
    {
        InitializeComponent();
        CreationTypeInput.ItemsSource = Enum.GetNames(typeof(Creations.CreationTypeEnum));

        if (creation is not null)
        {
            UpdateEntries(creation);
        }
        else
        {
            GoBackToView();
        }
    }

    private void UpdateEntries(Creations creation)
    {
        CreationID = creation.ID;

        TitleInput.Text = creation.Title;
        CreationTypeInput.SelectedItem = creation.CreationType.ToString();
        SubtypeInput.Text = creation.CreationSubtype;
        CreationDesc.Text = creation.Description;
        CreationFeature.Text = creation.RPGSystem;
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

    private async void SubmitCreation(object? sender, EventArgs e)
    {
        if (CreationID == -1)
        {
            GoBackToView();
            return;
        }
        
        string title = TitleInput.Text;
        Creations.CreationTypeEnum type;
        string subtype = SubtypeInput.Text;
        string description = CreationDesc.Text;
        string feature = CreationFeature.Text;

        if (IsEmptyInput(title) || IsEmptyInput(subtype))
        {
            ShowError("Please enter title and subtype");
            return;
        }
        
        if (CreationTypeInput.SelectedIndex != -1)
        {
            type = (Creations.CreationTypeEnum)CreationTypeInput.SelectedIndex;
        }
        else
        {
            ShowError("Please select a type");
            return;
        }
        
        RemoveError();
        LoadingUIState(true);
        
        bool success = await updateCreation(title, type, subtype, description, feature);
        
        if (success)
        {
            GoBackToView();
        }
        else
        {
            LoadingUIState(false);
        }
    }

    private async Task<bool> updateCreation(string title, Creations.CreationTypeEnum type, string subtype, string description, string feature)
    {
        try
        {
            string type_icon = type.ToString().ToLower() + "_tag_icon.svg";
            
            var newCreation = new Creations()
            {
                ID = CreationID,
                Title = title,
                CreationType = type,
                TypeIconIMGSource = type_icon,
                CreationSubtype = subtype,
                Description = description,
                RPGSystem = feature
            };

            bool result = await _database.updatedCreationInfo(newCreation);

            if (!result)
            {
                ShowError("Unknown error has occurred");
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            return false;
        }
    }
    
    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }
    
    private void ShowError(String errorMsg)
    {
        InputError.Text = errorMsg;
        InputError.IsVisible = true;
    }
    
    private void RemoveError()
    {
        InputError.IsVisible = false;
    }
    
    private void LoadingUIState(bool isLoading)
    {
        if (isLoading)
        {
            SubmitBtn.IsEnabled = false;
            LoadingIndicator.IsRunning = true;
        }
        else
        {
            SubmitBtn.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void GoBackToView()
    {
        try
        {
            setAllButtonState(false);
            
            await Navigation.PopModalAsync();
        }
        catch
        {
            setAllButtonState(true);
        }
    }
}