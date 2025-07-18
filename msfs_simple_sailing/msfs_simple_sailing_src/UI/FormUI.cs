using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reactive.Joins;
using System.Windows.Forms;
using msfs_simple_sail_core.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace msfs_simple_sail_core.UI
{
    public partial class FormUI : Form
    {
        private readonly Controller controller;
        private TimeSpan StopwatchOffset = TimeSpan.Zero;
        System.Diagnostics.Stopwatch stopwatch;
        private bool alwaysontop = true;
        private string log = "";

        private double Speed = 0.0;
        private double WindDir = 0.0;
        public double WindSpeed = 0.0;
        private double boomAngle = 0.0;
        private double jibAngle = 0.0;
        private double mainDraftPerc;
        private double jibDraftPerc;
        private bool IsSailSet = false;


        private static readonly Color myDarkControl = Color.FromArgb(0x20, 0x20, 0x20);

        private TimeSpan redrawTimeout = TimeSpan.FromMilliseconds(100);
        private DateTime lastDrawn = DateTime.MinValue;
        private int portJibTrim;
        private int mainTrim;
        private int starJibTrim;

        public FormUI(Controller controller)
        {

            this.controller = controller;
            InitializeComponent();
            var config = Config.GetInstance();

            this.Text = "MSFS Simple Sailing " + VersionHelper.GetVersion();

#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (Application.IsDarkModeEnabled)
            {
                // init components manually
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CA1416 // Validate platform compatibility


            if (config.transparencyPercent > 0)
            {
                // Clamp between 0 and 100
                int percent = Math.Max(0, Math.Min(90, config.transparencyPercent));

                // Convert percent to a value between 0.0 (fully transparent) and 1.0 (fully opaque)
                this.Opacity = 1 - percent / 100.0;
            }

            alwaysontop = config.alwaysOnTop;
            this.checkBoxAlwaysOnTop.Checked = alwaysontop;

            timer1.Start();

        }

        private void buttonToggleSail_Click(object sender, EventArgs e)
        {
            this.IsSailSet = !IsSailSet;
            controller.setSail(IsSailSet);
        }

        public void IsSailUp(bool isSailUp)
        {
            this.IsSailSet = isSailUp;
        }

        public void setLog(string log)
        {
            this.log = log;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // just to check it periodically
            if (alwaysontop && !this.TopMost) this.TopMost = alwaysontop;
            this.textBoxLog.Text = this.log;
            this.numericUpDownPortJib.Value = this.portJibTrim;
            this.numericUpDownStarJib.Value = this.starJibTrim;
            this.numericUpDownMainSheet.Value = this.mainTrim;
        }

        public void SetSpeed(double speed)
        {
            this.Speed = speed;
        }

        public void SetWind(double windSpeed, double windDir)
        {
            if ((DateTime.Now - lastDrawn) < redrawTimeout)
            {
                return;
            }
            lastDrawn = DateTime.Now;
            this.WindSpeed = windSpeed;
            this.WindDir = windDir;
            panelRose.Invalidate(); // Redraw the panel
        }

        private void panelRose_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = SystemColors.Control;
            Color foreColor = SystemColors.ControlText;
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (Application.IsDarkModeEnabled)
            {
                backColor = myDarkControl;
                foreColor = Color.White;
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CA1416 // Validate platform compatibility

            Graphics g = e.Graphics;
            // g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // g.Clear(backColor);
            // g.DrawImage(panelRose.BackgroundImage, new Rectangle(0, 0, 512, 512));

            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 3); // width, height
            Pen arrowPen = new Pen(Color.Blue, 5)
            {
                CustomEndCap = bigArrow
            };

            // Convert to canvas angle (0° = up, rotate clockwise)
            double windDirTo = (WindDir + 180) % 360;
            double angleRad = (windDirTo - 90) * Math.PI / 180.0;

            float centerX = 256;
            float centerY = 256;
            float length = 256;

            // Calculate start and end points so arrow pivots around center
            float dx = (float)(Math.Cos(angleRad) * length * 0.928);
            float dy = (float)(Math.Sin(angleRad) * length * 0.928);
            float dx2 = (float)(Math.Cos(angleRad) * length * 0.73);
            float dy2 = (float)(Math.Sin(angleRad) * length * 0.73);

            PointF start = new PointF(centerX - dx, centerY - dy); // tail
            PointF end = new PointF(centerX - dx2, centerY - dy2); // arrowhead

            g.DrawLine(arrowPen, start, end);

            using Font drawFontW = new Font("Arial", 16, FontStyle.Bold);
            using Brush drawBrushW = new SolidBrush(Color.Blue);
            g.DrawString($"{this.WindSpeed,4:0.0} knots", drawFontW, drawBrushW, 400, 0);

            using Font drawFontS = new Font("Arial", 16, FontStyle.Bold);
            using Brush drawBrushS = new SolidBrush(foreColor);
            g.DrawString($"{this.Speed,4:0.0} knots", drawFontS, drawBrushS, 0, 0);

            // this.boomAngle = -45; // 45° down right
            Pen boomPen = new Pen(Color.DarkGray, 5);
            float centerBoomX = 259;
            float centerBoomY = 223;
            float centerJibX = 259;
            float centerJibY = 120;
            double boomAngleRad = IsSailSet ? ((-90 - boomAngle) * Math.PI / 180.0) : -1.5707963267948966; // adjust for canvas orientation
            float boomLength = 120f; // or any length you like

            float boomDx = (float)(Math.Cos(boomAngleRad) * boomLength);
            float boomDy = (float)(Math.Sin(boomAngleRad) * boomLength);

            PointF boomEnd = new PointF(centerBoomX - boomDx, centerBoomY - boomDy); // Y flipped
            g.DrawLine(boomPen, new PointF(centerBoomX, centerBoomY), boomEnd);

            if (IsSailSet)
            {
                using Pen sailPen = new Pen(Color.LightGray, 3);

                // Boom start and end
                PointF boomStart = new PointF(centerBoomX, centerBoomY);

                // Midpoint between start and end
                PointF boomMid = new PointF(
                    (boomStart.X + boomEnd.X) / 2,
                    (boomStart.Y + boomEnd.Y) / 2
                );

                // Perpendicular (normal) vector for curvature
                float boomVecX = boomEnd.X - boomStart.X;
                float boomVecY = boomEnd.Y - boomStart.Y;
                float nx = -boomVecY / boomLength;
                float ny = boomVecX / boomLength;

                // Control points offset from midpoint
                float curvature = (float)mainDraftPerc * 15f; // Change for deeper sails
                curvature = boomAngle > 0 ? -curvature : curvature;
                PointF control1 = new PointF(
                    boomStart.X + boomVecX * 0.33f + nx * curvature,
                    boomStart.Y + boomVecY * 0.33f + ny * curvature
                );
                PointF control2 = new PointF(
                    boomStart.X + boomVecX * 0.66f + nx * curvature,
                    boomStart.Y + boomVecY * 0.66f + ny * curvature
                );

                g.DrawBezier(sailPen, boomStart, control1, control2, boomEnd);


                PointF jibStart = new PointF(centerJibX, centerJibY);
                float jibLength = 90f;
                double jibAngleRad = (-90 - jibAngle) * Math.PI / 180.0; // Convert to canvas angle

                // Compute end point of jib
                float jibDx = (float)(Math.Cos(jibAngleRad) * jibLength);
                float jibDy = (float)(Math.Sin(jibAngleRad) * jibLength);
                PointF jibEnd = new PointF(centerJibX - jibDx, centerJibY - jibDy); // Y flipped

                // Midpoint
                PointF jibMid = new PointF(
                    (centerJibX + jibEnd.X) / 2,
                    (centerJibY + jibEnd.Y) / 2
                );

                // Direction vector
                float jibVecX = jibEnd.X - centerJibX;
                float jibVecY = jibEnd.Y - centerJibY;
                float jibVecLen = (float)Math.Sqrt(jibVecX * jibVecX + jibVecY * jibVecY);

                // Normal vector
                float jnx = -jibVecY / jibVecLen;
                float jny = jibVecX / jibVecLen;

                // Adjust curvature based on angle (deeper with larger angle)
                float jibCurvature = (float)jibDraftPerc * 10f;
                jibCurvature = jibAngle > 0 ? -jibCurvature : jibCurvature; // Flip bulge direction for starboard

                // Control points
                PointF jibControl1 = new PointF(
                    centerJibX + jibVecX * 0.33f + jnx * jibCurvature,
                    centerJibY + jibVecY * 0.33f + jny * jibCurvature
                );
                PointF jibControl2 = new PointF(
                    centerJibX + jibVecX * 0.66f + jnx * jibCurvature,
                    centerJibY + jibVecY * 0.66f + jny * jibCurvature
                );

                // Draw jib sail
                g.DrawBezier(sailPen, new PointF(centerJibX, centerJibY), jibControl1, jibControl2, jibEnd);
            }

        }

        internal void SetBoomDeflection(double boomDeflectionDeg = 0, double jibDeflectionDeg = 0, double mainDraft = 0, double jibDraft = 0)
        {
            this.boomAngle = boomDeflectionDeg;
            this.jibAngle = jibDeflectionDeg;
            this.mainDraftPerc = mainDraft;
            this.jibDraftPerc = jibDraft;
        }

        internal void SetTrims(int port = 0, int main = 0, int star = 0)
        {
            this.portJibTrim = port;
            this.mainTrim = main;
            this.starJibTrim = star;
        }

        private void textBoxLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.alwaysontop = checkBoxAlwaysOnTop.Checked;
            this.TopMost = this.alwaysontop;
            var config = Config.GetInstance();
            config.alwaysOnTop = this.alwaysontop;
            config.Save();
        }

        private void labelTrans_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDownTrans_ValueChanged(object sender, EventArgs e)
        {
            this.Opacity = (double)(1 - numericUpDownTrans.Value / 100);
        }

        private void numericUpDownPortJib_ValueChanged(object sender, EventArgs e)
        {
            controller.SetJibSheetPort((int) this.numericUpDownPortJib.Value);
        }

        private void numericUpDownMainSheet_ValueChanged(object sender, EventArgs e)
        {
            controller.SetMainSheet((int)this.numericUpDownMainSheet.Value);
        }

        private void numericUpDownStarJib_ValueChanged(object sender, EventArgs e)
        {
            controller.SetJibSheetStar((int)this.numericUpDownStarJib.Value);
        }
    }
}
