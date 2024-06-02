using Client.Models;
using Client.Services;
using Microsoft.Extensions.Configuration;
namespace Client;

public partial class LessonPage : ContentPage
{
    private readonly NetworkService networkService;
    private readonly string contentUrl;
    private readonly IConfiguration configuration;
    private readonly ILessonContentParserService contentParserService;
    private List<PageElementModel>? elements;
    public LessonPage(NetworkService networkService, string contentUrl, IConfiguration configuration)
	{
        InitializeComponent();
        this.networkService = networkService;
        this.contentUrl = contentUrl;
        this.configuration = configuration;
        this.contentParserService = new LessonContentParserService(configuration);
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
            Application.Current.Windows[0].SizeChanged += LessonPage_SizeChanged;
        InitPage();
        
    }

    private void LessonPage_SizeChanged(object? sender, EventArgs e)
    {
        if (elements is not null && ContentStack.Children.Count > 0)
            contentParserService.UpdateStyles(elements, ContentStack);
    }

    private async void InitPage()
    {
        elements = await GetPageElements();
        if (elements is null) return;
        contentParserService.Parse(elements, ContentStack, OnContentButtonClicked);
    }
    private async Task<List<PageElementModel>?> GetPageElements()
    {
        return await networkService.GetRequest<List<PageElementModel>>(contentUrl);
    }
    private void OnContentButtonClicked(object sender, EventArgs e)
    {

    }
}