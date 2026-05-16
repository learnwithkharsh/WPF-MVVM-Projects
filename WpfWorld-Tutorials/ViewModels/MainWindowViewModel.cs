using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        /*   public User user { get; set; } = new User();

           //[ObservableProperty]
           public string FullName
           {
               get => user.FullName;
               set => SetProperty(user.FullName, value, user, (u, n) => u.FullName = n);
           }

           [ObservableProperty]
           public string email;

           [ObservableProperty]
           public string password;
           [ObservableProperty]
           public string gender;
           [ObservableProperty]
           public string[] countries;
           [ObservableProperty]

           public string selectedCountry;

           [ObservableProperty]
           public bool skillCSharp;
           [ObservableProperty]
           public bool skillWpf;
           [ObservableProperty]
           public bool skillDotNet;

           [ObservableProperty]
           public int experience;

           [ObservableProperty]
           [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
           public bool isTermsAccepted;
           [RelayCommand(CanExecute = nameof(CanRegister))]
           private void Register(object? parameter)
           {
               MessageBox.Show($"Full name:{user.FullName}\n email: {user.Email}\ngender:{user.Gender}\n" +
                 $"contry:{user.SelectedCountry}\nwpf skill:{user.SkillWpf}\nc# skill:{user.SkillCSharp}\n" +
                 $"Dot net skill:{user.SkillDotNet}\nExp:{user.Experience}\n Term :{user.IsTermsAccepted}" +
                 "Command Parameter:" + parameter.ToString());
           }
           bool CanRegister(object? parameter)
           {
               return isTermsAccepted;
           }
        */

        public MainWindowViewModel()
        {
            ShowView("Dashboard");
        }

        [ObservableProperty]
        public object currentView;
        private readonly Dictionary<string, Lazy<object>> _lazyVMs = new()
        {
            { "Dashboard", new Lazy<object>(() => new DashboardViewModel()) },
            { "Employee", new Lazy<object>(() => new EmployeeViewModel()) },
            { "ChatView", new Lazy<object>(() => new ChatViewModel()) }
        };
        

        [RelayCommand]
        private void ShowView(string viewModel)
        {
            if (_lazyVMs.TryGetValue(viewModel, out var vm))
            {
                CurrentView = vm.Value;
            }
        }

    }
}
