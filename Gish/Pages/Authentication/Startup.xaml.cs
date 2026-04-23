using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Pages;

public partial class Startup : ContentPage
{
    public Startup()
    {
        InitializeComponent();
    }

    private void RedirectToLogin(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new SignInPage());
    }

    private void RedirectToSignup(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new SignUpPage());
    }
}