namespace CarRentals.Types
{
    public enum CarType
    {
        Sedan = 0,
        SUV = 1,
        Van = 2
    }


    public class Car
    {
        public Guid Id { get; } = Guid.NewGuid();
        public CarType Type { get; }

        public Car(CarType type)
        {
            Type = type;
        }
    }
}
