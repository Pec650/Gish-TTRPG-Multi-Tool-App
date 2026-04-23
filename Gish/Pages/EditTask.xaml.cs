using System.Diagnostics;
using Newtonsoft.Json;

namespace ToDoList.Pages;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class EditTask : ContentPage
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    private ToDoClass? _item;

    public string ItemId
    {
        set
        {
            _item = ToDoService.Items.FirstOrDefault(x => x.item_id.ToString() == value);
            if (_item != null)
            {
                titleEntry.Text = _item.item_name;
                detailsEditor.Text = _item.item_description;
            }
        }
    }

    public EditTask()
    {
        InitializeComponent();
    }

    private async void SaveEdit(object sender, EventArgs e)
    {
        if (_item == null || string.IsNullOrWhiteSpace(titleEntry.Text)) return;

        var payload = new
        {
            item_id = _item.item_id,
            item_name = titleEntry.Text,
            item_description = detailsEditor.Text ?? string.Empty
        };

        bool success = await UpdateTaskOnServer(payload);

        if (success)
        {
            ToDoService.Update(_item, titleEntry.Text, detailsEditor.Text ?? string.Empty);
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Error", "Could not update task on the server.", "OK");
        }
    }

    private async Task<bool> UpdateTaskOnServer(object data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("https://todo-list.dcism.org/editItem_action.php", content);
        
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Update Error: {ex.Message}");
            return false;
        }
    }

    private async void CancelEdit(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}