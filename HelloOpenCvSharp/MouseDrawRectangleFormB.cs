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
        private bool _isDrawing, _isResizing, _isMoving;
        private Rectangle _rect;
        private Point _startPoint, _lastLocation;

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
                _isResizing = true;
                _startPoint = e.Location;
            }
            else if (IsMouseInMoveZone(e.Location))
            {
                _isMoving = true;
                _lastLocation = e.Location;
            }
            else
            {
                _isDrawing = true;
                _startPoint = e.Location;
                _rect = new Rectangle(e.Location, new Size());
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                _rect.Size = new Size(e.X - _startPoint.X, e.Y - _startPoint.Y);
                this.panel1.Invalidate();
            }
            else if (_isResizing)
            {
                _rect.Size = new Size(e.X - _rect.Left, e.Y - _rect.Top);
                this.panel1.Invalidate();
            }
            else if (_isMoving)
            {
                int dx = e.X - _lastLocation.X;
                int dy = e.Y - _lastLocation.Y;
                _rect = new Rectangle(_rect.X + dx, _rect.Y + dy, _rect.Width, _rect.Height);
                _lastLocation = e.Location;
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
            _isDrawing = _isMoving = _isResizing = false;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_rect.Width > 0 && _rect.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Black, _rect);
            }
        }

        private bool IsMouseInResizeZone(Point location)
        {
            Rectangle resizeZone = new Rectangle(_rect.Right - 10, _rect.Bottom - 10, 10, 10);
            return resizeZone.Contains(location);
        }

        private bool IsMouseInMoveZone(Point location)
        {
            Rectangle moveZone = new Rectangle(_rect.Left, _rect.Top, 10, 10);
            return moveZone.Contains(location);
        }

        private Panel panel1;
    }
}
