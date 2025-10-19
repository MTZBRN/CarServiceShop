using CarServiceShopBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CarServiceShopBackend.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Part> Parts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Seed Cars
            modelBuilder.Entity<Car>().HasData(
                new Car
                {
                    Id = 1,
                    LicensePlate = "ABC-123",
                    Brand = "Toyota",
                    Model = "Corolla",
                    YearOfManufacture = 2018,
                    DateOfTechnicalInspection = new DateTime(2025, 12, 15)
                },
                new Car
                {
                    Id = 2,
                    LicensePlate = "XYZ-789",
                    Brand = "BMW",
                    Model = "320d",
                    YearOfManufacture = 2020,
                    DateOfTechnicalInspection = new DateTime(2026, 3, 20)
                },
                new Car
                {
                    Id = 3,
                    LicensePlate = "DEF-456",
                    Brand = "Volkswagen",
                    Model = "Golf",
                    YearOfManufacture = 2019,
                    DateOfTechnicalInspection = new DateTime(2025, 8, 10)
                },
                new Car
                {
                    Id = 4,
                    LicensePlate = "GHI-321",
                    Brand = "Audi",
                    Model = "A4",
                    YearOfManufacture = 2021,
                    DateOfTechnicalInspection = new DateTime(2026, 5, 30)
                },
                new Car
                {
                    Id = 5,
                    LicensePlate = "JKL-654",
                    Brand = "Mercedes-Benz",
                    Model = "C-Class",
                    YearOfManufacture = 2022,
                    DateOfTechnicalInspection = new DateTime(2026, 11, 5)
                }
            );

            // Seed Services
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    CarId = 1,
                    WorkHours = 2,
                    WorkHourPrice = 15000,
                    ServiceDate = new DateTime(2025, 9, 15),
                    ServiceDescription = "Olajcsere és szűrőcsere"
                },
                new Service
                {
                    Id = 2,
                    CarId = 1,
                    WorkHours = 4,
                    WorkHourPrice = 15000,
                    ServiceDate = new DateTime(2025, 10, 1),
                    ServiceDescription = "Fékbetét csere"
                },
                new Service
                {
                    Id = 3,
                    CarId = 2,
                    WorkHours = 2,
                    WorkHourPrice = 18000,
                    ServiceDate = new DateTime(2025, 10, 5),
                    ServiceDescription = "Klíma karbantartás"
                },
                new Service
                {
                    Id = 4,
                    CarId = 3,
                    WorkHours = 2,
                    WorkHourPrice = 16000,
                    ServiceDate = new DateTime(2025, 9, 20),
                    ServiceDescription = "Futómű ellenőrzés és beállítás"
                },
                new Service
                {
                    Id = 5,
                    CarId = 4,
                    WorkHours = 4,
                    WorkHourPrice = 20000,
                    ServiceDate = new DateTime(2025, 10, 10),
                    ServiceDescription = "Motor diagnosztika"
                }
            );

            // Seed Parts
            modelBuilder.Entity<Part>().HasData(
                new Part
                {
                    Id = 1,
                    ServiceId = 1,
                    PartNumber = "OIL-001",
                    Name = "Motorolaj 5W-30",
                    Price = 8500,
                    Description = "Leírás",
                    Quantity = 5
                },
                new Part
                {
                    Id = 2,
                    ServiceId = 1,
                    PartNumber = "FLT-002",
                    Name = "Olajszűrő",
                    Price = 2500,
                    Description = "Leírás",
                    Quantity = 1
                },
                new Part
                {
                    Id = 3,
                    ServiceId = 2,
                    PartNumber = "BRK-101",
                    Name = "Fékbetét szett elöl",
                    Price = 18000,
                    Description = "Leírás",
                    Quantity = 1
                },
                new Part
                {
                    Id = 4,
                    ServiceId = 2,
                    PartNumber = "BRK-102",
                    Name = "Féktárcsa pár",
                    Price = 25000,
                    Description = "Leírás",
                    Quantity = 1
                },
                new Part
                {
                    Id = 5,
                    ServiceId = 3,
                    PartNumber = "AC-301",
                    Name = "Klíma szűrő",
                    Price = 4500,
                    Description = "Leírás",
                    Quantity = 1
                },
                new Part
                {
                    Id = 6,
                    ServiceId = 3,
                    PartNumber = "AC-302",
                    Name = "Klíma gáz töltés",
                    Description = "Leírás",
                    Price = 12000,
                    Quantity = 1
                },
                new Part
                {
                    Id = 7,
                    ServiceId = 4,
                    PartNumber = "SUS-201",
                    Name = "Lengéscsillapító pár",
                    Price = 45000,
                    Description = "Leírás",
                    Quantity = 1
                },
                new Part
                {
                    Id = 8,
                    ServiceId = 5,
                    PartNumber = "ENG-401",
                    Name = "Gyújtógyertya szett",
                    Price = 16000,
                    Description = "Leírás",
                    Quantity = 1
                }
            );
        }
    }
}
