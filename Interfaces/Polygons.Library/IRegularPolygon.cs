namespace Polygons.Library
{
    //Best Practice: Program to an abstraction rather that a Concrete Type
    //Interface is same as a pure abstract class. Pure Abstract class have all memebers declared as abstract.
    public interface IRegularPolygon
    {
        string Name { get; set; }
        int NumberOfSides { get; set; }
        int SideLength { get; set; }
        double GetPerimeter();
        double GetArea();
        void Display();
    }
}

/*
     * Concreate Class - No Compile-time checking
     * Abstract Class - Compile-time checking
     * Interface - Compile-time checking
     * 
     * Comparison - Interface vs Abstract
     * Abstract classes can contain implementation Code but interfaces cannot
     * Classes can inherit a single Abstract class but implement any number of interfaces
     * Abstact classes can have access modifiers but in Interface everything needs to be public
     * Abstract classes can contain anything that is put in a normal class (Fields, Properties, Constructors, Destructors, Methods, Events, Indexers)
     * Interfaces can contain only Properties, Methods, Events, Indexers
 */