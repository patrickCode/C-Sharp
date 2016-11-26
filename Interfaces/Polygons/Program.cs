using System;
using Polygons.Library;

namespace Polygons
{
    class Program
    {
        static void Main(string[] args)
        {
            //Square
            //var square = new Square(14);
            //square.Display();

            //Triangle
            //var triangle = new Triangle(14);
            //triangle.Display();

            //Octagon
            var octagon = new Octagon(14);
            octagon.Display();

            Console.ReadLine();
        }
    }
}