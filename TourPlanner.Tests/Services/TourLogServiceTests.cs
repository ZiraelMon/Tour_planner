using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;

namespace TourPlanner.Tests.Services {
    [TestFixture]
    public class TourLogServiceTests {
        private AppDbContext _context;
        private TourLogService _tourLogService;

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

            _tourLogService = new TourLogService(_context);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAllTourLogs_ShouldReturnAllTourLogs() {
            var tourLogs = _tourLogService.GetAllTourLogs();
            ClassicAssert.AreEqual(2, tourLogs.Count());
        }

        [Test]
        public void AddTourLog_ShouldAddNewTourLog() {
            var newTourLog = new TourLog { TourLogId = 3, TourId = 1, Comment = "Log 3", StatusMessage = "Best" };
            _tourLogService.AddTourLog(newTourLog);

            var tourLogs = _tourLogService.GetAllTourLogs();
            ClassicAssert.AreEqual(3, tourLogs.Count());
            ClassicAssert.IsTrue(tourLogs.Any(tl => tl.TourLogId == 3));
        }

        [Test]
        public void DeleteTourLog_ShouldRemoveSelectedTourLog() {
            var tourLog = _context.TourLogs.First();
            _tourLogService.DeleteTourLog(tourLog);

            var tourLogs = _tourLogService.GetAllTourLogs();
            ClassicAssert.AreEqual(1, tourLogs.Count());
            ClassicAssert.IsFalse(tourLogs.Any(tl => tl.TourLogId == tourLog.TourLogId));
        }

        [Test]
        public void SearchTourLogs_ShouldReturnFilteredResults() {
            var results = _tourLogService.SearchTourLogs("Better");
            ClassicAssert.AreEqual(1, results.Count());
            ClassicAssert.AreEqual("Better", results.First().StatusMessage);

            results = _tourLogService.SearchTourLogs("Test Tour");
            ClassicAssert.AreEqual(2, results.Count());

            results = _tourLogService.SearchTourLogs("Nonexistent");
            ClassicAssert.AreEqual(0, results.Count());
        }
    }
}
