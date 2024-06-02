
using Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;

namespace Client.Services;

class TestParserService
{
    private readonly IConfiguration configuration;
    private readonly VerticalStackLayout questionsStack;

    public TestParserService(IConfiguration configuration, VerticalStackLayout questionsStack)
    {
        this.configuration = configuration;
        this.questionsStack = questionsStack;
    }
    public void Parse(List<QuestionModel> questions)
    {
        questionsStack.Clear();
        questionsStack.Spacing = 10;
        foreach (var question in questions)
        {
            var questionGrid = new Grid();
            if (!string.IsNullOrEmpty(question.Label))
            {
                questionGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));
                Label label = new Label();
                ApplyStyles(question.Styles, label);
                label.Text = question.Label;
                Grid.SetRow(label, 0);
                questionGrid.Add(label);
            }

            if (!string.IsNullOrEmpty(question.Image))
            {
                questionGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));
                Image image = new Image();
                image.Source = question.Image;
                if(string.IsNullOrEmpty(question.Label))
                    Grid.SetRow(image, 0);
                else
                    Grid.SetRow(image, 1);
                questionGrid.Add(image);
            }
            if (question.Pairs is not null || question.Options is not null)
                questionGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));

            switch (question.Type)
            {
                case "single":
                    CreateSingleQuestion(question, questionGrid);
                    break;
                case "multiple":
                    CreateMultipleQuestion(question, questionGrid);
                    break;
                case "matching":
                    CreateMatchingQuestion(question, questionGrid);
                    break;

            }
            questionsStack.Add(questionGrid);

        }
    }
    private void CreateMatchingQuestion(QuestionModel question, Grid questionGrid)
    {
        if (question.Pairs is null) return;
        FlexLayout layout = new FlexLayout()
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Column,
            Wrap = Microsoft.Maui.Layouts.FlexWrap.NoWrap,
        };
        Grid.SetRow(layout, questionGrid.RowDefinitions.Count - 1);
        questionGrid.Children.Add(layout);

        var leftOptions = new Dictionary<string, string>();
        var allRightOptions = new Dictionary<string, string>();

        foreach (var pair in question.Pairs)
        {
            leftOptions.Add(pair.LeftId, pair.Left);
            allRightOptions.Add(pair.RightId, pair.Right);
        }
        if (question.Options is not null)
        {
            foreach (var pair in question.Options)
            {
                allRightOptions.Add(pair.Id, pair.Text);
            }
        }

        foreach (var pair in question.Pairs)
        {
            Grid rowStack = new Grid();
            rowStack.ColumnSpacing = 10;
            rowStack.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            rowStack.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var leftLabel = new Label
            {
                Text = leftOptions[pair.LeftId],
                VerticalOptions = LayoutOptions.Center
            };

            var picker = new Picker
            {
                Title = "Select",
                BindingContext = pair.LeftId 
            };

            if (question.Styles.ContainsKey("option-font-size") && double.TryParse(question.Styles["option-font-size"], out double value))
            {
                leftLabel.FontSize = value;
                picker.FontSize = value;
            }

            foreach (var option in allRightOptions)
            {
                picker.Items.Add(option.Value);
                picker.ItemDisplayBinding = new Binding("Value");
            }

            picker.SelectedIndexChanged += (s, e) =>
            {
                if (picker.SelectedIndex != -1)
                {
                    picker.BindingContext = new KeyValuePair<string, string>(
                        pair.LeftId, allRightOptions.ElementAt(picker.SelectedIndex).Key
                    );
                }
            };

            Grid.SetColumn(leftLabel, 0);
            Grid.SetColumn(picker, 1);
            rowStack.Add(leftLabel);
            rowStack.Add(picker);
            layout.Add(rowStack);
        }
    }




    private void CreateMultipleQuestion(QuestionModel question, Grid questionGrid)
    {
        if (question.Options is null) return; 
        FlexLayout layout = new FlexLayout()
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,

        };
        Grid.SetRow(layout, questionGrid.RowDefinitions.Count - 1);
        questionGrid.Children.Add(layout);
        foreach (var option in question.Options)
        {
            Grid temp = new Grid();
            temp.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
            temp.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
            var checkbox = new CheckBox()
            {
                ClassId = $"{question.Id};{option.Id.ToString()}"
            };
            Grid.SetColumn(checkbox, 0);
            temp.Add(checkbox);
            var label = new Label()
            {
                Text = option.Text,
                VerticalTextAlignment = TextAlignment.Center,
            };
            if(question.Styles.ContainsKey("option-font-size") && double.TryParse(question.Styles["option-font-size"], out double value))
            {
                label.FontSize = value;
            }
            Grid.SetColumn(label, 1);
            temp.Add(label);
            if (question.Styles.ContainsKey("option-margin"))
            {
                string[] margins = question.Styles["option-margin"].Trim().Split(' ');
                if (margins.Length == 1)
                {
                    double result;
                    if (double.TryParse(margins[0], out result))
                        temp.Margin = new Thickness(result);
                }
                else if (margins.Length == 2)
                {
                    if(double.TryParse(margins[0], out double left) && double.TryParse(margins[1], out double top))
                        temp.Margin = new Thickness(left, top);
                }
                else if (margins.Length == 4)
                {
                    if(double.TryParse(margins[0], out double left) && double.TryParse(margins[1], out double top)
                        && double.TryParse(margins[2], out double right) && double.TryParse(margins[3], out double bottom))
                        temp.Margin = new Thickness(left, top, right, bottom);
                }
            }
            layout.Children.Add(temp);
        }
    }
    private void CreateSingleQuestion(QuestionModel question, Grid questionGrid)
    {
        if (question.Options is null) return;
        FlexLayout layout = new FlexLayout()
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            
        };
        Grid.SetRow(layout, questionGrid.RowDefinitions.Count - 1);
        questionGrid.Children.Add(layout);

        foreach(var option in question.Options)
        {
            RadioButton radioButton = new RadioButton()
            {
                GroupName = question.Id.ToString(),
                Content = option.Text,
                Value = option.Id,
            };
            ApplyStyles(question.Styles, radioButton);
            layout.Children.Add(radioButton);
        }
    }
    private void ApplyStyles(Dictionary<string, string> styles, View element)
    {
        foreach(var style in styles)
        {
            switch (style.Key)
            {
                case "label-font-size":
                    if(element is Label label)
                    {
                        if(double.TryParse(style.Value, out var value))
                            label.FontSize = value;
                    }
                    break;
                case "option-font-size":
                    if(element is RadioButton radioButton)
                    {
                        if (double.TryParse(style.Value, out var value))
                            radioButton.FontSize = value;
                    }
                    break;
                case "option-margin":
                    string[] margins = style.Value.Trim().Split(' ');
                    if (margins.Length == 1)
                    {
                        double result;
                        if (double.TryParse(margins[0], out result))
                            element.Margin = new Thickness(result);
                    }
                    else if (margins.Length == 2)
                    {
                        double left;
                        double top;
                        if (!double.TryParse(margins[0], out left))
                            break;
                        if (!double.TryParse(margins[1], out top))
                            break;
                        element.Margin = new Thickness(left, top);
                    }
                    else if (margins.Length == 4)
                    {
                        double left;
                        double top;
                        double right;
                        double bottom;
                        if (!double.TryParse(margins[0], out left))
                            break;
                        if (!double.TryParse(margins[1], out top))
                            break;
                        if (!double.TryParse(margins[2], out right))
                            break;
                        if (!double.TryParse(margins[3], out bottom))
                            break;
                        element.Margin = new Thickness(left, top, right, bottom);
                    }
                    break;
                case "label-text-aligment":
                    if(element is Label label1)
                    {
                        label1.HorizontalOptions = LayoutOptions.FillAndExpand;
                        if(style.Value == "center")
                            label1.HorizontalTextAlignment = TextAlignment.Center;
                        else if(style.Value == "right")
                            label1.HorizontalTextAlignment = TextAlignment.End;
                        else if (style.Value == "left")
                            label1.HorizontalTextAlignment = TextAlignment.Start;
                    }
                    break;
            }
        }
    }
}
