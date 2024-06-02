using Client.Services;
using Client.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using System.Windows.Input;
namespace Client;

public partial class HomePage : ContentPage, INotifyPropertyChanged
{
    private readonly NetworkService networkService;
    private readonly IConfiguration configuration;
    private ObservableCollection<LessonModel>? _lessons;
    private ObservableCollection<TestModel>? _tests;
    private IDisplayInfoService displayInfo;
    public ObservableCollection<LessonModel>? Lessons
    {
        get => _lessons;
        set
        {
            _lessons = value;
            OnPropertyChanged("Lessons");
        }
    }
    public ObservableCollection<TestModel>? Tests
    {
        get => _tests;
        set
        {
            _tests = value;
            OnPropertyChanged("Tests");
        }
    }

    private string _usernameText = string.Empty;
    public string UsernameText
    {
        get => _usernameText;
        set
        {
            _usernameText = value;
            OnPropertyChanged("UsernameText");
        }
    }

    private int _lessonsSpanCount;
    public int LessonsSpanCount
    {
        get => _lessonsSpanCount;
        set
        {
            _lessonsSpanCount = value;
            OnPropertyChanged("LessonsSpanCount");
        }
    }

    private string _activeMenuItem = "Lessons";
    public string ActiveMenuItem
    {
        get => _activeMenuItem;
        set
        {
            _activeMenuItem = value;
            OnPropertyChanged();
        }
    }

    private double _contentWidth = 0;
    public double ContentWidth 
    {
        get => _contentWidth;
        set
        {
            _contentWidth = value;
            OnPropertyChanged("ContentWidth");
        }
    }

    public ICommand SetActiveMenuItemCommand { get; }
    public ICommand LessonItemTappedCommand { get; }
    public ICommand TestItemTappedCommand { get; }

    AccountModel? account;
    protected override bool OnBackButtonPressed()
    {
        return true;
    }
    public HomePage(NetworkService networkService, IConfiguration configuration)
	{
        
        InitializeComponent();
        displayInfo = new DisplayInfoService();
        this.networkService = networkService;
        this.configuration = configuration;
        GetData();
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
            Application.Current.Windows[0].SizeChanged += HomePage_SizeChanged;
        LessonItemTappedCommand = new Command<LessonModel>(OnLessonItemTapped);
        TestItemTappedCommand = new Command<TestModel>(OnTestItemTapped);
        SetActiveMenuItemCommand = new Command<string>(SetActiveMenuItem);
        ActiveMenuItem = "Lessons";
        BindingContext = this;


    }
    private void SetActiveMenuItem(string menuItem)
    {
        ActiveMenuItem = menuItem;
    }

    private void HomePage_SizeChanged(object? sender, EventArgs e)
    {
        double width = displayInfo.GetDisplaySize().Width;
        UpdateSpanCount(width);
    }

    private async void GetData()
    {
        account = await networkService.GetRequest<AccountModel>("api/account");
        UsernameText = $"Вітаю, {account?.Username}!";
        var tempLessons = await networkService.GetRequest<ObservableCollection<LessonModel>>("api/lessons/get_all");
        if (tempLessons is not null)
        {
            foreach (var lesson in tempLessons)
            {
                lesson.ImageUrl = lesson.ImageUrl.StartsWith("http") ? lesson.ImageUrl : configuration["Server:Domain"] + lesson.ImageUrl;
                lesson.ContentUrl = lesson.ContentUrl.StartsWith("http") ? lesson.ContentUrl : configuration["Server:Domain"] + lesson.ContentUrl;
            }
        }
        Lessons = tempLessons;
        var tempTests = await networkService.GetRequest<ObservableCollection<TestModel>>("api/test/get_all_tests");
        if (tempTests is not null)
        {
            foreach (var test in tempTests)
            {
                test.ContentUrl = test.ContentUrl.StartsWith("http") ? test.ContentUrl : configuration["Server:Domain"] + test.ContentUrl;
            }
        }
        Tests = tempTests;
        UpdateSpanCount(displayInfo.GetDisplaySize().Width);
    }
    private void UpdateSpanCount(double width)
    {
        if (width < 400)
            LessonsSpanCount = 1;
        else if (width < 1100 && DeviceInfo.Platform == DevicePlatform.WinUI)
            LessonsSpanCount = 2;
        else if (width < 1100 && DeviceInfo.Platform == DevicePlatform.Android)
            LessonsSpanCount = 1;
        else if (width < 1600)
            LessonsSpanCount = 4;
        else
            LessonsSpanCount = 6;
        ContentWidth = width / LessonsSpanCount * 0.9;
    }

    private async void OnLessonItemTapped(LessonModel item)
    {
        if (item == null)
            return;
        await Navigation.PushAsync(new LessonPage(networkService, item.ContentUrl, configuration));
    }
    private async void OnTestItemTapped(TestModel item)
    {
        if (item == null)
            return;
        await Navigation.PushAsync(new TestPage(networkService, configuration, item));
    }
    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        networkService.DeleteTokenCookie();
        await Shell.Current.GoToAsync("..");
    }

    private void CurrentPasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(!string.IsNullOrEmpty(CurrentPasswordEntry.Text) && !string.IsNullOrEmpty(NewPasswordEntry.Text))
            SetNewPasswordButton.IsEnabled = true;
        else
            SetNewPasswordButton.IsEnabled = false;

    }

    private async void DeleteProfileButton_Clicked(Object sender, EventArgs e)
    {
        await networkService.DeleteRequest("api/account/delete_account");
        networkService.DeleteTokenCookie();
        await Shell.Current.GoToAsync("..");
    }
    private async void SetNewPasswordButton_Clicked(object sender, EventArgs e)
    {
        PasswordChangeRequest message = new PasswordChangeRequest()
        {
            CurrentPassword = CurrentPasswordEntry.Text,
            NewPassword = NewPasswordEntry.Text,
        };
        var response = await networkService.PatchRequest<PasswordChangeRequest, PasswordChangeResponse>("api/account/change_own_password", message);
        if (response is null) return;
        switch (response.Code)
        {
            case 0:
                ChangePasswordResultLabel.IsVisible = true;
                ChangePasswordResultLabel.Text = "Пароль успішно змінений";
                break;
            case 1:
                await Shell.Current.GoToAsync("..");
                break;
            case 2:
                ChangePasswordResultLabel.IsVisible = true;
                ChangePasswordResultLabel.Text = "Введено неправильний поточний пароль";
                break;
        }
    }
}