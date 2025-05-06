using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ManagementCompanyApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ManagementViewModel();
        }
    }

    public class ManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public ICommand ShowBuildingsCommand { get; }
        public ICommand ShowRequestsCommand { get; }
        public ICommand ShowPaymentsCommand { get; }
        public ICommand AddBuildingCommand { get; }
        public ICommand DeleteBuildingCommand { get; }


        public ObservableCollection<Building> Buildings { get; } = new ObservableCollection<Building>();
        public ObservableCollection<Request> Requests { get; } = new ObservableCollection<Request>();
        public ObservableCollection<Payment> Payments { get; } = new ObservableCollection<Payment>();


        private Building _selectedBuilding;
        public Building SelectedBuilding
        {
            get => _selectedBuilding;
            set { _selectedBuilding = value; OnPropertyChanged(nameof(SelectedBuilding)); }
        }


        public string BuildingsCount => $"Домов: {Buildings.Count}";
        public string ActiveRequestsCount => $"Активных заявок: {Requests.Count}";


        private bool _showBuildings = true;
        private bool _showRequests;
        private bool _showPayments;

        public bool ShowBuildings
        {
            get => _showBuildings;
            set { _showBuildings = value; OnPropertyChanged(nameof(ShowBuildings)); }
        }

        public bool ShowRequests
        {
            get => _showRequests;
            set { _showRequests = value; OnPropertyChanged(nameof(ShowRequests)); }
        }

        public bool ShowPayments
        {
            get => _showPayments;
            set { _showPayments = value; OnPropertyChanged(nameof(ShowPayments)); }
        }

        public ManagementViewModel()
        {

            ShowBuildingsCommand = new RelayCommand(() =>
            {
                ShowBuildings = true;
                ShowRequests = false;
                ShowPayments = false;
            });

            ShowRequestsCommand = new RelayCommand(() =>
            {
                ShowBuildings = false;
                ShowRequests = true;
                ShowPayments = false;
            });

            ShowPaymentsCommand = new RelayCommand(() =>
            {
                ShowBuildings = false;
                ShowRequests = false;
                ShowPayments = true;
            });

            AddBuildingCommand = new RelayCommand(AddBuilding);
            DeleteBuildingCommand = new RelayCommand(DeleteBuilding, () => SelectedBuilding != null);

            LoadTestData();
        }

        private void LoadTestData()
        {
            Buildings.Add(new Building { Address = "ул. Ленина, 10", Floors = 5 });
            Buildings.Add(new Building { Address = "ул. Гагарина, 25", Floors = 9 });

            Requests.Add(new Request { BuildingAddress = "ул. Ленина, 10", Description = "Протечка крана", Status = "В работе" });
            Requests.Add(new Request { BuildingAddress = "ул. Гагарина, 25", Description = "Не работает лифт", Status = "Новая" });

            Payments.Add(new Payment { Address = "ул. Ленина, 10", Amount = 2500, Status = "Оплачено" });
            Payments.Add(new Payment { Address = "ул. Гагарина, 25", Amount = 3200, Status = "Ожидает оплаты" });
        }

        private void AddBuilding()
        {
            var newBuilding = new Building { Address = "Новый дом", Floors = 1 };
            Buildings.Add(newBuilding);
            SelectedBuilding = newBuilding;

            OnPropertyChanged(nameof(BuildingsCount));
        }

        private void DeleteBuilding()
        {
            if (SelectedBuilding != null)
            {
                Buildings.Remove(SelectedBuilding);
                SelectedBuilding = null;

                OnPropertyChanged(nameof(BuildingsCount));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Building
    {
        public string Address { get; set; }
        public int Floors { get; set; }
    }

    public class Request
    {
        public string BuildingAddress { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    public class Payment
    {
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class BoolToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}