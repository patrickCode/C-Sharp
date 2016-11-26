using System;
using PersonLibrary;
using People.Library;
using System.Configuration;

namespace People.Viewer
{
    public static class RepostioryFactory
    {
        public static IPersonRepository GetRepository(string repositoryType)
        {
            IPersonRepository repo = null;
            switch (repositoryType)
            {
                case "Default":
                    repo = new PeopleRepository();
                    break;
            }

            return repo;
        }

        //An additional build step is required which will actually bring all the repositories in the bin folder, else dynamic loading won't work
        public static IPersonRepository GetRepository()
        {
            var typeName = ConfigurationManager.AppSettings["RepostoryType"];
            Type repoType = Type.GetType(typeName);

            object repoInstance = Activator.CreateInstance(repoType);
            IPersonRepository repo = repoInstance as IPersonRepository;

            return repo;
        }
    }
}