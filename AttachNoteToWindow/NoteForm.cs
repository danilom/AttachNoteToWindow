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
    public partial class NoteForm : Form
    {
        CreateForm _createForm;

        string _windowTitle;
        Dictionary<string, string> _notes = new Dictionary<string,string>();

        public NoteForm()
        {
            InitializeComponent();

            _createForm = new CreateForm();
            _createForm.CreateNote += _createForm_CreateNote;

            // Load notes
            LoadNotes();
        }

        void _createForm_CreateNote(object sender, EventArgs e)
        {
            _notes.Add(_windowTitle, "");
            NoteChanged(_windowTitle);
        }

        void SwitchToCreateForm()
        {
            // Must set AFTER showing
            _createForm.Width = 24;
            _createForm.Height = 24;
            this.Hide();

            PositionNextToTarget(_createForm);
            _createForm.Show();
        }
        void SwitchToNoteForm()
        {
            _createForm.Hide();

            PositionNextToTarget(this);
            this.Show();
        }

        void NoteChanged(string newWindowTitle)
        {
            // Store old note
            if (_windowTitle != null && 
                _notes.ContainsKey(_windowTitle))
            {
                _notes[_windowTitle] = tbNote.Text;
                SaveNotes();
            }

            // Switch
            _windowTitle = newWindowTitle;

            // Load note
            tbLineName.Text = _windowTitle;
            if (_notes.ContainsKey(_windowTitle))
            {
                // Note exists
                tbNote.Text = _notes[_windowTitle];
                tbNote.BackColor = Color.LightYellow;

                // Show window
                SwitchToNoteForm();
            }
            else
            {
                tbNote.Text = "";

                // Note does not exist
                SwitchToCreateForm();
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            var appName = Win32.GetTopWindowName();
            if (appName.StartsWith(MyProcessName))
            {
                return;
            }

            var newWindowTitle = appName + ": " + Win32.GetTopWindowText();

            if (newWindowTitle != _windowTitle)
            {
                NoteChanged(newWindowTitle);
            }

            // Positioning
            if (this.Visible)
            {
                PositionNextToTarget(this);
            }
            if (_createForm.Visible)
            {
                PositionNextToTarget(_createForm);
            }
        }

        static void PositionNextToTarget(Form form)
        {
            var target = Win32.GetTopWindowBounds();

            // Attach to the left of the target
            var bounds = new Rectangle(
                target.X - form.Width,
                target.Y,
                form.Width, form.Height);

            // Reposition if off-screen
            var wa = Screen.FromPoint(target.Location).WorkingArea;
            if (bounds.X < wa.X)
            {
                bounds.X = wa.X;
            }

            form.Bounds = bounds;
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
            // Store and save the current note
            NoteChanged("");
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
