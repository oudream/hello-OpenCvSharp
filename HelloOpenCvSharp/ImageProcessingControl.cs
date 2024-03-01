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

        private PictureBox _pictureBox;

        private bool _isAnnotationDrawing, _isAnnotationResizing, _isAnnotationMoving;
        private Rectangle _annotationRect;
        private System.Drawing.Point _annotationStartPoint, _annotationLastLocation;
        private Mat _originalImage; // 原始图像
        private Mat _currentImage; // 当前图像
        private Bitmap _displayImage; // 显示图像

        // 标注列表
        private List<Annotation> _annotations = new List<Annotation>();

        // 高亮的矩形框
        private Annotation _highlightedAnnotation = null;

        // 当前模式
        private OperationMode _currentMode = OperationMode.None;

        // 拖动、放大、缩小
        private System.Drawing.Point _mouseDownPoint = new System.Drawing.Point(); // 记录拖拽过程鼠标位置
        private bool _isMoving = false;    // 判断鼠标在picturebox上移动时，是否处于拖拽过程(鼠标左键是否按下)
        private double _zoomStep = 0.15d;      // 缩放步长

        // 父控件需要触发变更事件
        public event EventHandler ParentInvalidate;
        protected virtual void OnParentInvalidate(EventArgs e)
        {
            // ?. 表示如果 ParentInvalidate 不为 null，则调用 Invoke
            ParentInvalidate?.Invoke(this, e);
        }

        // 标注的标签
        private string _annotationLabel;

        public ImageProcessingControl(PictureBox pictureBox, string label)
        {
            _pictureBox = pictureBox;
            _pictureBox.MouseDown += pictureBox_MouseDown;
            _pictureBox.MouseMove += pictureBox_MouseMove;
            _pictureBox.MouseUp += pictureBox_MouseUp;
            _pictureBox.Paint += pictureBox_Paint;
            _pictureBox.MouseWheel += pictureBox1_MouseWheel;

            _annotationLabel = label;
        }
       
        ~ImageProcessingControl()
        {
            // 释放
            _pictureBox.MouseDown -= pictureBox_MouseDown;
            _pictureBox.MouseMove -= pictureBox_MouseMove;
            _pictureBox.MouseUp -= pictureBox_MouseUp;
            _pictureBox.Paint -= pictureBox_Paint;
            _pictureBox.MouseWheel -= pictureBox1_MouseWheel;

            // 释放资源
            _originalImage?.Dispose();
            _currentImage?.Dispose();
        }

        public void OpenImage(string filename)
        {
            // 使用OpenCvSharp加载和处理图像
            Mat imageOrg = Cv2.ImRead(filename, ImreadModes.Unchanged);
            // 对图像进行归一化处理，MinMax相当于把窗宽窗位拉满
            _originalImage?.Dispose();
            _originalImage = new Mat();
            Cv2.Normalize(imageOrg, _originalImage, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            // 从原始图像复制
            _currentImage?.Dispose();
            _currentImage = CloneOriginal();

            //
            _displayImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(_currentImage);

            _pictureBox.Image = _displayImage;
            _pictureBox.SizeMode = PictureBoxSizeMode.Zoom; //设置picturebox为缩放模式
            _pictureBox.Width = _displayImage.Width;
            _pictureBox.Height = _displayImage.Height;

            // 清除标注
            _annotations.Clear();
        }
        public void SetNearestNeighbor(bool value)
        {
            NearestNeighborMode = value;
            _pictureBox.Invalidate();
        }

        public void SetCurrentMode(OperationMode mode)
        {
            // 如果当前模式已经是被点击的模式，则重置为None
            _currentMode = mode;
            _pictureBox.Invalidate();
        }
        public OperationMode GetCurrentMode() => _currentMode;

        public void SetLabel(string label) => _annotationLabel = label;

        #region 鼠标事件
        private System.Drawing.Point currentMousePosition;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (_currentMode)
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
                        _mouseDownPoint.X = Cursor.Position.X;
                        _mouseDownPoint.Y = Cursor.Position.Y;
                        _isMoving = true;
                        _pictureBox.Focus();
                    }
                    break;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            switch (_currentMode)
            {
                case OperationMode.DrawAnnotation:
                    currentMousePosition = e.Location;
                    ContinueDrawingAnnotation(sender, e);
                    _pictureBox.Invalidate(); // 仅触发PictureBox的重绘
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
                    _pictureBox.Focus();
                    if (_isMoving)
                    {
                        int x, y;
                        int moveX, moveY;
                        moveX = Cursor.Position.X - _mouseDownPoint.X;
                        moveY = Cursor.Position.Y - _mouseDownPoint.Y;
                        x = _pictureBox.Location.X + moveX;
                        y = _pictureBox.Location.Y + moveY;
                        _pictureBox.Location = new System.Drawing.Point(x, y);
                        _mouseDownPoint.X = Cursor.Position.X;
                        _mouseDownPoint.Y = Cursor.Position.Y;

                        _pictureBox.Invalidate(); // 触发重绘
                        OnParentInvalidate(EventArgs.Empty);
                    }
                    break;
            }

        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch (_currentMode)
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
                        _isMoving = false;
                    }
                    break;
            }

        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = _pictureBox.Width;
            int oh = _pictureBox.Height;
            int VX, VY;
            var zoomFactor = _pictureBox.Width / (double)_currentImage.Width;
            int zoomWidth = (int)(_currentImage.Width * _zoomStep * zoomFactor);
            int zoomHeight = (int)(_currentImage.Height * _zoomStep * zoomFactor);

            if (e.Delta > 0)
            {
                // 限制放大范围到7
                if (_pictureBox.Width > 19000 || _pictureBox.Height > 19000)
                    return;
                _pictureBox.Width += zoomWidth;
                _pictureBox.Height += zoomHeight;

                PropertyInfo pInfo = _pictureBox.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(_pictureBox, null);

                _pictureBox.Width = rect.Width;
                _pictureBox.Height = rect.Height;
            }
            if (e.Delta < 0)
            {
                // 限制缩小范围到0.2
                if (_pictureBox.Width < 400 || _pictureBox.Height < 400)
                    return;

                _pictureBox.Width -= zoomWidth;
                _pictureBox.Height -= zoomHeight;
                PropertyInfo pInfo = _pictureBox.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(_pictureBox, null);
                _pictureBox.Width = rect.Width;
                _pictureBox.Height = rect.Height;
            }

            VX = (int)((double)x * (ow - _pictureBox.Width) / ow);
            VY = (int)((double)y * (oh - _pictureBox.Height) / oh);
            _pictureBox.Location = new System.Drawing.Point(_pictureBox.Location.X + VX, _pictureBox.Location.Y + VY);

            //showInfo($"pictureBox1.Width: {pictureBox1.Width}  ==>  CurrentImage.Width: {currentImage.Width}  ==>  zoomFactor: {pictureBox1.Width / (double)currentImage.Width}");
            //showInfo($"pictureBox1.Height: {pictureBox1.Height}  ==>  CurrentImage.Height: {currentImage.Height}");
            //showInfo($"pictrueBox1.Location: {pictureBox1.Location} ");
            _pictureBox.Invalidate(); // 触发重绘
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
            if (_pictureBox.Image != null)
            {
                if (NearestNeighborMode)
                {
                    // 设置插值模式为最近邻插值，不进行平滑处理
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    // 创建图像绘制的目标矩形，当前图像窗口的矩形
                    Rectangle destRect = new Rectangle(0, 0, _pictureBox.Width, _pictureBox.Height);

                    // 绘制图像
                    e.Graphics.DrawImage(_pictureBox.Image, destRect);
                }

                //var zoomFactor = pictureBox1.Width / (float)currentImage.Width;
                //e.Graphics.ScaleTransform(zoomFactor, zoomFactor);

                // 绘制所有矩形框
                foreach (var annotation in _annotations)
                {
                    // 转换为pictureBox的坐标
                    //var rect = ToPictureBoxAnnotationRect(annotation);
                    //e.Graphics.DrawRectangle(Pens.Red, rect);
                    //DrawAnnotation(currentImage, annotation);
                    DrawAnnotation(e.Graphics, annotation);
                }
            }

            // 当前模式为标注模式
            if (_currentMode == OperationMode.DrawAnnotation)
            {
                // 绘制所有矩形框
                foreach (var annotation in _annotations)
                {
                    // 如果是高亮的矩形框，则使用不同的颜色
                    if (annotation == _highlightedAnnotation)
                    {
                        // 转换为pictureBox的坐标
                        var rect = ToPictureBoxAnnotationRect(annotation);
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
                    g.DrawLine(pen, new System.Drawing.Point(x, 0), new System.Drawing.Point(x, _pictureBox.Height)); // 竖线
                    g.DrawLine(pen, new System.Drawing.Point(0, y), new System.Drawing.Point(_pictureBox.Width, y)); // 横线
                }
            }

            // 仅在正在标注时绘制矩形框
            if ((_isAnnotationDrawing || _isAnnotationResizing || _isAnnotationMoving) && _annotationRect.Width > 0 && _annotationRect.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Red, _annotationRect);
            }
        }

        private Mat CloneOriginal()
        {
            if (_originalImage.Channels() == 1)
            {
                var image = new Mat();
                Cv2.CvtColor(_originalImage, image, ColorConversionCodes.GRAY2BGR);
                return image;
            }
            else
            {
                return _originalImage.Clone();
            }
        }

        #region 矩形标注模式
        void StartDrawingAnnotation(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMouseInResizeZone(e.Location) is Annotation annotationResize)
                {
                    _annotationRect = ToPictureBoxAnnotationRect(annotationResize);
                    _annotations.Remove(annotationResize);
                    //
                    _isAnnotationResizing = true;
                    _annotationStartPoint = e.Location;
                }
                else if (IsMouseInMoveZone(e.Location) is Annotation annotationMove)
                {
                    _annotationRect = ToPictureBoxAnnotationRect(annotationMove);
                    _annotations.Remove(annotationMove);
                    //
                    _isAnnotationMoving = true;
                    _annotationLastLocation = e.Location;
                }
                else
                {
                    /* 实现开始绘制标注的逻辑 */
                    _isAnnotationDrawing = true;
                    _annotationStartPoint = e.Location;
                    _annotationRect = new Rectangle(e.Location, new System.Drawing.Size());
                }
            }
        }
        private Annotation IsMouseInResizeZone(System.Drawing.Point location)
        {
            var mousePoint = ToOrgAnnotationPoint(location);
            foreach (var annotation in _annotations)
            {
                Rectangle resizeZone = new Rectangle(annotation.GetRight() - 10, annotation.GetBottom() - 10, 10, 10);
                if (resizeZone.Contains(mousePoint))
                {
                    // 如果找到，返回当前的调整区域矩形
                    return annotation;
                }
            }
            return null;
        }
        private Annotation IsMouseInMoveZone(System.Drawing.Point location)
        {
            var mousePoint = ToOrgAnnotationPoint(location);
            foreach (var annotation in _annotations)
            {
                Rectangle moveZone = new Rectangle(annotation.GetLeft(), annotation.GetTop(), 10, 10);
                if (moveZone.Contains(mousePoint))
                {
                    // 如果找到，返回当前的调整区域矩形
                    return annotation;
                }
            }
            return null;
        }
        void ContinueDrawingAnnotation(object sender, MouseEventArgs e)
        {
            if (_isAnnotationDrawing)
            {
                _annotationRect.Width = e.X - _annotationStartPoint.X;
                _annotationRect.Height = e.Y - _annotationStartPoint.Y;
            }
            else if (_isAnnotationResizing)
            {
                _annotationRect.Width = e.X - _annotationRect.X;
                _annotationRect.Height = e.Y - _annotationRect.Y;
            }
            else if (_isAnnotationMoving)
            {
                int dx = e.X - _annotationLastLocation.X;
                int dy = e.Y - _annotationLastLocation.Y;
                _annotationRect = new Rectangle(_annotationRect.X + dx, _annotationRect.Y + dy, _annotationRect.Width, _annotationRect.Height);
                _annotationLastLocation = e.Location;
            }
            else
            {
                if (IsMouseInResizeZone(e.Location) is Annotation annotationResize)
                {
                    this._pictureBox.Cursor = Cursors.SizeNWSE;
                }
                else if (IsMouseInMoveZone(e.Location) is Annotation annotationMove)
                {
                    this._pictureBox.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this._pictureBox.Cursor = Cursors.Default;
                    // 如果高亮的矩形框发生变化
                    // 转换成原图像坐标点
                    var mousePoint = ToOrgAnnotationPoint(e.Location);
                    var previousHighlighted = _highlightedAnnotation;
                    _highlightedAnnotation = _annotations.FirstOrDefault(ann => ann.Rect.Contains(mousePoint));
                }
            }
        }
        void FinishDrawingAnnotation(object sender, MouseEventArgs e)
        {
            /* 实现完成绘制标注的逻辑 */
            if (_isAnnotationDrawing || _isAnnotationResizing || _isAnnotationMoving)
            {
                // 添加标注
                {
                    if (_annotationRect.Width < 5 || _annotationRect.Height < 5)
                        return;
                    // 转换成原图像坐标矩形
                    var annotation = CreateOrgAnnotation(_annotationRect);
                    annotation.Id = AssignAnnotationId();
                    annotation.Label = _annotationLabel;
                    _annotations.Add(annotation);
                    // 在原始图像上绘制最终的矩形框
                    //DrawAnnotation(currentImage, annotation);
                }

                {
                    // 更新显示
                    {
                        _displayImage?.Dispose(); // Dispose previous Bitmap
                        _displayImage = BitmapConverter.ToBitmap(_currentImage);
                        _pictureBox.Image = _displayImage;
                        _pictureBox.Invalidate(); // 更新视图
                    }
                }
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    // 转换成原图像坐标点
                    var mousePoint = ToOrgAnnotationPoint(e.Location);
                    var deletedAnnotation = _annotations.FirstOrDefault(ann => ann.Rect.Contains(mousePoint));
                    if (deletedAnnotation != null)
                    {
                        _annotations.Remove(deletedAnnotation);
                        // 剩下的矩形框重新绘制
                        // 从原始图像复制
                        _currentImage?.Dispose();
                        _currentImage = CloneOriginal();
                        foreach (var annotation in _annotations)
                        {
                            //DrawAnnotation(currentImage, annotation);
                        }

                        // 更新显示
                        {
                            _displayImage?.Dispose(); // Dispose previous Bitmap
                            _displayImage = BitmapConverter.ToBitmap(_currentImage);
                            _pictureBox.Image = _displayImage;
                            _pictureBox.Invalidate(); // 更新视图
                        }
                    }
                }
            }

            _isAnnotationDrawing = _isAnnotationMoving = _isAnnotationResizing = false;
        }
        // 分配标注的ID
        int AssignAnnotationId()
        {
            // 检查 annotations.Count 内顺序号都有没有被占用，没有则返回
            for (int i = 0; i < _annotations.Count; i++)
            {
                // i 在 annotations 中没有被占用
                if (!_annotations.Any(ann => ann.Id == i))
                {
                    return i;
                }
            }
            return _annotations.Count;
        }
        // 在图像上绘制最终的矩形框
        void DrawAnnotation(Mat image, Annotation annotation)
        {
            // 绘制矩形框
            Cv2.Rectangle(image,
                          new OpenCvSharp.Point(annotation.GetLeft(), annotation.GetTop()),
                          new OpenCvSharp.Point(annotation.GetRight(), annotation.GetBottom()),
                          Scalar.Red, 2);
            // 绘制文本
            // 注意：我们需要将 System.Drawing.Point 转换为 OpenCvSharp.Point
            Cv2.PutText(image,
                        $"{annotation.Id} {annotation.Label}",
                        new OpenCvSharp.Point(annotation.GetLeft(), annotation.GetTop() - 10),
                        HersheyFonts.HersheySimplex, 0.5, Scalar.Blue, thickness: 1);
        }
        // 在图像上绘制最终的矩形框
        void DrawAnnotation(Graphics graphics, Annotation annotation)
        {
            // 绘制矩形框
            var rect = ToPictureBoxAnnotationRect(annotation);
            graphics.DrawRectangle(Pens.Red, rect);

            var zoomFactor = _pictureBox.Width / (double)_currentImage.Width;

            // 绘制文本
            // 注意：我们需要将 System.Drawing.Point 转换为 OpenCvSharp.Point
            // 创建字体和画刷
            using (Font font = new Font("Arial", (int)(zoomFactor * 9))) // 字体大小也会相应缩放
            using (Brush brush = new SolidBrush(Color.Red))
            {
                // 设置文本的原始位置
                PointF point = new PointF(rect.Left, rect.Top - (int)(zoomFactor * 16));

                // 绘制文本
                graphics.DrawString($"{annotation.Id} {annotation.Label}", font, brush, point);
            }
        }
        // 转换成原图像坐标矩形
        // rect 为相对于pictureBox的坐标
        Annotation CreateOrgAnnotation(Rectangle rect)
        {
            var zoomFactor = _pictureBox.Width / (double)_currentImage.Width;
            return new Annotation(rect.X / zoomFactor, rect.Y / zoomFactor, rect.Width / zoomFactor, rect.Height / zoomFactor);
        }
        // point 为相对于pictureBox的坐标
        System.Drawing.Point ToOrgAnnotationPoint(System.Drawing.Point point)
        {
            var zoomFactor = _pictureBox.Width / (double)_currentImage.Width;
            return new System.Drawing.Point((int)(point.X / zoomFactor), (int)(point.Y / zoomFactor));
        }
        // rect 为原图像坐标
        Rectangle ToPictureBoxAnnotationRect(Annotation annotation)
        {
            var zoomFactor = _pictureBox.Width / (double)_currentImage.Width;
            var location = new System.Drawing.Point((int)(annotation.X * zoomFactor), (int)(annotation.Y * zoomFactor));
            var size = new System.Drawing.Size((int)(annotation.Width * zoomFactor), (int)(annotation.Height * zoomFactor));
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
        public double X;

        public double Y;

        public double Width;

        public double Height;

        public int Id { get; set; }
        
        public string Label { get; set; }
        
        public Annotation(double x, double y, double width, double height)
        {
            X = x; Y = y; Width = width; Height = height;
            Rect = new Rectangle(Round(X), Round(Y), Round(Width), Round(Height));
        }
        public Rectangle Rect { get; private set; }

        public int GetX() => Round(X);
        public int GetY() => Round(Y);
        public int GetWidth() => Round(Width);
        public int GetHeight() => Round(Height);
        public int GetLeft() => Round(X);
        public int GetTop() => Round(Y);
        public int GetRight() => Round(X + Width);
        public int GetBottom() => Round(Y + Height);

        private int Round(double d)
        {
            return (int) Math.Round(d, MidpointRounding.AwayFromZero);
        }
    }
}
