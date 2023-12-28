using System;
using NUnit.Framework;

namespace BinSplitterTest
{
    public class ProgramTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void SplitBytesByHeadersTest()
        {
            // byte[] bytes{ 0000, 0001, 0002, 0x03, 0x04, 0005, 0006, 0007, 0008, 0009, };
            byte[] bytes = { 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, };
            long[] headers = { 4, 7, };

            var result = BinSplitter.Program.SplitBytesByHeaders(bytes,headers);

            CollectionAssert.AreEqual(new byte[] { 0x7, 0x8, 0x9, }, result[0]);
            CollectionAssert.AreEqual(new byte[] { 0xA,0xB,0xC, }, result[1]);
        }

        [Test]
        public void FindPatternAddressesTest()
        {
            // byte[] bytes{ 000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, };
            byte[] bytes = { 0x0, 0x1, 0x2, 0x3, 0x1, 0x2, 0x3, 0x4, 0x1, 0x2, 0x3, };
            byte[] pattern = { 0x1, 0x2, 0x3, };

            var addresses = BinSplitter.Program.FindPatternAddresses(bytes, pattern);

            CollectionAssert.AreEqual(new byte[] { 1, 4, 8, }, addresses);
        }
    }
}