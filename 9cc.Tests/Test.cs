using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Data;

namespace _9cc.Tests
{
    [TestClass]
    public class RunTest
    {
        public string GenRunFile(string value){
            var compiler = new Compiler();
            var objctCode = compiler.compile(value);

            File.WriteAllText(@"tmp.s",objctCode);

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
        public void RunTest1()
        {   
            var input = "0";            
            var expected = "0";
            var result = GenRunFile(input);
            Assert.AreEqual(result,expected);
        }

        [TestMethod]
        public void RunTest2()
        {   
            var input = "42";        
            var expected = "42";
            var result = GenRunFile(input);
            Assert.AreEqual(result,expected);
        }

        [TestMethod]
        public void RunTest3()
        {   
            var input = "5+20-4";        
            var expected = "21";
            var result = GenRunFile(input);
            Assert.AreEqual(result,expected);
        }
    }
}
