using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Xml;
using System.IO;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        #region Instance Variables

        // used to hold relationships between cellNames and the actual cell
        private Dictionary<string, Cell> sheet;

        //used to hold relationships between cells, used for preventing circular logic
        private DependencyGraph dg;

        //keep track of if the file has been changed
        private bool hasBeenChanged = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Zero argument Constructor for creating an empty spreadsheet
        /// </summary>
        public Spreadsheet() :
            base(s => true, s => s, "default")
        {
            sheet = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        // ADDED FOR PS5 - 3 Argument Constructor
        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string _version):
            base(isValid, normalize, _version)
        {
            sheet = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        // ADDED FOR PS5 - 4 Argument Constructor
        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(string _filePath, Func<string, bool> isValid, Func<string, string> normalize, string _version) :
            base(isValid, normalize, _version)
        {
            sheet = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

            //reconstruct the spreadsheet from the filepath
            try
            {
                constructSheetFromFile(_filePath);
            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException("Could not reconstruct the spreadsheet from the given file");
            }
            
        }

        #endregion

        #region PS5 Methods

        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return hasBeenChanged;
            }
            protected set
            {
                hasBeenChanged = value;
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new SpreadsheetReadWriteException("File could not be found");
            }

            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                return reader["version"];
                            default:
                                throw new SpreadsheetReadWriteException("Could not get the version of the saved file");
                        }
                    }
                    else
                    {
                        throw new SpreadsheetReadWriteException("Could not get the version of the saved file");
                    }
                }
            }
            catch (SpreadsheetReadWriteException ex)
            {
                throw new SpreadsheetReadWriteException("There was an error reading the XML file");
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();

                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (KeyValuePair<string, Cell> kv in sheet)
                    {
                        writer.WriteStartElement("cell");

                        writer.WriteElementString("name", kv.Key);

                        //Determine what type the contents is before adding it to the XML
                        if (kv.Value.Contents is String)
                            writer.WriteElementString("contents", (string)kv.Value.Contents);
                        else if (kv.Value.Contents is Double)
                            writer.WriteElementString("contents", kv.Value.Contents.ToString());
                        else
                            writer.WriteElementString("contents", "=" + kv.Value.Contents.ToString());

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    //we just saved so there have been no "changes" made at this point
                    Changed = false;
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Could not save the file");
            }               
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            string nameNormalized;

            if (!isValidCellName(name, out nameNormalized))
            {
                throw new InvalidNameException();
            }

            if (sheet.ContainsKey(nameNormalized))
            {
                return sheet[nameNormalized].Value;
            }
            else
            {
                // the cell is empty
                return String.Empty;
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }

            //this is the first and only place we need to normalize the cell name
            //every other 'set' method stems from here
            string nameNormalized;

            if (!isValidCellName(name, out nameNormalized))
            {
                throw new InvalidNameException();
            }

            double result;

            ISet<string> cellsToRecalculate;

            //OPTION 1 - Double
            //if the contents parses as a double then we set the cell
            //contents and return all direct and indirect values         
            if (Double.TryParse(content, out result))
            {
                cellsToRecalculate = SetCellContents(nameNormalized, result);
            }
            else if (content != String.Empty && content[0] == '=')
            {
                //OPTION 2 - Formula
                //remove the "=" and attempt to set the cell contents with the formula

                    cellsToRecalculate = SetCellContents(nameNormalized, new Formula(content.Substring(1), Normalize, IsValid));
            }
            else
            {
                //OPTION 3 - String
                //set cell contents to a string

                cellsToRecalculate = SetCellContents(nameNormalized, content);
            }

            //Recalculate all the cells that need to be recalculated
            foreach (String cellName in cellsToRecalculate)
            {
                if (sheet.ContainsKey(cellName))
                {
                    if (sheet[cellName].Contents is Formula)
                    {
                        Formula currentF = (Formula)sheet[cellName].Contents;
                        sheet[cellName].Value = currentF.Evaluate(lookupValues);
                    }
                }
            }

            return cellsToRecalculate;
        }

        #endregion

        #region PS4 Methods

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //walk through each cell in the sheet hashmap, it should only contain non empty cells
            foreach (KeyValuePair<string, Cell> kv in sheet)
            {
                    yield return kv.Key;               
            }          
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {           
            string nameNormalized;

            //if the cell name is null or invalid throw an InvalidNameException
            if (!isValidCellName(name, out nameNormalized))
            {
                throw new InvalidNameException();
            }

            //if our spreadsheet already contains the now normalized cell name, modify its contents
            if (sheet.ContainsKey(nameNormalized))
            {
                return sheet[nameNormalized].Contents;
            }

            //if the cell doesn't exist in the hashset then it is assumed to exist but its contents is empty
            return string.Empty;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            string nameNormalized = name;

            //remove dependees and replace them with an empty list
            dg.ReplaceDependees(nameNormalized, new HashSet<string>());

            //check to see if our dictionary contains the cell name already
            //if it does then we will modify its contents
            if (sheet.ContainsKey(nameNormalized))
            {
                //update contents
                sheet[nameNormalized].Contents = number;

                //update value
                sheet[nameNormalized].Value = number;
            }
            else
            {
                //otherwise we will add a new cell to the dictionary
                sheet.Add(nameNormalized, new Cell(nameNormalized, number));
            }

            hasBeenChanged = true;
            return getDirectAndIndirectDependents(nameNormalized);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            string nameNormalized = name;

            //remove dependees and replace them with an empty list
            dg.ReplaceDependees(nameNormalized, new HashSet<string>());

            //check to see if our dictionary contains the cell name already
            //if it does then we will modify its contents
            if (sheet.ContainsKey(nameNormalized))
            {
                //if the text is an empty string then
                //we remove the cell from our dictionary
                if (text.Equals(String.Empty))
                {
                    sheet.Remove(nameNormalized);
                }
                else 
                {
                    //update the contents
                    sheet[nameNormalized].Contents = text;

                    //update the value
                    sheet[nameNormalized].Value = text;
                }
                Changed = true;
            }
            else
            {
                //otherwise we will add a new cell to the dictionary
                //only if the string is not empty
                if (!text.Equals(string.Empty))
                {
                    sheet.Add(nameNormalized, new Cell(nameNormalized, text));
                    Changed = true;
                }                 
            }

            return getDirectAndIndirectDependents(nameNormalized);
        }
      
        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            string nameNormalized = name;

            //Get the old dependees before replacing them
            IEnumerable<string> oldDependees = dg.GetDependees(name);

            //replace all the dependees with the newDependees
            dg.ReplaceDependees(nameNormalized, formula.GetVariables());

            HashSet<string> dependencySet;

            //check if the newly added dependency will throw a circular exception
            try
            {
                dependencySet = new HashSet<string>(GetCellsToRecalculate(nameNormalized));
            }
            catch (CircularException ce)
            {
                dg.ReplaceDependees(nameNormalized, oldDependees);

                //throw the exception
                throw ce;
            }

            //if we get to this point then we have determined that changing the cell to the
            //formula will not cause a circular dependency
            //check to see if our dictionary contains the cell name already
            //if it does then we will modify its contents
            if (sheet.ContainsKey(nameNormalized))
            {
                //modify the contents of the cell to be the new formula
                sheet[nameNormalized].Contents = formula;

                //Evaluate what the value of the formula is and store it in the cell
                sheet[nameNormalized].Value = formula.Evaluate(lookupValues);
            }
            else
            {
                //otherwise we will add a new cell to the dictionary
                sheet.Add(nameNormalized, new Cell(nameNormalized, formula));

                //Evaluate what the value of the formula is and store it in the cell
                sheet[nameNormalized].Value = formula.Evaluate(lookupValues);
            }

            hasBeenChanged = true;
            return getDirectAndIndirectDependents(nameNormalized);
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //if we get to this point then we have already determined that the name is neither
            //null nor invalid
            //if the cell has no dependents, it should just return an empyt hashset
            //at this point the name has already been normalized as well
            //it is only currently called through getCellsToRecalculate
            return dg.GetDependents(name);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines if a cell name is legal according to previously outlined specifications
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameNormalized"></param>
        /// <returns></returns>
        private bool isValidCellName(string name, out string nameNormalized)
        {
            nameNormalized = name;

            //if the cell name is null then return false
            if (name == null)
                return false;

            //only normalize the name of the cell once we determine that it is not null
            nameNormalized = Normalize(name);

            //the cell under scrutiny must pass our validity test
            //if we get passed this point then we test its pattern
            if (!IsValid(nameNormalized))
                return false;

            //If we reach this point then we have passed IsValid
            string tPattern = @"^[A-Za-z]+[1-9]+[0-9]*$";

            return Regex.IsMatch(nameNormalized, tPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns a HashSet of all the direct and indirect dependents of a given cell
        /// assumes valid input for the name
        /// this will only be called if we are certain there is no circular dependency
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private HashSet<string> getDirectAndIndirectDependents(string name)
        {
            //if the cell has dependents then we need to get all
            //of its direct and indirect dependents, this has luckily 
            //already been implemented for us
            //name at this point has already been normalized
            if (dg.HasDependents(name))
            {
                return new HashSet<string>(GetCellsToRecalculate(name));
            }
            else
            {
                //if the cell has no dependents then 
                //return a hashet with the cells name
                return new HashSet<string> { name };
            }
        }

        /// <summary>
        /// returns the value in a the given cell
        /// used to aid the evaluate function in determining the value of a formula
        /// </summary>
        private double lookupValues(string name)
        {
            double result;

            //if our spreadsheet contains the cell then it is not empty
            //we must determine its value
            if (sheet.ContainsKey(name))
            {              
                    if (double.TryParse(sheet[name].Value.ToString(), out result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
            }
            else
            {
                //We have no value associated with that cell
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Helper method used to reconstruct the sheet from
        /// the xml file specified at the path
        /// </summary>
        /// <param name="filePath"></param>
        private void constructSheetFromFile(string filePath)
        {
            if (Version != this.GetSavedVersion(filePath))
                throw new SpreadsheetReadWriteException("Versions do not match!");

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                string cellName = string.Empty;
                string contents = string.Empty;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                break;

                            case "cell":
                                break;

                            case "name":
                                reader.Read();
                                cellName = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                            default:
                                throw new SpreadsheetReadWriteException("Read an unexpected token");
                        }

                        //only add the cell if we have all the values and are
                        //ready to add a new cell
                        if (contents != "" && cellName != "")
                        {
                            SetContentsOfCell(cellName, contents);
                            cellName = string.Empty;
                            contents = string.Empty;
                        }  
                    }
                }
            }

            Changed = false;
        }

        #endregion      

        #region Cell Class

        /// <summary>
        /// Representation of a spreadsheet cell, contains the content, value, 
        /// name and a property indicating if the cell is empty
        /// </summary>
        private class Cell
        {
            #region Instance Variables

            private string name;
            private Object contents;
            private Object value = null;

            #endregion

            #region Constructors

            /// <summary>
            /// Create a cell that takes a non-empty string as its contents
            /// </summary>
            /// <param name="_name"></param>
            /// <param name="_contents"></param>
            public Cell(string _name, string _contents)
            {
                this.name = _name;
                this.contents = _contents;
                this.value = _contents;
            }

            /// <summary>
            /// Create a cell that takes a double as its contents
            /// </summary>
            /// <param name="_name"></param>
            /// <param name="_contents"></param>
            public Cell(string _name, double _contents)
            {
                this.name = _name;
                contents = _contents;
                this.value = _contents;
            }

            /// <summary>
            /// Create a cell that takes a valid formula object as its contents
            /// </summary>
            /// <param name="_name"></param>
            /// <param name="_contents"></param>
            public Cell(string _name, Formula _contents)
            {
                this.name = _name;
                contents = _contents;
                //the value should be set and computed in the calling method
                //immediately after creating the cell
            }

            #endregion

            #region Properties

            /// <summary>
            /// Returns the contents of the cell
            /// </summary>
            public object Contents
            {
                get
                {
                    return contents; 
                }

                //We can only add to this cells contents currently, if it isn't an empty string
                set
                {
                    contents = value;
                }
            }

            /// <summary>
            /// Returns the value of a cell
            /// </summary>
            public object Value 
            { 
                get
                {
                    return value;
                }
                set
                {
                    this.value = value;
                }
            }

            #endregion
        }
        #endregion
    }
}
