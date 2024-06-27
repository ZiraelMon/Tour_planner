using NUnit.Framework;
using Moq;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;

namespace TourPlanner.Tests.Services {
    [TestFixture]
    public class TourServiceTests {
        private AppDbContext _context;
        private TourService _tourService;
        private Mock<TourReportService> _reportServiceMock;

        [SetUp]
        public void SetUp() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            _context.Tours.AddRange(new List<Tour>
            {
                new Tour
                {
                    TourId = 1,
                    TourName = "Test Tour 1",
                    Description = "Description 1",
                    From = "From 1",
                    To = "To 1",
                    TransportType = "Transport 1",
                    TourDistance = 10.0,
                    EstimatedTime = TimeSpan.FromHours(1),
                    RouteImage = "Route1.png",
                    Popularity = 5,
                    ChildFriendliness = 3
                },
                new Tour
                {
                    TourId = 2,
                    TourName = "Test Tour 2",
                    Description = "Description 2",
                    From = "From 2",
                    To = "To 2",
                    TransportType = "Transport 2",
                    TourDistance = 20.0,
                    EstimatedTime = TimeSpan.FromHours(2),
                    RouteImage = "Route2.png",
                    Popularity = 4,
                    ChildFriendliness = 2
                }
            });

            _context.SaveChanges();

            _reportServiceMock = new Mock<TourReportService>();
            _tourService = new TourService(_context, _reportServiceMock.Object);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAllTours_ShouldReturnAllTours() {
            var tours = _tourService.GetAllTours();
            ClassicAssert.AreEqual(2, tours.Count());
        }

        [Test]
        public void GetTourById_ShouldReturnCorrectTour() {
            var tour = _tourService.GetTourById(1);
            ClassicAssert.IsNotNull(tour);
            ClassicAssert.AreEqual(1, tour.TourId);
            ClassicAssert.AreEqual("Test Tour 1", tour.TourName);
        }

        [Test]
        public void AddTour_ShouldAddNewTour() {
            var newTour = new Tour {
                TourId = 3,
                TourName = "Test Tour 3",
                Description = "Description 3",
                From = "From 3",
                To = "To 3",
                TransportType = "Transport 3",
                TourDistance = 30.0,
                EstimatedTime = TimeSpan.FromHours(3),
                RouteImage = "Route3.png",
                Popularity = 3,
                ChildFriendliness = 1
            };
            _tourService.AddTour(newTour);

            var tours = _tourService.GetAllTours();
            ClassicAssert.AreEqual(3, tours.Count());
            ClassicAssert.IsTrue(tours.Any(t => t.TourId == 3));
        }

        [Test]
        public void DeleteTour_ShouldRemoveSelectedTour() {
            var tour = _context.Tours.First();
            _tourService.DeleteTour(tour);

            var tours = _tourService.GetAllTours();
            ClassicAssert.AreEqual(1, tours.Count());
            ClassicAssert.IsFalse(tours.Any(t => t.TourId == tour.TourId));
        }

        [Test]
        public void SearchTours_ShouldReturnFilteredResults() {
            var results = _tourService.SearchTours("Test Tour 1");
            ClassicAssert.AreEqual(1, results.Count());
            ClassicAssert.AreEqual("Test Tour 1", results.First().TourName);

            results = _tourService.SearchTours("Description 2");
            ClassicAssert.AreEqual(1, results.Count());
            ClassicAssert.AreEqual("Description 2", results.First().Description);

            results = _tourService.SearchTours("Nonexistent");
            ClassicAssert.AreEqual(0, results.Count());
        }
    }
}
