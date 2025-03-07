using CarRentals.Types;

namespace CarRentals.Tests
{
    public class RentalSystemTests
    {
        private static void ResetRentalSystem()
        {
            RentalSystem.Cars.Clear();
            foreach (CarType type in Enum.GetValues(typeof(CarType)))
            {
                RentalSystem.Cars[type] = new List<Car>();
            }
            RentalSystem.Reservations.Clear();
        }

        [Fact]
        public void TestRentalSystem_InitializesProperly()
        {
            Assert.NotNull(RentalSystem.Reservations);
            Assert.NotNull(RentalSystem.Cars);
            Assert.NotNull(RentalSystem.Cars[CarType.Sedan]);
            Assert.NotNull(RentalSystem.Cars[CarType.SUV]);
            Assert.NotNull(RentalSystem.Cars[CarType.Van]);

            Assert.Empty(RentalSystem.Reservations);
            Assert.Empty(RentalSystem.Cars[CarType.Sedan]);
            Assert.Empty(RentalSystem.Cars[CarType.SUV]);
            Assert.Empty(RentalSystem.Cars[CarType.Van]);
        }

        [Fact]
        public void TestInventory()
        {
            ResetRentalSystem();

            // Add 3 of each type of car to inventory
            RentalSystem.AddCars(CarType.Sedan, 1);
            RentalSystem.AddCars(CarType.SUV, 2);
            RentalSystem.AddCars(CarType.Van, 3);

            // Confirm count of each type of car
            Assert.True(RentalSystem.Cars[CarType.Sedan].Count == 1);
            Assert.True(RentalSystem.Cars[CarType.SUV].Count == 2);
            Assert.True(RentalSystem.Cars[CarType.Van].Count == 3);
        }

        [Fact]
        public void TestDuplicateCarFails()
        {
            ResetRentalSystem();

            Car car = new Car(CarType.Sedan);

            RentalSystem.AddCar(car);

            Assert.Throws<Exception>(() => { RentalSystem.AddCar(car); } );
        }

        [Fact]
        public void TestAddCarFunctions()
        {
            ResetRentalSystem();

            RentalSystem.AddCar(CarType.Sedan);
            Assert.True(RentalSystem.Cars[CarType.Sedan].Count == 1);

            Car car = new Car(CarType.SUV);
            RentalSystem.AddCar(car);
            Assert.True(RentalSystem.Cars[CarType.SUV].Count == 1);

            List<Car> cars = new List<Car>();
            cars.Add(new Car(CarType.Van));
            cars.Add(new Car(CarType.Van));
            RentalSystem.AddCars(cars);
            Assert.True(RentalSystem.Cars[CarType.Van].Count == 2);
        }

        [Fact]
        public void TestOverlap()
        {
            DateTime now = DateTime.Now.AddHours(1);
            DateTime nowPlus3 = now.AddDays(3);
            DateTime nowPlus5 = now.AddDays(5);
            DateTime nowPlus7 = now.AddDays(7);

            // No overlap
            Assert.False(RentalSystem.Overlap(now, nowPlus3, nowPlus5, nowPlus7));

            // Full overlap
            Assert.True(RentalSystem.Overlap(now, nowPlus7, nowPlus3, nowPlus5));

            // Partial frontal overlap
            Assert.True(RentalSystem.Overlap(now, nowPlus5, nowPlus3, nowPlus7));

            // Partial backend overlap
            Assert.True(RentalSystem.Overlap(nowPlus3, nowPlus7, now, nowPlus5));
        }

        [Fact]
        public void TestGetAvailableCar_ReturnsNullWhenNoCars()
        {
            ResetRentalSystem();

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            Assert.Null(RentalSystem.GetAvailableCar(start, end, CarType.Sedan));
            Assert.Null(RentalSystem.GetAvailableCar(start, end, CarType.SUV));
            Assert.Null(RentalSystem.GetAvailableCar(start, end, CarType.Van));
        }

        [Fact]
        public void TestGetAvailableCar_ReturnsNullWhenNoCarOfType()
        {
            ResetRentalSystem();

            Car car = new Car(CarType.Sedan);
            Car car2 = new Car(CarType.Van);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            RentalSystem.AddCar(car);
            RentalSystem.AddCar(car2);

            Car? testCar = RentalSystem.GetAvailableCar(start, end, CarType.SUV);

            Assert.Null(testCar);
        }

        [Fact]
        public void TestGetAvailableCar_ReturnsNullWhenAllCarsOfTypeReserved()
        {
            ResetRentalSystem();

            Car car = new Car(CarType.Sedan);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            RentalSystem.AddCar(car);

            Car? testCar1 = RentalSystem.GetAvailableCar(start, end, CarType.Sedan);

            RentalSystem.RequestReservation(start, end, CarType.Sedan);

            Car? testCar2 = RentalSystem.GetAvailableCar(start, end, CarType.Sedan);

            Assert.NotNull(testCar1);
            Assert.Null(testCar2);
        }

        [Fact]
        public void TestGetAvailableCar_ReturnsCarWhenAvailable()
        {
            ResetRentalSystem();

            Car car = new Car(CarType.Sedan);
            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            RentalSystem.AddCar(car);

            Car? car2 = RentalSystem.GetAvailableCar(start, end, CarType.Sedan);

            Assert.Equal(car, car2);
        }

        [Fact]
        public void TestGetAvailableCar_RetryOnceAvailable()
        {
            ResetRentalSystem();

            Car car = new Car(CarType.Sedan);
            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            DateTime start2 = end.AddDays(1);
            DateTime end2 = start2.AddDays(3);

            RentalSystem.AddCar(car);

            Car? car2 = RentalSystem.GetAvailableCar(start, end, CarType.Sedan);
            Car? car3 = RentalSystem.GetAvailableCar(start, end, CarType.Sedan);

            Assert.NotNull(car2);
            Assert.NotNull(car3);

            Assert.Equal(car2, car3);
        }

        [Fact]
        public void TestRequestReservation_ReturnsNullWhenNoCarAvailable()
        {
            ResetRentalSystem();

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            Assert.Null(RentalSystem.RequestReservation(start, end, CarType.Sedan));
        }

        [Fact]
        public void TestRequestReservation_ThrowsWhenStartDateInPast()
        {
            ResetRentalSystem();

            DateTime pastStart = DateTime.Now.AddDays(-2);
            DateTime end = pastStart.AddDays(3);

            Assert.Throws<Exception>(() => { RentalSystem.RequestReservation(pastStart, end, CarType.Sedan); } );
        }

        [Fact]
        public void TestRequestReservation_ThrowsWhenSameStartAndEndDate()
        {
            ResetRentalSystem();

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start;

            Assert.Throws<Exception>(() => { RentalSystem.RequestReservation(start, end, CarType.Sedan); } );
        }

        [Fact]
        public void TestRequestReservation_ThrowsWhenNumDaysNegative()
        {
            ResetRentalSystem();

            DateTime start = DateTime.Now.AddHours(1);

            Assert.Throws<Exception>(() => { RentalSystem.RequestReservation(start, -2, CarType.Sedan); } );
        }

        [Fact]
        public void TestRequestReservation_SingleReservation()
        {
            ResetRentalSystem();

            RentalSystem.AddCar(CarType.Sedan);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            Reservation res = RentalSystem.RequestReservation(start, end, CarType.Sedan);

            Assert.NotNull(res);
        }

        [Fact]
        public void TestRequestReservationOverload_SingleReservation()
        {
            ResetRentalSystem();

            RentalSystem.AddCar(CarType.Sedan);

            DateTime start = DateTime.Now.AddHours(1);

            Reservation res = RentalSystem.RequestReservation(start, 3, CarType.Sedan);

            Assert.NotNull(res);
        }

        [Fact]
        public void TestRequestReservation_MultipleSimultanousReservations()
        {
            ResetRentalSystem();

            RentalSystem.AddCars(CarType.Sedan, 2);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            Reservation res = RentalSystem.RequestReservation(start, end, CarType.Sedan);
            Reservation res2 = RentalSystem.RequestReservation(start, end, CarType.Sedan);

            Assert.NotNull(res);
            Assert.NotNull(res2);

            Assert.NotEqual(res, res2);
            Assert.NotEqual(res.Car, res2.Car);
        }

        [Fact]
        public void TestRequestReservation_OverlappingReservationRequestRetusnNullIdenticalTimes()
        {
            ResetRentalSystem();

            RentalSystem.AddCars(CarType.Sedan, 1);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            Reservation res = RentalSystem.RequestReservation(start, end, CarType.Sedan);
            Reservation res2 = RentalSystem.RequestReservation(start, end, CarType.Sedan);

            Assert.NotNull(res);
            Assert.Null(res2);
        }

        [Fact]
        public void TestRequestReservation_OverlappingReservationRequestRetusnNullStaggeredTimes()
        {
            ResetRentalSystem();

            RentalSystem.AddCars(CarType.Sedan, 1);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            DateTime start2 = start.AddDays(1);
            DateTime end2 = start2.AddDays(3);

            Reservation res = RentalSystem.RequestReservation(start, end, CarType.Sedan);
            Reservation res2 = RentalSystem.RequestReservation(start2, end2, CarType.Sedan);

            Assert.NotNull(res);
            Assert.Null(res2);
        }

        [Fact]
        public void TestReservations_StoresInformationAsExpected()
        {
            ResetRentalSystem();

            RentalSystem.AddCars(CarType.Sedan, 3);
            RentalSystem.AddCars(CarType.SUV, 3);
            RentalSystem.AddCars(CarType.Van, 3);

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = start.AddDays(3);

            RentalSystem.RequestReservation(start, 2, CarType.Sedan);
            RentalSystem.RequestReservation(start, 3, CarType.Sedan);
            RentalSystem.RequestReservation(start, 1, CarType.SUV);
            RentalSystem.RequestReservation(start, end, CarType.SUV);
            RentalSystem.RequestReservation(start, end, CarType.Van);

            Assert.True(RentalSystem.Reservations.Count == 5);

            Assert.True(RentalSystem.Reservations.Where(r => r.Car.Type == CarType.Sedan).Count() == 2);
            Assert.True(RentalSystem.Reservations.Where(r => r.Car.Type == CarType.SUV).Count() == 2);
            Assert.True(RentalSystem.Reservations.Where(r => r.Car.Type == CarType.Van).Count() == 1);

            Assert.True(RentalSystem.Reservations.Where(r => r.Car.Type == CarType.Van).First().End == end);

            Assert.Distinct(RentalSystem.Reservations.Select(r => r.Id).ToList());
        }
    }
}
