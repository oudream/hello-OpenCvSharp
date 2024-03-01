using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HelloOpenCvSharp
{
    public class ImageProcessingControl
    {
        public enum OperationMode
        {
            None,
            DrawAnnotation,
            DrawDistanceLine,
            SelectGrayscale
        }

        private PictureBox pictureBox1;

        private bool isAnnotating;
        private Rectangle annotationRect;
        private System.Drawing.Point annotationStartPoint;
        private Mat originalImage; // 原始图像
        private Mat currentImage; // 当前图像
        private Bitmap displayImage; // 显示图像

        // 标注列表
        private List<Annotation> annotations = new List<Annotation>();

        // 高亮的矩形框
        private Annotation highlightedAnnotation = null;

        // 当前模式
        private OperationMode currentMode = OperationMode.None;

        private System.Drawing.Point mouseDownPoint = new System.Drawing.Point(); // 记录拖拽过程鼠标位置
        private bool isMoving = false;    // 判断鼠标在picturebox上移动时，是否处于拖拽过程(鼠标左键是否按下)
        private double zoomStep = 0.15d;      // 缩放步长

        // S7驱动模板状态变更事件
        public event EventHandler ParentInvalidate;
        protected virtual void OnParentInvalidate(EventArgs e)
        {
            // ?. 表示如果 ParentInvalidate 不为 null，则调用 Invoke
            ParentInvalidate?.Invoke(this, e);
        }

        private string theLabel;


        public ImageProcessingControl(PictureBox pictureBox, string label)
        {
            pictureBox1 = pictureBox;
            pictureBox1.MouseDown += pictureBox_MouseDown;
            pictureBox1.MouseMove += pictureBox_MouseMove;
            pictureBox1.MouseUp += pictureBox_MouseUp;
            pictureBox1.Paint += pictureBox_Paint;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            theLabel = label;
        }
       
        ~ImageProcessingControl()
        {
            // 释放
            pictureBox1.MouseDown -= pictureBox_MouseDown;
            pictureBox1.MouseMove -= pictureBox_MouseMove;
            pictureBox1.MouseUp -= pictureBox_MouseUp;
            pictureBox1.Paint -= pictureBox_Paint;
            pictureBox1.MouseWheel -= pictureBox1_MouseWheel;

            // 释放资源
            originalImage?.Dispose();
            currentImage?.Dispose();
        }

        public void OpenImage(string filename)
        {
            // 使用OpenCvSharp加载和处理图像
            Mat imageOrg = Cv2.ImRead(filename, ImreadModes.Unchanged);
            // 对图像进行归一化处理，MinMax相当于把窗宽窗位拉满
            originalImage?.Dispose();
            originalImage = new Mat();
            Cv2.Normalize(imageOrg, originalImage, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            // 从原始图像复制
            currentImage?.Dispose();
            currentImage = CloneOriginal();

            //
            displayImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(currentImage);

            pictureBox1.Image = displayImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //设置picturebox为缩放模式
            pictureBox1.Width = displayImage.Width;
            pictureBox1.Height = displayImage.Height;

            // 清除标注
            annotations.Clear();
        }
        public void SetNearestNeighbor(bool value)
        {
            NearestNeighborMode = value;
            pictureBox1.Invalidate();
        }

        public void SetCurrentMode(OperationMode mode)
        {
            // 如果当前模式已经是被点击的模式，则重置为None
            currentMode = mode;
            pictureBox1.Invalidate();
        }
        public OperationMode GetCurrentMode() => currentMode;



        #region 鼠标事件
        private System.Drawing.Point currentMousePosition;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (currentMode)
            {
                case OperationMode.DrawAnnotation:
                    StartDrawingAnnotation(sender, e);
                    break;
                case OperationMode.DrawDistanceLine:
                    StartDrawingDistanceLine(sender, e);
                    break;
                case OperationMode.SelectGrayscale:
                    StartSelectingGrayscale(sender, e);
                    break;
                default:
                    if (e.Button == MouseButtons.Left)
                    {
                        mouseDownPoint.X = Cursor.Position.X;
                        mouseDownPoint.Y = Cursor.Position.Y;
                        isMoving = true;
                        pictureBox1.Focus();
                    }
                    break;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            switch (currentMode)
            {
                case OperationMode.DrawAnnotation:
                    currentMousePosition = e.Location;
                    ContinueDrawingAnnotation(sender, e);
                    pictureBox1.Invalidate(); // 仅触发PictureBox的重绘
                    break;
                case OperationMode.DrawDistanceLine:
                    ContinueDrawingDistanceLine(sender, e);
                    break;
                case OperationMode.SelectGrayscale:
                    // 可能不需要在MouseMove中处理
                    break;
                default:
                    // 重置
                    currentMousePosition.X = 0;
                    currentMousePosition.Y = 0;
                    // 移动
                    pictureBox1.Focus();
                    if (isMoving)
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

                        pictureBox1.Invalidate(); // 触发重绘
                        OnParentInvalidate(EventArgs.Empty);
                    }
                    break;
            }

        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch (currentMode)
            {
                case OperationMode.DrawAnnotation:
                    FinishDrawingAnnotation(sender, e);
                    break;
                case OperationMode.DrawDistanceLine:
                    FinishDrawingDistanceLine(sender, e);
                    break;
                case OperationMode.SelectGrayscale:
                    FinishSelectingGrayscale(sender, e);
                    break;
                default:
                    if (e.Button == MouseButtons.Left)
                    {
                        isMoving = false;
                    }
                    break;
            }

        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;
            var zoomFactor = pictureBox1.Width / (double)currentImage.Width;
            int zoomWidth = (int)(currentImage.Width * zoomStep * zoomFactor);
            int zoomHeight = (int)(currentImage.Height * zoomStep * zoomFactor);

            if (e.Delta > 0)
            {
                // 限制放大范围到7
                if (pictureBox1.Width > 19000 || pictureBox1.Height > 19000)
                    return;
                pictureBox1.Width += zoomWidth;
                pictureBox1.Height += zoomHeight;

                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);

                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            if (e.Delta < 0)
            {
                // 限制缩小范围到0.2
                if (pictureBox1.Width < 400 || pictureBox1.Height < 400)
                    return;

                pictureBox1.Width -= zoomWidth;
                pictureBox1.Height -= zoomHeight;
                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }

            VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
            VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
            pictureBox1.Location = new System.Drawing.Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);

            //showInfo($"pictureBox1.Width: {pictureBox1.Width}  ==>  CurrentImage.Width: {currentImage.Width}  ==>  zoomFactor: {pictureBox1.Width / (double)currentImage.Width}");
            //showInfo($"pictureBox1.Height: {pictureBox1.Height}  ==>  CurrentImage.Height: {currentImage.Height}");
            //showInfo($"pictrueBox1.Location: {pictureBox1.Location} ");
            pictureBox1.Invalidate(); // 触发重绘
            OnParentInvalidate(EventArgs.Empty);
        }

        private void showInfo(string v)
        {
            Console.WriteLine(v);
        }
        #endregion

        private bool NearestNeighborMode = false;
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            // 绘制图像模式为最近邻插值
            if (pictureBox1.Image != null)
            {
                if (NearestNeighborMode)
                {
                    // 设置插值模式为最近邻插值，不进行平滑处理
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    // 创建图像绘制的目标矩形，当前图像窗口的矩形
                    Rectangle destRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);

                    // 绘制图像
                    e.Graphics.DrawImage(pictureBox1.Image, destRect);
                }
            }

            // 当前模式为标注模式
            if (currentMode == OperationMode.DrawAnnotation)
            {
                // 绘制所有矩形框
                foreach (var annotation in annotations)
                {
                    // 如果是高亮的矩形框，则使用不同的颜色
                    if (annotation == highlightedAnnotation)
                    {
                        // 转换为pictureBox的坐标
                        var rect = ToPictureBoxAnnotationRect(annotation.Rect);
                        e.Graphics.DrawRectangle(Pens.LimeGreen, rect);
                    }
                }

                if (!currentMousePosition.IsEmpty)
                {
                    // 绘制十字线
                    var x = currentMousePosition.X;
                    var y = currentMousePosition.Y;
                    var g = e.Graphics;
                    var pen = new Pen(Color.Yellow, 1); // 选择颜色和线宽
                    g.DrawLine(pen, new System.Drawing.Point(x, 0), new System.Drawing.Point(x, pictureBox1.Height)); // 竖线
                    g.DrawLine(pen, new System.Drawing.Point(0, y), new System.Drawing.Point(pictureBox1.Width, y)); // 横线
                }
            }

            // 仅在正在标注时绘制矩形框
            if (isAnnotating && annotationRect.Width > 0 && annotationRect.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Red, annotationRect);
            }
        }

        private Mat CloneOriginal()
        {
            if (originalImage.Channels() == 1)
            {
                var image = new Mat();
                Cv2.CvtColor(originalImage, image, ColorConversionCodes.GRAY2BGR);
                return image;
            }
            else
            {
                return originalImage.Clone();
            }
        }

        #region 矩形标注模式
        void StartDrawingAnnotation(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /* 实现开始绘制标注的逻辑 */
                isAnnotating = true;
                annotationStartPoint = e.Location;
                annotationRect = new Rectangle(e.Location, new System.Drawing.Size());
            }
        }
        void ContinueDrawingAnnotation(object sender, MouseEventArgs e)
        {
            /* 实现继续绘制标注的逻辑 */
            if (isAnnotating)
            {
                annotationRect.Width = e.X - annotationStartPoint.X;
                annotationRect.Height = e.Y - annotationStartPoint.Y;
                //pictureBox1.Invalidate(); // 仅触发PictureBox的重绘
                // showInfo($"annotationRect: {annotationRect}");
            }
            else // 如果高亮的矩形框发生变化
            {
                // 转换成原图像坐标点
                var mousePoint = ToOrgAnnotationPoint(e.Location);
                var previousHighlighted = highlightedAnnotation;
                highlightedAnnotation = annotations.FirstOrDefault(ann => ann.Rect.Contains(mousePoint));
                //if (highlightedAnnotation != previousHighlighted)
                //{
                //    pictureBox1.Invalidate(); // 如果高亮的矩形框发生变化，则需要重绘
                //}
            }
        }
        void FinishDrawingAnnotation(object sender, MouseEventArgs e)
        {
            /* 实现完成绘制标注的逻辑 */
            if (isAnnotating)
            {
                isAnnotating = false;

                // 添加标注
                {
                    if (annotationRect.Width < 5 || annotationRect.Height < 5)
                        return;
                    // 转换成原图像坐标矩形
                    var rect = ToOrgAnnotationRect(annotationRect);
                    var annotation = new Annotation { Rect = rect, Id = AssignAnnotationId(), Label = theLabel };
                    annotations.Add(annotation);
                    // 在原始图像上绘制最终的矩形框
                    DrawAnnotation(currentImage, annotation);
                }

                {
                    // 更新显示
                    {
                        displayImage?.Dispose(); // Dispose previous Bitmap
                        displayImage = BitmapConverter.ToBitmap(currentImage);
                        pictureBox1.Image = displayImage;
                        pictureBox1.Invalidate(); // 更新视图
                    }
                }


            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    // 转换成原图像坐标点
                    var mousePoint = ToOrgAnnotationPoint(e.Location);
                    var deletedAnnotation = annotations.FirstOrDefault(ann => ann.Rect.Contains(mousePoint));
                    if (deletedAnnotation != null)
                    {
                        annotations.Remove(deletedAnnotation);
                        // 剩下的矩形框重新绘制
                        // 从原始图像复制
                        currentImage?.Dispose();
                        currentImage = CloneOriginal();
                        foreach (var annotation in annotations)
                        {
                            DrawAnnotation(currentImage, annotation);
                        }

                        // 更新显示
                        {
                            displayImage?.Dispose(); // Dispose previous Bitmap
                            displayImage = BitmapConverter.ToBitmap(currentImage);
                            pictureBox1.Image = displayImage;
                            pictureBox1.Invalidate(); // 更新视图
                        }
                    }
                }
            }
        }
        // 分配标注的ID
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
        // 在图像上绘制最终的矩形框
        void DrawAnnotation(Mat image, Annotation annotation)
        {
            // 绘制矩形框
            Cv2.Rectangle(image,
                          new OpenCvSharp.Point(annotation.Rect.Left, annotation.Rect.Top),
                          new OpenCvSharp.Point(annotation.Rect.Right, annotation.Rect.Bottom),
                          Scalar.Red, 2);
            // 绘制文本
            // 注意：我们需要将 System.Drawing.Point 转换为 OpenCvSharp.Point
            Cv2.PutText(image,
                        $"{annotation.Id} {annotation.Label}",
                        new OpenCvSharp.Point(annotation.Rect.Left, annotation.Rect.Top - 10),
                        HersheyFonts.HersheySimplex, 0.5, Scalar.Blue, thickness: 1);
        }
        // 转换成原图像坐标矩形
        // rect 为相对于pictureBox的坐标
        Rectangle ToOrgAnnotationRect(Rectangle rect)
        {
            var zoomFactor = pictureBox1.Width / (double)currentImage.Width;
            var location = new System.Drawing.Point((int)(rect.X / zoomFactor), (int)(rect.Y / zoomFactor));
            var size = new System.Drawing.Size((int)(rect.Width / zoomFactor), (int)(rect.Height / zoomFactor));
            return new Rectangle(location, size);
        }
        // point 为相对于pictureBox的坐标
        System.Drawing.Point ToOrgAnnotationPoint(System.Drawing.Point point)
        {
            var zoomFactor = pictureBox1.Width / (double)currentImage.Width;
            return new System.Drawing.Point((int)(point.X / zoomFactor), (int)(point.Y / zoomFactor));
        }
        // rect 为原图像坐标
        Rectangle ToPictureBoxAnnotationRect(Rectangle rect)
        {
            var zoomFactor = pictureBox1.Width / (double)currentImage.Width;
            var location = new System.Drawing.Point((int)(rect.X * zoomFactor), (int)(rect.Y * zoomFactor));
            var size = new System.Drawing.Size((int)(rect.Width * zoomFactor), (int)(rect.Height * zoomFactor));
            return new Rectangle(location, size);
        }
        #endregion

        #region 绘制距离线
        void StartDrawingDistanceLine(object sender, MouseEventArgs e)
        {
            /* 实现开始绘制距离线的逻辑 */
        }
        void ContinueDrawingDistanceLine(object sender, MouseEventArgs e)
        {
            /* 实现继续绘制距离线的逻辑 */
        }
        void FinishDrawingDistanceLine(object sender, MouseEventArgs e)
        {
            /* 实现完成绘制距离线的逻辑 */
        }
        #endregion

        #region 框选灰度值
        void StartSelectingGrayscale(object sender, MouseEventArgs e)
        {
            /* 实现开始框选灰度值的逻辑 */
        }
        // MouseMove可能不需要在选择灰度模式下处理
        void FinishSelectingGrayscale(object sender, MouseEventArgs e)
        {
            /* 实现完成框选灰度值的逻辑 */
        }
        #endregion

        void HelloCV2Data()
        {
            // 假设你已经有了imageData（IntPtr）和图像的宽度、高度
            int img_width = 640; // 图像的宽度
            int img_height = 480; // 图像的高度
            IntPtr imageData = Marshal.AllocHGlobal(img_width * img_height * 2);
            // 使用OpenCvSharp创建Mat对象
            Mat imageMat = new Mat(img_height, img_width, MatType.CV_16U, imageData);




            // 计算数据的大小（例如，对于8位的灰度图像）
            int dataSize = img_width * img_height;

            // 创建一个托管数组来存储复制的数据
            byte[] managedArray = new byte[dataSize];

            // 从非托管内存复制数据到托管数组
            Marshal.Copy(imageData, managedArray, 0, dataSize);

            // 使用复制的数据创建一个Mat对象
            Mat imageMat2 = Mat.FromImageData(managedArray, ImreadModes.Grayscale);
        }

        //private void UpdateDeviceStatusLabel(string status)
        //{
        //    switch (status)
        //    {
        //        case "Connected":
        //            deviceStatusLabel.ForeColor = Color.Green;
        //            deviceStatusLabel.BackColor = Color.LightGreen;
        //            deviceStatusLabel.Text = "设备已连接";
        //            break;
        //        case "Disconnected":
        //            deviceStatusLabel.ForeColor = Color.Red;
        //            deviceStatusLabel.BackColor = Color.LightPink;
        //            deviceStatusLabel.Text = "设备未连接";
        //            break;
        //        case "Busy":
        //            deviceStatusLabel.ForeColor = Color.Orange;
        //            deviceStatusLabel.BackColor = Color.LightYellow;
        //            deviceStatusLabel.Text = "设备忙";
        //            break;
        //        default:
        //            deviceStatusLabel.ForeColor = Color.Black;
        //            deviceStatusLabel.BackColor = Color.LightGray;
        //            deviceStatusLabel.Text = "未知状态";
        //            break;
        //    }
        //}

        //private void UpdateIndicatorButton(string status)
        //{
        //    switch (status)
        //    {
        //        case "On":
        //            indicatorButton.BackColor = Color.Green;
        //            indicatorButton.Text = "开";
        //            break;
        //        case "Off":
        //            indicatorButton.BackColor = Color.Red;
        //            indicatorButton.Text = "关";
        //            break;
        //        default:
        //            indicatorButton.BackColor = Color.Gray;
        //            indicatorButton.Text = "未知";
        //            break;
        //    }
        //}
    }

    class Annotation
    {
        public Rectangle Rect { get; set; }
        public int Id { get; set; }
        public string Label { get; set; }
    }
}
