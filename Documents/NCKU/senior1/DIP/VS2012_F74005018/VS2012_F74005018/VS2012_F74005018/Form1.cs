using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

using AviFile;

namespace VS2012_F74005018
{
    public partial class Form1 : Form
    {
        static VideoStream videoStream;
        AviManager avifile;
        Bitmap frameBuffer;
        Bitmap imageBuffer;
        Bitmap processedFrame;
        Bitmap processedFrameBuffer;
        Bitmap previousFrame;
        LockBitmap lockBitmapProcessedFrameBuffer;
        LockBitmap lockBitmapPreviousFrame;
        LockBitmap lockBitmapProcessedFrame;
        bool videoStatus = false;
        int face = 0;
        Thread t_playVideo;
        static int frameNum = 240;

        public struct Hsv
        {
            public double H;
            public double S;
            public double V;
        }

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            this.pictureBox_video.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox_processedVideo.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (t_playVideo != null && t_playVideo.IsAlive)
            {
                videoStatus = false;
                videoStream.GetFrameClose();
                avifile.Close();

                // Close thread
                t_playVideo.Suspend();
                t_playVideo.Resume();
                t_playVideo.Abort();
            }
        }

        private void button_loadVideo_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "./";
            openFileDialog1.Filter = "AVI files (*.avi)|*.avi|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Show path name
                    label_path.Text = openFileDialog1.FileName;

                    // open avi file stream
                    avifile = new AviManager(openFileDialog1.FileName, true);

                    if ((videoStream = avifile.GetVideoStream()) != null)
                    {
                        t_playVideo = new Thread(this.PlayVideo);
                        t_playVideo.Start();
                        videoStatus = true;
                        //process_video();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button_playPause_Click(object sender, EventArgs e)
        {
            if (t_playVideo != null)
            {
                if (videoStatus)
                {
                    t_playVideo.Suspend();
                    button_playPause.Text = "Play";
                }
                else
                {
                    t_playVideo.Resume();
                    button_playPause.Text = "Pause";
                }
                videoStatus = !videoStatus;
            }
        }

        private void ProcessImage()
        {
            processedFrame = new Bitmap(imageBuffer);
            lockBitmapProcessedFrame = new LockBitmap(processedFrame);

            //ImageSubtraction();
            SkinFilter();
            //Dilation(3);
            VerticalProjection();

            PlayProcessedVideo();
        }
        
        private void ImageSubtraction()
        {
            if (frameNum == 0)
            {
                // Save first frame
                previousFrame = new Bitmap(imageBuffer);
                lockBitmapPreviousFrame = new LockBitmap(previousFrame);
                processedFrame = null;
                return;
            }
            else
            {
                Parallel.For(0, lockBitmapProcessedFrame.Height, h =>
                {
                    for (int w = 0; w < lockBitmapProcessedFrame.Width * lockBitmapProcessedFrame.Step; w += 4)
                    {
                        lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w]
                            = (byte)Math.Abs(lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w] - lockBitmapPreviousFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w]);
                        lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1]
                            = (byte)Math.Abs(lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1] - lockBitmapPreviousFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1]);
                        lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2]
                            = (byte)Math.Abs(lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2] - lockBitmapPreviousFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2]);
                    }
                });
                previousFrame = new Bitmap(imageBuffer);
                lockBitmapPreviousFrame = new LockBitmap(previousFrame);
            }
        }

        private void SkinFilter()
        {
            Parallel.For(0, lockBitmapProcessedFrame.Height, h =>
            {
                Hsv hsv;
                for (short w = 0; w < lockBitmapProcessedFrame.Width * 4; w += 4)
                {
                    // Convert pixel from RGB to HSV
                    hsv = Cvt2HSV(lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w],
                            lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1],
                            lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2]);

                    if ((hsv.H > 7 && hsv.H < 38) && (hsv.S >= 0.15 && hsv.S <= 0.80))
                    {
                        lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w]
                            = lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1]
                            = lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2]
                            = 255;
                    }
                    else
                    {
                        lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w]
                            = lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1]
                            = lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2]
                            = 0;
                    }
                }
            });
        }

        // Update picturebox every 33 milliseconds. 
        private void PlayVideo()
        {
            videoStream.GetFrameOpen();
            while (frameNum < videoStream.CountFrames)
            {
                frameBuffer = videoStream.GetBitmap(frameNum);

                // Deep copy frame to process 
                imageBuffer = new Bitmap(frameBuffer);
                
                ProcessImage();

                this.pictureBox_video.Image = frameBuffer;

                ++frameNum;
                System.Threading.Thread.Sleep(33);
            }

            videoStream.GetFrameClose();
            avifile.Close();
            videoStatus = false;

            this.pictureBox_video.Image = null;
        }

        // Update picturebox after processing. 
        private void PlayProcessedVideo()
        {
            lockBitmapProcessedFrame.UnlockBits();
            processedFrameBuffer = processedFrame;
            pictureBox_processedVideo.Image = processedFrameBuffer;
        }

        public Hsv Cvt2HSV(byte B, byte G, byte R)
        {
            double r, g, b, min, max, Cmax, Cmin, delta;
            Hsv temp;

            b = (double)B / 255;
            g = (double)G / 255;
            r = (double)R / 255;

            Cmax = Math.Max(Math.Max(r, g), b);
            Cmin = Math.Min(Math.Min(r, g), b);
            delta = Cmax - Cmin;

            temp.H = (Cmax == r) ? 60 * ((g - b) / delta) :
                    ((Cmax == g) ? 60 * ((b - r) / delta + 2) :
                        60 * ((r - g) / delta + 4));
            temp.S = (Cmax == 0) ? 0 : delta / Cmax;
            temp.V = Cmax;

            //if (temp.H < 0) temp.H += 360;
            return temp;
        }

        int[] bucket;
        public void VerticalProjection()
        {
            bucket = new int[lockBitmapProcessedFrame.Width];
            Array.Clear(bucket, 0, bucket.Length);

            Parallel.For(0, lockBitmapProcessedFrame.Height, h =>
            {
                for (int w = 0; w < lockBitmapProcessedFrame.Width * lockBitmapProcessedFrame.Step; w += 4)
                {
                    if (lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w] == 255)
                    {
                        ++bucket[w/4];
                    }
                }
            });
            /*
            Console.Write("bucket:\n");
            for (int w = 0; w < lockBitmapProcessedFrame.Width; ++w)
            {
                Console.Write(bucket[w] + " ");
            }
            Console.Write("\n");
            */
            DetectFace();
        }

        public void DetectFace()
        {
            const int continuous = 30;
            double prob = 0.8;
            int recordContinuity = 0;
            int recordDiscontinuity = 0;
            for (int w = 0; w < lockBitmapProcessedFrame.Width; ++w)
            {
                if (bucket[w] > 120)
                {
                    ++recordContinuity;
                }
                else
                {
                    ++recordDiscontinuity;
                }
                if (recordDiscontinuity > 2 * recordContinuity)
                {
                    recordContinuity = 0;
                    recordDiscontinuity = 0;
                }
                if (recordContinuity >= continuous)
                {
                    ++face;
                    Console.WriteLine("Face=" + face);
                    recordContinuity = 0;
                    recordDiscontinuity = 0;
                }
            }
        }

        // n*n window
        public void Dilation(int kernelSize)
        {
            int ds = (kernelSize - 1) / 2;
            Parallel.For(0, lockBitmapProcessedFrame.Height, h =>
            {
                for (int w = 0; w < lockBitmapProcessedFrame.Width * 4; w += 4)
                {
                    if (lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w] == 255)
                    {
                        continue;
                    }
                    for (int i=0; i < kernelSize; ++i) {
                        for (int j=0; j<kernelSize; ++j) {
                            // Boundary
                            if (h + i - ds < 0 || w + j - ds < 0 || (h + i - ds) >= lockBitmapProcessedFrame.Height || (w + j - ds) >= lockBitmapProcessedFrame.Width) continue;
                            if (lockBitmapProcessedFrame.Pixels[(h + i - ds)* lockBitmapProcessedFrame.Width * 4 + (w + j - ds)] == 255) {
                                lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w]
                                    = lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 1]
                                    = lockBitmapProcessedFrame.Pixels[h * lockBitmapProcessedFrame.Width * 4 + w + 2]
                                    = 255;
                            }
                        }
                    }
                }
            });
        }
    }
}
