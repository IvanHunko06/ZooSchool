using Client.Models;
using Microsoft.Extensions.Configuration;

namespace Client.Services;

public class LessonContentParserService : ILessonContentParserService
{
    private readonly IConfiguration configuration;
    private readonly IDisplayInfoService displayerInfoService;

    public LessonContentParserService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.displayerInfoService = new DisplayInfoService();
    }


    public void Parse(List<PageElementModel> elements, VerticalStackLayout target, EventHandler OnButtonClicked)
    {
        target.Clear();
        foreach (var element in elements)
        {
            View? view = null;
            switch (element.Type)
            {
                case "Image":
                    var image = new Image
                    {
                        Source = element.Value.StartsWith("http") ? element.Value : configuration["Server:Domain"] + element.Value,
                        Aspect = Aspect.AspectFit
                    };
                    ApplyStyles(image, element.Styles);
                    view = image;
                    break;
                case "Text":
                    var label = new Label { Text = element.Value };
                    ApplyStyles(label, element.Styles);
                    view = label;
                    break;
                case "Button":
                    var button = new Button { CommandParameter = element.Value };
                    ApplyStyles(button, element.Styles);
                    button.Clicked += OnButtonClicked;
                    view = button;
                    break;
            }
            if (view != null)
            {
                target.Children.Add(view);
            }
        }

    }
    public void UpdateStyles(List<PageElementModel> elements, VerticalStackLayout target)
    {
        for (int i = 0; i < target.Children.Count; i++)
        {
            ApplyStyles((View)target.Children[i], elements[i].Styles);
        }
    }
    private void ApplyStyles(View element, Dictionary<string, string> styles)
    {
        var mainDisplayInfo = displayerInfoService.GetDisplaySize();
        var screenWidth = mainDisplayInfo.Width;
        var screenHeight = mainDisplayInfo.Height;
        foreach (var style in styles)
        {
            switch (style.Key)
            {
                case "text":
                    if (element is Button button)
                        button.Text = style.Value;
                    break;
                case "margin":
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
                case "width":
                    if (style.Value.Contains("%") && double.TryParse(style.Value.Trim('%'), out double widthPercent))
                    {
                        element.WidthRequest = screenWidth * widthPercent / 100.0;
                        element.HorizontalOptions = LayoutOptions.FillAndExpand;
                    }
                    break;
                //case "height":
                //    if (style.Value.Contains("%") && double.TryParse(style.Value.Trim('%'), out double heightPercent))
                //    {
                //        element.HeightRequest = screenHeight * heightPercent / 100.0;
                //        element.VerticalOptions = LayoutOptions.FillAndExpand;
                //    }
                //    break;
                case "horizontalAligment":
                    if (style.Value == "center")
                        element.HorizontalOptions = LayoutOptions.Center;
                    else if (style.Value == "right")
                        element.HorizontalOptions = LayoutOptions.End;
                    else if (style.Value == "left")
                        element.HorizontalOptions = LayoutOptions.Start;
                    break;
                case "font-size":

                    double fontSize;
                    if (!double.TryParse(style.Value, out fontSize))
                        break;
                    if (element is Button btn)
                    {
                        btn.FontSize = fontSize;
                    }
                    else if (element is Label lbl)
                    {
                        lbl.FontSize = fontSize;
                    }
                    break;

            }
        }
    }
}
