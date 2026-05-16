using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using WpfApp.CURD;
using WpfApp.Interfaces;

namespace WpfApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        public ISeries[] genderSeries;
        [ObservableProperty]
        public ISeries[] countrySeries;
        [ObservableProperty]
        public ISeries[] companySeries;
        [ObservableProperty]
        public ISeries[] experienceSeries;


        private IRepository<Employee> _repository = EmployeeRepository.Instance;
        ObservableCollection<Employee> Employees;
        public DashboardViewModel()
        {
            ChangeNotification.NotifyEmployeesChanged = async () =>
            {
                await LoadEmployee()
               .ContinueWith(t => { LoadCharts(); });
            };
            ChangeNotification.NotifyEmployeesChanged();
        }

        private void LoadCharts()
        {
            if (Employees != null)
            {
                GenderSeries = Employees.Where(x => !string.IsNullOrWhiteSpace(x.Gender))
                                         .GroupBy(x => x.Gender.Trim())
                                         .Select(g => new PieSeries<int>
                                         {
                                             Name = g.Key,
                                             Values = new[] { g.Count() }
                                         })
                                         .Cast<ISeries>()
                                         .ToArray();


                CountrySeries = Employees.GroupBy(x => x.Country).
                                          Select(g => new PieSeries<int>
                                          {
                                              Name = g.Key,
                                              Values = new[] { g.Count() }
                                          })
                                          .Cast<ISeries>()
                                          .ToArray();


                ExperienceSeries = Employees.GroupBy(x => x.Experience)
                                  .OrderBy(x => x.Key)
                                   .Select(g => new PieSeries<int>
                                   {
                                       Name = $"{g.Key} yrs",
                                       Values = new[] { g.Count() }
                                   })
                                  .Cast<ISeries>()
                                  .ToArray();

                CompanySeries = Employees.GroupBy(x => x.Company)
                         .Select(g => new PieSeries<int>
                         {
                             Name = g.Key,
                             Values = new[] { g.Count() }
                         })
                         .Cast<ISeries>()
                         .ToArray();
            }
        }

        private async Task LoadEmployee()
        {
            //var json = File.ReadAllText("Resources/employees_100k.json");
            //Employees = JsonSerializer.Deserialize<ObservableCollection<Employee>>(json);
            _repository = Utilities.SelectedDatabase == "Sql Server" ? EmployeeRepository.Instance : EmployeeSqliteRepository.Instance;
            Utilities.AllEmployees = await _repository.GetAllAsync();
            Employees = new ObservableCollection<Employee>(Utilities.AllEmployees);
        }
    }
}
