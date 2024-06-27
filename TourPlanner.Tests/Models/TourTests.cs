using NUnit.Framework;
using NUnit.Framework.Legacy;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace TourPlanner.Tests.Models {
    [TestFixture]
    public class TourTests {
        [Test]
        public void TourName_SetProperty_RaisesPropertyChangedEvent() {
            // Arrange
            var tour = new Tour();
            bool eventRaised = false;
            tour.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(Tour.TourName))
                    eventRaised = true;
            };

            // Act
            tour.TourName = "Test Tour";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void TourDistance_SetProperty_RaisesPropertyChangedEvent() {
            // Arrange
            var tour = new Tour();
            bool eventRaised = false;
            tour.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(Tour.TourDistance))
                    eventRaised = true;
            };

            // Act
            tour.TourDistance = 100;

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }
    }
}