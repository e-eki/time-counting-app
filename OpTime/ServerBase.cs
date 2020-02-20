using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpTime
{
    /// <summary>Класс для работы с соединением по TCP\IP в качестве сервера для работы с единственным клиентом.</summary>
    public abstract class ServerBase
    {
        int port;
        /// <summary>Использьзуемый порт TCP/IP.</summary>
        public int Port 
        { 
            get { return port; }
            protected set { port = value; }
        }

        protected string Name;

        /// <summary>Событие изменения состояния подключения.</summary>
        public event ServerStateChangedHandler StateChanged;

        ServerState state = ServerState.Stop;
        /// <summary>Состояние: отключен, ожидает, подключен.</summary>
        public ServerState State
        {
            get { return state; }
            private set
            {
                if (state != value)
                {
                    state = value;

                    switch (state)
                    {
                        case ServerState.Stop:
                            StateString = "Остановлено";
                            break;
                        case ServerState.Waiting:
                            StateString = "Ожидание подключения";
                            break;
                        case ServerState.Connected:
                            StateString = "Подключено";
                            if (client.Client.RemoteEndPoint is IPEndPoint)
                                StateString += ": " + ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                            else if (client.Client.RemoteEndPoint is DnsEndPoint)
                                StateString += ": " + ((DnsEndPoint)client.Client.RemoteEndPoint).Host;
                            break;
                        case ServerState.Error:
                            StateString = "Ошибка работы";
                            break;
                    }

                    //AppController.Current.WriteToLog(Name + ":" +StateString, LogMsgType.Info);

                    if (StateChanged != null)
                        StateChanged(this);
                }
            }
        }

        /// <summary>Строка состояния для отображения на ГИП.</summary>
        public string StateString { get; private set; }

        TcpListener server = null;  // Объект сервера.
        TcpClient client = null;    // Подключенный клиент.
        NetworkStream ns = null;    // Поток для обмена информацией.

        protected NetworkStream NetStream { get { return ns; } }

        Task task;                  // Задача для работы с подклбчением в отдельном потоке.
        bool stopFlag = false;      // Флаг остановки.

        /// <summary>Запуск сервера на прослушивание подключений от клиентов.</summary>
        public void Start()
        {
            if (server != null)
                server.Stop();
            try
            {
                stopFlag = false;
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                task = Task.Run((Action)ServerWork);
            }
            catch
            {
                State = ServerState.Error;
            }
        }

        /// <summary>Остановка сервера.</summary>
        public void Stop()
        {
            if (task != null)
            {
                stopFlag = true;
                if (client != null)
                    client.Close();
                server.Stop();
                task.Wait();

                server = null;
                client = null;
                task.Dispose();
                task = null;

                State = ServerState.Stop;
            }
        }

        // Обработка входящих подключений.
        private void ServerWork()
        {
            while (!stopFlag)
            {
                State = ServerState.Waiting;

                try
                {
                    client = server.AcceptTcpClient();

                    State = ServerState.Connected;

                    using (ns = client.GetStream())
                        ClientWork();

                }
                catch
                { }

                if (client != null)
                    client.Close();
            }
        }

        /// <summary>Целевой обмен с подключенным клиентом.</summary>
        /// <param name="ns">Сетевой поток для взаимодействия.</param>
        protected abstract void ClientWork();
    }

    /// <summary>
    /// Возможные состояния сервера TCP/IP.
    /// </summary>
    public enum ServerState
    {
        /// <summary>Ожидание подключения.</summary>
        Waiting,
        /// <summary>Клиент подключен.</summary>
        Connected,
        /// <summary>Сервер остановлен.</summary>
        Stop,
        /// <summary>Ошибка в работе сервера.</summary>
        Error
    }

    /// <summary>Обработчик для события изменения состояния сервера TCP/IP.</summary>
    /// <param name="server">Сервер, состояние которого изменилось.</param>
    public delegate void ServerStateChangedHandler(ServerBase server);
}
