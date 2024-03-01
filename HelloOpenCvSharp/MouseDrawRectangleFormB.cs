using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloOpenCvSharp
{
    public partial class MouseDrawRectangleFormB : Form
    {
        private bool _isAnnotationDrawing, _isAnnotationResizing, _isAnnotationMoving;
        private Rectangle _annotationRect;
        private Point _annotationStartPoint, _annotationLastLocation;

        public MouseDrawRectangleFormB()
        {
            InitializeComponent();
            this.panel1 = new Panel();
            this.panel1.Dock = DockStyle.Fill;
            this.Controls.Add(this.panel1);

            this.panel1.MouseDown += Panel1_MouseDown;
            this.panel1.MouseMove += Panel1_MouseMove;
            this.panel1.MouseUp += Panel1_MouseUp;
            this.panel1.Paint += Panel1_Paint;
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsMouseInResizeZone(e.Location))
            {
                _isAnnotationResizing = true;
                _annotationStartPoint = e.Location;
            }
            else if (IsMouseInMoveZone(e.Location))
            {
                _isAnnotationMoving = true;
                _annotationLastLocation = e.Location;
            }
            else
            {
                _isAnnotationDrawing = true;
                _annotationStartPoint = e.Location;
                _annotationRect = new Rectangle(e.Location, new Size());
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isAnnotationDrawing)
            {
                _annotationRect.Size = new Size(e.X - _annotationStartPoint.X, e.Y - _annotationStartPoint.Y);
                this.panel1.Invalidate();
            }
            else if (_isAnnotationResizing)
            {
                _annotationRect.Size = new Size(e.X - _annotationRect.Left, e.Y - _annotationRect.Top);
                this.panel1.Invalidate();
            }
            else if (_isAnnotationMoving)
            {
                int dx = e.X - _annotationLastLocation.X;
                int dy = e.Y - _annotationLastLocation.Y;
                _annotationRect = new Rectangle(_annotationRect.X + dx, _annotationRect.Y + dy, _annotationRect.Width, _annotationRect.Height);
                _annotationLastLocation = e.Location;
                this.panel1.Invalidate();
            }
            else
            {
                if (IsMouseInResizeZone(e.Location))
                {
                    this.panel1.Cursor = Cursors.SizeNWSE;
                }
                else if (IsMouseInMoveZone(e.Location))
                {
                    this.panel1.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this.panel1.Cursor = Cursors.Default;
                }
            }
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _isAnnotationDrawing = _isAnnotationMoving = _isAnnotationResizing = false;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_annotationRect.Width > 0 && _annotationRect.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Black, _annotationRect);
            }
        }

        private bool IsMouseInResizeZone(Point location)
        {
            Rectangle resizeZone = new Rectangle(_annotationRect.Right - 10, _annotationRect.Bottom - 10, 10, 10);
            return resizeZone.Contains(location);
        }

        private bool IsMouseInMoveZone(Point location)
        {
            Rectangle moveZone = new Rectangle(_annotationRect.Left, _annotationRect.Top, 10, 10);
            return moveZone.Contains(location);
        }

        private Panel panel1;
    }
}
