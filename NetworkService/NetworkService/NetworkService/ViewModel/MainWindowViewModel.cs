using MVVMLight.Messaging;
using NetworkService.Assets;
using NetworkService.Model;
using NetworkService.Views;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel: BindableBase
    {
        #region Lists
        public ObservableCollection<Server> Servers;
        public List<ServerType> ServerTypes;
        public Stack<string> DestinationHistory;
        #endregion

        #region Views
        private BindableBase currentViewModel;
        private HomeViewModel HomeViewModel;
        private NetworkEntitiesViewModel NetworkEntitiesViewModel;
        private MeasurementGraphViewModel MeasurementGraphViewModel;
        private NetworkDisplayViewModel NetworkDisplayViewModel;

        public BindableBase CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }

            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }
        #endregion

        #region ToastNotification
        private NotificationManager notificationManager;

        private void ShowToastNotification(NotificationContent notificationContent)
        {
            notificationManager.Show(notificationContent, "WindowNotificationArea", ShowXbtn: false,expirationTime: new TimeSpan(0,0,3));
        }
        #endregion

        #region Constructor,OnLoad
        public MainWindowViewModel()
        {
            Servers=new ObservableCollection<Server>();
            ServerTypes=new List<ServerType>();
            DestinationHistory = new Stack<string>();

            OnLoad();

            HomeViewModel = new HomeViewModel();
            NetworkEntitiesViewModel=new NetworkEntitiesViewModel(Servers, ServerTypes);
            MeasurementGraphViewModel = new MeasurementGraphViewModel(Servers);
            NetworkDisplayViewModel = new NetworkDisplayViewModel(Servers);
           
            NavCommand = new MyICommand<string>(OnNav);
            ShowShortCut = new MyICommand(ShowShortCutTab);
            ChangeView = new MyICommand<string>(ChangeTheTableView);
            Undo = new MyICommand(backToLastView);

            createListener();
            
            CurrentViewModel=HomeViewModel;
            
            notificationManager = new NotificationManager();
            Messenger.Default.Register<NotificationContent>(this, ShowToastNotification);
        }

        private void OnLoad()
        {
            ServerType serverType = new ServerType("Web Server", "slika1");
            serverType.Path = "/Resources/Images/WebServer.png";
            ServerTypes.Add(serverType);

            serverType = new ServerType("File Server", "slika1");
            serverType.Path = "/Resources/Images/FileServer.png";
            ServerTypes.Add(serverType);

            serverType = new ServerType("Database Server", "slika1");
            serverType.Path = "/Resources/Images/DataBaseServer.png";
            ServerTypes.Add(serverType);

            DestinationHistory.Push("home");

            //using at development
            if (false)
            {
                Server s1 = new Server();
                s1.Name = "Oracle";
                s1.Identificator = 1;
                s1.IpAddress = "127.0.0.1";
                s1.Usage = 79;
                s1.Type = ServerTypes.ElementAt(0);

                Server s2 = new Server();
                s2.Name = "Steam";
                s2.Identificator = 2;
                s1.Usage = 100;
                s2.IpAddress = "127.0.0.1";
                s2.Type = ServerTypes.ElementAt(0);

                Server s3 = new Server();
                s3.Name = "www wwww23wwww";
                s3.Identificator = 123;
                s3.Usage = 2;
                s3.IpAddress = "127.110.220.111";
                s3.Type = ServerTypes.ElementAt(2);

                Server s4 = new Server();
                s4.Name = "Microsoft";
                s4.Identificator = 14;
                s4.Usage = 50;
                s4.IpAddress = "127.0.0.1";
                s4.Type = ServerTypes.ElementAt(1);

                Server s5 = new Server();
                s5.Name = "Ubisoft";
                s5.Identificator = 13;
                s4.Usage = 50;
                s5.IpAddress = "192.0.0.1";
                s5.Type = ServerTypes.ElementAt(2);

                Server s6 = new Server();
                s6.Name = "Riot games";
                s6.Identificator = 7;
                s6.Usage = 50;
                s6.IpAddress = "127.0.120.1";
                s6.Type = ServerTypes.ElementAt(0);

                Servers.Add(s1);
                Servers.Add(s2);
                Servers.Add(s3);
                Servers.Add(s4);
                Servers.Add(s5);
                Servers.Add(s6);
            }
        }
        #endregion

        #region ShortCut Tab Values

        private bool shortCutTabVisibility = false;
        
        public bool ShortCutTabVisibility
        {
            get
            {
                return shortCutTabVisibility;
            }
            set
            {
                if (value != shortCutTabVisibility)
                {
                    shortCutTabVisibility = value;
                    OnPropertyChanged(nameof(ShortCutTabVisibility));
                }
            }
        }

        #endregion

        #region Commands
        public MyICommand ShowShortCut { get; set; }
        public MyICommand Undo { get; set; }
        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<string> ChangeView { get; set; }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "home":

                    if (!CurrentViewModel.Equals(HomeViewModel))
                    {
                        DestinationHistory.Push(destination);
                    }

                    CurrentViewModel = HomeViewModel;
                    break;

                case "entitiesView":

                    if (!CurrentViewModel.Equals(NetworkEntitiesViewModel))
                    {
                        DestinationHistory.Push(destination);
                    }

                    CurrentViewModel = NetworkEntitiesViewModel;
                    break;

                case "measurementGraph":

                    if (!CurrentViewModel.Equals(MeasurementGraphViewModel))
                    {
                        DestinationHistory.Push(destination);
                    }

                    CurrentViewModel = MeasurementGraphViewModel;
                    break;

                case "networkDisplay":

                    if (!CurrentViewModel.Equals(NetworkDisplayViewModel))
                    {
                        DestinationHistory.Push(destination);
                    }

                    CurrentViewModel = NetworkDisplayViewModel;
                    break;
            }
            ShortCutTabVisibility=false;
        }

        public void ShowShortCutTab()
        {
            if (!shortCutTabVisibility)
            {
                ShortCutTabVisibility = true;
            }
            else
            {
                ShortCutTabVisibility = false;
            }
        }

        private void backToLastView()
        {
            if(DestinationHistory.Count > 1)
            {
                string destination = DestinationHistory.Pop();

                switch (destination)
                {
                    case "home":
                        if (CurrentViewModel.Equals(HomeViewModel))
                        {
                            destination= DestinationHistory.Pop();
                        }
                        Messenger.Default.Send<string>("DefaultEntityTable");
                        break;

                    case "entitiesView":

                        if (CurrentViewModel.Equals(NetworkEntitiesViewModel))
                        {
                            destination = DestinationHistory.Pop();
                        }
                        break;

                    case "measurementGraph":

                        if (CurrentViewModel.Equals(MeasurementGraphViewModel))
                        {
                            destination = DestinationHistory.Pop();
                        };
                        break;

                    case "networkDisplay":

                        if (CurrentViewModel.Equals(NetworkDisplayViewModel))
                        {
                            destination = DestinationHistory.Pop();
                        }
                        break;
                }

                switch (destination)
                {
                    case "home":
                        Messenger.Default.Send<string>("DefaultEntityTable");
                        CurrentViewModel = HomeViewModel;
                        break;

                    case "entitiesView":
                        CurrentViewModel = NetworkEntitiesViewModel;
                        break;

                    case "measurementGraph":
                        CurrentViewModel = MeasurementGraphViewModel;
                        break;

                    case "networkDisplay":
                        CurrentViewModel = NetworkDisplayViewModel;
                        break;
                }
            }
            else if (DestinationHistory.Count == 1) 
            {
                CurrentViewModel = HomeViewModel;
            }
        }

        private void ChangeTheTableView(string state)
        {
            Messenger.Default.Send<string>(state);

            if(state.Equals("DefaultEntityTable") || state.Equals("AddEntityTable") || state.Equals("FilterEntityTable"))
            {
                OnNav("entitiesView");
                if (ShortCutTabVisibility)
                {
                    shortCutTabVisibility = false;
                }
            }
        }
        #endregion

        #region Connection to the Simulator
        public Thread listenToSimulator;

        public void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

             listenToSimulator = new Thread(() =>
             {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);

                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        if (incomming.Equals("Need object count"))
                        {
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntitiesViewModel.Servers.Count().ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            Console.WriteLine(incomming); 

                            string[] messageParts=incomming.Split(new string[] {"_",":"},StringSplitOptions.None);

                            if (int.Parse(messageParts[1]) < NetworkEntitiesViewModel.Servers.Count())
                            {
                                 Servers.ElementAt(int.Parse(messageParts[1])).Usage = int.Parse(messageParts[2]);
                                 Messenger.Default.Send<Server>(Servers.ElementAt(int.Parse(messageParts[1])));

                                LogWriter(Servers.ElementAt(int.Parse(messageParts[1])));
                            }
                        }
                    }, null);
                }
             });

            listenToSimulator.IsBackground = true;
            listenToSimulator.Start();
        }
        #endregion

        #region LogWriter
        private void LogWriter(Server server)
        {
            using (StreamWriter sr = File.AppendText("Log.txt"))
            {
                DateTime dateTime = DateTime.Now;
                sr.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}|{server.Identificator}|{server.Usage}|UPDATE|");
            }
        }
        #endregion
    }
}
