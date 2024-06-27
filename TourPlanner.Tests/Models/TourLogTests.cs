using NUnit.Framework;
using NUnit.Framework.Legacy;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace TourPlanner.Tests.Models {
    [TestFixture]
    public class TourLogTests {
        [Test]
        public void Comment_SetProperty_RaisesPropertyChangedEvent() {
            // Arrange
            var tourLog = new TourLog();
            bool eventRaised = false;
            tourLog.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(TourLog.Comment))
                    eventRaised = true;
            };

            // Act
            tourLog.Comment = "New Comment";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void Rating_SetProperty_RaisesPropertyChangedEvent() {
            // Arrange
            var tourLog = new TourLog();
            bool eventRaised = false;
            tourLog.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(TourLog.Rating))
                    eventRaised = true;
            };

            // Act
            tourLog.Rating = 5;

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }
    }
}