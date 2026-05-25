using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages.Creations_Pages;

public partial class NewCreationPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public NewCreationPage()
    {
        InitializeComponent();
        CreationTypeInput.ItemsSource = Enum.GetNames(typeof(Creations.CreationTypeEnum));
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        setAllButtonState(true);
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

    private async void SubmitCreation(object? sender, EventArgs e)
    {
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
        
        bool success = await addCreation(title, type, subtype, description, feature);
        
        if (success)
        {
            Navigation.PopModalAsync();
        }
        else
        {
            LoadingUIState(false);
        }
    }

    private async Task<bool> addCreation(string title, Creations.CreationTypeEnum type, string subtype, string description, string feature)
    {
        try
        {
            string type_icon = type.ToString().ToLower() + "_tag_icon.svg";
            
            var newCreation = new Creations()
            {
                Title = title,
                UserID = App.GetUserId(),
                CreationType = type,
                TypeIconIMGSource = type_icon,
                CreationSubtype = subtype,
                Description = description,
                RPGSystem = feature,
                ModifyDate = DateTime.UtcNow
            };

            int result = await _database.SaveCreationAsync(newCreation);

            if (result <= 0)
            {
                ShowError("Unknown error has occurred");
                return false;
            }
            
            return true;
        }
        catch (SQLite.SQLiteException e) when (e.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase))
        {
            ShowError(title + " already exists");
            return false;
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
}