using MahApps.Metro;
using System;
using System.Linq;
using System.Windows;

namespace Class_Management
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ChangeAccent(string accentUrl)
        {
            ResourceDictionary resourceDict1 = new ResourceDictionary();
            resourceDict1.Source = new Uri(accentUrl, UriKind.Absolute);
            App.Current.Resources.MergedDictionaries[1].Clear();
            App.Current.Resources.MergedDictionaries.Add(resourceDict1);
            var accent = ThemeManager.Accents.First(x => x.Name == "Purple");
            ThemeManager.ChangeAppStyle(Current, accent, null);
        }
    }
}
