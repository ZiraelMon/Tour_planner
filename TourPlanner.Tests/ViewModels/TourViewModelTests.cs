using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework.Legacy;

namespace TourPlanner.Tests.ViewModels {
    [TestFixture]
    public class TourViewModelTests {
        private AppDbContext _context;
        private TourViewModel _tourViewModel;
        private TourService _tourService;
        private Mock<OpenRouteService> _routeServiceMock;
        private TourReportService _tourReportService;

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

            _tourService = new TourService(_context, new TourReportService());
            _routeServiceMock = new Mock<OpenRouteService>(new HttpClient(), Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>());
            _tourReportService = new TourReportService();

            _tourViewModel = new TourViewModel(_tourService, _routeServiceMock.Object, _tourReportService);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void TourViewModel_Initialization_ShouldInitializeCorrectly() {
            // Assert
            ClassicAssert.IsNotNull(_tourViewModel.Tours);
            ClassicAssert.AreEqual(2, _tourViewModel.Tours.Count);
            ClassicAssert.IsNull(_tourViewModel.SelectedTour);
        }

        [Test]
        public void AddTourCommand_ShouldAddNewTour() {
            // Act
            _tourViewModel.AddTourCommand.Execute(null);

            // Assert
            var tours = _context.Tours.ToList();
            ClassicAssert.AreEqual(3, tours.Count);
            ClassicAssert.AreEqual("New Tour", tours.Last().TourName);
        }

        [Test]
        public void DeleteTourCommand_ShouldRemoveSelectedTour() {
            // Arrange
            _tourViewModel.SelectedTour = _tourViewModel.Tours.First();
            _context.Entry(_tourViewModel.SelectedTour).State = EntityState.Unchanged;

            // Act
            _tourViewModel.DeleteTourCommand.Execute(null);

            // Assert
            var tours = _context.Tours.ToList();
            ClassicAssert.AreEqual(1, tours.Count);
            ClassicAssert.IsFalse(tours.Any(t => t.TourId == _tourViewModel.SelectedTour.TourId));
        }

        [Test]
        public void CanExecuteTourCommand_ShouldReturnFalseWhenNoTourSelected() {
            // Assert
            ClassicAssert.IsFalse(_tourViewModel.UpdateTourCommand.CanExecute(null));
            ClassicAssert.IsFalse(_tourViewModel.DeleteTourCommand.CanExecute(null));
        }

        [Test]
        public void CanExecuteTourCommand_ShouldReturnTrueWhenTourSelected() {
            // Arrange
            _tourViewModel.SelectedTour = _tourViewModel.Tours.First();

            // Assert
            ClassicAssert.IsTrue(_tourViewModel.UpdateTourCommand.CanExecute(null));
            ClassicAssert.IsTrue(_tourViewModel.DeleteTourCommand.CanExecute(null));
        }
    }
}
