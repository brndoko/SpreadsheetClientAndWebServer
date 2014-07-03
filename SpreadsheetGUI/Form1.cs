using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using SS;
using System.Text.RegularExpressions;
using System.IO;
using SpreadsheetClientModel;
using MessageMachine;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Form that is built when the Spreadsheet GUI is opened
    /// </summary>
    public partial class Form1 : Form
    {
        #region Member Variables

        // Member variable
        private int currentRow = 0;
        private int currentCol = 0;
        private string currentCellName;      
        private string currentFileName;
        private Spreadsheet sheet;
        private Model model = new Model();
        private Machine messageMachine;
        private int version = -1;
        private string originalContents;
        private bool connected = false;
        private bool restarting = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to create a new instance of a form
        /// and creates a spreadsheet object to keep track of the cells in the sheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Begin with authentication screen
            authentication_mode(true);

            // Register the displaySelection event
            mySpreadsheetPanel.SelectionChanged += displaySelection;

            // Set the initial selection of the panel to A1
            mySpreadsheetPanel.SetSelection(0, 0);
            currentCellName = "A1";
            cellNameTextBox.Text = currentCellName;

            // Create a spreadsheet that converts all input to uppercase
            // save version will be 3505
            sheet = new Spreadsheet(extraValidation, s => s.ToUpper(), "3505");

            // currentFileName is set to an empty string
            currentFileName = String.Empty;

            // Cause the cellContentsTextBox to come into focus
            cellContentsTextBox.Focus();

            // Set the originalContents to be empty
            originalContents = String.Empty;

            // This will set the title of the spreadsheet
            // to the name of the file
            this.Text = currentFileName;

            // Instantiate the message machine
            messageMachine = new Machine();

        }

        /// <summary>
        /// This constructor is only called when we need to create
        /// a new form from an existing file, otherwise it is the same
        /// as the previous constructor
        /// </summary>
        /// <param name="fileName"></param>
        public Form1(string fileName)
        {
            InitializeComponent();

            // Begin with authentication screen
            authentication_mode(true);

            //when the selectionChanged event occurs then we will call displaySelection
            mySpreadsheetPanel.SelectionChanged += displaySelection;

            //Create a spreadsheet that converts all input to uppercase
            //save version will be ps6
            try
            {
                sheet = new Spreadsheet(fileName, extraValidation, s => s.ToUpper(), "ps6");

                #region Construct the Spreadsheet View
                //Reconstruct the view according to what is in the sheet
                foreach (string cellName in sheet.GetNamesOfAllNonemptyCells())
                {
                    currentRow = (Convert.ToInt32(cellName.Substring(1)) - 1);
                    currentCol = (Convert.ToInt32(cellName[0]) - 'A');

                    //set the cell in the grid to display the appropriate value
                    if (sheet.GetCellValue(cellName) is FormulaError)
                    {
                        mySpreadsheetPanel.SetValue(currentCol, currentRow, "Formula Error");
                    }
                    else
                    {
                        mySpreadsheetPanel.SetValue(currentCol, currentRow, sheet.GetCellValue(cellName).ToString());
                    }

                    //Set the cellValueTextBox to the correct cell value
                    if (sheet.GetCellValue(cellName) is FormulaError)
                    {
                        cellValueTextBox.Text = "Formula Error";
                    }
                    else
                    {
                        cellValueTextBox.Text = sheet.GetCellValue(cellName).ToString();
                    }
                }

                currentFileName = fileName;

                #endregion
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not open the spreadsheet");

                //Create a new blank spreadsheet
                sheet = new Spreadsheet(extraValidation, s => s.ToUpper(), "ps6");
            }
                
            //Set the initial selection of the panel to A1
            mySpreadsheetPanel.SetSelection(0, 0);
            currentCellName = "A1";
            cellNameTextBox.Text = currentCellName;

            //set the contents of the box to the correct value
            Object originalContents = sheet.GetCellContents(currentCellName);

            if (originalContents is Formula)
                cellContentsTextBox.Text = "=" + originalContents.ToString();
            else
                cellContentsTextBox.Text = originalContents.ToString();

            // Set the originalContents variable
            this.originalContents = cellContentsTextBox.Text;

            //lastly set the value box to have the correct value
            cellValueTextBox.Text = sheet.GetCellValue(currentCellName).ToString();

            currentRow = 0;
            currentCol = 0;

            //Sets the form name to the name of the file being modified
            this.Text = currentFileName;

            // Instantiate the message machine
            messageMachine = new Machine();
        }

        /// <summary>
        /// Connect to the server and register events for various incoming lines
        /// </summary>
        public bool createModel(String ip, int port)
        {
            // Disconnect from any previous connection
            model.Disconnect();

            // If we can successfuly connect to the server then add the event listeners
            if (model.Connect(ip, port))
            {
                // Used while authenticating client and opening spreadsheet
                model.InvalidLineEvent += InvalidLineReceived;
                model.FileListLineEvent += FileListLineReceived;

                // Used while on the spreadsheet
                model.UpdateLineEvent += UpdateReceived;
                model.SavedLineEvent += SaveReceived;
                model.ErrorLineEvent += ErrorReceived;

                // Register the general line event
                model.GeneralLineEvent += GeneralLineReceived;

                // Register server disconnects
                model.ServerDisconnectEvent += ServerDisconnectReceived;

                return true;
            }
            else
                return false;
        }

        #endregion

        #region Event Handlers

        #region Triggers to send ENTER message to server

        /// <summary>
        /// If the content box loses focus then we will update the current cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellContentsTextBox_Leave(object sender, EventArgs e)
        {
            // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
            // Send the version + 1
            if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
            {
                model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
            }
                
            setUpNewCell(mySpreadsheetPanel);
            cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 
        }

        /// <summary>
        /// If the user clicks any cell when a change has been made to the contents text box then the currently
        /// selected cell will be updated
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            String message = messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text);

            // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
            // Send the version + 1
            if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
            {
                model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
            }

            setUpNewCell(ss);
            cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 
        }

        /// <summary>
        /// If the user hits enter when a change has been made to the contents text box then the currently
        /// selected cell will be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellContentsTextBox_EnterKeyPress(object sender, KeyPressEventArgs e)
        {
            //if they pressed enter in the cellContentsTextBox
            if (e.KeyChar == 13)
            {
                e.Handled = true;           
                mySpreadsheetPanel.SetSelection(currentCol, currentRow + 1);

                // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
                // Send the version + 1
                if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
                {
                    model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
                }

                setUpNewCell(mySpreadsheetPanel);
                cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 
            }
        }

        /// <summary>
        /// Control the arrow keys as well as the delete functionality
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
       {
            // Used to prevent unecessary server sends on the wrong panel
            if (OpenPanel.Visible || AuthPanel.Visible)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            //Handle all the arrow key presses
            if (keyData == Keys.Left)
            {
                mySpreadsheetPanel.SetSelection(currentCol-1, currentRow);

                // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
                // Send the version + 1
                if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
                {
                    model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
                }

                setUpNewCell(mySpreadsheetPanel);
                cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 

                return true;
            }
            else if (keyData == Keys.Right || keyData == Keys.Tab)
            {
                mySpreadsheetPanel.SetSelection(currentCol + 1, currentRow);

                // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
                // Send the version + 1
                if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
                {
                    model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
                }

                setUpNewCell(mySpreadsheetPanel);
                cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 

                return true;
            }
            else if (keyData == Keys.Up)
            {
                mySpreadsheetPanel.SetSelection(currentCol, currentRow - 1);

                // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
                // Send the version + 1
                if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
                {
                    model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
                }

                setUpNewCell(mySpreadsheetPanel);
                cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 

                return true;
            }
            else if (keyData == Keys.Down)
            {
                mySpreadsheetPanel.SetSelection(currentCol, currentRow + 1);

                // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
                // Send the version + 1
                if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
                {
                    model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
                }

                setUpNewCell(mySpreadsheetPanel);
                cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 

                return true;
            }
            else if (keyData == (Keys.Control | Keys.Z))
            {
                String message = messageMachine.build_undo(this.version + 1);
                model.SendMessage(message);

                return true;
            }
            else if (keyData == Keys.Delete)
            {
                cellContentsTextBox.Text = String.Empty;

                // Construct an ENTER message and send to the server ONLY if the new contents is different AND the contents isn't an invalid formula
                // Send the version + 1
                if (cellContentsTextBox.Text != originalContents && !isInvalidFormula())
                {
                    model.SendMessage(messageMachine.build_enter(this.version + 1, currentCellName, cellContentsTextBox.Text));
                }

                setUpNewCell(mySpreadsheetPanel);

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Called when the undo button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            // Construct the undo message and send to server
            String message = messageMachine.build_undo(this.version + 1);
            model.SendMessage(message);
        }

        #endregion

        #region Response From Server Received

        /// <summary>
        /// Called when we receive a response from the server that the password is invalid
        /// </summary>
        private void InvalidLineReceived()
        {
            // We received an incorrect password, stay in authentication mode and notify user
            mySpreadsheetPanel.Invoke(new Action(() => { authentication_mode(true); }));
            mySpreadsheetPanel.Invoke(new Action(() => { AuthError.Text = "Incorrect Server Password, Please Try Again"; }));
            mySpreadsheetPanel.Invoke(new Action(() => { AuthError.Visible = true; }));
        }

        /// <summary>
        /// Called when we receive a response that the password was valid
        /// </summary>
        private void FileListLineReceived(MessageMachine.data d)
        {
            // If the server has determined that the password was valid, then move to the open file screen
            mySpreadsheetPanel.Invoke(new Action(() => {authentication_mode(false);}));
            mySpreadsheetPanel.Invoke(new Action(() => { open_mode(true); }));

            // Populate the list box with the possible spreadsheets
            foreach (String file in d.file_names)
            {
                listBox1.Items.Add(file);
            }
        }

        /// <summary>
        /// Called when we receive a request to update the spreadsheet cell contents
        /// </summary>
        private void UpdateReceived(MessageMachine.data d)
        {
            // Check if we are opening a new spreadsheet or an existing spreadsheet
            mySpreadsheetPanel.Invoke(new Action(() => {open_mode(false);}));
            mySpreadsheetPanel.Invoke(new Action(() => { spreadsheet_mode(true); }));

            // If we are opening a new spreadsheet then we need to set th
            if (this.version < 0)
            {
                this.version = Int32.Parse(d.version) - 1;
            }   
            // If the incoming update is a sync then update the version no questions asked
            else if (d.type == "SYNC")
            {
                this.version = Int32.Parse(d.version);
            }
            // Otherwise check to ensure that the version received is this.version + 1
            else if ((this.version + 1) != Int32.Parse(d.version))
            {
                // Then we need to resync
                String message = messageMachine.build_resync();
                model.SendMessage(message);
                return;
            }

            // Update the current version
            this.version = Int32.Parse(d.version);

            int i = 0;
            int tempRow;
            int tempCol;


            string tempCellContents = cellContentsTextBox.Text;

            foreach(String cellName in d.cell_names)
            {

                // Get the row and column that need to be updated
                tempRow = (Convert.ToInt32(cellName.Substring(1)) - 1);
                tempCol = (Convert.ToInt32(cellName[0]) - 'A');

                mySpreadsheetPanel.Invoke(new Action(() => {updateCells(mySpreadsheetPanel, cellName, d.cell_contents.ElementAt(i), tempCol, tempRow);}));

                i++;
            }

            if (tempCellContents != "")
                cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Text = tempCellContents; }));

            cellContentsTextBox.Invoke(new Action(() => { cellContentsTextBox.Select(cellContentsTextBox.Text.Length, 0); })); 

            
        }

        /// <summary>
        /// Called when we the server has saved our spreadsheet
        /// </summary>
        private void SaveReceived(MessageMachine.data d)
        {
            // Populate the message box with a message stating that the sheet has been saved
            mySpreadsheetPanel.Invoke(new Action(() => { spreadsheetMessageLabel.Text = "Spreadsheet Saved To Server"; }));
        }

        /// <summary>
        /// Called when we receive an error from the server
        /// </summary>
        private void ErrorReceived(MessageMachine.data d)
        {
            // If we are in authMode or openMode display a popup box
            if (AuthPanel.Visible || OpenPanel.Visible)
            {
                mySpreadsheetPanel.Invoke(new Action(() => { MessageBox.Show(d.error_message); }));
            }           
            else
            {
                mySpreadsheetPanel.Invoke(new Action(() => { spreadsheetMessageLabel.ForeColor = Color.Red; }));
                mySpreadsheetPanel.Invoke(new Action(() => { spreadsheetMessageLabel.Text = "Error: " + d.error_message; }));
                mySpreadsheetPanel.Invoke(new Action(() => { setUpNewCell(mySpreadsheetPanel); }));
            }
            
        }

        /// <summary>
        /// Used to clear the message text and do whatever else everytime a message is received
        /// </summary>
        private void GeneralLineReceived()
        {
            mySpreadsheetPanel.Invoke(new Action(() => { spreadsheetMessageLabel.ForeColor = Color.Black; }));
            mySpreadsheetPanel.Invoke(new Action(() => { spreadsheetMessageLabel.Text = ""; }));
        }

        /// <summary>
        /// Server has disconnected, abort all and restart.
        /// </summary>
        private void ServerDisconnectReceived()
        {
            mySpreadsheetPanel.Invoke(new Action(() => { restarting = true; }));

            mySpreadsheetPanel.Invoke(new Action(() => { DialogResult result = MessageBox.Show("Server Has Disconncted", "Important Message"); }));

            mySpreadsheetPanel.Invoke(new Action(() => { Application.Restart(); }));
        }

        #endregion

        #region Triggers for Menu Clicks

        /// <summary>
        /// Event handler for when the new item in the drop down is 
        /// selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            MyApplicationContext.getAppContext().RunForm(new Form1());
        }

        /// <summary>
        /// event handler that is called when closing the spreadsheet from the file menu, calls the
        /// close method that will determine if data loss will occur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Provide an open dialog that allows the user to choose a spreadsheet, 
        /// defaults to the .ss file extension unless the user chooses otherwise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Spreadsheet Document (.ss)|*.ss|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            DialogResult result = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (result == DialogResult.OK)
            {
                // Open the selected file to read.
                string fileName = openFileDialog1.FileName;

                //Create a new spreadsheet gui from the file if possible
                MyApplicationContext.getAppContext().RunForm(new Form1(fileName));
            }
        }

        /// <summary>
        /// opens a save file dialog that lets the user save the spreadsheet to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set filter options and filter index.
            saveFileDialog1.Filter = "Spreadsheet Document (.ss)|*.ss|All Files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;

            DialogResult result = saveFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (result == DialogResult.OK)
            {
                //get the filename that is to be saved to
                string fileName = saveFileDialog1.FileName;

                //Create a new spreadsheet gui from the file if possible
                try
                {
                    sheet.Save(fileName);
                    currentFileName = fileName;
                    this.Text = currentFileName;
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Save could not be completed!");
                }
            }
        }

        /// <summary>
        /// Sends a save message to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String saveMessage = messageMachine.build_save(this.version + 1);
            model.SendMessage(saveMessage);
        }


        /// <summary>
        /// if the user has already saved the file once, then it will default to the previous file name,
        /// otherwise it will redirect to the Save As menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveLocallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if we haven't previously saved this spreadsheet
            //call the Save As stuff
            if (currentFileName == String.Empty)
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                try
                {
                    sheet.Save(currentFileName);
                }
                catch (Exception e2)
                {
                    MessageBox.Show("Save could not be completed!");
                }
            } 
        }

        /// <summary>
        /// Warns against data loss when forms are closed in any manner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!restarting)
            {
                if (sheet.Changed == true)
                {
                    //Warning!
                    DialogResult result = MessageBox.Show("Your spreadsheet has been saved to the Cowboy's Server, but it is not saved locally.  Would you like to save a local version?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            saveLocallyToolStripMenuItem_Click(sender, e);
                            break;
                        case DialogResult.No:
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }

                    if (e.Cancel)
                        return;
                }
            }
            // Shut the socket down
            model.Disconnect();
        }

        /// <summary>
        /// When clicked will implement the .chm help menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "PS6Help.chm");
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks to see if the current cell contents is a formula, if it is checks to see if the contents are invalid, if they are
        /// it will display an error message and the contents will not be sent
        /// Converts the formula variables to all uppercase
        /// </summary>
        /// <returns></returns>
        private bool isInvalidFormula()
        {
            if(cellContentsTextBox.Text.StartsWith("="))
            {
                try
                {
                    // If we get past this then convert the variables to uppercase
                    Formula f = new Formula(cellContentsTextBox.Text.Replace("=", ""), s => s.ToUpper(), extraValidation);

                    String upperContents = cellContentsTextBox.Text.ToUpper();

                    cellContentsTextBox.Text = upperContents;

                    return false;
                }
                catch(Exception e)
                {
                    // Invalid!
                    spreadsheetMessageLabel.ForeColor = Color.Red;
                    spreadsheetMessageLabel.Text = "Error: " + e.Message;
                    cellContentsTextBox.Text = "";

                    return true;
                }
            }
            else
            {
                // Is not an invalid formula, or a formula at all
                return false;
            }
            
        }

        /// <summary>
        /// Only once an update request is received from the server will we make the change
        /// Nothing changes unless the server says so!
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="cellName"></param>
        /// <param name="cellContents"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        private void updateCells(SpreadsheetPanel ss, String cellName, String cellContents, int column, int row)
        {
            //Store the old value in the cell if we encounter an error
            Object oldCellContents = sheet.GetCellContents(cellName);

            //cells to recalculate after changing our selected cell
            HashSet<string> cellsToRecalculate = new HashSet<string>();

            //Attempt to add the new cell to our spreadsheet model
            try
            {
                //simultaneously add the new cell to the sheet and get the cellsToRecalculate
                cellsToRecalculate = (HashSet<string>) sheet.SetContentsOfCell(cellName, cellContents);        
            }
            catch (Exception e)
            {
                //if it is a Circular Exception then give an informative message and restore old state
                if (e is CircularException)
                {
                    MessageBox.Show("Circular Exception, returning to previous value!");

                    //if oldCellContents was a formula then prepend an =
                    if(oldCellContents is Formula)
                        sheet.SetContentsOfCell(cellName, "=" + oldCellContents.ToString());
                    else
                        sheet.SetContentsOfCell(cellName, oldCellContents.ToString());
                }
                else //if the Formula Input is invalid then show an informative message and restore old state
                {
                    MessageBox.Show("Invalid Formula Format, returning to previous value!");

                    //if oldCellContents was a formula then prepend an =
                    if (oldCellContents is Formula)
                        sheet.SetContentsOfCell(cellName, "=" + oldCellContents.ToString());
                    else
                        sheet.SetContentsOfCell(cellName, oldCellContents.ToString());
                }
            }

            //set the cell in the grid to display the appropriate value
            if (sheet.GetCellValue(cellName) is FormulaError)
            {
                ss.SetValue(column, row, "Formula Error");
            }
            else
            {
                ss.SetValue(column, row, sheet.GetCellValue(cellName).ToString());
            }

            updateDependentCells(ss, cellsToRecalculate);
            setUpNewCell(ss);
        }

        /// <summary>
        /// change all cells that are dependent on a newly changed cell
        /// by refreshing the view
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="cellsToRecalculate"></param>
        private void updateDependentCells(SpreadsheetPanel ss, ISet<string> cellsToRecalculate)
        {
            //int originalRow = currentRow;
            //int originalCol = currentCol;

            int tempRow = currentRow;
            int tempCol = currentCol;

            //keep in mind that the cells in the sheet are already updated
            //we just need to reflect that in the view
            foreach (string cellName in cellsToRecalculate)
            {
                tempRow = (Convert.ToInt32(cellName.Substring(1)) - 1);
                tempCol = (Convert.ToInt32(cellName[0]) - 'A');

                //set the cell in the grid to display the appropriate value
                if (sheet.GetCellValue(cellName) is FormulaError)
                {
                    ss.SetValue(tempCol, tempRow, "Formula Error");

                }
                else
                {
                    ss.SetValue(tempCol, tempRow, sheet.GetCellValue(cellName).ToString());
                }

                //Set the cellValueTextBox to the correct cell value
                if (sheet.GetCellValue(cellName) is FormulaError)
                {
                    cellValueTextBox.Text = "Formula Error";
                }
                else
                {
                    cellValueTextBox.Text = sheet.GetCellValue(cellName).ToString();
                }
            }

            //currentRow = originalRow;
            //currentCol = originalCol;
        }

        /// <summary>
        /// if we click away from a cell then we must set up the new cell
        /// </summary>
        /// <param name="ss"></param>
        private void setUpNewCell(SpreadsheetPanel ss)
        {
            //Grab the row and column of the newly selected cell
            int row, col;
            ss.GetSelection(out col, out row);

            //set our member variables to the appropriate values
            currentRow = row;
            currentCol = col;

            //String value;
            //ss.GetValue(col, row, out value);

            //Set currentCellName to the correct cell
            currentCellName = GetColumnName(currentCol).ToString() + (currentRow + 1).ToString();

            //Update the cell name in the cellNameTextBox
            cellNameTextBox.Text = currentCellName;

            //Set the cellContentsBox to the contents of the cell, if it is a formula be sure
            //to include an =
            if (sheet.GetCellContents(currentCellName) is Formula)
            {
                cellContentsTextBox.Text = "=" + sheet.GetCellContents(currentCellName).ToString();
            }
            else
            {
                cellContentsTextBox.Text = sheet.GetCellContents(currentCellName).ToString();
            }

            // Set the originalContents variable
            originalContents = cellContentsTextBox.Text;

            //Set the cellValueTextBox to the correct cell value
            if (sheet.GetCellValue(currentCellName) is FormulaError)
            {
                cellValueTextBox.Text = "Formula Error";
            }
            else
            {
                cellValueTextBox.Text = sheet.GetCellValue(currentCellName).ToString();
            }

            //Regain focus
            cellContentsTextBox.Focus();
        }

        /// <summary>
        /// provides a letter associated with the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            value += letters[index];

            return value;
        }

        /// <summary>
        /// Imposes extra validation on the sheet, if the cell has multiple characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool extraValidation(string s)
        {
            string tPattern = @"^[A-Z][1-9][0-9]?$";

            return Regex.IsMatch(s.ToUpper(), tPattern, RegexOptions.IgnoreCase);
        }

        #endregion 

        #region CS 3505 -- Panel Flow

        /// <summary>
        /// This method allows for enabling and disabling the GUI's authentication mode.
        /// When this method is passed true, the GUI for authentication is shown.
        /// When this method is passed false, the GUI for authentication is hidden.
        /// </summary>
        /// <param name="mode"></param>
        public void authentication_mode(bool mode)
        {
            if (mode)
            {
                this.Width = 560;
                this.Height = 530;
                // Display Authentication panel.
                menuStrip1.Visible = false;
                AuthPanel.Show();
            }
            else
            {
                // MOVE to only happen if password is valid
                menuStrip1.Visible = true; // Menu strip was hidden for login page
                AuthPanel.Hide();
            }
        }

        /// <summary>
        /// This method allows for enabling and disabling the GUI's open/new mode.
        /// When this method is passed true, the GUI for open/new is shown.
        /// When this method is passed false, the GUI for open/new is hidden.
        /// </summary>
        /// <param name="mode"></param>
        public void open_mode(bool mode)
        {
            if (mode)
            {
                this.Width = 680;
                this.Height = 260;
                menuStrip1.Visible = false;
                OpenPanel.Visible = true;
            }
            else
            {
                menuStrip1.Visible = true;
                OpenPanel.Hide();
            }
        }

        /// <summary>
        /// This method allows for enabling and disabling the GUI's spreadsheet visibility.
        /// It changes the default window size and makes the menu strip visible.
        /// </summary>
        /// <param name="mode"></param>
        public void spreadsheet_mode(bool mode)
        {
            if (mode)
            {
                this.Width = 800;
                this.Height = 600;
                menuStrip1.Visible = true;

            }
            else
            {
                menuStrip1.Visible = false;
            }
        }

        /// <summary>
        /// Send the input to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthLoginButton_Click(object sender, EventArgs e)
        {
            int port = 0;

            // If these are not empty then proceed
            if (serverIpTextBox.Text != "" && portTextBox.Text != "" && AuthTextBox.Text != "")
            {
                // If the port is not a number, error out
                if (!Int32.TryParse(portTextBox.Text, out port))
                {
                    AuthError.Text = "Must enter a valid number for port";
                    AuthError.Visible = true;
                    return;
                }

                // Attempt to connect
                if (!connected && createModel(serverIpTextBox.Text, port))
                {
                    // Send password authentication message to server
                    String message = messageMachine.build_password(AuthTextBox.Text);
                    model.SendMessage(message);
                    connected = true;
                }
                else if (model.isConnected())
                {
                    // Send password authentication message to server
                    String message = messageMachine.build_password(AuthTextBox.Text);
                    model.SendMessage(message);
                }
                else
                {
                    AuthError.Text = "Could not connect to server.";
                    AuthError.Visible = true;
                    return;
                }
            }
            else
            {
                AuthError.Text = "Please enter valid credentials to continue.";
                AuthError.Visible = true;
                return;
            }
        }

        /// <summary>
        /// Hide the error message if the user is typing in another password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthTextBox_TextChanged(object sender, EventArgs e)
        {
            AuthError.Visible = false;
        }

        /// <summary>
        /// New button located on the second panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewButton_Click(object sender, EventArgs e)
        {
            NewPanel.Show();
        }

        /// <summary>
        /// Open button located on the second panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenButton_Click(object sender, EventArgs e)
        {
            this.Width = 563;
            this.Height = 421;

            OpenExistingPanel.Show();
        }

        /// <summary>
        /// Back button located on the "New" Panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            NewPanel.Hide();
        }

        /// <summary>
        /// Enter Button located on the "New" Panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterNewName_Click(object sender, EventArgs e)
        {
            // Check to see if the user input a new file name
            // No extensions
            // No whitespace
            if (textBox1.Text != "" && !Path.HasExtension(textBox1.Text) && !textBox1.Text.Contains(" "))
            {
                String message = messageMachine.build_create(textBox1.Text);
                model.SendMessage(message);
            }
            else
            {
                String error = "File name is invalid";
                //messageLabel.Text = error;
            }
        }

        /// <summary>
        /// Open button located on the "Open" panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExistingOpen_Click(object sender, EventArgs e)
        {
            // Send an open request to the server
            if (listBox1.Text != "")
            {
                String message = messageMachine.build_open(listBox1.Text);
                model.SendMessage(message);
            }     
        }

        /// <summary>
        /// Back button located on the "Open" panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExistingBack_Click(object sender, EventArgs e)
        {
            this.Width = 680;
            this.Height = 260;
            OpenExistingPanel.Hide();
        }

        #endregion

        
    }
}
