using System.Windows;

namespace Client
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var v = new MainWindowView();
            var vm = new MainWindowViewModel();
            v.DataContext = vm;
            v.Show();
        }
    }
}
