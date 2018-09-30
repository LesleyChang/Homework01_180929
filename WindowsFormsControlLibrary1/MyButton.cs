using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsControlLibrary1
{
    public class MyButton : Control
    {
        public MyButton()
        {
            FillColor1 = Color.Cyan;
            FillColor2 = Color.Salmon;
        }
        private Color _color1;
        private Color _color2;
        private Shape _shape;
        public Color FillColor1
        {
            get { return _color1; }
            set
            {
                _color1 = value;
                this.Invalidate();
            }
        }
        public Color FillColor2
        {
            get { return _color2; }
            set
            {
                _color2 = value;
                this.Invalidate();
            }
        }
        public int Index { get; set; }
        public enum Shape { Ellipse,Rectangle}
        public Shape FillShape
        {
            get { return _shape; }
            set { _shape = value; }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            LinearGradientBrush brush1 = new LinearGradientBrush
                (this.ClientRectangle,_color1,_color2,LinearGradientMode.BackwardDiagonal);
            switch (_shape)
            {
                case Shape.Ellipse:
                    e.Graphics.FillEllipse(brush1, this.ClientRectangle);
                    break;
                case Shape.Rectangle:
                    e.Graphics.FillRectangle(brush1, this.ClientRectangle);
                    break;
            }
            float x = (this.ClientRectangle.Width - (e.Graphics.MeasureString(base.Text, base.Font)).Width) / 2;
            float y = (this.ClientRectangle.Height - (e.Graphics.MeasureString(base.Text, base.Font)).Height) / 2;
            e.Graphics.DrawString(base.Text, base.Font, Brushes.White, x, y);    
            base.OnPaint(e);
        }
    }
}
