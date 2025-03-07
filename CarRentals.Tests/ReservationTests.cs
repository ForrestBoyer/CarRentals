using CarRentals.Types;

namespace CarRentals.Tests
{
    public class ReservationTests
    {
        [Fact]
        public void TestCreateReservation()
        {
            Car testCar = new Car(CarType.Sedan);
            DateTime start = DateTime.Now;
            DateTime end = start.AddDays(2);

            Reservation res = new Reservation(start, end, testCar);

            Assert.NotNull(res);
            Assert.True(res.Id != null);
            Assert.True(res.Id != Guid.Empty);
            Assert.Equal(res.Car, testCar);
            Assert.Equal(res.Start, start);
            Assert.Equal(res.End, end);
        }

        [Fact]
        public void TestReservationsAreUnique()
        {
            Car testCar = new Car(CarType.Sedan);
            DateTime start = DateTime.Now;
            DateTime end = start.AddDays(2);

            Reservation res = new Reservation(start, end, testCar);
            Reservation res2 = new Reservation(start, end, testCar);

            Assert.NotEqual(res, res2);
            Assert.NotEqual(res.Id, res2.Id);
        }

        [Fact]
        public void TestThrowsOnInvalidRange()
        {
            Car testCar = new Car(CarType.Sedan);
            DateTime start = DateTime.Now;
            DateTime end = start.AddDays(-2);

            Assert.Throws<Exception>(() => { new Reservation(start, end, testCar); });
        }

        [Fact]
        public void TestNullCarThrows()
        {
            Car? nullCar = null;
            DateTime start = DateTime.Now;
            DateTime end = start.AddDays(2);

            Assert.Throws<Exception>(() => {new Reservation(start, end, nullCar); });
        }

        [Fact]
        public void TestChangingProps()
        {
            Car car1 = new Car(CarType.Sedan);
            DateTime start1 = DateTime.Now;
            DateTime end1 = start1.AddDays(2);

            Reservation res = new Reservation(start1, end1, car1);
            Guid id = res.Id;

            Car car2 = new Car(CarType.Sedan);
            DateTime start2 = DateTime.Now.AddDays(3);
            DateTime end2 = start2.AddDays(3);

            res.Car = car2;

            Assert.NotEqual(car1, res.Car);

            res.End = end2;

            Assert.NotEqual(end1, res.End);

            res.Start = start2;

            Assert.NotEqual(start1, res.Start);

            // Confirm that although properties changed the Id did not
            Assert.Equal(id, res.Id);
        }

    }
}
