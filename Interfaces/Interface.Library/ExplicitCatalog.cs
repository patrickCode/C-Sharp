/* EXPLICIT IMPLEMENTATION
 * Explicit implementation is required when 2 methods have the same name but the retun type and the parameters are same. Overload is not possible because they have the same parameters. In such sitations at least of them needs to have explicit implementation.
 */

using System;

namespace Interface.Library
{
    public class ExplicitCatalog : ISaveable, IPersistable, IVoidSaveable
    {   
        public string Save()
        {
            return "Catalog Save";
        }

        void IVoidSaveable.Save()
        {
            Console.WriteLine("Void Saveable");
        }

        //Access Modifers are not allowed on Explicit interfaces
        string ISaveable.Save()
        {
            return "Saveable Save";
        }

        string IPersistable.Save()
        {
            return "Persistable Save";
        }
    }

    public class ExplicitCatalog_2 : ISaveable, IPersistable
    {
        //Access Modifers are not allowed on Explicit interfaces
        string ISaveable.Save()
        {
            return "Saveable Save";
        }

        string IPersistable.Save()
        {
            return "Persistable Save";
        }
    }
}