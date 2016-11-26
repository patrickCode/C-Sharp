using System;
using Interface.Library;

namespace Viewer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Standard
            StandardCatalog standardCatalog = new StandardCatalog();
            ISaveable saveable = new StandardCatalog();
            IPersistable persistable = new StandardCatalog();

            Console.WriteLine("Standard Implementation");
            Console.WriteLine("Concrete Class: " + standardCatalog.Save());
            Console.WriteLine("ISaveable: " + saveable.Save());
            Console.WriteLine("IPersistable: " + persistable.Save());

            Console.ReadLine();

            //Explicit
            ExplicitCatalog explicitCatalog = new ExplicitCatalog();
            ISaveable eSaveable = new ExplicitCatalog();
            IPersistable ePersistable = new ExplicitCatalog();

            Console.WriteLine("Explicit Implementation");
            Console.WriteLine("Concrete Class: " + explicitCatalog.Save());
            Console.WriteLine("ISaveable: " + eSaveable.Save());
            Console.WriteLine("IPersistable: " + ePersistable.Save());
            Console.ReadLine();

            //Casting
            Console.WriteLine("Explicit Implementation");
            Console.WriteLine("Concrete Class: " + explicitCatalog.Save());
            Console.WriteLine("(ISaveable)Concrete: " + ((ISaveable)explicitCatalog).Save());
            Console.WriteLine("(IPersistable)Concrete: " + ((IPersistable)explicitCatalog).Save());
            Console.ReadLine();

            //Exlicit Catalog with no Implementation
            ExplicitCatalog_2 explicitCatalog2 = new ExplicitCatalog_2();
            ISaveable eSaveable2 = new ExplicitCatalog_2();
            IPersistable ePersistable2 = new ExplicitCatalog_2();

            Console.WriteLine("Explicit Implementation (with no implicit implementation)");
            //Console.WriteLine("Concrete Class: " + explicitCatalog2.Save()); --Won't work because there is no implicit implementation
            Console.WriteLine("ISaveable: " + eSaveable2.Save());
            Console.WriteLine("IPersistable: " + ePersistable2.Save());
            Console.ReadLine();
        }
    }
}