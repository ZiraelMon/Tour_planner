using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Legacy;

namespace TourPlanner.Tests.ViewModels {
    [TestFixture]
    public class MainViewModelTests {
        private AppDbContext _context;
        private MainViewModel _mainViewModel;
        private TourService _tourService;

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
            _mainViewModel = new MainViewModel(_context, _tourService);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void MainViewModel_Initialization_ShouldInitializeCorrectly() {
            // Assert
            ClassicAssert.IsNotNull(_mainViewModel.Tours);
            ClassicAssert.AreEqual(2, _mainViewModel.Tours.Count);
            ClassicAssert.IsNull(_mainViewModel.SelectedTour);
        }

        [Test]
        public void AddTourCommand_ShouldAddNewTourToCollection() {
            // Act
            _mainViewModel.AddTourCommand.Execute(null);

            // Assert
            var tours = _context.Tours.ToList();
            ClassicAssert.AreEqual(3, tours.Count);
            ClassicAssert.AreEqual("New Tour", tours.Last().TourName);
        }

        [Test]
        public void DeleteTourCommand_ShouldRemoveSelectedTourFromCollection() {
            // Arrange
            _mainViewModel.SelectedTour = _mainViewModel.Tours.First();
            _context.Entry(_mainViewModel.SelectedTour).State = EntityState.Unchanged;

            // Act
            _mainViewModel.DeleteTourCommand.Execute(null);

            // Assert
            var tours = _context.Tours.ToList();
            ClassicAssert.AreEqual(1, tours.Count);
            ClassicAssert.IsFalse(tours.Any(t => t.TourId == _mainViewModel.SelectedTour.TourId));
        }

        [Test]
        public void CanExecuteTourCommand_ShouldReturnFalseWhenNoTourSelected() {
            // Assert
            ClassicAssert.IsFalse(_mainViewModel.UpdateTourCommand.CanExecute(null));
            ClassicAssert.IsFalse(_mainViewModel.DeleteTourCommand.CanExecute(null));
        }

        [Test]
        public void CanExecuteTourCommand_ShouldReturnTrueWhenTourSelected() {
            // Arrange
            _mainViewModel.SelectedTour = _mainViewModel.Tours.First();

            // Assert
            ClassicAssert.IsTrue(_mainViewModel.UpdateTourCommand.CanExecute(null));
            ClassicAssert.IsTrue(_mainViewModel.DeleteTourCommand.CanExecute(null));
        }
    }
}
