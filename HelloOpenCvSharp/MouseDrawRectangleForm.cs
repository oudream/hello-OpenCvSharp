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
    // 只有画矩形的实现
    public partial class MouseDrawRectangleForm : Form
    {
        private bool isAnnotating;
        private Rectangle annotationRect;
        private Point annotationStartPoint;

        public MouseDrawRectangleForm()
        {
            this.DoubleBuffered = true; // 减少闪烁
            this.MouseDown += new MouseEventHandler(Form_MouseDown);
            this.MouseMove += new MouseEventHandler(Form_MouseMove);
            this.MouseUp += new MouseEventHandler(Form_MouseUp);
            this.Paint += new PaintEventHandler(Form_Paint);
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isAnnotating = true;
            annotationStartPoint = e.Location;
            annotationRect = new Rectangle(e.Location, new Size());
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isAnnotating)
            {
                // 更新矩形的宽度和高度
                annotationRect.Width = e.X - annotationStartPoint.X;
                annotationRect.Height = e.Y - annotationStartPoint.Y;

                // 强制重新绘制Form，触发Paint事件
                this.Invalidate();
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (isAnnotating)
            {
                isAnnotating = false;

                // 最后一次更新矩形大小并重新绘制
                annotationRect.Width = e.X - annotationStartPoint.X;
                annotationRect.Height = e.Y - annotationStartPoint.Y;
                this.Invalidate(); // 可能不需要，取决于你是否需要在鼠标松开时更新视图
            }
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (annotationRect != null && annotationRect.Width > 0 && annotationRect.Height > 0)
            {
                // 使用Pen绘制矩形
                e.Graphics.DrawRectangle(Pens.Black, annotationRect);
            }
        }
    }
}
