using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttachNoteToWindow
{
    public partial class CreateForm : Form
    {
        public CreateForm()
        {
            InitializeComponent();
        }

        private void CreateForm_Click(object sender, EventArgs e)
        {
            if (Click != null) { Click(this, e); }
        }

        public event EventHandler Click;
    }
}
