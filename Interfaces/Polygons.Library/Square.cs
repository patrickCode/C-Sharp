using System;

namespace Polygons.Library
{
    public class Square : ConcreteRegularPolygon
    {
        public Square(int length) : base(4, length)
        {
        }

        public override double GetArea()
        {
            return SideLength * SideLength;
        }

        public void Display()
        {
            Console.WriteLine("SQUARE");
            Console.WriteLine("Number of Sides: " + NumberOfSides);
            Console.WriteLine("Side Length: " + SideLength);
            Console.WriteLine("Permimeter: " + GetPerimeter());
            Console.WriteLine("Area: " + GetArea());
        }
    }
}