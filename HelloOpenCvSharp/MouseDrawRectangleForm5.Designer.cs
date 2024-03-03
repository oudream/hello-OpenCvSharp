namespace HelloOpenCvSharp
{
    partial class MouseDrawRectangleForm5
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
            this.NearestNeighborCheckBox = new System.Windows.Forms.CheckBox();
            this.btnSelectGrayscale = new System.Windows.Forms.Button();
            this.btnDrawDistanceLine = new System.Windows.Forms.Button();
            this.btnDrawAnnotation = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openImageButton = new System.Windows.Forms.Button();
            this.labelTextBox = new System.Windows.Forms.TextBox();
            this.saveAnnotationsButton = new System.Windows.Forms.Button();
            this.loadAnnotationsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // NearestNeighborCheckBox
            // 
            this.NearestNeighborCheckBox.AutoSize = true;
            this.NearestNeighborCheckBox.Location = new System.Drawing.Point(13, 396);
            this.NearestNeighborCheckBox.Name = "NearestNeighborCheckBox";
            this.NearestNeighborCheckBox.Size = new System.Drawing.Size(106, 22);
            this.NearestNeighborCheckBox.TabIndex = 10;
            this.NearestNeighborCheckBox.Text = "插值模式";
            this.NearestNeighborCheckBox.UseVisualStyleBackColor = true;
            // 
            // btnSelectGrayscale
            // 
            this.btnSelectGrayscale.Location = new System.Drawing.Point(12, 329);
            this.btnSelectGrayscale.Name = "btnSelectGrayscale";
            this.btnSelectGrayscale.Size = new System.Drawing.Size(107, 51);
            this.btnSelectGrayscale.TabIndex = 7;
            this.btnSelectGrayscale.Text = "测灰度";
            this.btnSelectGrayscale.UseVisualStyleBackColor = true;
            // 
            // btnDrawDistanceLine
            // 
            this.btnDrawDistanceLine.Location = new System.Drawing.Point(12, 272);
            this.btnDrawDistanceLine.Name = "btnDrawDistanceLine";
            this.btnDrawDistanceLine.Size = new System.Drawing.Size(107, 51);
            this.btnDrawDistanceLine.TabIndex = 8;
            this.btnDrawDistanceLine.Text = "测距离";
            this.btnDrawDistanceLine.UseVisualStyleBackColor = true;
            // 
            // btnDrawAnnotation
            // 
            this.btnDrawAnnotation.Location = new System.Drawing.Point(12, 215);
            this.btnDrawAnnotation.Name = "btnDrawAnnotation";
            this.btnDrawAnnotation.Size = new System.Drawing.Size(107, 51);
            this.btnDrawAnnotation.TabIndex = 9;
            this.btnDrawAnnotation.Text = "画标注";
            this.btnDrawAnnotation.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(309, 221);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(786, 458);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // openImageButton
            // 
            this.openImageButton.Location = new System.Drawing.Point(12, 12);
            this.openImageButton.Name = "openImageButton";
            this.openImageButton.Size = new System.Drawing.Size(107, 84);
            this.openImageButton.TabIndex = 6;
            this.openImageButton.Text = "打开图像";
            this.openImageButton.UseVisualStyleBackColor = true;
            // 
            // labelTextBox
            // 
            this.labelTextBox.Location = new System.Drawing.Point(13, 425);
            this.labelTextBox.Name = "labelTextBox";
            this.labelTextBox.Size = new System.Drawing.Size(106, 28);
            this.labelTextBox.TabIndex = 11;
            this.labelTextBox.Text = "solder ball";
            // 
            // saveAnnotationsButton
            // 
            this.saveAnnotationsButton.Location = new System.Drawing.Point(12, 491);
            this.saveAnnotationsButton.Name = "saveAnnotationsButton";
            this.saveAnnotationsButton.Size = new System.Drawing.Size(107, 63);
            this.saveAnnotationsButton.TabIndex = 6;
            this.saveAnnotationsButton.Text = "保存标注";
            this.saveAnnotationsButton.UseVisualStyleBackColor = true;
            // 
            // loadAnnotationsButton
            // 
            this.loadAnnotationsButton.Location = new System.Drawing.Point(12, 560);
            this.loadAnnotationsButton.Name = "loadAnnotationsButton";
            this.loadAnnotationsButton.Size = new System.Drawing.Size(107, 63);
            this.loadAnnotationsButton.TabIndex = 6;
            this.loadAnnotationsButton.Text = "加载标注";
            this.loadAnnotationsButton.UseVisualStyleBackColor = true;
            // 
            // MouseDrawRectangleForm5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 847);
            this.Controls.Add(this.labelTextBox);
            this.Controls.Add(this.NearestNeighborCheckBox);
            this.Controls.Add(this.btnSelectGrayscale);
            this.Controls.Add(this.btnDrawDistanceLine);
            this.Controls.Add(this.btnDrawAnnotation);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.loadAnnotationsButton);
            this.Controls.Add(this.saveAnnotationsButton);
            this.Controls.Add(this.openImageButton);
            this.Name = "MouseDrawRectangleForm5";
            this.Text = "MouseDrawRectangleForm5";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox NearestNeighborCheckBox;
        private System.Windows.Forms.Button btnSelectGrayscale;
        private System.Windows.Forms.Button btnDrawDistanceLine;
        private System.Windows.Forms.Button btnDrawAnnotation;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button openImageButton;
        private System.Windows.Forms.TextBox labelTextBox;
        private System.Windows.Forms.Button saveAnnotationsButton;
        private System.Windows.Forms.Button loadAnnotationsButton;
    }
}