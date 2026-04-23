using System.Diagnostics;
using Newtonsoft.Json;

namespace ToDoList.Pages;

public partial class NewTask : ContentPage
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public NewTask()
    {
        InitializeComponent();
    }

    private async void AddTask(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleEntry.Text))
        {
            await DisplayAlert("Error", "Please enter a task title.", "OK");
            return;
        }

        var payload = new
        {
            item_name = titleEntry.Text,
            item_description = detailsEditor.Text ?? "N/A",
            user_id = App.CurrentUser.id
        };

        bool success = await SendTaskToServer(payload);

        if (success)
        {
            ToDoService.Add(titleEntry.Text, detailsEditor.Text ?? "N/A");

            titleEntry.Text = string.Empty;
            detailsEditor.Text = string.Empty;

            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Error", "Could not save task to server.", "OK");
        }
    }
    
    private async Task<bool> SendTaskToServer(object data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://todo-list.dcism.org/addItem_action.php", content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Server Response: {result}");
                return true;
            }
        
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Add Task Error: {ex.Message}");
            return false;
        }
    }

    private async void CancelNewTask(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}