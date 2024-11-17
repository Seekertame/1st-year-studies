namespace Self2_2
{
    public class Polygon
    {
        public int Sides { get; set; } 
        public double SideLength { get; set; } 

        public Polygon(int sides, double sideLength)
        {
            Sides = sides;
            SideLength = sideLength;
        }

        public double CalculateArea()
        {
            // Формула площади правильного многоугольника: (n * s^2) / (4 * tan(π / n))
            return (Sides * Math.Pow(SideLength, 2)) / (4 * Math.Tan(Math.PI / Sides));
        }
    }
    
    public class Person(
        string name,
        DateTime birthDate,
        bool isMale
    )
    {
        private string _name = name;
        private DateTime _birthDate = birthDate;
        private bool _isMale = isMale;

        public virtual string ShowInfo()
        {
            return $"Name: {_name}, BirthDate: {_birthDate}, {(_isMale ? "Male" : "Female")}";
        }
    }

    public class Student(
        string name,
        DateTime birthDate,
        bool isMale,
        string institute,
        string speciality
    ) : Person(name, birthDate, isMale)
    {
        private string _institute = institute;
        private string _speciality = speciality;

        public override string ShowInfo()
        {
            return base.ShowInfo() + $", Institute: {_institute}, Speciality: {_speciality}";
        }
    }

    public class Employee(
        string name,
        DateTime birthDate,
        bool isMale,
        string companyName,
        string post,
        string schedule,
        decimal salary
    ) : Person(name, birthDate, isMale)
    {
        private string _companyName = companyName;
        private string _post = post;
        private string _schedule = schedule;
        private decimal _salary = salary;

        public override string ShowInfo()
        {
            return base.ShowInfo() +
                   $", Company: {_companyName}, Post: {_post}, Schedule: {_schedule}, Salary: {_salary}";
        }
    }

    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.Write("Введите количество многоугольников: ");
            int count = int.Parse(Console.ReadLine());

            Polygon[] polygons = new Polygon[count];

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\nМногоугольник {i + 1}:");
                Console.Write("Введите количество сторон: ");
                int sides = int.Parse(Console.ReadLine());

                Console.Write("Введите длину стороны: ");
                double sideLength = double.Parse(Console.ReadLine());

                polygons[i] = new Polygon(sides, sideLength);
            }

            double minArea = double.MaxValue;
            double maxArea = double.MinValue;
            foreach (Polygon polygon in polygons)
            {
                double area = polygon.CalculateArea();
                if (area < minArea) minArea = area;
                if (area > maxArea) maxArea = area;
            }

            Console.WriteLine("\nПлощади многоугольников:");
            foreach (Polygon polygon in polygons)
            {
                double area = polygon.CalculateArea();
                
                if (area == minArea)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (area == maxArea)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.WriteLine($"Многоугольник с {polygon.Sides} сторонами и длиной стороны {polygon.SideLength}: Площадь = {area:F2}");
            }
            Console.ResetColor();
            
            Console.WriteLine("\nSELF 2-3\n");
            
            Person[] persons = {
                new Person("Naruto", new DateTime(2002, 08, 09), true),
                new Student("Diana", new DateTime(2006, 11, 05), false, "HSE", "Management"),
                new Employee("David", new DateTime(1999, 12, 31), true, "Yandex", "General Director", "Monday-Friday 9AM-5PM", 2000000),
            };
        
            foreach (Person person in persons)
            {
                Console.WriteLine($"Class: {person.GetType().Name}, {person.ShowInfo()}");
                Console.WriteLine();
            }
        }
    }
}