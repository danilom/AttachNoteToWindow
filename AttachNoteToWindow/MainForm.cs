using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace AttachNoteToWindow
{
    public partial class MainForm : Form
    {
        string _windowTitle;
        Dictionary<string, string> _notes = new Dictionary<string,string>();

        public MainForm()
        {
            InitializeComponent();

            // Load notes
            LoadNotes();
        }

        void StoreCurrentNote()
        {
            // Store old note
            if (_windowTitle != null)
            {
                if (tbNote.Text.Trim() == "")
                {
                    _notes.Remove(_windowTitle);
                }
                else
                {
                    _notes[_windowTitle] = tbNote.Text;
                }
            }
        }

        void ShowNewNote(string windowTitle)
        {
            // Load note
            tbLineName.Text = windowTitle;
            if (_notes.ContainsKey(windowTitle))
            {
                // Note exists
                tbNote.Text = _notes[windowTitle];
                tbNote.BackColor = Color.LightYellow;

                // Show window
                this.TopMost = true;
            }
            else
            {
                // Note does not exist
                tbNote.Text = "";
                tbNote.BackColor = Color.White;

                this.TopMost = mTopmost.Checked;
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            var appName = Win32.GetTopWindowName();
            if (appName.StartsWith(MyProcessName))
            {
                return;
            }

            // check vs an app list
            var newWindowTitle = appName + ": " + Win32.GetTopWindowText();
            if (newWindowTitle == _windowTitle)
            {
                return;
            }

            StoreCurrentNote();
            SaveNotes();

            // Switch
            _windowTitle = newWindowTitle;

            ShowNewNote(_windowTitle);

            // Move to the edge of the note
            var bounds = Win32.GetTopWindowBounds();

            if (_notes.ContainsKey(_windowTitle))
            {
                // Suggested bounds
                var noteBounds = new Rectangle(bounds.Left - 400, bounds.Top, 400, 250); 

                // Fit within screen                
                var screen = Screen.FromPoint(bounds.Location);
                if (screen.WorkingArea.Left > noteBounds.Left)
                {
                    noteBounds.X = screen.WorkingArea.Left;
                }
                if (screen.WorkingArea.Top > noteBounds.Top)
                {
                    noteBounds.Y = screen.WorkingArea.Top;
                }

                // Set
                this.Bounds = noteBounds;
            }

        }

        string _myProcessName;
        string MyProcessName
        {
            get
            {
                if (_myProcessName == null)
                {
                    _myProcessName = Process.GetCurrentProcess().ProcessName;
                }
                return _myProcessName;
            }
        }

        #region Load/Save data
        static string Encrypt(string plaintext)
        {
            byte[] data = Encoding.UTF8.GetBytes(plaintext);
            data = data.Select(x => (byte)(x ^ (byte)0x47)).ToArray(); // XOR
            return Convert.ToBase64String(data);
        }
        static string Decrypt(string cryptext)
        {
            byte[] data = Convert.FromBase64String(cryptext);
            data = data.Select(x => (byte)(x ^ (byte)0x47)).ToArray(); // XOR
            return Encoding.UTF8.GetString(data);
        }

        void SaveNotes()
        {
            var fname = GetNotesFilename();
            var json = JsonConvert.SerializeObject(this._notes, Formatting.Indented);

            if (fname.EndsWith(".enc"))
            {
                json = Encrypt(json);
            }
            File.WriteAllText(fname, json);
        }

        void LoadNotes()
        {
            var fname = GetNotesFilename();
            if (File.Exists(fname))
            {
                var json = File.ReadAllText(fname);
                if (fname.EndsWith(".enc"))
                {
                    json = Decrypt(json);
                }                

                try
                {
                    this._notes = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                    // Special case: empty JSON file
                    if (this._notes == null)
                    {
                        this._notes = new Dictionary<string, string>();
                    }
                }
                catch(JsonReaderException)
                {
                    // File was corrupt
                    MessageBox.Show("File: " + fname + " was corrupted. Will clear all data.");
                    this._notes = new Dictionary<string, string>();
                }
            }
        }

        string GetNotesFilename()
        {
            if (Properties.Settings.Default.FilePath == "")
            {
                // Default path
                var myDocsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = Path.Combine(myDocsPath, "AttachNoteToWindow.json");
                Properties.Settings.Default.FilePath = path;
            }
            return Properties.Settings.Default.FilePath;
        }

        bool SelectNotesFile()
        {
            var fd = new OpenFileDialog();
            fd.CheckFileExists = false;
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }
            Properties.Settings.Default.FilePath = fd.FileName;
            Properties.Settings.Default.Save();
            return true;
        }
        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoreCurrentNote();
            SaveNotes();
        }

        private void mChangeDataFileLocation_Click(object sender, EventArgs e)
        {
            var oldNotesFile = GetNotesFilename();
            if (SelectNotesFile())
            {
                File.Delete(oldNotesFile);
                SaveNotes();
            }
        }

        private void bSettings_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            settingsMenu.Show(ptLowerLeft);
        }

        private void mTopmost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = mTopmost.Checked;
        }

        private void tbNote_LinkClicked(object sender, LinkClickedEventArgs e)
        {           
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void tbNote_TextChanged(object sender, EventArgs e)
        {
            // TODO: ensure a blank line comes after a URL
        }
    }
}
