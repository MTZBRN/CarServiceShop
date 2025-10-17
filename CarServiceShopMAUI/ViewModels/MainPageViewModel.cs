using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using CarServiceShopMAUI.Views;

public partial class MainPageViewModel : ObservableObject
{
    public IAsyncRelayCommand NavigateToCarListCommand { get; }

    public MainPageViewModel()
    {
        NavigateToCarListCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(CarListPage)));
    }
}

