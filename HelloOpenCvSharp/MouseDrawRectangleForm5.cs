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
using static HelloOpenCvSharp.ImageProcessingControl;

namespace HelloOpenCvSharp
{
    public partial class MouseDrawRectangleForm5 : Form
    {
        private ImageProcessingControl imageProcessingControl;

        public MouseDrawRectangleForm5()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            //originalImage = new Bitmap(pictureBox1.Image); // 假设pictureBox1已经加载了图像

            imageProcessingControl = new ImageProcessingControl(this.pictureBox1, this.labelTextBox.Text);
            imageProcessingControl.ParentInvalidate += (sender, e) => { this.Invalidate(); };

            openImageButton.Click += new EventHandler(openImageButton_Click);
            NearestNeighborCheckBox.CheckedChanged += new EventHandler(NearestNeighborCheckBox_CheckedChanged);
            InitializeButtonEvents();
         }

        private void openImageButton_Click(object sender, EventArgs e)
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
            imageProcessingControl.OpenImage(filename);
            this.Text = filename;
        }

        #region 操作模式切换
        private void InitializeButtonEvents()
        {
            btnDrawAnnotation.Click += (sender, e) => { ToggleMode(OperationMode.DrawAnnotation); };
            btnDrawDistanceLine.Click += (sender, e) => { ToggleMode(OperationMode.DrawDistanceLine); };
            btnSelectGrayscale.Click += (sender, e) => { ToggleMode(OperationMode.SelectGrayscale); };
        }

        private void ToggleMode(OperationMode mode)
        {
            // 先将所有按钮恢复到默认样式
            ResetButtonStyles(); 
            // 如果当前模式已经是被点击的模式，则重置为None
            if (imageProcessingControl.GetCurrentMode() == mode)
            {
                imageProcessingControl.SetCurrentMode(OperationMode.None); // OperationMode.None;
            }
            else
            {
                imageProcessingControl.SetCurrentMode(mode);
                SetCurrentMode(mode);
            }
        }

        private void SetCurrentMode(OperationMode mode)
        {
            // 根据当前模式更新UI
            switch (mode)
            {
                case OperationMode.DrawAnnotation:
                    btnDrawAnnotation.BackColor = Color.LightBlue; // 表示选中
                    break;
                case OperationMode.DrawDistanceLine:
                    btnDrawDistanceLine.BackColor = Color.LightBlue; // 表示选中
                    break;
                case OperationMode.SelectGrayscale:
                    btnSelectGrayscale.BackColor = Color.LightBlue; // 表示选中
                    break;
                default:
                    break;
            }
        }

        private void ResetButtonStyles()
        {
            // 设置所有按钮到默认背景颜色
            btnDrawAnnotation.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnDrawDistanceLine.BackColor = Color.FromKnownColor(KnownColor.Control);
            btnSelectGrayscale.BackColor = Color.FromKnownColor(KnownColor.Control);
        }
        #endregion

        private void NearestNeighborCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            imageProcessingControl.SetNearestNeighbor(NearestNeighborCheckBox.Checked);
        }

    }
}
