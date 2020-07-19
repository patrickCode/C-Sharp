using System.Linq;

namespace HashCodeIssue
{
    public class Object_EqualsWithBaseHashCode: BaseObject
    {
        public override bool Equals(object obj)
        {
            var other = obj as Object_EqualsWithBaseHashCode;
            if (other == null)
                return false;

            return (Id == other.Id && Values.SequenceEqual(other.Values));
        }
    }
}