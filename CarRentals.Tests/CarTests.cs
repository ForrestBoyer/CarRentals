using CarRentals.Types;

namespace CarRentals.Tests
{
    public class CarTests
    {
        [Fact]
        public void TestCreateCar()
        {
            Car car = new Car(CarType.Sedan);

            Assert.NotNull(car);
            Assert.True(car.Id != null);
            Assert.True(car.Id != Guid.Empty);
            Assert.True(car.Type == CarType.Sedan);
        }

        [Fact]
        public void CarsAreUnique()
        {
            Car car1 = new Car(CarType.Sedan);
            Car car2 = new Car(CarType.Sedan);

            Assert.True(car1 != car2);
            Assert.True(car1.Id != car2.Id);
        }

        [Fact]
        public void CarTypesAreDifferent()
        {
            Car car1 = new Car(CarType.Sedan);
            Car car2 = new Car(CarType.SUV);
            Car car3 = new Car(CarType.Van);

            Assert.True(car1.Type != car2.Type);
            Assert.True(car1.Type != car3.Type);
            Assert.True(car2.Type != car3.Type);
        }
    }
}
