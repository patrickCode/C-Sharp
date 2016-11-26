using System;

namespace Polygons.Library
{
    public abstract class AbstractRegularPolygon
    {
        public string Name { get; set; }
        public int NumberOfSides { get; set; }
        public int SideLength { get; set; }
        public AbstractRegularPolygon(int sides, int length)
        {
            NumberOfSides = sides;
            SideLength = length;
        }

        public double GetPerimeter()
        {
            return NumberOfSides * SideLength;
        }

        //Child class must implement GetArea
        public abstract double GetArea();

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