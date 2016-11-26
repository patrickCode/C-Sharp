using System;
using System.Linq;
using System.Collections.Generic;

namespace Query.Library
{
    public class Builder
    {
        #region Range
        public IEnumerable<int> BuildIntegerSequence()
        {
            var integers = Enumerable.Range(0, 10);
            return integers;
        }
        public IEnumerable<int> BuildArithemticIntegerSequence()
        {
            //Select statement is used for projections
            var integers = Enumerable.Range(0, 10)
                        .Select(i => 5 + (10 * i));
            return integers;
        }
        public IEnumerable<string> BuildStringSequence()
        {
            var characters = Enumerable.Range(0, 26)
                .Select(i => ((char)('A' + i)).ToString());
            return characters;
        }
        public IEnumerable<int> BuildRandomSequence()
        {
            var rand = new Random();
            var integers = Enumerable.Range(0, 26)
                .Select(i => i + rand.Next(0, 26));
            return integers;
        }
        #endregion

        #region Repeat
        public IEnumerable<int> BuildIntegerSequence(bool repeat)
        {
            var integers = Enumerable.Repeat(-1, 10);
            return integers;
        }
        public IEnumerable<string> BuildStringSequence(bool repeat)
        {
            var characters = Enumerable.Repeat("A", 10);
            return characters;
        }
        #endregion

        #region Compare
        public IEnumerable<int> IntersectSequences()
        {
            var seq1 = Enumerable.Range(0, 10);
            var seq2 = Enumerable.Range(0, 10)
                .Select(i => i * i);

            return seq1.Intersect(seq2);
        }
        public IEnumerable<int> ConcatSequences()
        {
            var seq1 = Enumerable.Range(0, 10);
            var seq2 = Enumerable.Range(0, 10)
                .Select(i => i * i);

            return seq1.Concat(seq2);
        }
        public IEnumerable<int> ConcatUniqueSequences()
        {
            var seq1 = Enumerable.Range(0, 10);
            var seq2 = Enumerable.Range(0, 10)
                .Select(i => i * i);

            //return seq1.Concat(seq2).Distinct();
            //Same As
            return seq1.Union(seq2);
        }
        #endregion
        
    }
}
