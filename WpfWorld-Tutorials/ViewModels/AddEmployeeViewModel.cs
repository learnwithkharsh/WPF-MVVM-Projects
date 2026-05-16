using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows;

namespace WpfApp.ViewModels
{
    public partial class AddEmployeeViewModel : ObservableObject
    {
        public List<string> Options { get; } = new()
        {
            "Form",
            "JSON"
        };
        public List<string> Genders { get; } = new()
        {
            "Male",
            "Female",
            "Other"
        };
        public List<string> Roles { get; } = new()
        {
            "Developer",
            "Tester",
            "Support",
            "Manager"

        };
        public List<string> Countries { get; } = new()
        {
            "India",
            "USA",
            "Canada",
            "UK",
            "Australia",
            "Germany",
            "France",
            "Japan",
            "China",
            "Brazil",
            "South Africa",
            "Singapore",
            "UAE",
            "Netherlands",
            "Sweden"
        };

        [ObservableProperty] private string title= "Add Employee";
        [ObservableProperty] private string name;
        [ObservableProperty] private string email;
        [ObservableProperty] private string company;
        [ObservableProperty] private string gender;
        [ObservableProperty] private string country;
        [ObservableProperty] private string role;
        [ObservableProperty] private int experience;
        [ObservableProperty] private string buttonText;
        [ObservableProperty] private string selectedOption = "Form";
        [ObservableProperty] private string jSONContent;
        [ObservableProperty] private Employee employee = new();
        [ObservableProperty] private List<Employee> bulkEmployee = new();
        [ObservableProperty] private Visibility formVisibility = Visibility.Visible;
        [ObservableProperty] private Visibility jsonVisibility = Visibility.Collapsed;

        public Action<bool> CloseAction { get; set; }
        public bool IsUpdate { get; set; }
        public AddEmployeeViewModel(Employee employee)
        {
            if (employee == null)
            {
                ButtonText = "Save";
                Title = "Add Employee";
            }
            else
            {
                ButtonText = "Update";
                Title = "Update Employee";
                this.Employee = employee;
            }

        }
        partial void OnSelectedOptionChanged(string value)
        {

            if (value == "JSON")
            {
                JsonVisibility = Visibility.Visible;
                FormVisibility = Visibility.Collapsed;
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                JSONContent = JsonSerializer.Serialize(new List<Employee> { Employee }, options);
            }
            else
            {
                JsonVisibility = Visibility.Collapsed;
                FormVisibility = Visibility.Visible;
            }
        }

        [RelayCommand]
        private void Close()
        {
            if (SelectedOption == "JSON")
                try
                {
                    BulkEmployee = JsonSerializer.Deserialize<List<Employee>>(
                        JSONContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Invalid JSON format");
                }


            CloseAction?.Invoke(true);
        }

    }
}
