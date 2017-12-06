using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashCodeIssue.Tests
{
    [TestClass]
    public class Tests_EqualsWithWrongHashCode
    {
        [TestMethod]
        public void Object_EqualsWithWrongHashCode_EqualityShouldBeTrue()
        {
            var obj_1 = new Object_EqualsWithWrongHashCode()
            {
                Id = 1,
                Values = new int[] { 1, 2, 3, 4, 5 }
            };
            var obj_2 = new Object_EqualsWithWrongHashCode()
            {
                Id = 1,
                Values = new int[] { 1, 2, 3, 4, 5 }
            };

            Assert.IsTrue(obj_1.Equals(obj_2));
        }
        
        [TestMethod]
        public void Object_EqualsWithWrongHashCode_HashCodeShouldNotBeEqual_ISSUE()
        {
            var obj_1 = new Object_EqualsWithWrongHashCode()
            {
                Id = 1,
                Values = new int[] { 1, 2, 3, 4, 5 }
            };
            var obj_2 = new Object_EqualsWithWrongHashCode()
            {
                Id = 1,
                Values = new int[] { 1, 2, 3, 4, 5 }
            };

            Debug.WriteLine($"Hash Code 1 - {obj_1.GetHashCode()}");
            Debug.WriteLine($"Hash Code 2 - {obj_2.GetHashCode()}");
            Assert.IsFalse(obj_1.GetHashCode() == obj_2.GetHashCode());
        }

        [TestMethod]
        public void Object_EqualsWithWrongHashCode_ShouldHaveSameDifferentKeyInHashset_ISSUE()
        {
            var obj_1 = new Object_EqualsWithWrongHashCode()
            {
                Id = 1,
                Values = new int[] { 1, 2, 3, 4, 5 }
            };
            var obj_2 = new Object_EqualsWithWrongHashCode()
            {
                Id = 1,
                Values = new int[] { 1, 2, 3, 4, 5 }
            };

            var set = new HashSet<Object_EqualsWithWrongHashCode>
            {
                obj_1,
                obj_2
            };

            Assert.IsFalse(set.Count == 1);
        }
    }
}