using MVVM1;
using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
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
        private BindableBase currentViewModel;
        private HomeViewModel homeViewModel= new HomeViewModel();
        private NetworkEntitiesViewModel NetworkEntitiesViewModel=new NetworkEntitiesViewModel();

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

        public MyICommand<string> NavCommand { get; private set; }

        public MainWindowViewModel()
        {
            NavCommand = new MyICommand<string>(OnNav);
            createListener();
            
            CurrentViewModel = homeViewModel;
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
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

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntitiesViewModel.Servers.Count().ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); 


                            string[] messageParts=incomming.Split(new string[] {"_",":"},StringSplitOptions.None);

                            foreach(Server s in NetworkEntitiesViewModel.Servers)
                            {
                                if (s.Identificator.ToString() == messageParts[1])
                                {
                                    s.Usage = int.Parse(messageParts[2]);
                                    LogWriter(s);
                                    break;
                                }
                            }
             
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
        
        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "home":
                    CurrentViewModel = homeViewModel;
                    break;
                case "entitiesView":
                    CurrentViewModel = NetworkEntitiesViewModel;
                    break;                
                case "networkDisplay":
                    //CurrentViewModel = networkDisplayViewModel;
                    break;
            }
        }

        private void LogWriter(Server server)
        {
            using (StreamWriter sr = File.AppendText("Log.txt"))
            {
                DateTime dateTime = DateTime.Now;
                sr.WriteLine($"{DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss")}|{server.Identificator}|{server.Usage}");
            }
        }

    }
}
