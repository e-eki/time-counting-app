using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OpTime
{
    public delegate void AddNewBinkHandler(string newBinkName);

    public class OptimeControl
    {
        private System.Threading.Timer mainTimer;
        private FileWork usedKpaFileWork;
        public Counter KpaCounter { get; private set; }          //public AdminForm
        public BinkInfo UsedBinkInfo { get; set; }               //public KomplektsForm
        private FileWork usedBinkFileWork;
        public Counter BinkCounter { get; private set; }         //public KomplektsForm
        public bool BinkCounterStarted;
        public BindingList<BinkInfo> BinkList { get; private set; }

        private int timerInterval;  // Интервал срабатывания таймера - секунды.
        private int writeInterval;  // Интервал записи в файл - секунды.
        private int writeCounter;   // Счётчик времени после последней записи.

        public event Action NewTime;    
        public event Action BinkSelected;
        public event Action StateChanged;

        private Control guiInvoker;  //объект класса Control 
        
        public string SelectedBinkId
        { 
            get
            { 
                if (UsedBinkInfo != null)
                    return UsedBinkInfo.BinkName;
                else 
                    return " "; 
            } 
        }
        public TimeSpan KpaSumTime
        {
            get
            {
                if (KpaCounter != null)
                    return KpaCounter.SumTime;
                else
                    return TimeSpan.Zero;
            } 
        }
        public TimeSpan KpaSessionTime 
        {
            get
            {
                if (KpaCounter != null)
                    return KpaCounter.SessionTime;
                else
                    return TimeSpan.Zero;
            }
        }
        public TimeSpan SelectedBinkSumTime 
        { 
            get 
            {
                if (BinkCounter != null)
                    return BinkCounter.SumTime;
                else if (UsedBinkInfo != null)
                    return FileWork.GetSumTime(UsedBinkInfo.FileName);
                else
                    throw new InvalidOperationException("Комплект не выбран");
            } 
        }
        public TimeSpan SelectedBinkSessionTime
        {
            get 
            {
                if (BinkCounter != null)
                    return BinkCounter.SessionTime;
                else
                    return TimeSpan.Zero;
            } 
        }

        public OptimeControl(string dir, int tInterval, int wInterval, Control guiInvoker)   //здесь в guiInvoker передается MainFоrm
        {
            timerInterval = tInterval;
            writeInterval = wInterval;
            writeCounter = 0;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Environment.CurrentDirectory = dir;    //задание текущей директории 

            TimeSpan kpaSumTime = TimeSpan.Zero;   //создание и запуск таймера комплекта КПА
       
            usedKpaFileWork = new FileWork("KPA.time", out kpaSumTime);  //создание или открытие (если уже есть) файла КПА

            KpaCounter = new Counter(kpaSumTime);
            KpaCounter.Start();

            mainTimer = new System.Threading.Timer(MainTimer_Tick, null, 0, tInterval*1000);  //создание таймера с заданным интервалом
            
            BinkList = new BindingList<BinkInfo>(FileWork.GetBinkList());    //создание списка комплектов БИНК из 
                                                                              //файлов *.bink.time, кот. лежат в папке по умолчани.
            BinkCounterStarted = false;

            this.guiInvoker = guiInvoker;
        }

        private void MainTimer_Tick(object state)
        {
            writeCounter += timerInterval;
            bool timeToWrite = false;
            if(writeCounter >= writeInterval)
            {
                timeToWrite = true;
                writeCounter = 0;
            }

            // Расчёт времени наработки КПА.
            if (KpaCounter.Working)
            {
                KpaCounter.Tick();

                if(timeToWrite)
                    usedKpaFileWork.Rewrite(KpaCounter.SumTime);
            }

            // Расчёт времени наработки БИНК.
            if (BinkCounter != null && BinkCounter.Working)
            {
                BinkCounter.Tick();
                BinkCounterStarted = true;
                if (timeToWrite)
                    usedBinkFileWork.Rewrite(BinkCounter.SumTime);
            }
            
            if (NewTime != null)
                guiInvoker.BeginInvoke((Action)NewTime.Invoke);   

        }

        private void OnBinkSelected()   
        {
            if (BinkSelected != null)
                guiInvoker.Invoke((Action)BinkSelected.Invoke);  
        }

        public void SelectBINK(string binkId)    
        {
            if (string.IsNullOrEmpty(binkId))
            {
                UsedBinkInfo = null;
                OnBinkSelected();   
                throw new Exception(" Отсутствует идентификатор комплекта . Введите идентификатор и повторите попытку.");
            }
            else
            {
                if (BinkCounter != null && BinkCounter.Working == true)
                {
                    throw new Exception(" Выбор комплекта невозможен, пока запущен таймер. Остановите таймер и повторите попытку.");    //проверить исключение в mainForm!
                }
                else
                {
                    foreach (BinkInfo c in BinkList)
                    {
                        if (c.BinkName == binkId)
                        {
                            UsedBinkInfo = c;
                            OnBinkSelected();
                            return;
                        }
                    }
                    UsedBinkInfo = AddNewBinkInternal(binkId);  //если в списке нет такого, то добавить
                    OnBinkSelected();
                }
            }
        }

        //публичный метод - для добавления из KomplektsForm
        public void AddNewBink(string id)
        {
            AddNewBinkInternal(id);
        }

        //приватный метод - для добавления из OptimeControl (возвращает новый)
        private BinkInfo AddNewBinkInternal(string id)
        {
            BinkInfo newBink = new BinkInfo()
            {
                BinkName = id,
                FileName = BinkInfo.FromNameToFile(id)    
            };
            
            guiInvoker.Invoke((Action<BinkInfo>)BinkList.Add,newBink);  //???

            return newBink;
        }

        public void DeleteBink(string id)    //метод для удаления из KomplektsForm
        {
            foreach (BinkInfo c in BinkList)
            {
                if (id == c.BinkName)
                {
                    guiInvoker.Invoke((Func<BinkInfo, bool>)BinkList.Remove, c);   
                    System.IO.File.Delete(c.FileName);
                    return;
                }
            }
        }

        private void OnStateChanged()
        {
            if (StateChanged != null)
                guiInvoker.BeginInvoke((Action)StateChanged.Invoke);
        }

        public void StartBink()
        {
            if (UsedBinkInfo == null)
                throw new Exception(" Комплект не выбран! Выберите комплект и повторите попытку.");
            else
            {
                try   //для того, чтобы в случае ошибки чтения файла не происходило ничего.
                {
                    TimeSpan sumBINKtime = TimeSpan.Zero;
                    usedBinkFileWork = new FileWork(UsedBinkInfo.FileName, out sumBINKtime);

                    BinkCounter = new Counter(sumBINKtime);
                    BinkCounter.Start();
                    BinkCounterStarted = true;

                    OnStateChanged();
                }
                finally
                { }
            }       
        }

        public void StopBink()
        {
            if (UsedBinkInfo == null)
                throw new Exception(" Комплект не выбран! Выберите комплект и повторите попытку.");
            else
            {
                BinkCounter.Stop();
                usedBinkFileWork.Rewrite(BinkCounter.SumTime);
                usedBinkFileWork.Close();
                BinkCounterStarted = false;

                OnStateChanged();
            }
        }

        public void FormCloseControl()
        {
            mainTimer.Dispose();

            if (KpaCounter != null)
                KpaCounter.Stop();
            if (usedKpaFileWork != null)
            {
                usedKpaFileWork.Rewrite(KpaCounter.SumTime);
                usedKpaFileWork.Close();
            }

            if (usedBinkFileWork != null)
            {
                if (BinkCounterStarted == true)
                {
                    BinkCounter.Stop();
                    usedBinkFileWork.Rewrite(BinkCounter.SumTime);
                    usedBinkFileWork.Close();
                }
            }
        }

        public void KpaDataRedact(TimeSpan newKpaSumTime)
        {
            KpaCounter.Stop();

            KpaCounter.Redact(newKpaSumTime);
            usedKpaFileWork.Rewrite(newKpaSumTime);

            KpaCounter.Start();
        }

        public void BinkDataRedact(string binkName, TimeSpan newBinkSumTime)
        {
            FileWork.RedactForClosedFile(BinkInfo.FromNameToFile(binkName), newBinkSumTime);

            if (UsedBinkInfo != null && binkName == UsedBinkInfo.BinkName && BinkCounter != null)
                BinkCounter.Redact(newBinkSumTime);
        }

        public static string TimeToString(TimeSpan time, bool flag)
        {
            string timeToString = "";

            if (flag)
            {
                if ((int)time.TotalDays > 0)
                    timeToString += (int)time.TotalDays + " дней ";

                if (time.Hours > 0)
                    timeToString += time.Hours + " ч ";

                if (time.Minutes > 0)
                    timeToString += time.Minutes + " мин ";

                timeToString += time.Seconds + " с ";
            }
            else
            {
                timeToString = (int)time.TotalHours + " ч";
            }

            return timeToString;
        }
    }
}
