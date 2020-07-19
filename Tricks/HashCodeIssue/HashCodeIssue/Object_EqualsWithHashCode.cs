using System.Linq;

namespace HashCodeIssue
{
    public class Object_EqualsWithHashCode: BaseObject
    {
        public Object_EqualsWithHashCode()
        {
        }

        public override bool Equals(object obj)
        {
            var other = obj as Object_EqualsWithHashCode;
            if (other == null)
                return false;

            return (Id == other.Id && Values.SequenceEqual(other.Values));
        }

        public override int GetHashCode()
        {
            int hash = Id;
            if (Values != null && Values.Any())
            {
                foreach(var value in Values)
                {
                    hash = (hash * 11) + value.GetHashCode();
                }
            }
            return hash;
        }
    }
}