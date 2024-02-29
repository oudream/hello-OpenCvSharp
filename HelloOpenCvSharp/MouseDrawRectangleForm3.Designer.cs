namespace HelloOpenCvSharp
{
    partial class MouseDrawRectangleForm3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openImageButton = new System.Windows.Forms.Button();
            this.btnDrawAnnotation = new System.Windows.Forms.Button();
            this.btnDrawDistanceLine = new System.Windows.Forms.Button();
            this.btnSelectGrayscale = new System.Windows.Forms.Button();
            this.NearestNeighborCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(250, 179);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(786, 458);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // openImageButton
            // 
            this.openImageButton.Location = new System.Drawing.Point(12, 12);
            this.openImageButton.Name = "openImageButton";
            this.openImageButton.Size = new System.Drawing.Size(107, 84);
            this.openImageButton.TabIndex = 2;
            this.openImageButton.Text = "打开图像";
            this.openImageButton.UseVisualStyleBackColor = true;
            // 
            // btnDrawAnnotation
            // 
            this.btnDrawAnnotation.Location = new System.Drawing.Point(12, 215);
            this.btnDrawAnnotation.Name = "btnDrawAnnotation";
            this.btnDrawAnnotation.Size = new System.Drawing.Size(107, 51);
            this.btnDrawAnnotation.TabIndex = 3;
            this.btnDrawAnnotation.Text = "画标注";
            this.btnDrawAnnotation.UseVisualStyleBackColor = true;
            // 
            // btnDrawDistanceLine
            // 
            this.btnDrawDistanceLine.Location = new System.Drawing.Point(12, 272);
            this.btnDrawDistanceLine.Name = "btnDrawDistanceLine";
            this.btnDrawDistanceLine.Size = new System.Drawing.Size(107, 51);
            this.btnDrawDistanceLine.TabIndex = 3;
            this.btnDrawDistanceLine.Text = "测距离";
            this.btnDrawDistanceLine.UseVisualStyleBackColor = true;
            // 
            // btnSelectGrayscale
            // 
            this.btnSelectGrayscale.Location = new System.Drawing.Point(12, 329);
            this.btnSelectGrayscale.Name = "btnSelectGrayscale";
            this.btnSelectGrayscale.Size = new System.Drawing.Size(107, 51);
            this.btnSelectGrayscale.TabIndex = 3;
            this.btnSelectGrayscale.Text = "测灰度";
            this.btnSelectGrayscale.UseVisualStyleBackColor = true;
            // 
            // NearestNeighborCheckBox
            // 
            this.NearestNeighborCheckBox.AutoSize = true;
            this.NearestNeighborCheckBox.ForeColor = System.Drawing.Color.White;
            this.NearestNeighborCheckBox.Location = new System.Drawing.Point(13, 396);
            this.NearestNeighborCheckBox.Name = "NearestNeighborCheckBox";
            this.NearestNeighborCheckBox.Size = new System.Drawing.Size(106, 22);
            this.NearestNeighborCheckBox.TabIndex = 4;
            this.NearestNeighborCheckBox.Text = "插值模式";
            this.NearestNeighborCheckBox.UseVisualStyleBackColor = true;
            // 
            // MouseDrawRectangleForm3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(1317, 885);
            this.Controls.Add(this.NearestNeighborCheckBox);
            this.Controls.Add(this.btnSelectGrayscale);
            this.Controls.Add(this.btnDrawDistanceLine);
            this.Controls.Add(this.btnDrawAnnotation);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.openImageButton);
            this.Name = "MouseDrawRectangleForm3";
            this.Text = "MouseDrawRectangleForm3";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button openImageButton;
        private System.Windows.Forms.Button btnDrawAnnotation;
        private System.Windows.Forms.Button btnDrawDistanceLine;
        private System.Windows.Forms.Button btnSelectGrayscale;
        private System.Windows.Forms.CheckBox NearestNeighborCheckBox;
    }
}