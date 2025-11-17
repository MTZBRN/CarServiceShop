using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharpCore.Drawing;
using System.Collections.ObjectModel;
using PdfSharpCore.Pdf;

public partial class WorksheetViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    public ObservableCollection<Car> Cars { get; set; } = new();
    public ObservableCollection<Service> Services { get; set; } = new();
    public Car SelectedCar { get; set; }
    public Service SelectedService { get; set; }

    public IAsyncRelayCommand ExportPdfCommand { get; }

    public WorksheetViewModel(ApiService apiService)
    {
        _apiService = apiService;
        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        LoadCarsAndServices();
    }

    private async void LoadCarsAndServices()
    {
        // Feltöltés API-ból
        var cars = await _apiService.GetCarsAsync();
        Cars.Clear();
        foreach (var c in cars) Cars.Add(c);

        var services = await _apiService.GetServicesAsync();
        Services.Clear();
        foreach (var s in services) Services.Add(s);
    }

    public async Task ExportPdfAsync()
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
        var fontBody = new XFont("Arial", 12);

        gfx.DrawString("Szerviz munkalap", fontTitle, XBrushes.Black, new XRect(0, 20, page.Width, 40), XStringFormats.TopCenter);

        int y = 80;
        gfx.DrawString($"Rendszám: {SelectedCar?.LicensePlate}", fontBody, XBrushes.Black, new XPoint(40, y)); y += 20;
        gfx.DrawString($"Márka: {SelectedCar?.Brand}", fontBody, XBrushes.Black, new XPoint(40, y)); y += 20;
        gfx.DrawString($"Modell: {SelectedCar?.Model}", fontBody, XBrushes.Black, new XPoint(40, y)); y += 20;
        gfx.DrawString($"Tulajdonos: {SelectedCar?.OwnerName}", fontBody, XBrushes.Black, new XPoint(40, y)); y += 30;

        if (SelectedService != null)
        {
            gfx.DrawString($"{SelectedService.ServiceDate.ToShortDateString()} - {SelectedService.ServiceDescription}", fontBody, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Munkaórák: {SelectedService.WorkHours} x {SelectedService.WorkHourPrice} Ft", fontBody, XBrushes.Black, new XPoint(60, y)); y += 20;
            if (SelectedService.Parts != null)
            {
                foreach (var part in SelectedService.Parts)
                {
                    decimal totalPartPrice = part.NetPrice * part.Quantity;
                    gfx.DrawString($" - {part.Name} ({part.Quantity} db): {totalPartPrice} Ft", fontBody, XBrushes.Black, new XPoint(80, y));
                    y += 20;
                }
            }
        }

        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SzervizMunkalap.pdf");
        using var stream = File.Create(filePath);
        document.Save(stream);
        await Shell.Current.DisplayAlert("Siker", $"PDF mentve ide: {filePath}", "OK");
    }
}
