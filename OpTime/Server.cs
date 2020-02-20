using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System;

namespace OpTime
{
    public class Server : ServerBase
    {
        OptimeControl OpControl;

        public Server(SettingDict sett, OptimeControl opControl)
        {
            Dictionary<string, string> serverSett = sett["сервер"];
            Port = int.Parse(serverSett["порт"]);
            Name = "Сервер";
            OpControl = opControl;
        }
        
        // Обработка запросов.
        protected override void ClientWork()
        {
            using (StreamReader reader = new StreamReader(NetStream, Encoding.Default))
            {
                string request;
                string answer;
                while (!reader.EndOfStream)
                {
                    request = reader.ReadLine();
                    if (!string.IsNullOrEmpty(request))
                    {
                        answer = ProcessRequest(request);
                        if (answer != null)
                            SendAnswer(answer);
                    }
                }
            }
        }

        byte[] msg = new byte[256];
        // Отправка ответа.
        private void SendAnswer(string answer)
        {
            int cnt = Encoding.Default.GetBytes(answer, 0, answer.Length, msg, 0);
            msg[cnt] = 10;
            NetStream.Write(msg, 0, cnt + 1);
        }

        #region Обработка команд

        //NumberFormatInfo numberFormat;

        private const string
            FormatErrorBlock = "ERROR 1 , Ошибка формата - блок группы команд",
            FormatErrorSubblock = "ERROR 2 , Ошибка формата - блок подгруппы команд",
            FormatErrorParam = "ERROR 3 , Ошибка формата - формат параметра",
            UnknownBlock = "ERROR 4 , Неизвестная группа команд",
            UnknownSubblock = "ERROR 5 , Неизвестная подгруппа команд",
            UnknownCommand = "ERROR 6 , Неизвестная команда",

            AnswOk = "OK",
            AnswFail = "FAIL",
            AnswPMPowFormar = "0.0",   //??
            AnswPMFreqFormar = "0.0";   //??

        private string ProcessRequest(string request)
        {
            //request = request.ToUpper();

            string answer = null;
            string cmd, param;

            string[] parts = request.Split(':');
            if (parts.Length < 2)
                answer = FormatErrorBlock;
            else
                switch (parts[0].ToUpper())
                {
                    case "PRG":   // Служебные команды и запросы к программе.
                        if (parts.Length == 2)   // Формат: PRG:Команда [Параметр].
                        {
                            parts = parts[1].Split(' ');
                            cmd = parts[0];
                            if (parts.Length > 1)
                                param = parts[1];
                            else
                                param = null;

                            answer = Process_PRG_Request(cmd, param);
                        }
                        else
                            answer = FormatErrorSubblock;
                        break;

                    case "OPTIME":   // Запросы от Программы управления КПА БИНК
                        if (parts.Length == 3)   // Формат: OPTIME:KPA/BINK:Команда [Параметр].
                        {
                            switch (parts[1].ToUpper())
                            {
                                case "KPA":      // Запросы времени наработки КПА
                                    parts = parts[2].Split(' ');
                                    cmd = parts[0];
                                    if (parts.Length > 1)
                                        param = parts[1];
                                    else
                                        param = null;

                                    answer = Process_OPTIME_KPA_Request(cmd, param);
                                    break;

                                case "BINK":     // Запросы времени наработки БИНК
                                    parts = parts[2].Split(' ');
                                    cmd = parts[0];
                                    if (parts.Length > 1)
                                        param = parts[1];
                                    else
                                        param = null;

                                    answer = Process_OPTIME_BINK_Request(cmd, param);
                                    break;

                                default:
                                    answer = FormatErrorSubblock;
                                    break;
                            }
                        }
                        else
                            answer = FormatErrorSubblock;
                        break;

                    default:
                        answer = UnknownBlock;
                        break;
                }
            return answer;
        }

        #endregion

        #region Служебные команды

        // Обработка служебных команд для программы.
        private string Process_PRG_Request(string cmd, string param)
        {
            string answer = null;
            switch (cmd.ToUpper())
            {
                case "TEST":
                    answer= "OK";
                    break;

                case "HELP?":
                    SendAnswer("Служебные команды:");    //"стандартные команды управления программой"
                    SendAnswer("PRG:TEST -> OK. Самотестирование программы");
                    SendAnswer("PRG:HELP? -> <Список допустимых команд>");
                    SendAnswer("PRG:VERSION? -> <Версия программы>");
                    SendAnswer("PRG:CS? -> <Контрольная сумма программы>");

                    SendAnswer("Команды контроля времени наработки КПА:");
                    SendAnswer("OPTIME:KPA:GET? -> <Текущее время наработки КПА, чч>");

                    SendAnswer("Команды контроля времени наработки БИНК:");
                    SendAnswer("OPTIME:BINK:LIST? -> <Список комплектов БИНК>");
                    SendAnswer("OPTIME:BINK:SELECT <Идентификатор комплекта БИНК> -> OK|FAIL. Выбор комплекта БИНК, испытания которого будут проводиться");
                    SendAnswer("OPTIME:BINK:SELECT? -> <Идентификатор выбранного комплекта БИНК>");
                    SendAnswer("OPTIME:BINK:START -> OK|FAIL. Запуск подсчета времени наработки выбранного комплекта БИНК");
                    SendAnswer("OPTIME:BINK:GET? -> <Текущее время наработки выбранного комплекта БИНК, чч>");
                    SendAnswer("OPTIME:BINK:STOP -> OK|FAIL. Останов подсчета времени наработки выбранного комплекта БИНК");
                    break;

                case "VERSION?":
                    answer= "ver" + Program.Version;
                    break;
                case "CS?":
                    answer = Program.CheckSum;
                    break;

                default:
                    answer = UnknownCommand;
                    break;
            }
            return answer;
        }

        #endregion

        #region Запросы от Программы управления КПА БИНК

        // Команды контроля времени наработки КПА.
        private string Process_OPTIME_KPA_Request(string cmd, string param)
        {
            string answer = null;
            switch (cmd.ToUpper())
            {
                case "GET?":
                    answer = ((int)OpControl.KpaSumTime.TotalHours).ToString();    
                    break;

                default:
                    answer = UnknownCommand;
                    break;
            }
            return answer;
        }

        // Команды контроля времени наработки БИНК.
        private string Process_OPTIME_BINK_Request(string cmd, string param)
        {
            string answer = null;
            switch (cmd.ToUpper())
            {
                case "LIST?":
                    if (OpControl.BinkList.Count != 0)
                    {
                        foreach (BinkInfo c in OpControl.BinkList)   //если в момент добавления/удаления отправляет запрос???
                        {
                            if (answer == null)
                                answer = c.BinkName; 
                            else
                                answer += ", " + c.BinkName;
                        }
                    }
                    else
                        answer = " ";
                        
                    break;

                case "SELECT":
                    try
                    {
                        OpControl.SelectBINK(param);   //почему глючит отображение в списке добавление нового комплекта?
                        answer = AnswOk;
                    }
                    catch (Exception ex)
                    {
                        answer = AnswFail + ex.Message;
                    }
                    break;

                case "SELECT?":
                    if (!string.IsNullOrEmpty(OpControl.SelectedBinkId))
                        answer = OpControl.SelectedBinkId;
                    else
                        answer = string.Empty;
                    break;

                case "START":
                    if (OpControl.BinkCounterStarted == false)
                    {
                        try
                        {
                            OpControl.StartBink();
                            answer = "OK";
                        }
                        catch (InvalidDataException ex)
                        {
                            answer = AnswFail + " " + ex.Message + ". Подсчет времени для указанного комплекта БИНК не может быть запущен.";
                        }
                        catch (Exception ex)
                        {
                            answer = AnswFail + ex.Message;
                        }
                    }
                    else
                        answer = AnswOk;
                    break;

                case "GET?":
                    if (!(string.IsNullOrEmpty(OpControl.SelectedBinkId)))
                        try
                        {
                            answer = ((int)OpControl.SelectedBinkSumTime.TotalHours).ToString();    
                        }
                        catch (Exception ex)
                        {
                            answer = AnswFail + " " + ex.Message + ". Информация о данном комплекте не будет отображена.";
                        }
                    else
                        answer = AnswFail;
                    break;

                case "STOP":
                    if (OpControl.BinkCounterStarted == true)
                    {
                        try
                        {
                            OpControl.StopBink();
                            answer = "OK";
                        }
                        catch (Exception ex)
                        {
                            answer = AnswFail + ex.Message;
                        }
                    }
                    else
                        answer = AnswOk;
                    break;

                default:
                    answer = UnknownCommand;
                    break;
            }

            return answer;
        }

        #endregion


    }



}