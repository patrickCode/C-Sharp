namespace Interface.Library
{
    public class StandardCatalog: ISaveable, IPersistable
    {
        public void Load()
        {

        }

        public string Save()
        {
            return "Catalog Saved";
        }
    }
}