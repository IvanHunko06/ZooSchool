using Client.Models;

namespace Client.Services
{
    public interface ILessonContentParserService
    {
        void Parse(List<PageElementModel> elements, VerticalStackLayout target, EventHandler OnButtonClicked);
        void UpdateStyles(List<PageElementModel> elements, VerticalStackLayout target);
    }
}