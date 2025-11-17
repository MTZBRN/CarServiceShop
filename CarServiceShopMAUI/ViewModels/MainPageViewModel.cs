using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using CarServiceShopMAUI.Views;

public partial class MainPageViewModel : ObservableObject
{
    public IAsyncRelayCommand NavigateToCarListCommand { get; }
    public IAsyncRelayCommand NavigateToWorksheetCommand { get; }

    public MainPageViewModel()
    {
        NavigateToCarListCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(CarListPage)));
        NavigateToWorksheetCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(WorksheetPage)));
    }
    
    [RelayCommand]
    public async Task  OpenCarList()
    {
        await Shell.Current.GoToAsync(nameof(CarListPage));
    }

    [RelayCommand]
    public async Task OpenWorksheet()
    {
        await Shell.Current.GoToAsync(nameof(WorksheetPage));
    }
}

