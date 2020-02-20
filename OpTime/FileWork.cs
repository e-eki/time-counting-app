using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace OpTime
{
    public class FileWork  //инициализирует файл, задает значение времени наработки OperatingTime 
                           // и времени наработки в секундах SumOperatingTime
    {
        private FileStream fS;     //поток, считывающий и записывающий байты
        private BinaryWriter bW;   //для конструктора, записывающее байты

        private string fileName;

        public FileWork(string fileName, out TimeSpan sumTime)
        {
            Int32 operatingTime;    //время наработки 

            this.fileName = fileName;
            if (System.IO.File.Exists(fileName))   //Иниц.файла и запись в него нач.значений
            {
                fS = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);   
                BinaryReader bR = new BinaryReader(fS);                                                      
                bW = new BinaryWriter(fS);
                try                                      
                {
                    operatingTime = GetSumTimeInternal(bR);
                }
                catch (IOException)
                {
                    if (MessageBox.Show("Возникла ошибка при чтении из указанного файла: " + Path.Combine(Environment.CurrentDirectory, fileName) + ".\nПерезаписать файл?", "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        bW.Write((int)0);
                        bW.Write((int)0);
                        bW.Write((int)0);
                        fS.Flush();

                        operatingTime = 0;
                    }
                    else
                    {
                        fS.Close();
                        throw new InvalidDataException("Возникла ошибка при чтении из указанного файла: " + Path.Combine(Environment.CurrentDirectory, fileName));
                    }
                }
            }
            else
            {
                fS = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                bW = new BinaryWriter(fS);

                bW.Write((int)0);                       
                bW.Write((int)0);
                bW.Write((int)0);
                fS.Flush();

                operatingTime = 0;                                             
            }
            sumTime = TimeSpan.FromSeconds(operatingTime);        
        }

        private static int GetSumTimeInternal(BinaryReader bR)
        {
            Int32 operatingTime;
            Int32 operatingTime1 = bR.ReadInt32();
            Int32 operatingTime2 = bR.ReadInt32();
            Int32 operatingTime3 = bR.ReadInt32();

            if (operatingTime1 != operatingTime2)
                operatingTime = operatingTime3;
            else
                operatingTime = operatingTime1;
            return operatingTime;
            
        }

        public static TimeSpan GetSumTime(string fileName)
        {
            TimeSpan sumTime;

            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    FileStream fS = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader bR = new BinaryReader(fS);
                    sumTime = TimeSpan.FromSeconds(GetSumTimeInternal(bR));
                    fS.Close();
                }
                else
                {
                    FileStream fS = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                    BinaryWriter bW = new BinaryWriter(fS);

                    bW.Write((int)0);
                    bW.Write((int)0);
                    bW.Write((int)0);
                    fS.Flush();
                    fS.Close();

                    sumTime = TimeSpan.Zero;
                }
            }
            finally
            { }
            return sumTime;           
        }

        public static List<BinkInfo> GetBinkList()         
        {
            List<BinkInfo> binkList = new List<BinkInfo>();

            foreach (string filename in Directory.GetFiles(Environment.CurrentDirectory,"*.bink.time"))
            {
                BinkInfo bink = new BinkInfo();
                bink.BinkName = BinkInfo.FromFileToName(Path.GetFileName(filename));
                bink.FileName = filename;
               
                binkList.Add(bink);
            }
            return binkList;
        }

        public void Rewrite(TimeSpan time)
        {
            bW.Seek(0, SeekOrigin.Begin);             //перемещает указатель в файле на позицию 0
            bW.Write((int)time.TotalSeconds);         //записывает в файл кол-во секунд в формате Int
            bW.Write((int)time.TotalSeconds);
            bW.Write((int)time.TotalSeconds);
            fS.Flush();
        }

        public static void RedactForClosedFile(string fileName, TimeSpan newTime)
        {
            FileStream fS = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bW = new BinaryWriter(fS);

            bW.Write((int)newTime.TotalSeconds);
            bW.Write((int)newTime.TotalSeconds);
            bW.Write((int)newTime.TotalSeconds);
            fS.Flush();
            fS.Close();
        }

        public void Close()
        {
            fS.Close();        
        }
    }   
}
