using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;


namespace SpreadsheetTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class PS4UnitTests
    {
        /// <summary>
        /// Testing SetCellContents
        /// </summary>
        [TestMethod]
        public void Test1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("a1", 95);

            Assert.IsTrue((double)sheet.GetCellContents("a1") == 95);

            sheet.SetCellContents("a1", "");

            Assert.IsTrue(sheet.GetCellContents("a1").Equals(string.Empty));
        }

        /// <summary>
        /// GetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1_1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.GetCellContents("45");
        }

        /// <summary>
        /// GetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        public void Test1_11()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Assert.AreEqual("", sheet.GetCellContents("___"));
        }

        /// <summary>
        /// GetCellContents returns an empty string for a cell that has not been given a value
        /// </summary>
        [TestMethod]
        public void Test1_2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Assert.AreEqual("", sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// SetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1_3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("45",45);
        }

        /// <summary>
        /// Testing that this does NOT cause a circular dependency
        /// </summary>
        [TestMethod]
        public void Test1_4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("a1", 95);
            sheet.SetCellContents("B1", new Formula("a1"));
            sheet.SetCellContents("C1", new Formula("B1 + a1"));
            sheet.SetCellContents("a1", new Formula("D1"));
            sheet.SetCellContents("D1", new Formula("85"));
            sheet.SetCellContents("C1", new Formula("B1 + 55"));
        }

        /// <summary>
        /// SetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1_5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.GetCellContents(null);
        }

        /// <summary>
        /// SetCellContents throws an ArgumentsNullException for null formulas
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test1_6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("A1", (Formula) null);
        }

        /// <summary>
        /// SetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1_7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("55", new Formula("55"));
        }

        /// <summary>
        /// SetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        public void Test1_8()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("A1", "");
            sheet.SetCellContents("B1", "");

            List<string> list = new List<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            CollectionAssert.AreEqual(list, new List<string>());

            sheet.SetCellContents("A1", "This is some text");
            sheet.SetCellContents("B1", 95);

            list = new List<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            CollectionAssert.AreEqual(list, new List<string> {"A1", "B1"});
        }

        /// <summary>
        /// SetCellContents throws an InvalidNameException for invalid cell names
        /// </summary>
        [TestMethod]
        public void Test1_9()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("_", "");
            sheet.SetCellContents("B", "");

            List<string> list = new List<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            CollectionAssert.AreEqual(list, new List<string>());

            sheet.SetCellContents("_", "This is some text");
            sheet.SetCellContents("B", 95);

            list = new List<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            CollectionAssert.AreEqual(list, new List<string> { "_", "B" });
        }

        /// <summary>
        /// Tests that SetCellContents is returning the correct set
        /// test only that it returns direct and indirect values
        /// </summary>
        [TestMethod]
        public void Test2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            
            sheet.SetCellContents("A1", 95);
            sheet.SetCellContents("B1", new Formula("A1*2"));
            sheet.SetCellContents("C1", new Formula("B1 + A1"));
            sheet.SetCellContents("D1", new Formula("B1 - C1"));

            HashSet<string> list = (HashSet<string>)sheet.SetCellContents("A1", 85);


            Assert.IsTrue(list.SetEquals(new HashSet<string> { "B1", "A1", "C1", "D1" }));
        }

        /// <summary>
        /// Tests that SetCellContents is returning the correct set of all direct and indirect values
        /// </summary>
        [TestMethod]
        public void Test3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("A1", 95);
            sheet.SetCellContents("B1", new Formula("A1*2"));
            sheet.SetCellContents("C1", new Formula("B1 + A1"));
            sheet.SetCellContents("C2", new Formula("C1 + 5"));

            HashSet<string> list = (HashSet<string>)sheet.SetCellContents("A1", 95);

            Assert.IsTrue(list.SetEquals(new HashSet<string> { "B1", "A1", "C1", "C2" }));
        }

        /// <summary>
        /// Tests that SetCellContents is returning the correct set of all direct and multiple layers of indirect values
        /// </summary>
        [TestMethod]
        public void Test3_1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("a1", 95);
            sheet.SetCellContents("B1", new Formula("a1"));
            sheet.SetCellContents("C1", new Formula("B1 + a1"));
            sheet.SetCellContents("a1", new Formula("D1"));
            sheet.SetCellContents("D1", new Formula("22"));
            sheet.SetCellContents("C1", new Formula("B1 + 55"));
            sheet.SetCellContents("C2", new Formula("C1 + 55"));

            HashSet<string> list = (HashSet<string>)sheet.SetCellContents("a1", new Formula("D1"));

            Assert.IsTrue(list.SetEquals(new HashSet<string> {"a1", "B1", "C1","C2"}));
        }

        /// <summary>
        /// Tests GetNamesOfAllNonEmptyCells
        /// </summary>
        [TestMethod]
        public void Test4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("B1", new Formula("A1*2"));
            sheet.SetCellContents("C1", new Formula("B1 + A1"));
            sheet.SetCellContents("A1", 95);
            sheet.SetCellContents("A2", "Test");
            sheet.SetCellContents("A2", "            ");
            HashSet<string> list = new HashSet<string>();

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(name);
            }

            Assert.IsTrue(list.SetEquals(new HashSet<string> { "B1", "A1", "C1", "A2" }));
        }

        /// <summary>
        /// Tests GetNamesOfAllNonEmptyCells
        /// </summary>
        [TestMethod]
        public void Test5()
        {

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 3);
            sheet.SetCellContents("B1", new Formula("A1*A1"));
            sheet.SetCellContents("C1", new Formula("B1 + A1"));
            sheet.SetCellContents("D1", new Formula("B1 - C1"));
            sheet.SetCellContents("D1", "               ");
            sheet.SetCellContents("D1", "");
            HashSet<string> list = new HashSet<string>();

            foreach (string s in sheet.GetNamesOfAllNonemptyCells())
            {
                list.Add(s);
            }

            Assert.IsTrue(list.SetEquals(new HashSet<string> { "B1", "A1", "C1"}));
        }

        /// <summary>
        /// Cause a circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test6()
        {

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 3);
            sheet.SetCellContents("B1", new Formula("D1*A1"));
            sheet.SetCellContents("C1", new Formula("B1 + A1"));
            sheet.SetCellContents("D1", new Formula("B1 - C1"));
        }

        /// <summary>
        /// Cause a circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test7()
        {

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("B1", new Formula("A1"));
            sheet.SetCellContents("A1", new Formula("B1"));
        }

        /// <summary>
        /// Cause a circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test8()
        {

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("A1 + 5"));
        }

        /// <summary>
        /// Cause a circular dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test9()
        {

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("B1*2"));
            sheet.SetCellContents("B1", new Formula("C1*2"));
            sheet.SetCellContents("C1", new Formula("A1*2"));

        }

        /// <summary>
        /// SetCellContent with text == null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test10()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", (string) null);
        }

        /// <summary>
        /// SetCellContent with text == null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test11()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("11", "This is a test");
        }

        /// <summary>
        /// SetCellContent with text == null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test12()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("#99"));
        }

        /// <summary>
        /// See what GetDirectDependents does when there are no dependents
        /// </summary>
        [TestMethod]
        public void Test13()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 5);
            //HashSet<string> list = (HashSet<string>) sheet.GetDirectDependents("A1");
            sheet.SetCellContents("A1", 123);
        }


    }
}
