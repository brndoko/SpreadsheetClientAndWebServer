namespace SpreadsheetGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mySpreadsheetPanel = new SS.SpreadsheetPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cellNameTextBox = new System.Windows.Forms.TextBox();
            this.cellNameLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cellValueLabel = new System.Windows.Forms.Label();
            this.cellValueTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.cellContentsLabel = new System.Windows.Forms.Label();
            this.cellContentsTextBox = new System.Windows.Forms.TextBox();
            this.UndoButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.spreadsheetMessageLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.AuthPanel = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.serverIpTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AuthError = new System.Windows.Forms.Label();
            this.AuthTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AuthLoginButton = new System.Windows.Forms.Button();
            this.loginLabel1 = new System.Windows.Forms.Label();
            this.NewOrOpenLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NewButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.OpenPanel = new System.Windows.Forms.Panel();
            this.OpenExistingPanel = new System.Windows.Forms.Panel();
            this.OpenExistingOpen = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.OpenExistingBack = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.NewPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.EnterNewName = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.saveToServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLocallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.AuthPanel.SuspendLayout();
            this.OpenPanel.SuspendLayout();
            this.OpenExistingPanel.SuspendLayout();
            this.NewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(859, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::SpreadsheetGUI.Properties.Resources.new1;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::SpreadsheetGUI.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToServerToolStripMenuItem,
            this.saveLocallyToolStripMenuItem});
            this.saveToolStripMenuItem.Image = global::SpreadsheetGUI.Properties.Resources.save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::SpreadsheetGUI.Properties.Resources.saveas;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save As..";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::SpreadsheetGUI.Properties.Resources.close;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click_1);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.08945F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.mySpreadsheetPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.65482F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(859, 478);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // mySpreadsheetPanel
            // 
            this.mySpreadsheetPanel.AutoScroll = true;
            this.mySpreadsheetPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel1.SetColumnSpan(this.mySpreadsheetPanel, 3);
            this.mySpreadsheetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mySpreadsheetPanel.Location = new System.Drawing.Point(3, 35);
            this.mySpreadsheetPanel.Name = "mySpreadsheetPanel";
            this.mySpreadsheetPanel.Size = new System.Drawing.Size(853, 440);
            this.mySpreadsheetPanel.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.2243F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.7757F));
            this.tableLayoutPanel2.Controls.Add(this.cellNameTextBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cellNameLabel, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(106, 26);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // cellNameTextBox
            // 
            this.cellNameTextBox.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.cellNameTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.cellNameTextBox.Enabled = false;
            this.cellNameTextBox.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cellNameTextBox.Location = new System.Drawing.Point(75, 3);
            this.cellNameTextBox.Name = "cellNameTextBox";
            this.cellNameTextBox.Size = new System.Drawing.Size(28, 20);
            this.cellNameTextBox.TabIndex = 3;
            // 
            // cellNameLabel
            // 
            this.cellNameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cellNameLabel.AutoSize = true;
            this.cellNameLabel.Location = new System.Drawing.Point(7, 6);
            this.cellNameLabel.Name = "cellNameLabel";
            this.cellNameLabel.Size = new System.Drawing.Size(58, 13);
            this.cellNameLabel.TabIndex = 4;
            this.cellNameLabel.Text = "Cell Name:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.cellValueLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cellValueTextBox, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(115, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(128, 26);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // cellValueLabel
            // 
            this.cellValueLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cellValueLabel.AutoSize = true;
            this.cellValueLabel.Location = new System.Drawing.Point(3, 6);
            this.cellValueLabel.Name = "cellValueLabel";
            this.cellValueLabel.Size = new System.Drawing.Size(57, 13);
            this.cellValueLabel.TabIndex = 0;
            this.cellValueLabel.Text = "Cell Value:";
            // 
            // cellValueTextBox
            // 
            this.cellValueTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.cellValueTextBox.Enabled = false;
            this.cellValueTextBox.Location = new System.Drawing.Point(67, 3);
            this.cellValueTextBox.Name = "cellValueTextBox";
            this.cellValueTextBox.Size = new System.Drawing.Size(58, 20);
            this.cellValueTextBox.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.cellContentsLabel, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.cellContentsTextBox, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.UndoButton, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 3, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(249, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(607, 26);
            this.tableLayoutPanel4.TabIndex = 6;
            // 
            // cellContentsLabel
            // 
            this.cellContentsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cellContentsLabel.AutoSize = true;
            this.cellContentsLabel.Location = new System.Drawing.Point(3, 6);
            this.cellContentsLabel.Name = "cellContentsLabel";
            this.cellContentsLabel.Size = new System.Drawing.Size(72, 13);
            this.cellContentsLabel.TabIndex = 0;
            this.cellContentsLabel.Text = "Cell Contents:";
            // 
            // cellContentsTextBox
            // 
            this.cellContentsTextBox.Location = new System.Drawing.Point(82, 3);
            this.cellContentsTextBox.Name = "cellContentsTextBox";
            this.cellContentsTextBox.Size = new System.Drawing.Size(169, 20);
            this.cellContentsTextBox.TabIndex = 1;
            this.cellContentsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cellContentsTextBox_EnterKeyPress);
            // 
            // UndoButton
            // 
            this.UndoButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.UndoButton.Location = new System.Drawing.Point(257, 3);
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.Size = new System.Drawing.Size(119, 20);
            this.UndoButton.TabIndex = 2;
            this.UndoButton.Text = "Undo";
            this.UndoButton.UseVisualStyleBackColor = true;
            this.UndoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.spreadsheetMessageLabel, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(382, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(222, 20);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // spreadsheetMessageLabel
            // 
            this.spreadsheetMessageLabel.AutoSize = true;
            this.spreadsheetMessageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetMessageLabel.Location = new System.Drawing.Point(3, 0);
            this.spreadsheetMessageLabel.Name = "spreadsheetMessageLabel";
            this.spreadsheetMessageLabel.Size = new System.Drawing.Size(216, 20);
            this.spreadsheetMessageLabel.TabIndex = 0;
            this.spreadsheetMessageLabel.Text = "No Message";
            this.spreadsheetMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // AuthPanel
            // 
            this.AuthPanel.AutoSize = true;
            this.AuthPanel.BackColor = System.Drawing.Color.Black;
            this.AuthPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("AuthPanel.BackgroundImage")));
            this.AuthPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.AuthPanel.Controls.Add(this.label9);
            this.AuthPanel.Controls.Add(this.label8);
            this.AuthPanel.Controls.Add(this.serverIpTextBox);
            this.AuthPanel.Controls.Add(this.portTextBox);
            this.AuthPanel.Controls.Add(this.label2);
            this.AuthPanel.Controls.Add(this.AuthError);
            this.AuthPanel.Controls.Add(this.AuthTextBox);
            this.AuthPanel.Controls.Add(this.label1);
            this.AuthPanel.Controls.Add(this.AuthLoginButton);
            this.AuthPanel.Controls.Add(this.loginLabel1);
            this.AuthPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthPanel.Location = new System.Drawing.Point(0, 0);
            this.AuthPanel.Name = "AuthPanel";
            this.AuthPanel.Size = new System.Drawing.Size(859, 478);
            this.AuthPanel.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(26, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Server IP Address:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(26, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Port Number:";
            // 
            // serverIpTextBox
            // 
            this.serverIpTextBox.Location = new System.Drawing.Point(29, 128);
            this.serverIpTextBox.Name = "serverIpTextBox";
            this.serverIpTextBox.Size = new System.Drawing.Size(115, 20);
            this.serverIpTextBox.TabIndex = 7;
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(29, 183);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(115, 20);
            this.portTextBox.TabIndex = 6;
            this.portTextBox.Text = "2500";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Batang", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(26, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(244, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Welcome. Please log in to begin.";
            // 
            // AuthError
            // 
            this.AuthError.AutoSize = true;
            this.AuthError.ForeColor = System.Drawing.Color.LightCoral;
            this.AuthError.Location = new System.Drawing.Point(24, 322);
            this.AuthError.Name = "AuthError";
            this.AuthError.Size = new System.Drawing.Size(186, 13);
            this.AuthError.TabIndex = 4;
            this.AuthError.Text = "Please enter server password to login.";
            this.AuthError.Visible = false;
            // 
            // AuthTextBox
            // 
            this.AuthTextBox.Location = new System.Drawing.Point(27, 239);
            this.AuthTextBox.Name = "AuthTextBox";
            this.AuthTextBox.Size = new System.Drawing.Size(117, 20);
            this.AuthTextBox.TabIndex = 3;
            this.AuthTextBox.Text = "password";
            this.AuthTextBox.UseSystemPasswordChar = true;
            this.AuthTextBox.TextChanged += new System.EventHandler(this.AuthTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(26, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Password:";
            // 
            // AuthLoginButton
            // 
            this.AuthLoginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AuthLoginButton.Location = new System.Drawing.Point(27, 280);
            this.AuthLoginButton.Name = "AuthLoginButton";
            this.AuthLoginButton.Size = new System.Drawing.Size(94, 26);
            this.AuthLoginButton.TabIndex = 1;
            this.AuthLoginButton.Text = "Login";
            this.AuthLoginButton.UseVisualStyleBackColor = true;
            this.AuthLoginButton.Click += new System.EventHandler(this.AuthLoginButton_Click);
            // 
            // loginLabel1
            // 
            this.loginLabel1.AutoSize = true;
            this.loginLabel1.Font = new System.Drawing.Font("Batang", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.loginLabel1.Location = new System.Drawing.Point(23, 33);
            this.loginLabel1.Name = "loginLabel1";
            this.loginLabel1.Size = new System.Drawing.Size(430, 24);
            this.loginLabel1.TabIndex = 0;
            this.loginLabel1.Text = "Cowboys Collaborative Spreadsheet";
            // 
            // NewOrOpenLabel
            // 
            this.NewOrOpenLabel.AutoSize = true;
            this.NewOrOpenLabel.BackColor = System.Drawing.Color.Transparent;
            this.NewOrOpenLabel.Font = new System.Drawing.Font("Batang", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewOrOpenLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.NewOrOpenLabel.Location = new System.Drawing.Point(13, 58);
            this.NewOrOpenLabel.Name = "NewOrOpenLabel";
            this.NewOrOpenLabel.Size = new System.Drawing.Size(578, 18);
            this.NewOrOpenLabel.TabIndex = 0;
            this.NewOrOpenLabel.Text = "Would you like to open an existing spreadsheet or start a new one?";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Batang", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(16, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "Log In Successful.";
            // 
            // NewButton
            // 
            this.NewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewButton.Location = new System.Drawing.Point(357, 123);
            this.NewButton.Name = "NewButton";
            this.NewButton.Size = new System.Drawing.Size(136, 30);
            this.NewButton.TabIndex = 2;
            this.NewButton.Text = "New";
            this.NewButton.UseVisualStyleBackColor = true;
            this.NewButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenButton.Location = new System.Drawing.Point(179, 123);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(136, 30);
            this.OpenButton.TabIndex = 3;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // OpenPanel
            // 
            this.OpenPanel.BackColor = System.Drawing.Color.Black;
            this.OpenPanel.Controls.Add(this.OpenExistingPanel);
            this.OpenPanel.Controls.Add(this.NewPanel);
            this.OpenPanel.Controls.Add(this.OpenButton);
            this.OpenPanel.Controls.Add(this.NewButton);
            this.OpenPanel.Controls.Add(this.label3);
            this.OpenPanel.Controls.Add(this.NewOrOpenLabel);
            this.OpenPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenPanel.Location = new System.Drawing.Point(0, 0);
            this.OpenPanel.Name = "OpenPanel";
            this.OpenPanel.Size = new System.Drawing.Size(859, 478);
            this.OpenPanel.TabIndex = 7;
            this.OpenPanel.Visible = false;
            // 
            // OpenExistingPanel
            // 
            this.OpenExistingPanel.Controls.Add(this.OpenExistingOpen);
            this.OpenExistingPanel.Controls.Add(this.listBox1);
            this.OpenExistingPanel.Controls.Add(this.OpenExistingBack);
            this.OpenExistingPanel.Controls.Add(this.label6);
            this.OpenExistingPanel.Controls.Add(this.label7);
            this.OpenExistingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenExistingPanel.Location = new System.Drawing.Point(0, 0);
            this.OpenExistingPanel.Name = "OpenExistingPanel";
            this.OpenExistingPanel.Size = new System.Drawing.Size(859, 478);
            this.OpenExistingPanel.TabIndex = 9;
            this.OpenExistingPanel.Visible = false;
            // 
            // OpenExistingOpen
            // 
            this.OpenExistingOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenExistingOpen.Location = new System.Drawing.Point(282, 321);
            this.OpenExistingOpen.Name = "OpenExistingOpen";
            this.OpenExistingOpen.Size = new System.Drawing.Size(234, 26);
            this.OpenExistingOpen.TabIndex = 10;
            this.OpenExistingOpen.Text = "Open";
            this.OpenExistingOpen.UseVisualStyleBackColor = true;
            this.OpenExistingOpen.Click += new System.EventHandler(this.OpenExistingOpen_Click);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(29, 94);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(487, 212);
            this.listBox1.TabIndex = 9;
            // 
            // OpenExistingBack
            // 
            this.OpenExistingBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenExistingBack.Location = new System.Drawing.Point(29, 322);
            this.OpenExistingBack.Name = "OpenExistingBack";
            this.OpenExistingBack.Size = new System.Drawing.Size(83, 25);
            this.OpenExistingBack.TabIndex = 8;
            this.OpenExistingBack.Text = "Back";
            this.OpenExistingBack.UseVisualStyleBackColor = true;
            this.OpenExistingBack.Click += new System.EventHandler(this.OpenExistingBack_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Batang", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(20, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(261, 18);
            this.label6.TabIndex = 6;
            this.label6.Text = "Open Existing Spreadsheet";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Batang", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(23, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(500, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = "Please select the spreadsheet that you would like to open.";
            // 
            // NewPanel
            // 
            this.NewPanel.BackColor = System.Drawing.Color.Black;
            this.NewPanel.Controls.Add(this.button1);
            this.NewPanel.Controls.Add(this.textBox1);
            this.NewPanel.Controls.Add(this.EnterNewName);
            this.NewPanel.Controls.Add(this.label4);
            this.NewPanel.Controls.Add(this.label5);
            this.NewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewPanel.Location = new System.Drawing.Point(0, 0);
            this.NewPanel.Name = "NewPanel";
            this.NewPanel.Size = new System.Drawing.Size(859, 478);
            this.NewPanel.TabIndex = 8;
            this.NewPanel.Visible = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(14, 177);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(126, 25);
            this.button1.TabIndex = 4;
            this.button1.Text = "Back";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(16, 115);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(327, 20);
            this.textBox1.TabIndex = 3;
            // 
            // EnterNewName
            // 
            this.EnterNewName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterNewName.Location = new System.Drawing.Point(359, 112);
            this.EnterNewName.Name = "EnterNewName";
            this.EnterNewName.Size = new System.Drawing.Size(126, 25);
            this.EnterNewName.TabIndex = 2;
            this.EnterNewName.Text = "Enter";
            this.EnterNewName.UseVisualStyleBackColor = true;
            this.EnterNewName.Click += new System.EventHandler(this.EnterNewName_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Batang", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(13, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 18);
            this.label4.TabIndex = 1;
            this.label4.Text = "Create New Spreadsheet";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Batang", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(13, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(407, 18);
            this.label5.TabIndex = 0;
            this.label5.Text = "Please enter a name for your new spreadsheet.";
            // 
            // saveToServerToolStripMenuItem
            // 
            this.saveToServerToolStripMenuItem.Name = "saveToServerToolStripMenuItem";
            this.saveToServerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToServerToolStripMenuItem.Text = "Save To Server";
            this.saveToServerToolStripMenuItem.Click += new System.EventHandler(this.saveToServerToolStripMenuItem_Click);
            // 
            // saveLocallyToolStripMenuItem
            // 
            this.saveLocallyToolStripMenuItem.Name = "saveLocallyToolStripMenuItem";
            this.saveLocallyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveLocallyToolStripMenuItem.Text = "Save Locally";
            this.saveLocallyToolStripMenuItem.Click += new System.EventHandler(this.saveLocallyToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(859, 478);
            this.Controls.Add(this.OpenPanel);
            this.Controls.Add(this.AuthPanel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.AuthPanel.ResumeLayout(false);
            this.AuthPanel.PerformLayout();
            this.OpenPanel.ResumeLayout(false);
            this.OpenPanel.PerformLayout();
            this.OpenExistingPanel.ResumeLayout(false);
            this.OpenExistingPanel.PerformLayout();
            this.NewPanel.ResumeLayout(false);
            this.NewPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SS.SpreadsheetPanel mySpreadsheetPanel;
        private System.Windows.Forms.TextBox cellNameTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label cellNameLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label cellValueLabel;
        private System.Windows.Forms.TextBox cellValueTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label cellContentsLabel;
        private System.Windows.Forms.TextBox cellContentsTextBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel AuthPanel;
        private System.Windows.Forms.Label loginLabel1;
        private System.Windows.Forms.Button AuthLoginButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AuthTextBox;
        private System.Windows.Forms.Label AuthError;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label NewOrOpenLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button NewButton;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Panel OpenPanel;
        private System.Windows.Forms.Panel NewPanel;
        private System.Windows.Forms.Button EnterNewName;
        private System.Windows.Forms.Panel OpenExistingPanel;
        private System.Windows.Forms.Button OpenExistingOpen;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button OpenExistingBack;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button UndoButton;
        private System.Windows.Forms.TextBox serverIpTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label spreadsheetMessageLabel;
        private System.Windows.Forms.ToolStripMenuItem saveToServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLocallyToolStripMenuItem;
    }
}

