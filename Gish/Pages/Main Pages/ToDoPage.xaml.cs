namespace ToDoList.Pages;

using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;

public partial class ToDoPage : ContentPage
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public class ItemProfile
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
    }
        
    public class TaskResponse
    {
        public int status { get; set; }
        public Dictionary<string, ItemProfile> data { get; set; }
        public int count { get; set; }
    }
    
    public ToDoPage()
    {
        InitializeComponent();
        todoCV.ItemsSource = ToDoService.Items;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
    
        if (!App.isLoggedIn())
        {
            await ReturnToSignIn();
            return;
        }
        
        await GetUserTasks();
    }
    
    private async Task GetUserTasks()
    {
        try
        {
            if (App.CurrentUser == null) return;

            int userID = App.CurrentUser.id;
            var response = await _httpClient.GetAsync($"https://todo-list.dcism.org/getItems_action.php?status=active&user_id={userID}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var root = JsonConvert.DeserializeObject<TaskResponse>(jsonString);

                if (root != null && root.status == 200 && root.data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ToDoService.Items.Clear();
                    
                        foreach (var entry in root.data.Values)
                        {
                            var newItem = new ToDoClass 
                            {
                                item_id = entry.item_id,
                                item_name = entry.item_name,
                                item_description = entry.item_description
                            };
                        
                            ToDoService.Items.Add(newItem);
                        }
                        UpdateTitle();
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error: {e.Message}");
        }
    }
    
    private async Task ReturnToSignIn()
    {
        // App.resetUser();
        // await Shell.Current.GoToAsync("//SignInPage");
    }
    
    // private async void CompleteToDoItem(object sender, EventArgs e)
    // {
    //     var button = (Button)sender;
    //     if (button == null || !int.TryParse(button.ClassId, out int itemId)) return;
    //
    //     bool success = await UpdateTaskStatusOnServer(itemId);
    //
    //     if (success)
    //     {
    //         MainThread.BeginInvokeOnMainThread(() =>
    //         {
    //             var item = ToDoService.Items.FirstOrDefault(x => x.item_id == itemId);
    //             if (item != null)
    //             {
    //                 ToDoService.Complete(item);
    //                 UpdateTitle();
    //             }
    //         });
    //     }
    //     else
    //     {
    //         await DisplayAlert("Error", "Server failed to update. Check connection.", "OK");
    //     }
    // }
    //
    // private async Task<bool> UpdateTaskStatusOnServer(int itemId)
    // {
    //     try 
    //     {
    //         var formData = new Dictionary<string, string>
    //         {
    //             { "status ", "inactive" },
    //             { "item_id", itemId.ToString() }
    //         };
    //
    //         var content = new FormUrlEncodedContent(formData);
    //
    //         _httpClient.DefaultRequestHeaders.Clear();
    //         _httpClient.DefaultRequestHeaders.Add("X-HTTP-Method-Override", "PUT");
    //
    //         var response = await _httpClient.PostAsync("https://todo-list.dcism.org/statusItem_action.php", content);
    //     
    //         string responseBody = await response.Content.ReadAsStringAsync();
    //         Debug.WriteLine($"SERVER RESPONSE: {responseBody}");
    //
    //         return response.IsSuccessStatusCode;
    //     } 
    //     catch (Exception ex) 
    //     { 
    //         Debug.WriteLine($"Network Error: {ex.Message}");
    //         return false; 
    //     }
    // }
    
    private async void CompleteToDoItem(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button == null || !int.TryParse(button.ClassId, out int itemId)) return;

        var payload = new
        {
            item_id = itemId,
            status = "inactive"
        };

        bool success = await UpdateTaskStatusOnServer(payload);

        if (success)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var item = ToDoService.Items.FirstOrDefault(x => x.item_id == itemId);
                if (item != null)
                {
                    ToDoService.Complete(item);
                    UpdateTitle();
                }
            });
        }
        else
        {
            await DisplayAlert("Error", "Could not update task on the server.", "OK");
        }
    }

    private async Task<bool> UpdateTaskStatusOnServer(object data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("https://todo-list.dcism.org/statusItem_action.php", content);
        
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Update Error: {ex.Message}");
            return false;
        }
    }
    
    private void UpdateTitle()
    {
        listTitle.Text = ToDoService.Items.Count == 0 ? "Empty List" : "Your To-Do List";
    }
    private async void EditListTitle(object? sender, TappedEventArgs e)
    {
        string result = await DisplayPromptAsync(
            "Rename List", "Enter a new title:",
            initialValue: listTitle.Text,
            maxLength: 30);

        if (!string.IsNullOrWhiteSpace(result))
            listTitle.Text = result;
    }

    private async void EditToDoItem(object? sender, EventArgs e)
    {
        var button = (Button)sender;
        await Shell.Current.GoToAsync($"{nameof(EditTask)}?itemId={button.ClassId}");
    }

    private async void GoToNewTask(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewTask));
    }
}