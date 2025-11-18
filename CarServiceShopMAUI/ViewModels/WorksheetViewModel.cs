using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(CarId), nameof(CarId))]
    [QueryProperty(nameof(ServiceId), nameof(ServiceId))]
    public partial class WorksheetViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        // Navigációs paraméterek
        [ObservableProperty]
        private int carId;

        [ObservableProperty]
        private int serviceId;

        // Listák az összes autóhoz és szervizhez
        public ObservableCollection<Car> Cars { get; set; } = new();
        public ObservableCollection<Service> Services { get; set; } = new();

        [ObservableProperty]
        private Car selectedCar;

        [ObservableProperty]
        private Service selectedService;

        // Megrendelő adatai
        public ObservableCollection<string> CustomerTypeOptions { get; } = new() { "Cég", "Magánszemély" };
        public ObservableCollection<string> OwnerRoleOptions { get; } = new() { "Tulajdonos", "Üzembentartó", "Meghatalmazott" };

        [ObservableProperty]
        private string customerType = "Magánszemély";

        [ObservableProperty]
        private string customerName = string.Empty;

        [ObservableProperty]
        private string customerAddress = string.Empty;

        [ObservableProperty]
        private string customerPhone = string.Empty;

        [ObservableProperty]
        private string customerIdentification = string.Empty;

        [ObservableProperty]
        private string ownerRole = "Tulajdonos";

        // Jármű adatok
        [ObservableProperty]
        private string plateNumber = string.Empty;

        [ObservableProperty]
        private string brand = string.Empty;

        [ObservableProperty]
        private string type = string.Empty;

        [ObservableProperty]
        private string mileage = string.Empty;

        [ObservableProperty]
        private string vin = string.Empty;

        [ObservableProperty]
        private string fuelTankInfo = string.Empty;

        // Felszerelések
        [ObservableProperty]
        private bool hasRegistration;

        [ObservableProperty]
        private bool hasInsurance;

        [ObservableProperty]
        private bool hasRadio;

        [ObservableProperty]
        private bool hasAntenna;

        [ObservableProperty]
        private bool hasSpareWheel;

        [ObservableProperty]
        private bool hasJack;

        [ObservableProperty]
        private bool hasWarningTriangle;

        [ObservableProperty]
        private bool hasMedKit;

        [ObservableProperty]
        private bool hasBulbSet;

        [ObservableProperty]
        private bool hasFireExtinguisher;

        [ObservableProperty]
        private string otherCarItems = string.Empty;

        // Sérülés / Hiba
        [ObservableProperty]
        private string carDamage = string.Empty;

        [ObservableProperty]
        private string workDescription = string.Empty;

        // Alkatrész igény
        [ObservableProperty]
        private bool wantsPartsBack;

        [ObservableProperty]
        private string suppliedMaterials = string.Empty;

        // Határidő, költség
        [ObservableProperty]
        private DateTime deadline = DateTime.Now.AddDays(7);

        [ObservableProperty]
        private string estimatedNetCost = string.Empty;

        // Nyilatkozat
        [ObservableProperty]
        private string acceptanceStatement = "Kijelentem, hogy a fent megadott adatok megfelelnek a valóságnak.";

        public IAsyncRelayCommand ExportPdfCommand { get; }

        public WorksheetViewModel(ApiService apiService)
        {
            _apiService = apiService;
            ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
            _ = LoadCarsAndServicesAsync();
        }

        // Ha CarId megváltozik, automatikusan betöltjük az adatokat
        partial void OnCarIdChanged(int value)
        {
            if (value > 0 && Cars.Count > 0)
            {
                _ = LoadCarDataAsync(value);
            }
        }

        // Ha ServiceId megváltozik, automatikusan betöltjük a szerviz adatokat
        partial void OnServiceIdChanged(int value)
        {
            if (value > 0 && Services.Count > 0)
            {
                _ = LoadServiceDataAsync(value);
            }
        }

        // Ha a felhasználó manuálisan választ autót
        partial void OnSelectedCarChanged(Car value)
        {
            if (value != null)
            {
                FillCarData(value);
            }
        }

        // Ha a felhasználó manuálisan választ szervizt
        partial void OnSelectedServiceChanged(Service value)
        {
            if (value != null)
            {
                FillServiceData(value);
            }
        }

        private async Task LoadCarsAndServicesAsync()
        {
            try
            {
                Debug.WriteLine("🔄 Loading cars and services...");

                var cars = await _apiService.GetCarsAsync();
                Cars.Clear();
                foreach (var c in cars) Cars.Add(c);

                var services = await _apiService.GetServicesAsync();
                Services.Clear();
                foreach (var s in services) Services.Add(s);

                Debug.WriteLine($"✅ Loaded {Cars.Count} cars and {Services.Count} services");

                // Ha már van CarId vagy ServiceId, töltsd be az adatokat
                if (CarId > 0)
                {
                    await LoadCarDataAsync(CarId);
                }
                if (ServiceId > 0)
                {
                    await LoadServiceDataAsync(ServiceId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading data: {ex.Message}");
            }
        }

        private async Task LoadCarDataAsync(int carId)
        {
            try
            {
                var car = await _apiService.GetCarByIdAsync(carId);
                if (car != null)
                {
                    SelectedCar = Cars.FirstOrDefault(c => c.Id == carId);
                    FillCarData(car);
                    Debug.WriteLine($"✅ Loaded car data for ID: {carId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading car: {ex.Message}");
            }
        }

        private async Task LoadServiceDataAsync(int serviceId)
        {
            try
            {
                SelectedService = Services.FirstOrDefault(s => s.Id == serviceId);
                if (SelectedService != null)
                {
                    FillServiceData(SelectedService);
                    Debug.WriteLine($"✅ Loaded service data for ID: {serviceId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading service: {ex.Message}");
            }
        }

        private void FillCarData(Car car)
        {
            PlateNumber = car.LicensePlate ?? string.Empty;
            Brand = car.Brand ?? string.Empty;
            Type = car.Model ?? string.Empty;
            Mileage = car.Mileage.ToString();
            Vin = car.Vin ?? string.Empty;
            CustomerName = car.OwnerName ?? string.Empty;
            CustomerAddress = car.OwnerAddress ?? string.Empty;
            CustomerPhone = car.OwnerPhone ?? string.Empty;
        }

        private void FillServiceData(Service service)
        {
            WorkDescription = service.ServiceDescription ?? string.Empty;

            // Becsült költség számítása
            double totalCost = service.WorkHours * service.WorkHourPrice;
            if (service.Parts != null && service.Parts.Any())
            {
                totalCost += service.Parts.Sum(p => (double)(p.NetPrice * p.Quantity));
            }
            EstimatedNetCost = totalCost.ToString("F0");

            Deadline = service.ServiceDate.AddDays(7);
        }

        public async Task ExportPdfAsync()
        {
            try
            {
                var document = new PdfDocument();
                document.Info.Title = "Szerviz Munkalap";

                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);
                var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
                var fontSection = new XFont("Arial", 14, XFontStyle.Bold);
                var fontBody = new XFont("Arial", 11);
                var fontSmall = new XFont("Arial", 9);

                int y = 40;
                int leftMargin = 40;
                int lineHeight = 20;

                // Cím
                gfx.DrawString("SZERVIZ MUNKALAP", fontTitle, XBrushes.Black,
                    new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
                y += 50;

                // Megrendelő adatai
                gfx.DrawString("MEGRENDELŐ ADATAI", fontSection, XBrushes.Black,
                    new XPoint(leftMargin, y));
                y += lineHeight + 5;

                gfx.DrawString($"Név: {CustomerName}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Cím: {CustomerAddress}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Telefon: {CustomerPhone}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Típus: {CustomerType}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                y += 10;

                // Jármű adatok
                gfx.DrawString("JÁRMŰ ADATOK", fontSection, XBrushes.Black,
                    new XPoint(leftMargin, y));
                y += lineHeight + 5;

                gfx.DrawString($"Rendszám: {PlateNumber}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Gyártmány: {Brand}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Típus: {Type}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Km óra állás: {Mileage} km", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                gfx.DrawString($"Alvázszám: {Vin}", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y)); y += lineHeight;
                y += 10;

                // Munka leírása
                gfx.DrawString("FELADAT / HIBA LEÍRÁSA", fontSection, XBrushes.Black,
                    new XPoint(leftMargin, y));
                y += lineHeight + 5;

                DrawMultilineText(gfx, WorkDescription, fontBody, leftMargin, ref y,
                    (int)page.Width - 80, lineHeight);
                y += 10;

                // Sérülés
                if (!string.IsNullOrWhiteSpace(CarDamage))
                {
                    gfx.DrawString("SÉRÜLÉS", fontSection, XBrushes.Black,
                        new XPoint(leftMargin, y));
                    y += lineHeight + 5;
                    DrawMultilineText(gfx, CarDamage, fontBody, leftMargin, ref y,
                        (int)page.Width - 80, lineHeight);
                    y += 10;
                }

                // Költség és határidő
                gfx.DrawString("KÖLTSÉGBECSLÉS ÉS HATÁRIDŐ", fontSection, XBrushes.Black,
                    new XPoint(leftMargin, y));
                y += lineHeight + 5;

                gfx.DrawString($"Becsült költség (nettó): {EstimatedNetCost} Ft",
                    fontBody, XBrushes.Black, new XPoint(leftMargin, y));
                y += lineHeight;
                gfx.DrawString($"Vállalási határidő: {Deadline:yyyy.MM.dd.}",
                    fontBody, XBrushes.Black, new XPoint(leftMargin, y));
                y += lineHeight + 10;

                // Aláírás terület
                y = (int)page.Height - 100;
                gfx.DrawString("_______________________________", fontBody, XBrushes.Black,
                    new XPoint(leftMargin, y));
                gfx.DrawString("Megrendelő aláírása", fontSmall, XBrushes.Black,
                    new XPoint(leftMargin, y + 15));

                gfx.DrawString("_______________________________", fontBody, XBrushes.Black,
                    new XPoint(leftMargin + 250, y));
                gfx.DrawString("Szerviz aláírása", fontSmall, XBrushes.Black,
                    new XPoint(leftMargin + 250, y + 15));

                // Fájl mentése
                string fileName = $"Munkalap_{PlateNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                using (var stream = File.Create(filePath))
                {
                    document.Save(stream);
                }

                Debug.WriteLine($"✅ PDF saved to: {filePath}");
                await Shell.Current.DisplayAlert("Siker",
                    $"Munkalap mentve!\n\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error exporting PDF: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba",
                    $"PDF létrehozása sikertelen: {ex.Message}", "OK");
            }
        }

        private void DrawMultilineText(XGraphics gfx, string text, XFont font,
            int x, ref int y, int maxWidth, int lineHeight)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var words = text.Split(' ');
            string currentLine = "";

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                var size = gfx.MeasureString(testLine, font);

                if (size.Width > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    gfx.DrawString(currentLine, font, XBrushes.Black, new XPoint(x, y));
                    y += lineHeight;
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                gfx.DrawString(currentLine, font, XBrushes.Black, new XPoint(x, y));
                y += lineHeight;
            }
        }
    }
}
