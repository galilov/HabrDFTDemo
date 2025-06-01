using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using System.Drawing.Imaging;

// https://blog.dotnetframework.org/2021/07/01/generate-video-from-collection-of-images-in-c-ffmpeg/
namespace FourierHabr
{
    public partial class Form1 : Form
    {
        private AbstractRecorder _chartRecorder = new ChartRecorder(10, 1f, 5, 5, 470, 450);
        //new PolarGraph(10, 6f, 485, 5, 450)
        // new CircleRecorder(10, 1f, 485, 5, 450);
        private AbstractRecorder _circleRecorder = new PolarGraph(10, 0f, 485, 5, 450);
        private int _angle = 0;
        private VideoEncoderSettings _videoSettings;
        private MediaOutput? _mediaBuilder;
        private Bitmap? _bitmap;
        private Image _logo;
        private int _frameNo = 1;

        public Form1()
        {
            InitializeComponent();
            FFmpegLoader.FFmpegPath = @"C:\ffmpeg-7.1.1-full_build-shared\bin\";
            _logo = Image.FromFile(@"C:\Videos\logo.png");
            var w = 960; var h = 544;
            box.Width = w;
            box.Height = h;
            _videoSettings = new VideoEncoderSettings(width: w, height: h, framerate: 30, codec: VideoCodec.H264);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //GlobalFFOptions.Configure(options => options.BinaryFolder = "./bin");
        }

        private void box_Paint(object sender, PaintEventArgs e)
        {
            if (!timer1.Enabled) return;
            Graphics g = e.Graphics;
            var w = Math.PI * _angle / 180.0;
            string formula = "y=4+6*sin(pi/6*t)";
            var arg =
                //2 * Math.Sin(w + Math.PI / 3) +
                //4 * Math.Cos(3 * w) +
                4 + 6 * Math.Sin(w);// 0.2 * Math.Sin(rads * 5) + 0.3 * Math.Cos(rads * 6) + 0.5 * Math.Sin(rads * 7);
            _chartRecorder.RegisterValue(arg);
            _circleRecorder.RegisterValue(arg);
            using (Graphics gimg = Graphics.FromImage(_bitmap))
            {
                gimg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gimg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gimg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gimg.FillRectangle(Brushes.Silver, new RectangleF(0, 0, _bitmap.Width, _bitmap.Height));
                _chartRecorder.Render(gimg);
                _circleRecorder.Render(gimg);
                gimg.DrawImage(_logo, 8, _bitmap.Height - 72, 64, 64);
                string s = string.Format("Кадр: {0:d4}\nФормула: {1}", _frameNo, formula);
                var sz = gimg.MeasureString(s, _chartRecorder._font);
                var pos = new PointF(96, _bitmap.Height - 68);
                gimg.FillRectangle(Brushes.White, new RectangleF(pos, sz));
                gimg.DrawString(s, _chartRecorder._font, Brushes.Black, pos);
            }
            g.DrawImageUnscaled(_bitmap, 0, 0);
            var bitLock = _bitmap.LockBits(new Rectangle(Point.Empty, _bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, _bitmap.Size);
            _mediaBuilder.Video.AddFrame(bitmapData); // Encode the frame
            _bitmap.UnlockBits(bitLock);
            _chartRecorder.NextStep();
            _circleRecorder.NextStep();
            _frameNo++;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _angle++;
            box.Invalidate();
            box.Update();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                _videoSettings.EncoderPreset = EncoderPreset.Medium;
                _videoSettings.CRF = 17;
                _mediaBuilder = MediaBuilder.CreateContainer(@"C:\Videos\out.mp4").WithVideo(_videoSettings).Create();
                _bitmap = new Bitmap(box.Width, box.Height);
                timer1.Tick += timer1_Tick;
                timer1.Start();
            }
            else
            {
                timer1.Stop();
                timer1.Tick -= timer1_Tick;
                _mediaBuilder.Dispose();
            }
        }
    }
}
