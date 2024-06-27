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
    public class TourLogViewModelTests {
        private AppDbContext _context;
        private TourLogViewModel _tourLogViewModel;
        private TourLogService _tourLogService;
        private TourService _tourService;

        [SetUp]
        public void SetUp() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            _context.Tours.Add(new Tour {
                TourId = 1,
                TourName = "Test Tour",
                Description = "Test Description",
                From = "Test From",
                To = "Test To",
                TransportType = "Test Transport",
                TourDistance = 10.0,
                EstimatedTime = TimeSpan.FromHours(1),
                RouteImage = "TestRoute.png",
                Popularity = 5,
                ChildFriendliness = 3
            });

            _context.TourLogs.AddRange(new List<TourLog>
            {
                new TourLog { TourLogId = 1, TourId = 1, Comment = "Log 1", StatusMessage = "Good" },
                new TourLog { TourLogId = 2, TourId = 1, Comment = "Log 2", StatusMessage = "Better" }
            });

            _context.SaveChanges();

            _tourService = new TourService(_context, new TourReportService());
            _tourLogService = new TourLogService(_context);
            _tourLogViewModel = new TourLogViewModel(_tourLogService, _tourService);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void TourLogViewModel_Initialization_ShouldInitializeCorrectly() {
            // Assert
            ClassicAssert.IsNotNull(_tourLogViewModel.TourLogs);
            ClassicAssert.AreEqual(2, _tourLogViewModel.TourLogs.Count);
            ClassicAssert.IsNull(_tourLogViewModel.SelectedTourLog);
        }

        [Test]
        public void AddTourLogCommand_ShouldAddNewTourLog() {
            // Act
            _tourLogViewModel.AddTourLogCommand.Execute(new TourLog {
                TourLogId = 3,
                TourId = 1,
                Comment = "Log 3",
                StatusMessage = "Best"
            });

            // Assert
            var tourLogs = _context.TourLogs.ToList();
            ClassicAssert.AreEqual(3, tourLogs.Count);
            ClassicAssert.AreEqual("Log 3", tourLogs.Last().Comment);
        }

        [Test]
        public void DeleteTourLogCommand_ShouldRemoveSelectedTourLog() {
            // Arrange
            _tourLogViewModel.SelectedTourLog = _tourLogViewModel.TourLogs.First();
            _context.Entry(_tourLogViewModel.SelectedTourLog).State = EntityState.Unchanged;

            // Act
            _tourLogViewModel.DeleteTourLogCommand.Execute(null);

            // Assert
            var tourLogs = _context.TourLogs.ToList();
            ClassicAssert.AreEqual(1, tourLogs.Count);
            ClassicAssert.IsFalse(tourLogs.Any(t => t.TourLogId == _tourLogViewModel.SelectedTourLog.TourLogId));
        }

        [Test]
        public void CanExecuteTourLogCommand_ShouldReturnFalseWhenNoTourLogSelected() {
            // Assert
            ClassicAssert.IsFalse(_tourLogViewModel.DeleteTourLogCommand.CanExecute(null));
        }

        [Test]
        public void CanExecuteTourLogCommand_ShouldReturnTrueWhenTourLogSelected() {
            // Arrange
            _tourLogViewModel.SelectedTourLog = _tourLogViewModel.TourLogs.First();

            // Assert
            ClassicAssert.IsTrue(_tourLogViewModel.DeleteTourLogCommand.CanExecute(null));
        }
    }
}
