﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using PCMgr.Lanuages;
using static PCMgr.NativeMethods;

namespace PCMgr.Ctls
{
    public partial class PerformancePageCpu : UserControl, IPerformancePage
    {
        public PerformancePageCpu()
        {
            InitializeComponent();
        }

        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetCpuL1Cache();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetCpuL2Cache();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetCpuL3Cache();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetCpuPackage();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetCpuNodeCount();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool MPERF_GetCpuInfos();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool MPERF_UpdatePerformance();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool MPERF_GetCpuName(StringBuilder buf, int size);
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int MPERF_GetCpuFrequency();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int MPERF_GetProcessNumber();

        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetThreadCount();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetHandleCount();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint MPERF_GetProcessCount();
        [DllImport(COREDLLNAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int64 MPERF_GetRunTime();

        private TimeSpan times;
        private int cpuCount = 0;
        private int cpuUseage = 0;
        private string maxSpeed = "";

        public bool PageIsActive { get; set; }
        public void PageFroceSetData(int s)
        {
            cpuUseage = s;
            performanceGridGlobal.AddData(s);
        }
        public void PageHide()
        {
            Visible = false;
        }
        public void PageShow()
        {
            Visible = true;
        }
        public void PageUpdate()
        {
            cpuUseage = (int)(MPERF_GetCupUseAge());
            performanceGridGlobal.AddData(cpuUseage);
            item_cpuuseage.Value = cpuUseage.ToString() + "%";
            performanceGridGlobal.Invalidate();
            if (MPERF_UpdatePerformance())
            {
                item_process_count.Value = MPERF_GetProcessCount().ToString();
                item_thread_count.Value = MPERF_GetThreadCount().ToString();
                item_handle_count.Value = MPERF_GetHandleCount().ToString();
                times = TimeSpan.FromMilliseconds(Convert.ToDouble(MPERF_GetRunTime()));
                item_run_time.Value = times.Days + ":" + times.Hours.ToString("00") + ":" + times.Minutes.ToString("00") + ":" + times.Seconds.ToString("00");
                if (cpuCount > 1) performanceCpus.Invalidate();
                performanceInfos.UpdateSpeicalItems();
            }
        }
        public void PageDelete()
        {
            MPERF_DestroyCpuDetalsPerformanceCounters();
        }
        public void PageSetGridUnit(string s)
        {
            performanceGridGlobal.LeftBottomText = s;
        }
        public bool PageUpdateSimple(out string customString, out int outdata1, out int outdata2)
        {
            cpuUseage = (int)(MPERF_GetCupUseAge() );
            customString = cpuUseage.ToString() + "%  " + maxSpeed;
            outdata1 = cpuUseage;
            outdata2 = -1;

            if (!PageIsActive)
                performanceGridGlobal.AddData(outdata1);

            return true;
        }

        PerformanceInfos.PerformanceInfoSpeicalItem item_cpuuseage = null;
        PerformanceInfos.PerformanceInfoSpeicalItem item_cpuuseage_freq = null;
        PerformanceInfos.PerformanceInfoSpeicalItem item_process_count = null;
        PerformanceInfos.PerformanceInfoSpeicalItem item_thread_count = null;
        PerformanceInfos.PerformanceInfoSpeicalItem item_handle_count = null;
        PerformanceInfos.PerformanceInfoSpeicalItem item_run_time = null;

        private StringFormat performanceCpusText = null;
        private Font performanceCpusTextFont = null;
        private Brush performanceCpusTextBrush = null;

        private void GetStaticInfos()
        {
            StringBuilder stringBuilder = new StringBuilder(64);
            if (MPERF_GetCpuName(stringBuilder, 64))
                performanceTitle.SmallTitle = stringBuilder.ToString();
            else performanceTitle.SmallTitle = "";

            cpuCount = MPERF_GetProcessNumber();

            maxSpeed = (MPERF_GetCpuFrequency() / 1024d).ToString("0.0") + " GHz";
            performanceInfos.StaticItems.Add(new PerformanceInfos.PerformanceInfoStaticItem(LanuageMgr.GetStr("MaxSpeed"), MPERF_GetCpuFrequency().ToString() + " MHz"));
            performanceInfos.StaticItems.Add(new PerformanceInfos.PerformanceInfoStaticItem(LanuageMgr.GetStr("CpuCpunt"), cpuCount.ToString()));

            if (MPERF_GetCpuInfos())
            {
                performanceInfos.StaticItems.Add(new PerformanceInfos.PerformanceInfoStaticItem(LanuageMgr.GetStr("CpuPackageCount"), MPERF_GetCpuPackage().ToString()));
                performanceInfos.StaticItems.Add(new PerformanceInfos.PerformanceInfoStaticItem("L" + 1 + LanuageMgr.GetStr("Cache"), FormatFileSize1(Convert.ToInt32(MPERF_GetCpuL1Cache()))));
                performanceInfos.StaticItems.Add(new PerformanceInfos.PerformanceInfoStaticItem("L" + 2 + LanuageMgr.GetStr("Cache"), FormatFileSize1(Convert.ToInt32(MPERF_GetCpuL2Cache()))));
                if (MPERF_GetCpuL3Cache() != 0) performanceInfos.StaticItems.Add(new PerformanceInfos.PerformanceInfoStaticItem("L" + 3 + LanuageMgr.GetStr("Cache"), FormatFileSize1(Convert.ToInt32(MPERF_GetCpuL3Cache()))));
            }
        }
        private void InitRuntimeInfo()
        {
            item_cpuuseage = new PerformanceInfos.PerformanceInfoSpeicalItem();
            item_cpuuseage.Name = LanuageMgr.GetStr("Useage");
            item_cpuuseage_freq = new PerformanceInfos.PerformanceInfoSpeicalItem();
            item_cpuuseage_freq.Name = "";
            item_cpuuseage_freq.Value = "              ";
            item_process_count = new PerformanceInfos.PerformanceInfoSpeicalItem();
            item_process_count.Name = FormMain.str_proc_count;
            item_thread_count = new PerformanceInfos.PerformanceInfoSpeicalItem();
            item_thread_count.Name = LanuageMgr.GetStr("ThreadCount");
            item_handle_count = new PerformanceInfos.PerformanceInfoSpeicalItem();
            item_handle_count.Name = LanuageMgr.GetStr("HandleCount");
            item_run_time = new PerformanceInfos.PerformanceInfoSpeicalItem();
            item_run_time.Name = LanuageMgr.GetStr("RunTime");
            performanceInfos.SpeicalItems.Add(item_cpuuseage);
            performanceInfos.SpeicalItems.Add(item_cpuuseage_freq);
            performanceInfos.SpeicalItems.Add(item_process_count);
            performanceInfos.SpeicalItems.Add(item_thread_count);
            performanceInfos.SpeicalItems.Add(item_handle_count);
            performanceInfos.SpeicalItems.Add(item_run_time);
        }
        private void InitCpusInfo()
        {
            performanceCpusTextFont = new Font("微软雅黑", 9);
            performanceCpusTextBrush = Brushes.Black;
            performanceCpusText = new StringFormat();
            performanceCpusText.Alignment = StringAlignment.Center;
            performanceCpusText.LineAlignment = StringAlignment.Center;

            MPERF_InitCpuDetalsPerformanceCounters();
        }

        private void PerformanceCpu_Load(object sender, EventArgs e)
        {
            GetStaticInfos();
            InitRuntimeInfo();
            InitCpusInfo();
        }

        private Color performanceCpus_GetColorFormValue(int i)
        {
            if (i == 0)
                return Color.White;
            else if (i > 0 && i <= 20)
                return Color.FromArgb(241, 246, 250);
            else if (i > 20 && i <= 30)
                return Color.FromArgb(180, 200, 240);
            else if (i > 30 && i <= 60)
                return Color.FromArgb(100, 180, 239);
            else if (i > 60 && i <= 80)
                return Color.FromArgb(80, 164, 236);
            else if (i > 80 && i <= 85)
                return Color.FromArgb(20, 146, 220);
            else if (i > 85 && i <= 90)
                return Color.SandyBrown;
            else if (i > 90 && i <= 95)
                return Color.DarkOrange;
            else if (i > 95 && i < 100)
                return Color.Tomato;
            else if (i >= 100)
                return Color.FromArgb(243, 90, 52);
            return Color.White;
        }
        private void performanceCpus_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (cpuCount > 1)
            {
                int width = performanceCpus.Width / cpuCount;
                int x = 0;
                int performanceCounterCpusListCount = MPERF_GetCpuDetalsPerformanceCountersCount();
                for (int i = 0; i < performanceCounterCpusListCount; i++)
                {
                    double useage = MPERF_GetCpuDetalsCpuUsage(i);

                    using (SolidBrush s = new SolidBrush(performanceCpus_GetColorFormValue((int)(useage))))
                    {
                        Rectangle r = new Rectangle(x, 1, width, performanceCpus.Height - 2);
                        g.FillRectangle(s, r);
                        g.DrawString(useage.ToString("0.0") + "%", performanceCpusTextFont, performanceCpusTextBrush, r, performanceCpusText);
                    }

                    x += width;
                }
            }
            g.DrawRectangle(performanceGridGlobal.DrawPen, 0, 0, performanceCpus.Width - 1, performanceCpus.Height - 1);
        }


    }
}
