using CarServiceShopBackend.DbContext;
using Microsoft.EntityFrameworkCore;
using CarServiceShopBackend.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=carshop.db"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // 🔧 JAVÍTÁS: Circular reference kezelés
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Cars.Any())
    {
        var cars = new List<Car>
    {
        new Car { LicensePlate = "ABC-123", Brand = "Toyota", Model = "Corolla", YearOfManufacture = 2018, DateOfTechnicalInspection = new DateTime(2025,12,15), OwnerName = "Varga Péter", OwnerAddress = "Budapest, Fő utca 12.", OwnerPhone = "+3612345678", Vin = "VF1234567890", Mileage = 150000, picture=null },
        new Car { LicensePlate = "XYZ-789", Brand = "BMW", Model = "320d", YearOfManufacture = 2020, DateOfTechnicalInspection = new DateTime(2026,3,20), OwnerName = "Kovács Anna", OwnerAddress = "Debrecen, Templom tér 7.", OwnerPhone = "+36701234567", Vin = "BMW3202020VIN", Mileage = 85000,picture=null },
        new Car { LicensePlate = "DEF-456", Brand = "Volkswagen", Model = "Golf", YearOfManufacture = 2019, DateOfTechnicalInspection = new DateTime(2025,8,10), OwnerName = "Nagy Eszter", OwnerAddress = "Szeged, Petőfi sétány 10.", OwnerPhone = "+36601234567", Vin = "VWGOLF2019VIN", Mileage = 120000 , picture = null},
        new Car { LicensePlate = "GHI-321", Brand = "Audi", Model = "A4", YearOfManufacture = 2021, DateOfTechnicalInspection = new DateTime(2026,5,30), OwnerName = "Szabó Gábor", OwnerAddress = "Pécs, Rákóczi utca 5.", OwnerPhone = "+36301234567", Vin = "AUDIA42021VIN", Mileage = 65000 , picture = null},
        new Car { LicensePlate = "JKL-654", Brand = "Mercedes-Benz", Model = "C-Class", YearOfManufacture = 2022, DateOfTechnicalInspection = new DateTime(2026,11,5), OwnerName = "Tóth Mária", OwnerAddress = "Miskolc, Kossuth tér 4.", OwnerPhone = "+36401234567", Vin = "MBCC2022VIN", Mileage = 40000 , picture = null}
    };
        context.Cars.AddRange(cars);
        context.SaveChanges();

        var services = new List<Service>
    {
        new Service { CarId = 1, WorkHours = 2, WorkHourPrice = 15000, ServiceDate = new DateTime(2025,9,15), ServiceDescription = "Olajcsere és szűrőcsere" },
        new Service { CarId = 1, WorkHours = 4, WorkHourPrice = 15000, ServiceDate = new DateTime(2025,10,1), ServiceDescription = "Fékbetét csere" },
        new Service { CarId = 2, WorkHours = 1, WorkHourPrice = 18000, ServiceDate = new DateTime(2025,10,5), ServiceDescription = "Klíma karbantartás" },
        new Service { CarId = 3, WorkHours = 3, WorkHourPrice = 16000, ServiceDate = new DateTime(2025,9,20), ServiceDescription = "Futómű ellenőrzés és beállítás" },
        new Service { CarId = 4, WorkHours = 5, WorkHourPrice = 20000, ServiceDate = new DateTime(2025,10,10), ServiceDescription = "Motor diagnosztika" }
    };
        context.Services.AddRange(services);
        context.SaveChanges();

        var parts = new List<Part>
    {
        new Part { ServiceId = 1, Description = "Motorolaj cseréhez", PartNumber = "OIL-001", Name = "Motorolaj 5W-30", NetPrice = 8500, Quantity = 5 },
        new Part { ServiceId = 1, Description = "Olajszűrő csere", PartNumber = "FLT-002", Name = "Olajszűrő", NetPrice = 2500, Quantity = 1 },
        new Part { ServiceId = 2, Description = "Fékbetét elöl", PartNumber = "BRK-101", Name = "Fékbetét szett elöl", NetPrice = 18000, Quantity = 1 },
        new Part { ServiceId = 2, Description = "Féktárcsa csere", PartNumber = "BRK-102", Name = "Féktárcsa pár", NetPrice = 25000, Quantity = 1 },
        new Part { ServiceId = 3, Description = "Klíma szűrő", PartNumber = "AC-301", Name = "Klíma szűrő", NetPrice = 4500, Quantity = 1 },
        new Part { ServiceId = 3, Description = "Klíma gáz töltés", PartNumber = "AC-302", Name = "Klíma gáz töltés", NetPrice = 12000, Quantity = 1 },
        new Part { ServiceId = 4, Description = "Lengéscsillapító pár", PartNumber = "SUS-201", Name = "Lengéscsillapító pár", NetPrice = 45000, Quantity = 1 },
        new Part { ServiceId = 5, Description = "Gyújtógyertya szett", PartNumber = "ENG-401", Name = "Gyújtógyertya szett", NetPrice = 16000, Quantity = 1 }
    };
        context.Parts.AddRange(parts);
        context.SaveChanges();
    }


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.UseHttpsRedirection();
    app.MapControllers();

    app.Run();
}
