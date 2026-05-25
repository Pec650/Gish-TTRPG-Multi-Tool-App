using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages.Profile_Pages;

public partial class EditProfilePage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    private byte[] ProfileImageResult = null;
    
    public EditProfilePage()
    {
        InitializeComponent();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        setAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        SetUserInfo();
        
        cachedButtons = App.GetAllButtons(this);
        cachedImgButtons = App.GetAllImageButtons(this);

        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.SetButtonState(cachedButtons, enable);
        App.SetImageButtonState(cachedImgButtons, enable);
    }
    
    public async void SetUserInfo()
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.GetUserId());

            if (user is not null)
            {
                UsernameInput.Text = user.Username;

                if (user.ProfileImage is not null)
                {
                    ProfileImageResult = user.ProfileImage;
                    ProfilePictureInput.Source = ImageSource.FromStream(() => new MemoryStream(user.ProfileImage));
                }
            }
            else
            {
                UsernameInput.Text = "";
            }
        }
        catch
        {
            UsernameInput.Text = "";
        }
    }

    private async void ReturnPage(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            await Navigation.PopModalAsync();
        }
        catch
        {
            setAllButtonState(true);
        }
    }

    private async void ConfirmChange(object? sender, EventArgs e)
    {
        string username = UsernameInput.Text;
        
        if (IsEmptyInput(username))
        {
            ShowError("Please input all fields");
            return;
        }
        
        RemoveError();
        LoadingUIState(true);

        bool success = await updateUserInfo(username);

        if (success)
        {
            await GoToProfilePage();
        }
        
        LoadingUIState(false);
    }
    
    private async Task<bool> updateUserInfo(String username)
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.GetUserId());

            if (user is null)
            {
                ShowError("Unknown error has occurred");
                return false;
            }

            user.Username = username;
            user.ProfileImage = ProfileImageResult;
            
            bool updateSuccess = await _database.updateUserInfo(user);

            if (!updateSuccess)
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
    
    private async Task GoToProfilePage()
    {
        try
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
        catch
        {
            ShowError("Unable to navigate to profile page");
        }
    }
    
    private void LoadingUIState(bool isLoading)
    {
        if (isLoading)
        {
            setAllButtonState(false);
            LoadingIndicator.IsRunning = true;
        }
        else
        {
            setAllButtonState(true);
            LoadingIndicator.IsRunning = false;
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

    private async void OnSelectFileClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Please select a file",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                ProfileImageResult = await _database.convertImageToByte(result);
                ProfilePictureInput.Source = ImageSource.FromFile(result.FullPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}