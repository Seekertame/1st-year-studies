using System.Text;
namespace Self2_1
{
    public class Circle
    {
        private double _r;

        public double Radius
        {
            get { return _r; }
            set { _r = value > 0 ? value : throw new ArgumentException("Радиус должен быть положительным."); }
        }

        public double S
        {
            get { return Math.PI * _r * _r; }
        }

        public Circle()
        {
            _r = 1;
        }
        public Circle(double radius)
        {
            Radius = radius;
        }
    }

    class Rectangle
    {
        private double height;
        private double width;

        public double Height
        {
            get { return height; }
            set { height = value > 0 ? value : 1; }
        }

        public double Width
        {
            get { return width; }
            set { width = value > 0 ? value : 1; }
        }

        public double Perimeter
        {
            get { return 2 * (width + height); }
        }

        public double Area
        {
            get { return width * height; }
        }

        public override string ToString()
        {
            return $"Rectangle: Height = {height}, Width = {width}, Perimeter = {Perimeter}, Area = {Area}";
        }

        public Rectangle()
        {
            height = 1;
            width = 1;
        }

        public Rectangle(double height, double width)
        {
            this.height = height;
            this.width = width;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Введите высоту первого прямоугольника:");
            double height1 = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите ширину первого прямоугольника:");
            double width1 = Convert.ToDouble(Console.ReadLine());

            Rectangle rect1 = new Rectangle(height1, width1);

            Console.WriteLine("Введите высоту второго прямоугольника:");
            double height2 = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите ширину второго прямоугольника:");
            double width2 = Convert.ToDouble(Console.ReadLine());

            Rectangle rect2 = new Rectangle(height2, width2);

            Console.WriteLine("\nПервый прямоугольник:");
            Console.WriteLine($"Площадь: {rect1.Area}");
            Console.WriteLine($"Периметр: {rect1.Perimeter}");

            Console.WriteLine("\nВторой прямоугольник:");
            Console.WriteLine($"Площадь: {rect2.Area}");
            Console.WriteLine($"Периметр: {rect2.Perimeter}");



            Console.WriteLine("Введите минимальное значение радиуса (Rmin):");
            double Rmin = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Введите максимальное значение радиуса (Rmax):");
            double Rmax = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Введите величину шага (delta):");
            double delta = Convert.ToDouble(Console.ReadLine());

            if (Rmin < 0 || Rmax <= Rmin || delta <= 0)
            {
                Console.WriteLine(
                    "Некорректные значения диапазона или шага. Убедитесь, что Rmin >= 0, Rmax > Rmin и delta > 0.");
                return;
            }

            Circle circle = new Circle();

            Console.WriteLine("\nЗначения площади круга для различных значений радиуса:");

            for (double r = Rmin; r <= Rmax; r += delta)
            {
                circle.Radius = r;
                Console.WriteLine($"Радиус: {r:F2}, Площадь: {circle.S:F2}");
            }
        }
    }
}