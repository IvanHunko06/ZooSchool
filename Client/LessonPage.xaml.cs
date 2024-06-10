using Client.Models;
using Client.Services;
using Microsoft.Extensions.Configuration;
namespace Client;

public partial class LessonPage : ContentPage
{
    private readonly NetworkService networkService;
    private readonly LessonModel model;
    private readonly IConfiguration configuration;
    private readonly ILessonContentParserService contentParserService;
    private List<PageElementModel>? elements;

    private bool _isFavourite = false;
    public bool IsFavourite
    {
        get => _isFavourite;
        set
        {
            _isFavourite = value;
            OnPropertyChanged();
        }
    }
    public LessonPage(NetworkService networkService, LessonModel model, IConfiguration configuration)
	{
        InitializeComponent();
        this.networkService = networkService;
        this.model = model;
        this.configuration = configuration;
        this.contentParserService = new LessonContentParserService(configuration);
        BindingContext = this;
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
            Application.Current.Windows[0].SizeChanged += LessonPage_SizeChanged;

        IsFavourite = model.IsFavourite;

        
    }

    private void LessonPage_SizeChanged(object? sender, EventArgs e)
    {
        if (elements is not null && ContentStack.Children.Count > 0)
            contentParserService.UpdateStyles(elements, ContentStack);
    }

    private async Task InitPage()
    {
        elements = await GetPageElements();
        if (elements is null) return;
        contentParserService.Parse(elements, ContentStack, OnContentButtonClicked);
    }
    private async Task<List<PageElementModel>?> GetPageElements()
    {
        return await networkService.GetRequest<List<PageElementModel>>(model.ContentUrl);
    }
    private void OnContentButtonClicked(object sender, EventArgs e)
    {

    }

    private async void FavouriteButton_Clicked(object sender, EventArgs e)
    {
        if (!IsFavourite)
        {
            int response = await networkService.PostRequest($"api/lessons/add_to_favourite/{model.Id}");
            if (response.ToString()[0] == '2')
                IsFavourite = true;
        }
        else
        {
            int response = await networkService.DeleteRequest($"api/lessons/remove_from_favourite/{model.Id}");
            if (response.ToString()[0] == '2')
                IsFavourite = false;
        }
    }

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        await InitPage();
    }
}