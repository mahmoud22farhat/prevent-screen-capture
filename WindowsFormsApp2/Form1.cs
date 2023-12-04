using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;
namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private Timer checkCaptureTimer;
        private GaussianBlur blurFilter;

        public Form1()
        {
            InitializeComponent();
            //لفحص ادوات التقاط الصور
            InitializeCapturePrevention();
            //لتشويش الصور بعد التقاطها 
            InitializeBlurFilter();
        }



        //لفحص ادوات التقاط الصور
        private void InitializeCapturePrevention()
        {
            // تهيئة مؤقت لفحص التقاط الشاشة بشكل دوري
            checkCaptureTimer = new Timer();
            checkCaptureTimer.Interval = 1000; // قم بتعيين فاصل زمني مناسب حسب احتياجاتك
            checkCaptureTimer.Tick += CheckCaptureTimer_Tick;
            checkCaptureTimer.Start();
        }
        //لتشويش الصور بعد التقاطها 
        private void InitializeBlurFilter()
        {
            blurFilter = new GaussianBlur();
            blurFilter.Sigma = 5; // يمكنك تعيين قيمة Sigma بحسب الحاجة
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CheckCaptureTimer_Tick(object sender, EventArgs e)
        {
            // افحص إذا كان هناك تقاط شاشة باستخدام حجب النوافذ
            if (IsCaptureWindowActive())
            {
                //this.Visible = false;
                //// يمكنك اتخاذ إجراءات هنا، مثل إخفاء المحتوى أو غلق التطبيق

                MessageBox.Show("لقطات الشاشه ممنوعه", "تحذير",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                //تشويش الصوره
                   ApplyBlurEffect();
                ////لغلق المشروع عند التقاط الشاشة
              //  Application.Exit();
            }
            else
            {
                //ShowContentAfterCapture();
                RemoveBlurEffect();
            }
        }
        private bool IsCaptureWindowActive()
        {
            // افحص إذا كانت نافذة التقاط الشاشة في الوضع النشط
            IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();

            // يمكنك تحديد نوافذ التقاط الشاشة الشهيرة
            if (foregroundWindow != IntPtr.Zero)
            {
                string windowTitle = NativeMethods.GetWindowTitle(foregroundWindow);

                // تحقق من أن القيمة المسترجعة من GetWindowTitle غير مهيأة (null)
                if (!string.IsNullOrEmpty(windowTitle))
                {
                    // قم بتحديد العناوين التي قد تكون لبرامج أخرى للتقاط الشاشة
                    string[] captureTitles = { "Snipping Tool", "Snip & Sketch", "Greenshot", "LightShot" , "Open Broadcaster Software Studio" };

                    foreach (string captureTitle in captureTitles)
                    {
                        // تجاهل حالة الأحرف باستخدام StringComparison.OrdinalIgnoreCase
                        if (windowTitle.IndexOf(captureTitle, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // دوال من مكتبة المستخدم الأساسية
        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

            public static string GetWindowTitle(IntPtr hWnd)
            {
                const int nChars = 256;
                System.Text.StringBuilder Buff = new System.Text.StringBuilder(nChars);
                if (GetWindowText(hWnd, Buff, nChars) > 0)
                {
                    return Buff.ToString();
                }
                return null;
            }
        }

        ////اخفاء المحتوي/////////////////////////////////////////////////////////


        private void ShowContentAfterCapture()
        {
            // إظهار المحتوى بعد انتهاء محاولة التقاط الشاشة
            this.Visible = true;
            // إظهار أي عناصر أخرى على الشاشة
            // مثال: label1.Visible = true;
            //         pictureBox1.Visible = true;
        }
        //لتشويش الصوره///////////////////////////////////////////////////////

        private void ApplyBlurEffect()
        {
            // قم بتشويش الشاشة باستخدام فلتر الضباب
            Bitmap originalImage = CaptureScreen();
            Bitmap blurredImage = blurFilter.Apply(originalImage);

            // عرض الصورة المشوشة على الفورم
            this.BackgroundImage = blurredImage;
        }

        private void RemoveBlurEffect()
        {
            // استعادة الشاشة الأصلية
            this.BackgroundImage = null;
        }

        private Bitmap CaptureScreen()
        {
            // قم بالتقاط الشاشة الحالية وإرجاعها كصورة Bitmap
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }
            return bitmap;
        }

       


       

       
    }
}
    

