using OpenCvSharp;
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
    // 画多个标注、鼠标选种的高亮显示矩形框
    public partial class MouseDrawRectangleForm2 : Form
    {
        private bool isAnnotating;
        private Rectangle annotationRect;
        private System.Drawing.Point annotationStartPoint;
        private Bitmap originalImage; // 原始图像
        private Bitmap currentImage; // 当前图像
        private List<Annotation> annotations = new List<Annotation>();

        private Annotation highlightedAnnotation = null;

        public MouseDrawRectangleForm2()
        {
            InitializeComponent(); // 确保已经在设计器中初始化了pictureBox1
            this.DoubleBuffered = true;
            //originalImage = new Bitmap(pictureBox1.Image); // 假设pictureBox1已经加载了图像

            pictureBox1.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox_MouseUp);
            pictureBox1.Paint += new PaintEventHandler(pictureBox_Paint);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            isAnnotating = true;
            annotationStartPoint = e.Location;
            annotationRect = new Rectangle(e.Location, new System.Drawing.Size());
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isAnnotating)
            {
                annotationRect.Width = e.X - annotationStartPoint.X;
                annotationRect.Height = e.Y - annotationStartPoint.Y;
                pictureBox1.Invalidate(); // 仅触发PictureBox的重绘
            }
            else // 如果高亮的矩形框发生变化
            {
                var mousePoint = new System.Drawing.Point(e.X, e.Y);
                var previousHighlighted = highlightedAnnotation;
                highlightedAnnotation = annotations.FirstOrDefault(ann => ann.Rect.Contains(mousePoint));

                if (highlightedAnnotation != previousHighlighted)
                {
                    pictureBox1.Invalidate(); // 如果高亮的矩形框发生变化，则需要重绘
                }
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isAnnotating)
            {
                isAnnotating = false;
                // 在原始图像上绘制最终的矩形框
                using (Graphics g = Graphics.FromImage(currentImage))
                {
                    annotations.Add(new Annotation { Rect = annotationRect, Id = AssignAnnotationId() });
                    g.DrawRectangle(Pens.Red, annotationRect);
                }
                pictureBox1.Image = currentImage; // 更新PictureBox以显示最新图像
                pictureBox1.Invalidate(); // 更新视图
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            // 绘制所有矩形框
            foreach (var annotation in annotations)
            {
                // 如果是高亮的矩形框，则使用不同的颜色
                if (annotation == highlightedAnnotation)
                {
                    e.Graphics.DrawRectangle(Pens.LimeGreen, annotation.Rect);
                }
            }

            // 仅在正在标注时绘制矩形框
            if (isAnnotating && annotationRect.Width > 0 && annotationRect.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Red, annotationRect);
            }
        }
        int AssignAnnotationId()
        {
            // 检查 annotations.Count 内顺序号都有没有被占用，没有则返回
            for (int i = 0; i < annotations.Count; i++)
            {
                // i 在 annotations 中没有被占用
                if (!annotations.Any(ann => ann.Id == i))
                {
                    return i;
                }
            }
            return annotations.Count;
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
            Mat imageOrg = Cv2.ImRead(dlg.FileName, ImreadModes.Unchanged);
            // 对图像进行归一化处理，MinMax相当于把窗宽窗位拉满
            Mat imageDst = new Mat();
            Cv2.Normalize(imageOrg, imageDst, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            //
            var myBmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(imageDst);
            originalImage = new Bitmap(myBmp); // 假设pictureBox1已经加载了图像
            currentImage = new Bitmap(myBmp);

            this.Text = filename;
            pictureBox1.Image = myBmp;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //设置picturebox为缩放模式
            pictureBox1.Width = myBmp.Width;
            pictureBox1.Height = myBmp.Height;
            
            // 清除标注
            annotations.Clear(); 
        }
    }

}
