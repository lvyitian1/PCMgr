﻿using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PCMgr.Ctls
{
    public class PerformanceTitle : Control
    {
        public PerformanceTitle()
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            TitleFont = new Font("微软雅黑", 18);
            SmallTitleFont = new Font("微软雅黑", 12);
            stringFormatRight = new StringFormat();
            stringFormatRight.Alignment = StringAlignment.Far;
            stringFormatRight.LineAlignment = StringAlignment.Far;
        }

        public Font TitleFont { get; set; }
        public Font SmallTitleFont { get; set; }
        [Localizable(true)]
        public string Title { get; set; }
        [Localizable(true)]
        public string SmallTitle { get; set; }

        private StringFormat stringFormatRight = null;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            using (SolidBrush s = new SolidBrush(ForeColor))
            {
                if (Title != "") g.DrawString(Title, TitleFont, s, 0, 0);
                if (SmallTitle != "") g.DrawString(SmallTitle, SmallTitleFont, s, new Rectangle(0, 0, Width, Height), stringFormatRight);
            }
        }
    }
}
