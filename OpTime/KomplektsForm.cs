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
    public partial class KomplektsForm : Form
    {
        private OptimeControl opControl;
        
        public KomplektsForm( OptimeControl opControl)  
        {
            InitializeComponent();

            this.opControl = opControl;
            opControl.StateChanged += ForbidListChanges;
            opControl.BinkList.ListChanged += binkList_ListChanged;
           
            ViewBINKlist();

            //если подсчет времени не остановлен, то кнопка "Добавить" недоступна
            if (opControl.BinkCounter != null && opControl.BinkCounter.Working == true)
                AddBINK_Btn.Enabled = false;
            else
                AddBINK_Btn.Enabled = true;
        }

        void binkList_ListChanged(object sender, ListChangedEventArgs e)
        {
            AddBINK_TB.Clear();
            ViewBINKlist();
        }

        private void ForbidListChanges()
        {
            if (opControl.BinkCounterStarted == true)
                AddBINK_Btn.Enabled = false;
            else
                AddBINK_Btn.Enabled = true;
        }

        private void ViewBINKlist()     //показать список комплектов в окне BINK_LV
        {
            BINK_LV.Items.Clear();

            foreach (BinkInfo c in opControl.BinkList)
            {
                if (opControl.UsedBinkInfo != null && c.BinkName == opControl.UsedBinkInfo.BinkName && opControl.BinkCounter != null)  //для выбранного в комбобоксе комплекта - отображается время не из файла, а из таймера на данный момент

                    BINK_LV.Items.Add(new ListViewItem(new string[] 
                    { 
                        c.BinkName,
                        OptimeControl.TimeToString(opControl.BinkCounter.SumTime, false),
                        OptimeControl.TimeToString(opControl.BinkCounter.SumTime, true) 
                    }));
                else                                                    //для всех остальных комплектов - время считывается из файлов
                {
                    TimeSpan sumBINKtime;

                    try
                    {
                        sumBINKtime = FileWork.GetSumTime(c.FileName);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Возникла ошибка при чтении из указанного файла: " + Path.Combine(Environment.CurrentDirectory, c.FileName) + "\n" + "Информация о данном комплекте не будет отображена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    BINK_LV.Items.Add(new ListViewItem(new string[] 
                    { 
                        c.BinkName, 
                        OptimeControl.TimeToString(sumBINKtime, false),
                        OptimeControl.TimeToString(sumBINKtime, true) 
                    }));                   
                }
            }
        }

        private void AddBINK_Btn_Click(object sender, EventArgs e)    //кнопка добавления комплекта
        {
            string newBinkName = AddBINK_TB.Text;

            foreach (BinkInfo c in opControl.BinkList)
            {
                if (newBinkName == c.BinkName)
                {
                    MessageBox.Show("Комплект БИНК с указанным идентификатором уже существует. Введите другой идентификатор.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            opControl.AddNewBink(newBinkName);
        }

        //удаление комплекта кнопкой Delete
        private void BINK_LV_KeyDown(object sender, KeyEventArgs e)
        {
            if (opControl.BinkCounterStarted == false)
            {
                if (opControl.BinkCounter == null || opControl.BinkCounter.Working == false)
                {
                    if (e.KeyCode == Keys.Delete)
                    {
                        if (BINK_LV.SelectedItems.Count > 1)
                        {
                            MessageBox.Show("Выберите один комплект для удаления", "Удаление комплекта", MessageBoxButtons.OK);
                        }
                        else if (BINK_LV.SelectedItems.Count == 1)
                        {
                            string binkName = BINK_LV.SelectedItems[0].Text;
                            if (MessageBox.Show(String.Format("Вы действительно хотите удалить информацию о комплекте {0}?", binkName), "Удаление комплекта", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                opControl.DeleteBink(binkName);
                        }
                    }
                }
            }
        }

        private void KomplektsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            opControl.BinkList.ListChanged -= binkList_ListChanged;
        }

        private void BINK_LV_SelectedIndexChanged(object sender, EventArgs e)
        {
            var t = BINK_LV.SelectedItems;
        }
    }
}
