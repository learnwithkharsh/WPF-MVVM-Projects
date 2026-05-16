using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using WpfApp.CURD;
using WpfApp.Interfaces;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        [ObservableProperty] public List<string> columnNames = ["Name", "Company", "Email", "Gender", "Country", "Role", "Experience"];
        [ObservableProperty] public string selectedColumn = "Name";
        [ObservableProperty] public List<int> pageSizes = [10, 20, 30, 40, 50];
        [ObservableProperty] public int selectedPageSize = 10;
        [ObservableProperty] private string searchText;
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int totalPages;
        [ObservableProperty] private string pageInfo;
        [ObservableProperty] public ObservableCollection<Employee> employees = new();
        [ObservableProperty] ICollectionView employeesView;
        [ObservableProperty] private List<string> databases = ["Sql Server", "SQlite"];
        [ObservableProperty] private string selectedDatabase = "Sql Server";

        //public EmployeeViewModel()
        //{
        //    LoadEmployee();
        //}

        //private void LoadEmployee()
        //{
        //    var json = File.ReadAllText("Resources/employees_100k.json");
        //    Employees = JsonSerializer.Deserialize<ObservableCollection<Employee>>(json);
        //}

        private IRepository<Employee> _repository = EmployeeRepository.Instance;

        public EmployeeViewModel()
        {
            EmployeesView = CollectionViewSource.GetDefaultView(Employees);
            EmployeesView.Filter = FilterEmployees;

            LoadData();

        }

        #region CURD

        private async void LoadData()
        {
            var result = await _repository.GetPagedAsync(CurrentPage, SelectedPageSize);


            Employees.Clear();

            foreach (var emp in result.employee)
                Employees.Add(emp);

            ChangeNotification.NotifyEmployeesChanged();

            EmployeesView.Refresh();

            var totalCount = result.totalEmployee;
            TotalPages = (int)Math.Ceiling((double)totalCount / SelectedPageSize);

            UpdatePageInfo(totalCount, result.skip);
        }

        [RelayCommand]
        private async Task OpenAddDialog()
        {
            var dialog = new AddEmployeeView();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.vm.SelectedOption == "JSON")
                {
                    try
                    {
                        var employees = dialog.vm.BulkEmployee;
                        await _repository.AddBulkAsync(employees);
                        foreach (var employee in employees)
                        {
                            Employees.Add(employee);
                        }
                    }
                    catch 
                    {

                      
                    }
                }
                else
                {
                    var employeeToBeAdded = dialog.vm.Employee;
                    await _repository.AddAsync(employeeToBeAdded);
                    Employees.Add(employeeToBeAdded);
                }
                LoadData();
            }
        }


        [RelayCommand]
        private async Task Edit(Employee emp)
        {
            var dialog = new AddEmployeeView(emp);
            if (dialog.ShowDialog() == true)
            {
                var employeeToBeAdded = dialog.vm.Employee;
                await _repository.UpdateAsync(employeeToBeAdded);
                employeeToBeAdded = dialog.vm.Employee;
            }
        }


        [RelayCommand]
        private async Task Delete(Employee emp)
        {
            await _repository.DeleteAsync(emp.Id);
            Employees.Remove(emp);
            LoadData();
        }
        #endregion

        #region OnPropertyChanged
        partial void OnSearchTextChanged(string value)
        {
            EmployeesView?.Refresh();
        }

        partial void OnSelectedColumnChanged(string value)
        {
            EmployeesView?.Refresh();
        }
        partial void OnSelectedDatabaseChanged(string value)
        {
            Utilities.SelectedDatabase = value;

            _repository = value == "Sql Server" ? EmployeeRepository.Instance : EmployeeSqliteRepository.Instance;

            LoadData();
        } 
        #endregion

        #region Filter

        private bool FilterEmployees(object obj)
        {
            if (obj is not Employee emp)
                return false;

            if (string.IsNullOrWhiteSpace(SearchText) || string.IsNullOrWhiteSpace(SelectedColumn))
                return true;

            return SelectedColumn switch
            {
                "Name" => emp.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase),
                "Email" => emp.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase),
                "Company" => emp.Company.Contains(SearchText, StringComparison.OrdinalIgnoreCase),
                "Country" => emp.Country.Contains(SearchText, StringComparison.OrdinalIgnoreCase),
                "Role" => emp.Role.Contains(SearchText, StringComparison.OrdinalIgnoreCase),
                "Gender" => emp.Gender.Contains(SearchText, StringComparison.OrdinalIgnoreCase),
                "Experience" => int.TryParse(SearchText, out int exp) && emp.Experience == exp,
                _ => true
            };
        }

        #endregion

        #region Pagination

        [RelayCommand(CanExecute = nameof(CanGoPrevious))]
        private void FirstPage()
        {
            CurrentPage = 1;
            LoadData();
        }

        [RelayCommand(CanExecute = nameof(CanGoPrevious))]

        private void PrevPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadData();
            }
        }
        [RelayCommand(CanExecute = nameof(CanGoNext))]

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadData();
            }
        }

        [RelayCommand(CanExecute = nameof(CanGoNext))]
        private void LastPage()
        {
            CurrentPage = TotalPages;
            LoadData();
        }

        private bool CanGoPrevious() => CurrentPage > 1;

        private bool CanGoNext() => CurrentPage < TotalPages;
        partial void OnSelectedPageSizeChanged(int oldValue, int newValue)
        {
            LoadData();
        }

        partial void OnCurrentPageChanged(int value)
        {
            NotifyCanExceute();
        }

        void UpdatePageInfo(int totalCount, int currentSkip)
        {
            if (totalCount == 0)
                PageInfo = "0 of 0 records";

            int start = currentSkip + 1;
            int end = Math.Min(currentSkip + SelectedPageSize, totalCount);

            PageInfo = $"{start:N0}–{end:N0} of {totalCount:N0} records";
        }

        partial void OnTotalPagesChanged(int value)
        {
            NotifyCanExceute();
        }
        private void NotifyCanExceute()
        {
            PrevPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            FirstPageCommand.NotifyCanExecuteChanged();
            LastPageCommand.NotifyCanExecuteChanged();
        }



        #endregion
    }
}
