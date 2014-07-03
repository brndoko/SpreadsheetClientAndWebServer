using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    /// <summary>
    /// Unit Tests for Problem Set 5
    /// </summary>
    [TestClass]
    public class PS5UnitTests
    {
        /// <summary>
        /// Ensure that formulas are being evaluated correctly
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            Spreadsheet sheet = new Spreadsheet(Validator,Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("c1", "=a1-b1");

            double result = (double)sheet.GetCellValue("c1");

            Assert.AreEqual(result, (double)-1.5);
        }

        /// <summary>
        /// Change the contents of a cell and re-evaluate a formula cell
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("c1", "=a1+b1");

            sheet.SetContentsOfCell("b1", "6.5");

            sheet.SetContentsOfCell("c1", "=a1+b1");

            double result = (double)sheet.GetCellValue("c1");

            Assert.AreEqual(result, (double)10.5);
        }

        /// <summary>
        /// Change a cell from a string to a formula, then see if the computation still works
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("c1", "This is a string");
            sheet.SetContentsOfCell("c1", "=a1+b1");

            sheet.SetContentsOfCell("b1", "6.5");

            sheet.SetContentsOfCell("c1", "=a1+b1");

            double result = (double)sheet.GetCellValue("c1");

            Assert.AreEqual(result, (double)10.5);
        }

        /// <summary>
        /// Change a cell from a double to a formula, then see if the computation still works
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("c1", "99");
            sheet.SetContentsOfCell("c1", "=a1+b1");

            sheet.SetContentsOfCell("b1", "6.5");

            sheet.SetContentsOfCell("c1", "=a1+b1");

            double result = (double)sheet.GetCellValue("c1");

            Assert.AreEqual(result, (double)10.5);
        }

        /// <summary>
        /// Change a cell from an old formula to a new formula, then see if the computation still works
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("c1", "=b1-a1");
            sheet.SetContentsOfCell("c1", "=a1+b1");

            sheet.SetContentsOfCell("b1", "6.5");

            sheet.SetContentsOfCell("c1", "=a1+b1");

            double result = (double)sheet.GetCellValue("c1");

            Assert.AreEqual(result, (double)10.5);
        }

        /// <summary>
        /// Change a cell from an old formula to string to a new formula
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("d1", "=b1-a1");
            sheet.SetContentsOfCell("d1", "Testing Time");
            sheet.SetContentsOfCell("d1", "9");
            sheet.SetContentsOfCell("c1", "=a1+d1");

            sheet.SetContentsOfCell("c1", "=a1+d1");

            double result = (double)sheet.GetCellValue("c1");

            Assert.AreEqual(result, (double)13);
        }

        /// <summary>
        /// Change a cell from an old formula to string and try and get its value
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("d1", "=b1-a1");
            sheet.SetContentsOfCell("d1", "Testing Time");


            string result = (string)sheet.GetCellValue("d1");

            Assert.AreEqual(result, "Testing Time");
        }

        /// <summary>
        /// Change a cell from an old formula to string, then attempt to get a list of all nonempty cells
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("d1", "=b1-a1");
            sheet.SetContentsOfCell("d1", "Testing Time");

            List<string> list = new List<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            CollectionAssert.AreEqual(list, new List<string> {"A1","B1","D1" });
        }

        /// <summary>
        /// Save a spreadsheet and attempt to get the correct saved version
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("d1", "=b1-a1");
            sheet.SetContentsOfCell("f1", "Testing Time 1");

            sheet.Save("TestMethod9.xml");

            string version = sheet.GetSavedVersion("TestMethod9.xml");

            Assert.AreEqual(version, "default");
        }

        /// <summary>
        /// Test if our 4 argument constructor is properly loading a saved spreadsheet with
        /// the same version
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            Spreadsheet sheet = new Spreadsheet(Validator, Normalizer, "default");

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("d1", "=b1-a1");
            sheet.SetContentsOfCell("f1", "Testing Time 1");

            sheet.Save("TestMethod10.xml");

            Spreadsheet sheet1 = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

            Assert.AreEqual((double)sheet1.GetCellContents("a1"), 4.00);
            Assert.AreEqual((double)sheet1.GetCellContents("b1"), 5.5);
            Assert.AreEqual(((Formula)sheet1.GetCellContents("d1")).ToString(), "B1-A1");
            Assert.AreEqual((string)sheet1.GetCellContents("f1"), "Testing Time 1");


        }

        /// <summary>
        /// Test that we are returning an empty value for f1 and that it is not longer included in our list of non-empty cells
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "4");
            sheet.SetContentsOfCell("b1", "5.5");
            sheet.SetContentsOfCell("d1", "=b1-a1");
            sheet.SetContentsOfCell("f1", "Testing Time 1");

            string value = (string)sheet.GetCellValue("z1");

            sheet.SetContentsOfCell("f1", String.Empty);

            Assert.AreEqual(value, String.Empty);

            List<string> list = new List<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            CollectionAssert.AreEqual(list, new List<string> { "a1", "b1", "d1" });
        }

        /// <summary>
        /// Test that the constructor throws an appropriate exception when trying to
        /// read a non existent xml file
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestMethod12()
        {
            Spreadsheet sheet = new Spreadsheet("notreal.xml",Validator, Normalizer, "1");
        }

        /// <summary>
        /// Test that the appropriate saved version errors are being thrown
        /// </summary>
        [TestMethod]
        public void TestMethod13()
        {
            bool one = false;
            bool two = false;

            try
            {
                //throw an error because versions do not match
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "1");
            }
            catch(SpreadsheetReadWriteException srwe)
            {
                one = true;
            }

            try
            {
                //throw an error because versions do not match and the filepath does not exist
                Spreadsheet sheet = new Spreadsheet("TestMethod10Fake.xml", Validator, Normalizer, "1");
            }
            catch (SpreadsheetReadWriteException srwe)
            {
                two = true;
            }

            Assert.IsTrue(one && two);
        }

        /// <summary>
        /// Test errors thrown when attempting to save a file
        /// </summary>
        [TestMethod]
        public void TestMethod14()
        {
            bool one = false;
            bool two = false;

            try
            {
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

                sheet.SetContentsOfCell("B3", "Jon is the best");

                sheet.Save(null);

            }
            catch (SpreadsheetReadWriteException srwe)
            {
                one = true;
            }

            try
            {
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

                sheet.SetContentsOfCell("B3", "Brandon is the best");

                sheet.Save("  ");
            }
            catch (SpreadsheetReadWriteException srwe)
            {
                two = true;
            }

            Assert.IsTrue(one && two);
        }

        /// <summary>
        /// Throw an error trying to get a illegal cell names value
        /// </summary>
        [TestMethod]
        public void TestMethod15()
        {
            bool one = false;

            try
            {
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

                sheet.SetContentsOfCell("B3", "Jon is the best");

                sheet.GetCellValue("ZZ");
            }
            catch (InvalidNameException INE)
            {
                one = true;
            }

            Assert.IsTrue(one);
        }

        /// <summary>
        /// Throw an error trying to set cell contents to null
        /// </summary>
        [TestMethod]
        public void TestMethod16()
        {
            bool one = false;
            bool two = false;
            bool three = false;
            try
            {
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

                //set the contents of a cell to null
                sheet.SetContentsOfCell("B3", null);
            }
            catch (ArgumentNullException INE)
            {
                one = true;
            }

            try
            {
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

                // set the name of a cell to an illegal value
                sheet.SetContentsOfCell("BB", "LegitimateValue");
            }
            catch (InvalidNameException INE)
            {
                two = true;
            }

            try
            {
                Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

                //try and retrieve a nonexistent saved version
                sheet.GetSavedVersion("Dummy.txt");
            }
            catch (SpreadsheetReadWriteException ec)
            {
                three = true;
            }

            Assert.IsTrue(one && two && three);
        }

        /// <summary>
        /// Throw an error trying to get a cells contents
        /// </summary>
        [TestMethod]
        public void TestMethod17()
        {
            bool one = false;
            Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

            try
            {
                sheet.GetCellContents("AA");
            }
            catch (Exception e)
            {
                one = true;
            }

            Assert.AreEqual(sheet.GetCellContents("z1"), String.Empty);
            Assert.IsTrue(one);
            
        }

        /// <summary>
        /// using a special validator that only accepts cell names that begin with b
        /// </summary>
        [TestMethod]
        public void TestMethod18()
        {
            bool one = false;
            bool two = false;
            bool three = false;

            Spreadsheet sheet = new Spreadsheet(specialValidator, Normalizer, "default");

            try
            {
                sheet.SetContentsOfCell("b1", "=null");
            }
            catch (Exception e)
            {
                one = true;
            }

            try
            {
                sheet.SetContentsOfCell("", "string");
            }
            catch (Exception e)
            {
                two = true;
            }

            try
            {
                sheet.SetContentsOfCell("aB", "string");
            }
            catch (Exception e)
            {
                three = true;
            }

            Assert.IsTrue(one && two && three);

        }

        /// <summary>
        /// Create a formula with erroneous variables
        /// </summary>
        [TestMethod]
        public void TestMethod19()
        {
            Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

            sheet.SetContentsOfCell("A1", "=z4 + z9");
            
            Assert.IsTrue(sheet.GetCellValue("a1") is FormulaError);

            
            sheet.SetContentsOfCell("z4", "string");
            sheet.SetContentsOfCell("A1", "=z4 + z9");
    
            Assert.IsTrue(sheet.GetCellValue("a1") is FormulaError);

        }

        /// <summary>
        /// Create a circular exception
        /// </summary>
        [TestMethod]
        public void TestMethod20()
        {
            Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

            bool one = false;
            try
            {
                sheet.SetContentsOfCell("A1", "=z4 + z9");
                sheet.SetContentsOfCell("z4", "=a1");
            }
            catch (CircularException e)
            {
                one = true;
            }

            Assert.IsTrue(one);
        }

        /// <summary>
        /// Create a circular exception
        /// </summary>
        [TestMethod]
        public void TestMethod21()
        {
            Spreadsheet sheet = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

            bool one = false;
            try
            {
                sheet.SetContentsOfCell("A1", "=e-9");
            }
            catch (FormulaFormatException e)
            {
                one = true;
            }

            Assert.IsTrue(one);
        }

        /// <summary>
        /// Create a circular exception
        /// </summary>
        [TestMethod]
        public void TestMethod22()
        {
            AbstractSpreadsheet sheet1 = new Spreadsheet();
            AbstractSpreadsheet sheet2 = new Spreadsheet(Validator, Normalizer, "sheet2");
            AbstractSpreadsheet sheet3 = new Spreadsheet("TestMethod10.xml", Validator, Normalizer, "default");

            sheet2 = sheet3;
            sheet1 = sheet2;
            sheet3 = sheet1;
        }

        /// <summary>
        /// Test to ensure that Pi remains somewhat accurate
        /// </summary>
        [TestMethod]
        public void TestMethod23()
        {
            AbstractSpreadsheet sheet1 = new Spreadsheet();

            sheet1.SetContentsOfCell("B1", Math.PI.ToString());

            double pi = (double)sheet1.GetCellValue("B1");

            Assert.IsTrue(Math.Abs(pi - Math.PI) <= 1e-9);
        }

        /// <summary>
        /// Test to ensure that Pi remains somewhat accurate
        /// </summary>
        [TestMethod]
        public void TestMethod24()
        {
            AbstractSpreadsheet sheet1 = new Spreadsheet(Validator, specialNormalizer, "default");

            sheet1.SetContentsOfCell("b1","=c1 + d1");

            IEnumerable<string> list =(sheet1.GetNamesOfAllNonemptyCells());

            HashSet<string> theList = new HashSet<string>(list);

            Assert.IsTrue(theList.SetEquals(new List<string>() { "xb1" }));
        }

        #region Helper Methods

        private string Normalizer(string s)
        {
            return s.ToUpper();
        }

        private bool Validator(string s)
        {
            return true;
            
        }

        private string specialNormalizer(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('x');
            foreach (char c in s.ToCharArray())
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        private bool specialValidator(string s)
        {
            return s.Substring(0) == "B";
        }
        #endregion
    }
}
