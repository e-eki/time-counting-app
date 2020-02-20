using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace OpTime
{
    public partial class MainForm : Form
    {
        public OptimeControl OpControl;   
       
        public const string SettingsFile = "settings.cfg";   //задается название файла настроек
        private Process NotepadProcess;

        public MainForm()
        {
            InitializeComponent();
        }

        //при загрузке формы
        private void MainForm_Load(object sender, EventArgs e)        
        {
            SettingDict sd = SettingDict.LoadSettings(SettingsFile);
            string dir = sd["общие"]["папка"];

            try
            {
                // папка, таймер (с), запись (с), объект ГИП.
                OpControl = new OptimeControl(dir, 1, 300, this);   
                OpControl.NewTime += ShowTime;
                OpControl.BinkSelected += BinkSelect;
                OpControl.StateChanged += StartStopState;

                UsedBINK_CB.DataSource = OpControl.BinkList;

                Server Serv = new Server(sd, OpControl);
                Serv.StateChanged += Serv_StateChanged;
                Serv.Start();

                if (OpControl.UsedBinkInfo == null)
                    StartBINK_Btn.Enabled = false;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message +"\n" + "Приложение будет закрыто.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();    
            }
            catch (Exception)
            {
                MessageBox.Show("Одновременно может быть запущено не более одного экземпляра программы. Запускаемый экземпляр будет закрыт.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }      
        }

        private void ShowTime()
        {
            KPAoperatingTime_TB.Text = OptimeControl.TimeToString(OpControl.KpaSumTime, true);
            KPAoperatingTimeHours_TB.Text = OptimeControl.TimeToString(OpControl.KpaSumTime, false);
            KPApreviousTime_TB.Text = OptimeControl.TimeToString(OpControl.KpaSessionTime, true);

            if (OpControl.BinkCounterStarted == true)
            {
                try
                {
                BINKoperatingTime_TB.Text = OptimeControl.TimeToString(OpControl.SelectedBinkSumTime, true);
                BINKoperatingTimeHours_TB.Text = OptimeControl.TimeToString(OpControl.SelectedBinkSumTime, false);
                BINKpreviousTime_TB.Text = OptimeControl.TimeToString(OpControl.SelectedBinkSessionTime, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "Информация о данном комплекте не будет отображена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }            
        }

        private void Serv_StateChanged(ServerBase server)
        {
            this.BeginInvoke((Action)delegate     
            {
                this.ServerState.Text = server.StateString;
                switch (server.State)
                {
                    case OpTime.ServerState.Waiting:
                        this.ServerState.BackColor = System.Drawing.Color.Yellow;
                        break;
                    case OpTime.ServerState.Connected:
                        this.ServerState.BackColor = System.Drawing.Color.LightGreen;
                        break;
                    case OpTime.ServerState.Stop:
                        this.ServerState.BackColor = System.Drawing.Color.LightGray;
                        break;
                    case OpTime.ServerState.Error:
                        this.ServerState.BackColor = System.Drawing.Color.Coral;
                        break;
                }
            });
        }

        private void UsedBINK_TB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UsedBINK_CB.SelectedItem != null)
 
            try
            {
                OpControl.SelectBINK(UsedBINK_CB.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
                StartBINK_Btn.Enabled = false;
        }

        private void BinkSelect()
        {
            UsedBINK_CB.Text = OpControl.SelectedBinkId;

            if (string.IsNullOrEmpty(OpControl.SelectedBinkId))
                StartBINK_Btn.Enabled = false;
            else
                StartBINK_Btn.Enabled = true;

            BINKoperatingTime_TB.Clear();
            BINKoperatingTimeHours_TB.Clear();
            BINKpreviousTime_TB.Clear();
        }

        //кнопка запуска/останова таймера выбранного комплекта 
        private void StartBINK_Btn_Click(object sender, EventArgs e)     
        {
            try
            {
                if (OpControl.BinkCounterStarted == false)
                    OpControl.StartBink();
                else
                    OpControl.StopBink();
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Подсчет времени для указанного комплекта БИНК не может быть запущен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void StartStopState()
        {
            if (OpControl.BinkCounterStarted == true)
            {
                StartBINK_Btn.Text = "Останов";
                UsedBINK_CB.Enabled = false;
            }
            else
            {
                StartBINK_Btn.Text = "Запустить";
                UsedBINK_CB.Enabled = true;
            }
        }

        //закрытие формы
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)  
        {
            if (NotepadProcess != null)
                NotepadProcess.CloseMainWindow();

            if(OpControl!=null)
                OpControl.FormCloseControl();
        }

        // вызов диалогового окна настроек программы
        private void programSettingsToolStripMenuItem_Click(object sender, EventArgs e)   
        {
            //запуск процесса блокнот с передачей ему параметром указанного файла
            if (NotepadProcess == null)
            {
                NotepadProcess = Process.Start("Notepad", Path.Combine(Application.StartupPath, SettingsFile));
                NotepadProcess.EnableRaisingEvents = true;

                NotepadProcess.Exited += ResetSettings;
            }
        }

        void ResetSettings(object sender, EventArgs e)
        {
            //NotepadProcess.Close();   
            NotepadProcess = null;
            this.BeginInvoke((Action)delegate       
            {
                if (MessageBox.Show("Для того, чтобы новые настройки вступили в силу, перезапустите программу. Завершить выполнение программы сейчас?", "Обновление настроек", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            });
        }

        // вызов диалогового окна режима администратора (после ввода пароля)
        private void adminSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpControl.BinkCounter != null && OpControl.BinkCounterStarted == true)
                MessageBox.Show("Включение режима администратора невозможно, пока запущен таймер БИНК. Остановите таймер и повторите попытку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                PasswordForm passwordForm = new PasswordForm(OpControl);
                passwordForm.ShowDialog();

                if (passwordForm.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    AdminForm adminForm = new AdminForm(OpControl);
                    adminForm.ShowDialog();
                }
            }
        }

        //вызов диалогового окна "о программе"
        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)   
        {
            AboutProgramForm a = new AboutProgramForm();
            a.ShowDialog();
        }

        // выход из программы
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)   
        {
            Close();
        }

        // ?свернуть-развернуть форму
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if(WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                TrayNotifyIcon.Visible = true;
                TrayNotifyIcon.ShowBalloonTip(3000, null, Text, ToolTipIcon.Info);
            }
        }

        private void TrayNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if(!Visible)
            {
                Show();
                WindowState = FormWindowState.Normal;
                TrayNotifyIcon.Visible = false;
            }
        }

        //вызов диалогового окна списка комплектов 
        private void devicesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object selectedItem = UsedBINK_CB.SelectedItem;

            KomplektsForm komplektsForm = new KomplektsForm(OpControl);
            komplektsForm.ShowDialog();

            if (selectedItem != UsedBINK_CB.SelectedItem)
                UsedBINK_TB_SelectedIndexChanged(null, null);
        }
    }

    //класс хеш-таблицы настроек
    public class SettingDict : Dictionary<string, Dictionary<string, string>>    
    {
        //метод, создающий и возвращающий таблицу настроек
        public static SettingDict LoadSettings(string path)     
        {
            SettingDict sett = new SettingDict();
            try
            {
                //если не существует файла с таким названием
                if (!File.Exists(path))        
                    throw new ApplicationException("Файл настроек не найден.");

                Dictionary<string, string> section = null;    //объявление экземпляра класса section

                string line;
                string[] parts;
                StreamReader rd = new StreamReader(path);     //считывает символы из файла 
                while (!rd.EndOfStream)
                {
                    line = rd.ReadLine();

                    if (String.IsNullOrEmpty(line))         //если строка пустая - продолжить, на новую строку
                        continue;

                    if (line.StartsWith("#"))               //если строка начинается с решетки (прав.формат)
                    {
                        line = line.TrimStart('#');          //получение идентификатора ("общие")
                        section = new Dictionary<string, string>();  //создание элемента таблицы (в нем ничего не задано)
                        sett.Add(line, section);        //добавление элемента с идентификатором в таблицу (общие, section)
                    }

                    else                                  //если строка не пустая и не начинается с решетки
                    {
                        if (section == null)           //если экземпляр section не был создан -значит и строка не соотв.

                            throw new ApplicationException("Файл настроек должен начинаться с определения секции настроек: " + line);

                        parts = line.Split('=');       //а если был - значит в этой строке уже содержатся настройки,
                                                       //разбиваем ее на две части, разделенные = .
                        if (parts.Length != 2)
                            throw new ApplicationException("Ошибка формата файла настроек: " + line);

                        if (parts[1].Contains(';'))    //все, что за ; - комментарии (!)

                            parts[1] = parts[1].Split(';')[0];

                        section.Add(parts[0].Trim(), parts[1].Trim());   //добавление элемента в элемент таблицы section
                                                                         // (папка, .\)
                    }
                }
                rd.Close();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sett;
        }
    }
}

