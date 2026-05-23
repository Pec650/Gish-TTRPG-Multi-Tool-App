using System.Collections.ObjectModel;
using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages.Tools_Pages;

public partial class Initiative_Tracker : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public ObservableCollection<Initiative> MyDataList { get; set; } = new();

    private bool deleteMode = false;
    
    public Initiative_Tracker()
    {
        InitializeComponent();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        setAllButtonState(true);
        
        try
        {
            var rawList = await _database.GetAllInitiativeInOrder();
        
            MyDataList.Clear();
        
            foreach (var item in rawList)
            {
                MyDataList.Add(item);
            }
        
            InitiativeLists.ItemsSource = MyDataList;
        } catch {}
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

    private void ToggleDeleteMode(object? sender, EventArgs e)
    {
        deleteMode = !deleteMode;
        UpdateDeleteItems();
    }

    private void UpdateDeleteItems()
    {
        if (deleteMode)
        {
            ToggleDeleteBtn.BackgroundColor = Color.FromArgb("#839788");
            LastColumnHeader.Text = "DELETE";
                
        }
        else
        {
            ToggleDeleteBtn.BackgroundColor = Color.FromArgb("#FE5F55"); 
            LastColumnHeader.Text = "TYPE";
        }
        
        if (MyDataList != null)
        {
            foreach (var item in MyDataList)
            {
                item.ShowDeleteIcon = deleteMode; 
            }
        }
    }

    private async void ClickedAddPlayer(object? sender, EventArgs e)
    {
        try
        {
            int result = await _database.AddInitiative(true);

            if (result > 0)
            {
                Initiative newItem = new Initiative()
                {
                    ID = result,
                    UserID = App.getUserID(),
                    isPlayer = true,
                    ShowDeleteIcon = deleteMode
                };
            
                MyDataList.Add(newItem);
            }
        } catch {}
    }

    private async void ClickedAddEnemy(object? sender, EventArgs e)
    {
        try
        {
            int result = await _database.AddInitiative(false);

            if (result > 0)
            {
                Initiative newItem = new Initiative()
                {
                    ID = result,
                    UserID = App.getUserID(),
                    isPlayer = false,
                    ShowDeleteIcon = deleteMode
                };
            
                MyDataList.Add(newItem);
            }
        } catch {}
    }

    private async void ClickedOrder(object? sender, EventArgs e)
    {
        try
        {
            var rawList = await _database.GetAllInitiativeInOrder();
        
            MyDataList.Clear();
        
            foreach (var item in rawList)
            {
                MyDataList.Add(item);
            }
            
            deleteMode = false;
            UpdateDeleteItems();
        
            InitiativeLists.ItemsSource = MyDataList;
        } catch {}
    }

    private async void ClickedReset(object? sender, EventArgs e)
    {
        bool isConfirmed = await DisplayAlert(
            "Reset Tracker?",
            "Are you sure you want to clear all initiatives? This cannot be undone.",
            "Continue", 
            "Cancel"
        );
        
        if (isConfirmed) {
            try
            {
                bool result = await _database.DeleteAllInitiative();
                if (result)
                {
                    try
                    {
                        var rawList = await _database.GetAllInitiativeInOrder();
        
                        MyDataList.Clear();
        
                        foreach (var item in rawList)
                        {
                            MyDataList.Add(item);
                        }

                        deleteMode = false;
                        UpdateDeleteItems();
                        
                        InitiativeLists.ItemsSource = MyDataList;
                    } catch {}
                }
            } catch {}
        }
    }
    
    private async void OnRowDataChanged(object? sender, TextChangedEventArgs textChangedEventArgs)
    {
        if (sender is Entry entry)
        {
            if (entry.BindingContext is Initiative alteredItem)
            {
                try
                {
                    await _database.UpdateInitiative(alteredItem);
                }
                catch {}
            }
        }
    }

    private async void OnDeleteItemClicked(object? sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            if (button.CommandParameter is Initiative selectedItem)
            {
                bool answer = await DisplayAlert("Delete Entry", $"Are you sure you want to remove this entry?", "Yes", "No");
                if (!answer) return;

                try
                {
                    bool isDeleted = await _database.DeleteInitiativeAsync(selectedItem.ID); 

                    if (isDeleted)
                    {
                        MyDataList.Remove(selectedItem);
                        InitiativeLists.ItemsSource = MyDataList;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to delete item from database.", "OK");
                    System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
                }
            }
        }
    }
}