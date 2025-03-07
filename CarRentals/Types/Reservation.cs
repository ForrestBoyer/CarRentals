using System.Data;

namespace CarRentals.Types
{
    public class Reservation
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Car Car { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Reservation(DateTime start, DateTime end, Car car)
        {
            if (end < start)
            {
                throw new Exception("End date cannot be before start date");
            }

            if (car == null)
            {
                throw new Exception("Car cannot be null");
            }

            Car = car;
            Start = start;
            End = end;
        }
    }
}
