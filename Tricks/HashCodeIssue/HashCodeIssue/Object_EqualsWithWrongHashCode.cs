using System.Linq;

namespace HashCodeIssue
{
    public class Object_EqualsWithWrongHashCode: BaseObject
    {
        public Object_EqualsWithWrongHashCode()
        {
        }

        public override bool Equals(object obj)
        {
            var other = obj as Object_EqualsWithWrongHashCode;
            if (other == null)
                return false;

            return (Id == other.Id && Values.SequenceEqual(other.Values));
        }

        //Chances of collision are high
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Values.GetHashCode();
        }
    }
}