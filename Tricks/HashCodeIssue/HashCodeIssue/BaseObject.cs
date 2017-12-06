namespace HashCodeIssue
{
    /*
     * The GetHashCode() method should reflect the Equals logic; the rules are:
     *   ** if two things are equal (Equals(...) == true) then they must return the same value for GetHashCode()
     *   ** if the GetHashCode() is equal, it is not necessary for them to be the same; this is a collision, and Equals will be called to see if it is a real equality or not.
     *   https://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overridden
     */
    public class BaseObject
    {
        public int Id { get; set; }
        public int[] Values { get; set; }
    }
}