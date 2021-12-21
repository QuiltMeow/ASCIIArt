using ASCIIArt.Properties;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ASCIIArt
{
    public partial class MainForm : Form
    {
        private bool pause;
        private Timer timer;
        private VideoCapture videoCapture;

        public MainForm()
        {
            InitializeComponent();
            HtmlDocument document = wbOutput.Document;
            document.Write(string.Empty);
        }

        private string content;

        private void btnConvert_Click(object sender, EventArgs e)
        {
            loadData();
        }

        private void preLoadImage()
        {
            Bitmap bmp = Resources.Meow;
            processImage(bmp, tbResize.Value, true);
        }

        private void loadData()
        {
            string path = txtPath.Text;
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("請指定輸入檔案", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int resize = tbResize.Value;

            try
            {
                if (ASCIIArt.isImageExtension(path))
                {
                    labelStatus.Text = "單一圖片";
                    Bitmap bmp = new Bitmap(path);
                    processImage(bmp, resize);
                }
                else
                {
                    startAsciiArtTimer(path, resize);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取檔案時發生錯誤 : " + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updateContentUI(bool footer = false)
        {
            StringBuilder output = new StringBuilder("<pre><font size=\"0\">");
            output.Append(content);
            if (footer)
            {
                output.Append("Quilt ASCII Art Generator");
            }
            output.Append("</font></pre>");
            wbOutput.DocumentText = output.ToString();
        }

        private static int getFPSDelay(double fps)
        {
            int ret = (int)(1000 / fps);
            if (ret <= 0)
            {
                ret = 1;
            }
            return ret;
        }

        private void startAsciiArtTimer(string path, int resize)
        {
            videoCapture = new VideoCapture(path);
            double fps = videoCapture.GetCaptureProperty(CapProp.Fps);
            if (fps <= 0)
            {
                MessageBox.Show("輸入檔案格式錯誤", "讀取失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            updateButtonUI(true);
            labelStatus.Text = "影片 FPS : " + (int)fps;

            pause = false;
            updatePlayPauseUI();

            timer = new Timer()
            {
                Interval = getFPSDelay(fps)
            };
            timer.Tick += (sender, e) => asciiArtHandler(resize);
            timer.Start();
        }

        private void updateButtonUI(bool start)
        {
            btnBrowse.Enabled = tbResize.Enabled = btnConvert.Enabled = !start;
            btnPlayPause.Enabled = btnStop.Enabled = btnStep.Enabled = start;
        }

        private void asciiArtHandler(int resize)
        {
            if (pause)
            {
                return;
            }
            processVideo(resize);
        }

        private void processVideo(int resize)
        {
            try
            {
                Mat mat = videoCapture.QueryFrame();
                Bitmap image = mat.Bitmap;
                if (image == null)
                {
                    stopVideo();
                    return;
                }
                processImage(image, resize);
            }
            catch
            {
                stopVideo();
            }
        }

        private void processImage(Bitmap bmp, int resize, bool footer = false)
        {
            bmp = ASCIIArt.resizeImage(bmp, resize);
            content = ASCIIArt.convertASCII(bmp);
            updateContentUI(footer);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "請選擇輸入檔案"
            };
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            txtPath.Text = ofd.FileName;
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            string save = (string)content.Clone();
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "文字檔案 (*.txt)|.txt|網頁檔案 (*.html)|.html",
                Title = "請選擇儲存位置"
            };
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (sfd.FilterIndex == 1)
            {
                save = save.Replace("&nbsp;", " ").Replace("<br />", "\r\n");
            }
            else
            {
                save = "<pre>" + save + "</pre>";
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.Write(save);
                }
                MessageBox.Show("檔案儲存完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("檔案儲存時發生錯誤 : " + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            Environment.Exit(Environment.ExitCode);
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            pause = !pause;
            updatePlayPauseUI();
        }

        private void updatePlayPauseUI()
        {
            btnPlayPause.Text = pause ? "播放" : "暫停";
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            pause = true;
            updatePlayPauseUI();
            processVideo(tbResize.Value);
        }

        private void stopVideo()
        {
            timer.Stop();
            updateButtonUI(false);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopVideo();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbResize_Scroll(object sender, EventArgs e)
        {
            labelResizeValue.Text = tbResize.Value.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            preLoadImage();
        }
    }
}