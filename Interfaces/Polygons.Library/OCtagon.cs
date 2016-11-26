using System;

namespace Polygons.Library
{
    public class Octagon : IRegularPolygon
    {
        public Octagon(int length)
        {
            NumberOfSides = 8;
            SideLength = length;
        }
        public string Name
        {
            get
            {
                return "Octagon";
            }
            set { }
        }
        public int NumberOfSides
        {
            get; set;
        }

        public int SideLength
        {
            get; set;
        }

        public double GetArea()
        {
            return SideLength * SideLength * (2 + 2 * Math.Sqrt(2));
        }

        public double GetPerimeter()
        {
            return NumberOfSides * SideLength;
        }

        public void Display()
        {
            Console.WriteLine(Name);
            Console.WriteLine("Number of Sides: " + NumberOfSides);
            Console.WriteLine("Side Length: " + SideLength);
            Console.WriteLine("Permimeter: " + GetPerimeter());
            Console.WriteLine("Area: " + GetArea());
        }
    }
}