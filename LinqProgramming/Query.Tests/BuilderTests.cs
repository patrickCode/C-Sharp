using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.Library;
using System.Diagnostics;
using System.Collections;

namespace Query.Tests
{
    [TestClass]
    public class BuilderTests
    {
        private Builder _builder;

        #region Range
        public BuilderTests()
        {
            //Arrange
            _builder = new Builder();
        }
        [TestMethod]
        public void BuildInegerSequence()
        {
            //Act
            var list = _builder.BuildIntegerSequence();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void BuildArithmenticInegerSequence()
        {
            //Act
            var list = _builder.BuildArithemticIntegerSequence();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void BuildStringInegerSequence()
        {
            //Act
            var list = _builder.BuildStringSequence();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void BuildRandomInegerSequence()
        {
            //Act
            var list = _builder.BuildRandomSequence();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        #endregion

        #region Repeat
        [TestMethod]
        public void BuildRepeatedInegerSequence()
        {
            //Act
            var list = _builder.BuildIntegerSequence(true);

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void BuildRepeatedStringSequence()
        {
            //Act
            var list = _builder.BuildStringSequence(true);

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        #endregion

        #region Compare
        [TestMethod]
        public void Intersect()
        {
            //Act
            var list = _builder.IntersectSequences();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void Concat()
        {
            //Act
            var list = _builder.ConcatSequences();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void ConcatUnique()
        {
            //Act
            var list = _builder.ConcatUniqueSequences();

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }
        #endregion
    }
}
