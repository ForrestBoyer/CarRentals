using CarRentals.Types;

namespace CarRentals
{
    public static class RentalSystem
    {
        public static Dictionary<CarType, List<Car>> Cars { get; set; }
        public static List<Reservation> Reservations { get; set; }

        static RentalSystem()
        {
            Reservations = new List<Reservation>();

            Cars = new Dictionary<CarType, List<Car>>();

            foreach (CarType type in Enum.GetValues(typeof(CarType)))
            {
                Cars[type] = new List<Car>();
            }
        }

        /// <summary>
        /// Adds car to inventory
        /// </summary>
        /// <param name="car"></param>
        public static void AddCar(Car car)
        {
            if (Cars[car.Type].Contains(car))
            {
                throw new Exception("Duplicate car cannot be added");
            }
            else
            {
                Cars[car.Type].Add(car);
            }
        }

        /// <summary>
        /// Adds car to inventory of specified type
        /// </summary>
        /// <param name="type"></param>
        public static void AddCar(CarType type)
        {
            Car car = new Car(type);
            Cars[type].Add(car);
        }

        /// <summary>
        /// Adds cars to inventory
        /// </summary>
        /// <param name="cars"></param>
        public static void AddCars(List<Car> cars)
        {
            foreach (Car car in cars)
            {
                AddCar(car);
            }
        } 

        /// <summary>
        /// Adds num cars of specified type to inventory
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        public static void AddCars(CarType type, int num)
        {
            for (int i = 0; i < num; i++)
            {
                AddCar(type);
            }
        }

        /// <summary>
        /// Removes car and all associated reservations
        /// </summary>
        public static void RemoveCar(Car car)
        {
            // Could consider removing all reservations associated with a car when it is removed
            // Reservations.RemoveAll(r => r.Car == car);
            Cars[car.Type].Remove(car);
        }

        /// <summary>
        /// Removes cars and all associated reservations
        /// </summary>
        public static void RemoveCars(List<Car> cars)
        {
            foreach (Car car in cars)
            {
                RemoveCar(car);
            }
        }   

        /// <summary>
        /// Gets reservation for specified car type for specified number of days starting at start date
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="carType"></param>
        /// <returns>Reservation on sucess, null on failure</returns>
        public static Reservation? RequestReservation(DateTime start, int numDays, CarType carType)
        {
            if (numDays <= 0)
            {
                throw new Exception("Number of days must be positive");
            }

            return RequestReservation(start, start.AddDays(numDays), carType);
        }

        /// <summary>
        /// Gets reservation for specified car type during specified time
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="carType"></param>
        /// <returns>Reservation on sucess, null on failure</returns>
        public static Reservation? RequestReservation(DateTime start, DateTime end, CarType carType)
        {
            if (start < DateTime.Now)
            {
                throw new Exception("Date is in the past");
            }
            else if (start == end)
            {
                throw new Exception("Start and end date must be different");
            }

            var car = GetAvailableCar(start, end, carType);

            if (car == null)
            {
                return null;
            }
            else
            {
                var res = new Reservation(start, end, car);
                Reservations.Add(res);
                return res;
            }
        }

        /// <summary>
        /// Gets a car of specified type that is not reserved for the specified time
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="carType"></param>
        /// <returns>Car or null if no car available</returns>
        public static Car? GetAvailableCar(DateTime start, DateTime end, CarType carType)
        {
            var possibleCars = Cars[carType];

            foreach (Car car in possibleCars)
            {
                var reservations = Reservations.Where(r => r.Car == car);

                bool overlap = false;
                foreach (Reservation res in reservations)
                {
                    if (Overlap(start, end, res.Start, res.End))
                    {
                        overlap = true;
                    }
                }

                if (!overlap)
                {
                    return car;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="e1"></param>
        /// <param name="s2"></param>
        /// <param name="e2"></param>
        /// <returns>True if date ranges overlap, false otherwise</returns>
        public static bool Overlap(DateTime s1, DateTime e1, DateTime s2, DateTime e2)
        {
            return s1 < e2 && s2 < e1;
        }
    }
}
