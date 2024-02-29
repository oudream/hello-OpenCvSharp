using OpenCvSharp;
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
    public partial class MainForm : Form
    {
        Bitmap myBmp;
        System.Drawing.Point mouseDownPoint = new System.Drawing.Point(); // 记录拖拽过程鼠标位置
        bool isMove = false;    // 判断鼠标在picturebox上移动时，是否处于拖拽过程(鼠标左键是否按下)
        int zoomStep = 50;      // 缩放步长

        public MainForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Tiff文件|*.tif|Bmp文件|*.bmp|Erdas img文件|*.img|EVNI文件|*.hdr|jpeg文件|*.jpg|raw文件|*.raw|vrt文件|*.vrt|所有文件|*.*";
            dlg.FilterIndex = 8;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filename = dlg.FileName;
            }
            if (filename == "")
            {
                return;
            }
            // 使用OpenCvSharp加载和处理图像
            Mat image = Cv2.ImRead(dlg.FileName, ImreadModes.Unchanged);


            /**
            // 应用直方图均衡化
            Mat equalized = new Mat();
            Cv2.EqualizeHist(image, equalized);
            */

            /**             
            // 调整亮度和对比度
            image.ConvertTo(image, -1, 1.2, 50); // alpha 为对比度控制（1.0-3.0），beta 为亮度控制（0-100）
            // 创建锐化内核
            // 创建一个3x3的锐化内核
            float[,] kernelData = new float[,]
            {
    { -1.0f, -1.0f, -1.0f },
    { -1.0f,  9.0f, -1.0f },
    { -1.0f, -1.0f, -1.0f }
            };
            Mat kernel = new Mat(3, 3, MatType.CV_32F, kernelData);
            // 应用锐化
            Mat sharpImage = new Mat();
            Cv2.Filter2D(image, sharpImage, MatType.CV_8U, kernel);
            */

            // 对图像进行归一化处理，MinMax相当于把窗宽窗位拉满
            Mat image2 = new Mat();
            Cv2.Normalize(image, image2, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);

            // 将图像保存为PNG格式
            Cv2.ImWrite("E:\\image9\\a1.png", image2);
            myBmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image2);

            /*
            // 检查是否为16位图像
            if (image.Depth() == MatType.CV_16U) 
            {
                // 将16位图像转换为8位
                Mat normalizedImage = new Mat();
                // OpenCV中没有直接的Min/Max函数，需要先归一化
                image.ConvertTo(normalizedImage, MatType.CV_8U, 1.0 / 256); // 16位到8位的缩放因子
                myBmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(normalizedImage);
            }
            else if (image.Depth() == MatType.CV_8U) // 如果已经是8位图像
            {
                myBmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            }
            else
            {
                throw new InvalidOperationException("不支持的图像深度。");
            }
            */

            //myBmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);

            //myBmp = new Bitmap(filename);
            //if (myBmp == null)
            //{
            //    MessageBox.Show("读取失败");
            //    return;
            //}
            this.Text = filename;
            pictureBox1.Image = myBmp;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //设置picturebox为缩放模式
            pictureBox1.Width = myBmp.Width;
            pictureBox1.Height = myBmp.Height;
            zoomStep = pictureBox1.Width / 10;
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
                pictureBox1.Focus();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (isMove)
            {
                int x, y;
                int moveX, moveY;
                moveX = Cursor.Position.X - mouseDownPoint.X;
                moveY = Cursor.Position.Y - mouseDownPoint.Y;
                x = pictureBox1.Location.X + moveX;
                y = pictureBox1.Location.Y + moveY;
                pictureBox1.Location = new System.Drawing.Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;
            if (e.Delta > 0)
            {
                pictureBox1.Width += zoomStep;
                pictureBox1.Height += zoomStep;

                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);

                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            if (e.Delta < 0)
            {

                if (pictureBox1.Width < myBmp.Width / 10)
                    return;

                pictureBox1.Width -= zoomStep;
                pictureBox1.Height -= zoomStep;
                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }

            VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
            VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
            pictureBox1.Location = new System.Drawing.Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);
        }
    }

    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            // 启用双缓冲
            this.DoubleBuffered = true;

            // 这些设置可以帮助减少闪烁和提高绘制效率
            this.SetStyle(ControlStyles.ResizeRedraw, true); // 在调整控件大小时重绘控件
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true); // 使用双缓冲重新绘制
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
        }
    }
}
