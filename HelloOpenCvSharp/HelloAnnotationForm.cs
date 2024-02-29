using OpenCvSharp.Extensions;
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
    public partial class HelloAnnotationForm : Form
    {
        private Mat originalImage;
        private Bitmap displayImage;
        private List<Annotation> annotations = new List<Annotation>();
        private System.Drawing.Point startPoint;
        private bool isDrawing = false;

        public HelloAnnotationForm()
        {
            InitializeComponent();

            InitializeCustomComponent();
        }

        private void InitializeCustomComponent()
        {
            // Assuming you have set up your PictureBox and Buttons
            // pictureBox1, buttonLoadImage, buttonSaveImage in the Form Designer
            buttonLoadImage.Click += ButtonLoadImage_Click;
            buttonSaveImage.Click += ButtonSaveImage_Click;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
        }

        private void ButtonLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    originalImage = Cv2.ImRead(ofd.FileName);
                    displayImage = BitmapConverter.ToBitmap(originalImage);
                    pictureBox1.Image = displayImage;
                    annotations.Clear(); // Clear existing annotations
                }
            }
        }

        private void ButtonSaveImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    RedrawImage(includeAnnotations: true); // Ensure annotations are drawn
                    Cv2.ImWrite(sfd.FileName, originalImage);
                }
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                isDrawing = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                for (int i = annotations.Count - 1; i >= 0; i--)
                {
                    if (annotations[i].Rect.Contains(e.Location))
                    {
                        annotations.RemoveAt(i);
                        RedrawImage(includeAnnotations: true);
                        break;
                    }
                }
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Optional: Update the UI to show the drawing rectangle
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = false;
                var endPoint = e.Location;
                var rect = new Rectangle(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y),
                                         Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y));

                if (rect.Width > 0 && rect.Height > 0)
                {
                    // 转换鼠标点击位置为屏幕坐标
                    System.Drawing.Point screenPoint = pictureBox1.PointToScreen(e.Location);

                    // 创建并显示 AnnotationForm
                    using (AnnotationForm annotationForm = new AnnotationForm())
                    {
                        // 设置 AnnotationForm 的起始位置
                        annotationForm.StartPosition = FormStartPosition.Manual;
                        annotationForm.Location = screenPoint;

                        // 显示 AnnotationForm 并等待用户完成输入
                        if (annotationForm.ShowDialog() == DialogResult.OK)
                        {
                            string annotationText = annotationForm.AnnotationText;
                            if (!string.IsNullOrWhiteSpace(annotationText))
                            {
                                var id = int.TryParse(annotationText, out int idInt) ? idInt : 1;
                                annotations.Add(new Annotation { Rect = rect, Id = id });
                                RedrawImage(includeAnnotations: true);
                            }
                        }
                        
                    }
                }
            }
        }

        private void RedrawImage(bool includeAnnotations)
        {
            if (originalImage == null) return;

            Mat imageToDraw = originalImage.Clone();
            if (includeAnnotations)
            {
                foreach (var annotation in annotations)
                {
                    // 绘制矩形框
                    Cv2.Rectangle(imageToDraw,
                                  new OpenCvSharp.Point(annotation.Rect.Left, annotation.Rect.Top),
                                  new OpenCvSharp.Point(annotation.Rect.Right, annotation.Rect.Bottom),
                                  Scalar.Red, 2);

                    // 绘制文本
                    // 注意：我们需要将 System.Drawing.Point 转换为 OpenCvSharp.Point
                    Cv2.PutText(imageToDraw,
                                $"{annotation.Id}",
                                new OpenCvSharp.Point(annotation.Rect.Left, annotation.Rect.Top - 10),
                                HersheyFonts.HersheySimplex, 0.5, Scalar.Blue, thickness: 1);
                }
            }

            displayImage?.Dispose(); // Dispose previous Bitmap
            displayImage = BitmapConverter.ToBitmap(imageToDraw);
            pictureBox1.Image = displayImage;
            pictureBox1.Refresh();

            imageToDraw.Dispose();
        }
    }

    class Annotation
    {
        public Rectangle Rect { get; set; }
        public int Id { get; set; }
    }
}
