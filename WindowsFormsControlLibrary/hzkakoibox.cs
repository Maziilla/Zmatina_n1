using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsControlLibrary
{
    public partial class hzkakoibox : TextBox
    {
        public hzkakoibox()
        {
            InitializeComponent();
        }

        public hzkakoibox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Text = "0";
        }     
        public Int16 Toint(string str)
        {
            return Int16.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }
        public void check()
        {
            if (Text == "")
                Text = "0";
        }       
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl((e.KeyChar)))
                e.Handled = true;
            base.OnKeyPress(e);
            check();
        }
    }
}

