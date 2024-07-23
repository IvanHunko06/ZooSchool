using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;

namespace Client.Services;

public interface IDisplayInfoService
{
    (double Width, double Height) GetDisplaySize();
}

public class DisplayInfoService : IDisplayInfoService
{
    public (double Width, double Height) GetDisplaySize()
    {
        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

        double width = mainDisplayInfo.Width / mainDisplayInfo.Density;
        double height = mainDisplayInfo.Height / mainDisplayInfo.Density;

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            var window = Application.Current.Windows[0];
            width = window.Width;
            height = window.Height;
        }

        return (width, height);
    }
}