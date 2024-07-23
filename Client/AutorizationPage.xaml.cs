using Client.Services;
using Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
namespace Client;

public partial class AutorizationPage : ContentPage
{
    private readonly NetworkService networkService;
    private readonly IConfiguration configuration;

    public AutorizationPage(NetworkService networkService, IConfiguration configuration)
    {
        InitializeComponent();
        this.networkService = networkService;
        this.configuration = configuration;

    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string login = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        if(string.IsNullOrEmpty(login) )
        {
            UsernameError.Text = "Помилка: Порожнє поле";
            UsernameError.IsVisible = true;
        }
        else
        {
            UsernameError.IsVisible = false;
        }
        if (string.IsNullOrEmpty(password))
        {
            PasswordError.Text = "Помилка: Порожнє поле";
            PasswordError.IsVisible = true;
        }
        else
        {
            PasswordError.IsVisible = false;
        }
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)) return;
        var model = new LoginModel() { login = login, password = password };

        LoginModelResponse? response = await networkService.PostRequest<LoginModel, LoginModelResponse>("api/account/login_to_account", model);
        if (response is null) { await DisplayAlert("Error", "Connection error or internal server error ", "OK"); return; }
        switch (response.code)
        {
            case 0:
                await Navigation.PushAsync(new HomePage(networkService, configuration));
                break;
            case 1:
                UsernameError.Text = "Помилка: Користувач не знайдений";
                UsernameError.IsVisible = true;
                break;
            case 2:
                PasswordError.Text = "Помилка: Неправильний пароль";
                PasswordError.IsVisible = true;
                break;



        }

    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string login = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        if (string.IsNullOrEmpty(login))
        {
            UsernameError.Text = "Помилка: Порожнє поле";
            UsernameError.IsVisible = true;
        }
        else
        {
            UsernameError.IsVisible = false;
        }
        if (string.IsNullOrEmpty(password))
        {
            PasswordError.Text = "Помилка: Порожнє поле";
            PasswordError.IsVisible = true;
        }
        else
        {
            PasswordError.IsVisible = false;
        }
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)) return;
        RegisterModel request = new RegisterModel()
        {
            Username = login,
            Password = password,
        };

        var response = await networkService.PostRequest<RegisterModel, RegisterModelResponse>("api/account/create_account", request);
        if (response is null) { await DisplayAlert("Error", "Connection error or internal server error ", "OK"); return; }

        var model = new LoginModel() { login = login, password = password };
        LoginModelResponse? response1 = await networkService.PostRequest<LoginModel, LoginModelResponse>("api/account/login_to_account", model);
        if (response1 is null) { await DisplayAlert("Error", "Connection error or internal server error ", "OK"); return; }
        if(response1.code == 0)
        {
            await Navigation.PushAsync(new HomePage(networkService, configuration));
        }

    }
}
