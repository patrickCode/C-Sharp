using PersonLibrary;
using System.Collections.Generic;

namespace People.Library
{
    public interface IPersonRepository
    {
        List<Person> GetPeople();
    }
}