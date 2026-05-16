using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp.CURD;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            using (var context = new EmployeeDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }

}
