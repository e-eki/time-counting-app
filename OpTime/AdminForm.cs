using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OpTime
{
    public partial class AdminForm : Form
    {
        OptimeControl OpControl;

        private bool totalHoursChanged;
        private bool sumTimeChanged;
        bool correctlyKpaTimeIput;
        bool correctlyBinkTimeInput;

        private TimeSpan kpaSumTime;
        private TimeSpan newKpaSumTime;
        private TimeSpan binkSumTime;
        private TimeSpan newBinkSumTime;

        public AdminForm(OptimeControl opControl)
        {
            InitializeComponent();

            this.OpControl = opControl;
            BinkList_CB.DataSource = opControl.BinkList;

            kpaSumTime = OpControl.KpaSumTime;
            newKpaSumTime = kpaSumTime;
            ViewKpaData();

            KpaTotalHours_TB.TextChanged += RewriteTotalHours;
            BinkTotalHours_TB.TextChanged += RewriteTotalHours;

            KpaSumDays_TB.TextChanged += RewriteNewSumTime;
            KpaSumHours_TB.TextChanged += RewriteNewSumTime;
            KpaSumMinutes_TB.TextChanged += RewriteNewSumTime;
            KpaSumSeconds_TB.TextChanged += RewriteNewSumTime;
            BinkSumDays_TB.TextChanged += RewriteNewSumTime;
            BinkSumHours_TB.TextChanged += RewriteNewSumTime;
            BinkSumMinutes_TB.TextChanged += RewriteNewSumTime;
            BinkSumSeconds_TB.TextChanged += RewriteNewSumTime;

            totalHoursChanged = false;
            sumTimeChanged = false;
            correctlyKpaTimeIput = true;
            correctlyBinkTimeInput = true;
        }

        private void ViewKpaData()
        {
            if (kpaSumTime != null)
            {
                //totalHoursChanged = true;
                //sumTimeChanged = true;

                KpaTotalHours_TB.Text = ((int)kpaSumTime.TotalHours).ToString();
                KpaSumDays_TB.Text = ((int)kpaSumTime.TotalDays).ToString();
                KpaSumHours_TB.Text = kpaSumTime.Hours.ToString();
                KpaSumMinutes_TB.Text = kpaSumTime.Minutes.ToString();
                KpaSumSeconds_TB.Text = kpaSumTime.Seconds.ToString();

                //totalHoursChanged = false;
                //sumTimeChanged = false;
            }
        }

        private void BinkList_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewBinkData();
        }

        private void ViewBinkData()
        {
            if (BinkList_CB.SelectedItem != null)
            {
                if (OpControl.UsedBinkInfo != null && BinkList_CB.Text == OpControl.UsedBinkInfo.BinkName && OpControl.BinkCounter != null)
                {
                    binkSumTime = OpControl.SelectedBinkSumTime;
                    newBinkSumTime = binkSumTime;
                    ViewBinkDataInternal(binkSumTime);
                }
                else
                {
                    try
                    {
                        binkSumTime = FileWork.GetSumTime(BinkInfo.FromNameToFile(BinkList_CB.Text));
                        newBinkSumTime = binkSumTime;
                        ViewBinkDataInternal(binkSumTime);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Возникла ошибка при чтении из указанного файла: " + Path.Combine(Environment.CurrentDirectory, BinkInfo.FromNameToFile(BinkList_CB.Text)) + "\n" + "Информация о данном комплекте не будет отображена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ViewBinkDataInternal(TimeSpan.Zero);
                        return;
                    }
                }
            }
        }

        private void ViewBinkDataInternal(TimeSpan binkTime)
        {
            BinkTotalHours_TB.Text = ((int)binkTime.TotalHours).ToString();
            BinkSumDays_TB.Text = ((int)binkTime.TotalDays).ToString();
            BinkSumHours_TB.Text = binkTime.Hours.ToString();
            BinkSumMinutes_TB.Text = binkTime.Minutes.ToString();
            BinkSumSeconds_TB.Text = binkTime.Seconds.ToString();
        }

        private void KpaSave_BTN_Click(object sender, EventArgs e)
        {
            if (correctlyKpaTimeIput)
            {
                kpaSumTime = newKpaSumTime;
                OpControl.KpaDataRedact(kpaSumTime);
                ViewKpaData();
            }
        }

        private void KpaCancel_TB_Click(object sender, EventArgs e)
        {
            kpaSumTime = OpControl.KpaSumTime;
            newKpaSumTime = kpaSumTime;
            ViewKpaData();
        }

        private void BinkSave_TB_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(BinkList_CB.Text) && (correctlyBinkTimeInput))
            {
                binkSumTime = newBinkSumTime;

                OpControl.BinkDataRedact(BinkList_CB.Text, newBinkSumTime);
                ViewBinkDataInternal(binkSumTime);
            }
        }

        private void BinkCancel_TB_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(BinkList_CB.Text))
                ViewBinkDataInternal(binkSumTime);
            else
                ViewBinkDataInternal(TimeSpan.Zero);
        }

        private void RewriteTotalHours(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = SystemColors.Window;
            ErrorFormat_TT.SetToolTip(((TextBox)sender), null);

            if (!sumTimeChanged)
            {
                if (((TextBox)sender).Name == "KpaTotalHours_TB")
                    RewriteTotalHoursInternal(out correctlyKpaTimeIput, true, out newKpaSumTime, KpaTotalHours_TB, KpaSumDays_TB, KpaSumHours_TB, KpaSumMinutes_TB, KpaSumSeconds_TB);
                else
                    RewriteTotalHoursInternal(out correctlyBinkTimeInput, false, out newBinkSumTime, BinkTotalHours_TB, BinkSumDays_TB, BinkSumHours_TB, BinkSumMinutes_TB, BinkSumSeconds_TB);
            }
        }

        private void RewriteTotalHoursInternal(out bool correctlyTimeInput, bool senderIsKpa, out TimeSpan newSumTime, TextBox totalHours_TB, TextBox sumDays_TB, TextBox sumHours_TB, TextBox sumMinutes_TB, TextBox sumSeconds_TB)
        {
            int i = 0;
            if (senderIsKpa)
                newSumTime = newKpaSumTime;
            else
                newSumTime = newBinkSumTime;

            CheckTimeString(out i, totalHours_TB, out correctlyTimeInput);
            if (correctlyTimeInput)
            {
                totalHoursChanged = true;

                newSumTime = TimeSpan.FromHours(i);
                sumDays_TB.Text = ((int)newSumTime.TotalDays).ToString();   //???
                sumHours_TB.Text = newSumTime.Hours.ToString();
                sumMinutes_TB.Clear();
                sumSeconds_TB.Clear();

                totalHoursChanged = false;
            }
        }

        private void CheckTimeString(out int i, TextBox textbox, out bool correctlyTimeInput)
        {
            correctlyTimeInput = false;
            i = 0;

            if (!string.IsNullOrEmpty(textbox.Text))
            {
                if (int.TryParse(textbox.Text, out i) && i >= 0)
                    correctlyTimeInput = true;
                else
                {
                    textbox.BackColor = Color.Coral;
                    ErrorFormat_TT.SetToolTip(textbox, "Ошибка: недопустимый формат времени.");
                }
            }
        }

        private void RewriteNewSumTime(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = SystemColors.Window;
            ErrorFormat_TT.SetToolTip(((TextBox)sender), null);

            if (!totalHoursChanged)
            {
                if (((TextBox)sender).Name == "KpaSumDays_TB" || ((TextBox)sender).Name == "KpaSumHours_TB" || ((TextBox)sender).Name == "KpaSumMinutes_TB" || ((TextBox)sender).Name == "KpaSumSeconds_TB")
                    RewriteNewSumTimeInternal(true, out newKpaSumTime, KpaTotalHours_TB, KpaSumDays_TB, KpaSumHours_TB, KpaSumMinutes_TB, KpaSumSeconds_TB);
                else
                    RewriteNewSumTimeInternal(false, out newBinkSumTime, BinkTotalHours_TB, BinkSumDays_TB, BinkSumHours_TB, BinkSumMinutes_TB, BinkSumSeconds_TB);
            }
        }

        private void RewriteNewSumTimeInternal(bool senderIsKpa, out TimeSpan newSumTime, TextBox totalHours_TB, TextBox sumDays_TB, TextBox sumHours_TB, TextBox sumMinutes_TB, TextBox sumSeconds_TB)
        {
            bool correctlyTimeInput;
            bool correctly;
            int i = 0;
            newSumTime = TimeSpan.Zero;
            sumTimeChanged = true;

            correctly = true;

            CheckTimeString(out i, sumDays_TB, out correctlyTimeInput);
            correctly = correctly & correctlyTimeInput;
            if (correctlyTimeInput)
                newSumTime += TimeSpan.FromDays(i);

            CheckTimeString(out i, sumHours_TB, out correctlyTimeInput);
            correctly = correctly & correctlyTimeInput;
            if (correctlyTimeInput)
                newSumTime += TimeSpan.FromHours(i);

            CheckTimeString(out i, sumMinutes_TB, out correctlyTimeInput);
            correctly = correctly & correctlyTimeInput;
            if (correctlyTimeInput)
                newSumTime += TimeSpan.FromMinutes(i);

            CheckTimeString(out i, sumSeconds_TB, out correctlyTimeInput);
            correctly = correctly & correctlyTimeInput;
            if (correctlyTimeInput)
                newSumTime += TimeSpan.FromSeconds(i);

            totalHours_TB.Text = ((int)newSumTime.TotalHours).ToString();

            sumTimeChanged = false;

            if (senderIsKpa)
                correctlyKpaTimeIput = correctly;
            else
                correctlyBinkTimeInput = correctly;
        }
    }
}


