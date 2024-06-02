using Client.Models;
using Client.Services;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Diagnostics;
namespace Client;

public partial class TestPage : ContentPage, INotifyPropertyChanged
{
    private readonly NetworkService networkService;
    private readonly IConfiguration configuration;
    private readonly TestModel testModel;
    private TestParserService testParserService;
    private bool _testResultVisible = false;
    public bool TestResultVisible
    {
        get => _testResultVisible;
        set
        {
            _testResultVisible = value;
            OnPropertyChanged("TestResultVisible");
        }
    }
    private List<QuestionModel>? Questions {  get; set; }


    private string _correctPercentText = "";
    public string CorrectPercentText
    {
        get => _correctPercentText;
        set
        {
            _correctPercentText = value;
            OnPropertyChanged("CorrectPercentText");
        }
    }

    private double _correctPercentValue = 0;
    public double CorrectPercentValue
    {
        get => _correctPercentValue;
        set
        {
            _correctPercentValue = value;
            OnPropertyChanged("CorrectPercentValue");
        }
    }
    public string TestName
    {
        get => testModel.Title;
    }
    private string _testSummaryText = "";
    public string TestSummaryText
    {
        get => _testSummaryText;
        set
        {
            _testSummaryText = value;
            OnPropertyChanged("TestSummaryText");
        }
    }

    public TestPage(NetworkService networkService, IConfiguration configuration, TestModel testModel)
	{
		InitializeComponent();
        this.networkService = networkService;
        this.configuration = configuration;
        this.testModel = testModel;
        this.testParserService = new TestParserService(configuration, TestsStackLayout);
        BindingContext = this;
        GetData();
    }
    private async void GetData()
    {
        Questions = await networkService.GetRequest<List<QuestionModel>>(testModel.ContentUrl);
        foreach(var question in Questions)
        {
            if(!string.IsNullOrEmpty(question.Image))
                question.Image = question.Image.StartsWith("http") ? question.Image : configuration["Server:Domain"] + question.Image;
        }
        testParserService.Parse(Questions);
    }

    private async void CheckButton_Clicked(object sender, EventArgs e)
    {
        if (Questions is null) return;
        List<UserAnswer> userAnswers = new List<UserAnswer>();
        foreach (var question in Questions)
        {
            UserAnswer answer = null;

            switch (question.Type)
            {
                case "single":
                    var selectedRadioButton = TestsStackLayout.Children
                        .OfType<Grid>()
                        .SelectMany(grid => grid.Children.OfType<FlexLayout>())
                        .SelectMany(layout => layout.Children.OfType<RadioButton>())
                        .FirstOrDefault(rb => rb.GroupName == question.Id.ToString() && rb.IsChecked);
                    if (selectedRadioButton != null)
                    {
                        answer = new UserAnswer { QuestionId = question.Id, AnswerValue = selectedRadioButton.Value.ToString() };
                    }
                    break;

                case "multiple":
                    var selectedCheckBoxes = TestsStackLayout.Children
                        .OfType<Grid>()
                        .SelectMany(grid => grid.Children.OfType<FlexLayout>())
                        .SelectMany(layout => layout.Children.OfType<Grid>())
                        .SelectMany(temp => temp.Children.OfType<CheckBox>())
                        .Where(cb => cb.IsChecked && cb.ClassId.Split(';')[0] == question.Id.ToString());
                    if (selectedCheckBoxes.Any())
                    {
                        var answerValue = string.Join(" ", selectedCheckBoxes.Select(cb => cb.ClassId.Split(';')[1]));
                        answer = new UserAnswer { QuestionId = question.Id, AnswerValue = answerValue };
                    }
                    break;

                case "matching":
                    var selectedPickers = TestsStackLayout.Children
                        .OfType<Grid>()
                        .SelectMany(grid => grid.Children.OfType<FlexLayout>())
                        .SelectMany(layout => layout.Children.OfType<Grid>())
                        .Select(grid => new
                        {
                            Pair = grid.Children.OfType<Picker>().FirstOrDefault()?.BindingContext as KeyValuePair<string, string>?,
                            SelectedItem = grid.Children.OfType<Picker>().FirstOrDefault()?.SelectedItem?.ToString()
                        })
                        .Where(p => p.Pair != null && p.SelectedItem != null);

                    if (selectedPickers.Any())
                    {
                        var answerValue = string.Join(" ", selectedPickers.Select(p => $"{p.Pair.Value.Key}{p.Pair.Value.Value}"));
                        answer = new UserAnswer { QuestionId = question.Id, AnswerValue = answerValue };
                    }
                    break;
            }

            if (answer != null)
            {
                userAnswers.Add(answer);
            }
        }
        var result = await networkService.PostRequest<List<UserAnswer>, ChechTestResult>($"api/test/check/{testModel.Id}", userAnswers);
        if (result is null) return;
        TestResultVisible = true;
        if (result.TotalPoints != 0)
            CorrectPercentValue = (double)result.PointsScored / (double)result.TotalPoints;
        CorrectPercentText = $"Ваш результат: {(int)Math.Round(CorrectPercentValue * 100)}%";
        if (CorrectPercentValue * 100 < 40)
            TestSummaryText = "Поганий результат. Треба повторити матеріал";
        else if (CorrectPercentValue * 100 < 70)
            TestSummaryText = "Непогано, але можна краще!";
        else
            TestSummaryText = "Віаю! Непоганий результат!";

    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void RetryButton_Clicked(object sender, EventArgs e)
    {
        testParserService.Parse(Questions);
        TestResultVisible = false;
    }
}