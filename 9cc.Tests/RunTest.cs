using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace _9cc.Tests
{
    [TestClass]
    public class RunTest
    {
        public string GenRunFile(string value)
        {
            var compiler = new Compiler();
            var objctCode = compiler.compile(value);

            File.WriteAllText(@"tmp.s", objctCode);

            var process = new Process();
            process.StartInfo.FileName = "gcc";
            process.StartInfo.Arguments = "-o tmp tmp.s";
            process.Start();
            process.WaitForExit();

            process.StartInfo.FileName = "./tmp";
            process.StartInfo.Arguments = "";
            process.Start();

            process.WaitForExit();
            var result = process.ExitCode.ToString();
            process.Close();

            File.Delete("tmp");
            File.Delete("tmp.s");

            return result;
        }

        [TestMethod]
        public void OneInteger1()
        {
            var input = "0";
            var expected = "0";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void OneInteger2()
        {
            var input = "42";
            var expected = "42";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void PlusAndSub()
        {
            var input = "5+20-4";
            var expected = "21";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void IncludeSpace()
        {
            var input = " 12 + 34 - 5 ";
            var expected = "41";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }
        
        [TestMethod]
        public void Multiple1()
        {
            var input = "5+6*7";
            var expected = "47";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void Multiple2()
        {
            var input = "5*(9-6)";
            var expected = "15";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void Division()
        {
            var input = "(3+5)/2";
            var expected = "4";
            var result = GenRunFile(input);
            Assert.AreEqual(result, expected);
        }
    }
}
