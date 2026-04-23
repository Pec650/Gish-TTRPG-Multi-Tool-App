using System.Diagnostics;
using Newtonsoft.Json;

namespace ToDoList.Pages;

using System.Linq;

public partial class CompletedPage : ContentPage
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
    
    public CompletedPage()
    {
        InitializeComponent();
        completedCV.ItemsSource = ToDoService.CompletedItems;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        ToDoService.PurgeExpired();
        
        if (!App.isLoggedIn())
        {
            await ReturnToSignIn();
            return;
        }

        await GetUserCompletedTasks();
    }

    private async Task GetUserCompletedTasks()
    {
        try
        {
            if (App.CurrentUser == null) return;

            int userID = App.CurrentUser.id;
            var response = await _httpClient.GetAsync($"https://todo-list.dcism.org/getItems_action.php?status=inactive&user_id={userID}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var root = JsonConvert.DeserializeObject<TaskResponse>(jsonString);

                if (root != null && root.status == 200 && root.data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ToDoService.CompletedItems.Clear();
                
                        foreach (var entry in root.data.Values)
                        {
                            var itemData = new ToDoClass 
                            {
                                item_id = entry.item_id,
                                item_name = entry.item_name,
                                item_description = entry.item_description
                            };

                            var completedWrapper = new CompletedTask
                            {
                                Item = itemData,
                                CompletedAt = DateTime.Now
                            };
                    
                            ToDoService.CompletedItems.Add(completedWrapper);
                        }
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error fetching completed tasks: {e.Message}");
        }
    }
    
    private async Task ReturnToSignIn()
    {
        // App.resetUser();
        // await Shell.Current.GoToAsync("//SignInPage");
    }

    // private async void UndoComplete(object sender, EventArgs e)
    // {
    //     var button = (Button)sender;
    //     if (button == null || string.IsNullOrEmpty(button.ClassId)) return;
    //
    //     var completed = ToDoService.CompletedItems
    //         .FirstOrDefault(x => x.Item.item_id.ToString() == button.ClassId);
    //
    //     if (completed == null) return;
    //
    //     var statusData = new
    //     {
    //         @status_ = "active", 
    //         item_id = completed.Item.item_id
    //     };
    //
    //     var payload = new Dictionary<string, object>
    //     {
    //         { "status", "active" },
    //         { "item_id", completed.Item.item_id }
    //     };
    //
    //     bool success = await ChangeStatusOnServer(payload);
    //
    //     if (success)
    //     {
    //         MainThread.BeginInvokeOnMainThread(() =>
    //         {
    //             ToDoService.Items.Add(completed.Item);
    //             ToDoService.CompletedItems.Remove(completed);
    //         });
    //     }
    // }
    //
    // private async Task<bool> ChangeStatusOnServer(Dictionary<string, object> payload)
    // {
    //     try
    //     {
    //         var json = JsonConvert.SerializeObject(payload);
    //         var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    //
    //         var response = await _httpClient.PutAsync("https://todo-list.dcism.org/statusItem_action.php", content);
    //     
    //         return response.IsSuccessStatusCode;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.WriteLine($"Status Update Error: {e.Message}");
    //         return false;
    //     }
    // }
    
    private async void UndoComplete(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button == null || !int.TryParse(button.ClassId, out int itemId)) return;
        
        var completed = ToDoService.CompletedItems
            .FirstOrDefault(x => x.Item.item_id.ToString() == button.ClassId);
        
        if (completed == null) return;

        var payload = new
        {
            item_id = itemId,
            status = "active"
        };

        bool success = await UpdateTaskStatusOnServer(payload);

        if (success)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ToDoService.Items.Add(completed.Item);
                ToDoService.CompletedItems.Remove(completed);
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

    private async void DeleteComplete(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button == null || string.IsNullOrEmpty(button.ClassId)) return;

        bool confirm = await DisplayAlert("Delete Task", "This cannot be undone. Proceed?", "Delete", "Cancel");
        if (!confirm) return;

        if (int.TryParse(button.ClassId, out int itemId))
        {
            bool success = await DeleteTaskFromServer(itemId);

            if (success)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var completed = ToDoService.CompletedItems
                        .FirstOrDefault(x => x.Item.item_id == itemId);
                    if (completed != null)
                        ToDoService.CompletedItems.Remove(completed);
                });
            }
            else
            {
                await DisplayAlert("Error", "Server failed to delete the item.", "OK");
            }
        }
    }

    private async void DeleteAll(object sender, EventArgs e)
    {
        if (ToDoService.CompletedItems.Count == 0) return;

        bool confirm = await DisplayAlert("Delete All", "Clear all items from the server?", "Delete All", "Cancel");
        if (!confirm) return;

        var itemsToDelete = ToDoService.CompletedItems.ToList();

        foreach (var task in itemsToDelete)
        {
            await DeleteTaskFromServer(task.Item.item_id);
        }

        MainThread.BeginInvokeOnMainThread(() => ToDoService.CompletedItems.Clear());
    }
    
    private async Task<bool> DeleteTaskFromServer(int itemId)
    {
        try
        {
            string url = $"https://todo-list.dcism.org/deleteItem_action.php?item_id={itemId}";
        
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Delete Error: {error}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Network Error: {ex.Message}");
            return false;
        }
    }
}