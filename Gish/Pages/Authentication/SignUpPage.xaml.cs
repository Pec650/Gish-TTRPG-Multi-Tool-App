namespace Gish.Pages.Authentication;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes;
using Main_Pages;

public partial class SignUpPage
{
    private readonly LocalDatabase _database = new LocalDatabase();

    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void SubmitSignUp(object? sender, EventArgs e)
    {
        try
        {
            string username = UsernameInput.Text;
            string email = EmailInput.Text;
            string password = PasswordInput.Text;
            string confirmPass = ConfirmPassInput.Text;
        
            if (IsEmptyInput(username) || IsEmptyInput(email) ||
                IsEmptyInput(password) || IsEmptyInput(confirmPass))
            {
                ShowError("Please input all fields");
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Invalid email address");
                return;
            }

            if (!IsValidPassword(password))
            {
                return;
            }

            if (!String.Equals(password, confirmPass))
            {
                ShowError("Passwords do not match");
                return;
            }
        
            RemoveError();
            LoadingUiState(true);

            bool success = await SignupUser(username, email, password);

            if (success)
            {
                GoToMain();
            }
            else
            {
                LoadingUiState(false);
            }
        }
        catch
        {
            ShowError("Unknown error has occurred");
            LoadingUiState(false);
        }
    }

    private async Task<bool> SignupUser(string username, string email, string password)
    {
        try
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new UserAccount
            {
                Username = username,
                EmailAddress = email,
                PasswordHashed = hashedPassword
            };

            int result = await _database.SaveUserAsync(newUser);

            if (result <= 0)
            {
                ShowError("Unknown error has occurred");
                return false;
            }

            int? userId = await _database.getUserID(email);

            if (userId is null)
            {
                ShowError("Unknown error has occurred");
                return false;
            }
            
            App.SetUserId(userId.Value);
            return true;
        }
        catch (SQLite.SQLiteException e) when (e.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Email address already exists");
            return false;
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            return false;
        }
    }

    private void GoToMain()
    {
        App.SetMainPage(new MainContainerPage());
    }
    
    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }

    private bool IsValidEmail(String email)
    {
        return Regex.IsMatch(email,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase);
    }

    private bool IsValidPassword(String password)
    {
        if (password.Length < 8 || password.Length > 16)
        {
            ShowError("Password must be 8-16 letters");
            return false;
        }

        if (!password.Any(char.IsLetterOrDigit))
        {
            ShowError("Password must have a digit [0-9]");
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            ShowError("Password must have a lowercase [a-z]");
            return false;
        }
        
        if (!password.Any(char.IsUpper))
        {
            ShowError("Password must have a uppercase [A-Z]");
            return false;
        }

        return true;
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
    
    private void LoadingUiState(bool isLoading)
    {
        if (isLoading)
        {
            SetAllButtonState(true);
            LoadingIndicator.IsRunning = true;
        }
        else
        {
            SetAllButtonState(true);
            LoadingIndicator.IsRunning = false;
        }
    }
    
    private void SetAllButtonState(bool isEnable)
    {
        BackButtonBtn.IsEnabled = isEnable;
        SubmitBtn.IsEnabled = isEnable;
    }
    
    private void InputChanged(object? sender, TextChangedEventArgs e)
    {
        RemoveError();
    }
    
    private void ReturnPage(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void ToggleShowPassword(object? sender, EventArgs e)
    {
        UpdatePasswordState(PasswordInput, TogglePasswordBtn);
    }

    private void ToggleShowConfirmPassword(object? sender, EventArgs e)
    {
        UpdatePasswordState(ConfirmPassInput, ToggleConfirmPasswordBtn);
    }
    
    private void UpdatePasswordState(Entry passwordInput, ImageButton toggleButton)
    {
        String openEye = "show_pass_eye.png";
        String closedEye = "show_pass_close_eye.png";
        
        passwordInput.IsPassword = !passwordInput.IsPassword;
        toggleButton.Source = (passwordInput.IsPassword) ? openEye : closedEye;
    }
}