using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloOpenCvSharp
{
    public partial class HelloZoomForm : Form
    {
        public HelloZoomForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }


        private void InitializeCustomComponents()
        {
            // Button Click 事件处理器，用于打开图像
            this.buttonOpenImage.Click += ButtonOpenImage_Click;

            // PictureBox 的 MouseWheel 事件处理器，用于缩放图像
            this.pictureBox1.MouseWheel += PictureBox1_MouseWheel;

            // 启用 PictureBox 的 MouseWheel 事件
            this.pictureBox1.MouseEnter += (sender, e) => pictureBox1.Focus();

            // 为 PictureBox 添加拖动功能
            bool isDragging = false;
            System.Drawing.Point lastCursor = Cursor.Position, lastForm = pictureBox1.Location;

            pictureBox1.MouseDown += (sender, e) =>
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = pictureBox1.Location;
            };

            pictureBox1.MouseMove += (sender, e) =>
            {
                if (isDragging)
                {
                    System.Drawing.Point deltaPos = System.Drawing.Point.Subtract(Cursor.Position, new System.Drawing.Size(lastCursor));
                    pictureBox1.Location = System.Drawing.Point.Add(lastForm, new System.Drawing.Size(deltaPos));
                }
            };

            pictureBox1.MouseUp += (sender, e) => isDragging = false;
        }

        private void ButtonOpenImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = new Bitmap(dlg.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                }
            }
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;

            float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f;
            Bitmap originalBitmap = new Bitmap(pictureBox1.Image);
            Bitmap resizedBitmap = ResizeImageWithOpenCV(originalBitmap, zoomFactor);

            pictureBox1.Image = resizedBitmap;
        }

        private Bitmap ResizeImageWithOpenCV(Bitmap originalBitmap, double scale)
        {
            Mat originalMat = BitmapConverter.ToMat(originalBitmap);
            Mat resizedMat = new Mat();
            Cv2.Resize(originalMat, resizedMat, new OpenCvSharp.Size(0, 0), scale, scale, InterpolationFlags.Linear);
            return BitmapConverter.ToBitmap(resizedMat);
        }
    }
}
