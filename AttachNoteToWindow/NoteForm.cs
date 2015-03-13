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
        Form _createForm;

        string _windowTitle;
        Dictionary<string, string> _notes = new Dictionary<string,string>();

        public NoteForm()
        {
            InitializeComponent();

            _createForm = new CreateForm();
            _createForm.Click += _createForm_Click;

            // Load notes
            LoadNotes();
        }

        void _createForm_Click(object sender, EventArgs e)
        {
            _notes.Add(_windowTitle, "");
            NoteChanged(_windowTitle);
        }

        void SwitchToCreateForm()
        {
            _createForm.Show();
            // Must set AFTER showing
            _createForm.Width = 22;
            _createForm.Height = 22;
            this.Hide();
        }
        void SwitchToNoteForm()
        {
            _createForm.Hide();
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
            var appName = GetTopWindowName();
            if (appName.StartsWith(MyProcessName))
            {
                return;
            }

            var newWindowTitle = appName + ": " + GetTopWindowText();
            // Positioning
            if (this.Visible)
            {

            }
            if (_createForm.Visible)
            {

            }

            
            if (newWindowTitle != _windowTitle)
            {
                NoteChanged(newWindowTitle);
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

        #region Win32
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        //  int GetWindowText(
        //      __in   HWND hWnd,
        //      __out  LPTSTR lpString,
        //      __in   int nMaxCount
        //  );
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        //  DWORD GetWindowThreadProcessId(
        //      __in   HWND hWnd,
        //      __out  LPDWORD lpdwProcessId
        //  );
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static string GetTopWindowText()
        {
            IntPtr hWnd = GetForegroundWindow();
            int length = GetWindowTextLength(hWnd);
            StringBuilder text = new StringBuilder(length + 1);
            GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }

        public static string GetTopWindowName()
        {
            IntPtr hWnd = GetForegroundWindow();
            uint lpdwProcessId;
            GetWindowThreadProcessId(hWnd, out lpdwProcessId);

            var p = Process.GetProcessById((int)lpdwProcessId);
            return p.ProcessName;
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
