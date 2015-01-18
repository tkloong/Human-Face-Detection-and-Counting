namespace VS2012_F74005018
{
    partial class Form1
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
            this.button_loadVideo = new System.Windows.Forms.Button();
            this.pictureBox_video = new System.Windows.Forms.PictureBox();
            this.pictureBox_processedVideo = new System.Windows.Forms.PictureBox();
            this.label_path = new System.Windows.Forms.Label();
            this.button_playPause = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_video)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_processedVideo)).BeginInit();
            this.SuspendLayout();
            // 
            // button_loadVideo
            // 
            this.button_loadVideo.Location = new System.Drawing.Point(12, 12);
            this.button_loadVideo.Name = "button_loadVideo";
            this.button_loadVideo.Size = new System.Drawing.Size(75, 23);
            this.button_loadVideo.TabIndex = 0;
            this.button_loadVideo.Text = "Load Video";
            this.button_loadVideo.UseVisualStyleBackColor = true;
            this.button_loadVideo.Click += new System.EventHandler(this.button_loadVideo_Click);
            // 
            // pictureBox_video
            // 
            this.pictureBox_video.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox_video.Location = new System.Drawing.Point(12, 41);
            this.pictureBox_video.Name = "pictureBox_video";
            this.pictureBox_video.Size = new System.Drawing.Size(476, 352);
            this.pictureBox_video.TabIndex = 1;
            this.pictureBox_video.TabStop = false;
            // 
            // pictureBox_processedVideo
            // 
            this.pictureBox_processedVideo.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox_processedVideo.Location = new System.Drawing.Point(494, 41);
            this.pictureBox_processedVideo.Name = "pictureBox_processedVideo";
            this.pictureBox_processedVideo.Size = new System.Drawing.Size(476, 352);
            this.pictureBox_processedVideo.TabIndex = 2;
            this.pictureBox_processedVideo.TabStop = false;
            // 
            // label_path
            // 
            this.label_path.AutoSize = true;
            this.label_path.Location = new System.Drawing.Point(93, 18);
            this.label_path.Name = "label_path";
            this.label_path.Size = new System.Drawing.Size(0, 13);
            this.label_path.TabIndex = 3;
            // 
            // button_playPause
            // 
            this.button_playPause.Location = new System.Drawing.Point(184, 399);
            this.button_playPause.Name = "button_playPause";
            this.button_playPause.Size = new System.Drawing.Size(75, 23);
            this.button_playPause.TabIndex = 4;
            this.button_playPause.Text = "Pause";
            this.button_playPause.UseVisualStyleBackColor = true;
            this.button_playPause.Click += new System.EventHandler(this.button_playPause_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 428);
            this.Controls.Add(this.button_playPause);
            this.Controls.Add(this.label_path);
            this.Controls.Add(this.pictureBox_processedVideo);
            this.Controls.Add(this.pictureBox_video);
            this.Controls.Add(this.button_loadVideo);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_video)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_processedVideo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_loadVideo;
        private System.Windows.Forms.PictureBox pictureBox_video;
        private System.Windows.Forms.PictureBox pictureBox_processedVideo;
        private System.Windows.Forms.Label label_path;
        private System.Windows.Forms.Button button_playPause;
    }
}

