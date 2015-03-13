namespace AttachNoteToWindow
{
    partial class NoteForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoteForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tbLineName = new System.Windows.Forms.TextBox();
            this.tbNote = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.settingsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mChangeDataFileLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.bSettings = new System.Windows.Forms.Button();
            this.settingsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Window";
            // 
            // tbLineName
            // 
            this.tbLineName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLineName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLineName.Location = new System.Drawing.Point(68, 5);
            this.tbLineName.Margin = new System.Windows.Forms.Padding(5);
            this.tbLineName.Name = "tbLineName";
            this.tbLineName.ReadOnly = true;
            this.tbLineName.Size = new System.Drawing.Size(276, 31);
            this.tbLineName.TabIndex = 1;
            // 
            // tbNote
            // 
            this.tbNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNote.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbNote.Location = new System.Drawing.Point(3, 43);
            this.tbNote.Margin = new System.Windows.Forms.Padding(2);
            this.tbNote.Name = "tbNote";
            this.tbNote.Size = new System.Drawing.Size(389, 230);
            this.tbNote.TabIndex = 2;
            this.tbNote.Text = "";
            this.tbNote.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.tbNote_LinkClicked);
            this.tbNote.TextChanged += new System.EventHandler(this.tbNote_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // settingsMenu
            // 
            this.settingsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mChangeDataFileLocation});
            this.settingsMenu.Name = "contextMenu";
            this.settingsMenu.Size = new System.Drawing.Size(213, 48);
            // 
            // mChangeDataFileLocation
            // 
            this.mChangeDataFileLocation.Name = "mChangeDataFileLocation";
            this.mChangeDataFileLocation.Size = new System.Drawing.Size(212, 22);
            this.mChangeDataFileLocation.Text = "Change Data File Location";
            this.mChangeDataFileLocation.Click += new System.EventHandler(this.mChangeDataFileLocation_Click);
            // 
            // bSettings
            // 
            this.bSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bSettings.Image = ((System.Drawing.Image)(resources.GetObject("bSettings.Image")));
            this.bSettings.Location = new System.Drawing.Point(352, 4);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(35, 34);
            this.bSettings.TabIndex = 3;
            this.bSettings.UseVisualStyleBackColor = true;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            // 
            // NoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 274);
            this.Controls.Add(this.bSettings);
            this.Controls.Add(this.tbNote);
            this.Controls.Add(this.tbLineName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "NoteForm";
            this.Text = "Attach Note To Window";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.settingsMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLineName;
        private System.Windows.Forms.RichTextBox tbNote;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip settingsMenu;
        private System.Windows.Forms.ToolStripMenuItem mChangeDataFileLocation;
        private System.Windows.Forms.Button bSettings;
    }
}

