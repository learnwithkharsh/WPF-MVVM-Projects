using MahApps.Metro.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    /// <summary>
    /// Interaction logic for AddEmployeeView.xaml
    /// </summary>
    public partial class AddEmployeeView : MetroWindow
    {
        public AddEmployeeViewModel vm;
        public AddEmployeeView(Employee employee = null)
        {
            InitializeComponent();
            vm = new AddEmployeeViewModel(employee);            
            vm.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };

            DataContext = vm;
        }
    }
}
