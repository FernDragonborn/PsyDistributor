using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anagram.Tests
{
    [TestClass()]
    public class AnagramTests
    {
        [TestMethod]
        [DataRow(1)]
        public void ReverseWordsTest()
        {
            // arrange
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string data = "  a1bcd    efg!h";
            string expectedResult = "  d1cba    hgf!e";
#pragma warning restore CS0219
#pragma warning disable CS0168 // Variable is declared but never used
            string actualResult;
#pragma warning restore CS0168 // Variable is declared but never used


            // act 
            //actualResult = Anagram.ReverseWords(data);

            // assert
            //Assert.AreEqual(expectedResult, actualResult);
        }
    }
}