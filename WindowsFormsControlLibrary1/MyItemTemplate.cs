using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace WindowsFormsControlLibrary1
{
    public partial class MyItemTemplate: UserControl
    {
        public event EventHandler MyMouseEnter;
        public event EventHandler MyMouseLeave;
        public event EventHandler MyCheckedChange;

        public MyItemTemplate()
        {
            InitializeComponent();
            RegisterMouseEvent();
        }

        public int Index { get; set; }
        public string Desc
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }
        public byte[] ImageByte
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Bmp);
                return ms.GetBuffer();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                pictureBox1.Image = Image.FromStream(ms);
            }
        }
        public bool Checked
        {
            get { return checkBox1.Checked; }
            set { this.checkBox1.Checked = value; }
        }
        private void RegisterMouseEvent()
        {
            this.pictureBox1.MouseEnter += (object sender, EventArgs e) =>
            { if (MyMouseEnter != null) { MyMouseEnter.Invoke(this, e); } };
            this.pictureBox1.MouseLeave += (object sender, EventArgs e) =>
            { if (MyMouseLeave != null) { MyMouseLeave.Invoke(this, e); } };
            this.label1.MouseEnter += (object sender, EventArgs e) =>
            { if (MyMouseEnter != null) { MyMouseEnter.Invoke(this, e); } };
            this.label1.MouseLeave += (object sender, EventArgs e) =>
            { if (MyMouseLeave != null) { MyMouseLeave.Invoke(this, e); } };
            this.MouseEnter += (object sender, EventArgs e) =>
            { if (MyMouseEnter != null) { MyMouseEnter.Invoke(this, e); } };
            this.MouseLeave += (object sender, EventArgs e) =>
            { if (MyMouseLeave != null) { MyMouseLeave.Invoke(this, e); } };
            this.MyMouseEnter += MyItemTemplate_MyMouseEnter;
            this.MyMouseLeave += MyItemTemplate_MyMouseLeave;

            this.checkBox1.CheckedChanged += (object sender, EventArgs e) =>
            { if (MyCheckedChange != null) { MyCheckedChange.Invoke(this, e); } };

        }

        private void MyMouseEnterInvoke(object sender, EventArgs e)
        {
            if (MyMouseEnter != null) { MyMouseEnter.Invoke(this, e); }
        }

        private void MyItemTemplate_MyMouseLeave(object sender, EventArgs e)
        {
            this.Width -= 20;
            this.pictureBox1.Width -= 20;
            this.pictureBox1.BackColor = Color.White;
        }

        private void MyItemTemplate_MyMouseEnter(object sender, EventArgs e)
        {
            this.Width += 20;
            this.pictureBox1.Width += 20;
            this.pictureBox1.BackColor = Color.LightGreen;
        }
    }
}
