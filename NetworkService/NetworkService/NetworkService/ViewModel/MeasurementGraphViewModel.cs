using MVVMLight.Messaging;
using NetworkService.Assets;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public  class MeasurementGraphViewModel:BindableBase
    {
        #region Lists
        public ObservableCollection<Server> Servers { get; set; }
        #endregion

        #region Selected Server
        private Server selectedServer;

        public Server SelectedServer
        {
            get
            {
                return selectedServer;
            }
            set
            {
                if (value != selectedServer)
                {
                    selectedServer = value;
                    OnPropertyChanged(nameof(SelectedServer));

                    if (value != null)
                    {
                        resetTheGraph();

                        readValuesFromTxt(value.Identificator.ToString());
                        OnPropertyChanged(nameof(FirstCircle));
                        OnPropertyChanged(nameof(SecondCircle));
                        OnPropertyChanged(nameof(ThirdCircle));
                        OnPropertyChanged(nameof(FourthCircle));
                        OnPropertyChanged(nameof(FifthCircle));
                    }
                }
            }
        }
        #endregion

        #region Commands
        public MyICommand ComboBoxCheck { get; set; }

        private void checkServer()
        {
            if (Servers.Count == 0)
            {
                Messenger.Default.Send<NotificationContent>(CreateNoEntityToastNotification());
            }
        }
        #endregion

        #region NotificationCreators
        private NotificationContent CreateNoEntityToastNotification()
        {
            var notificationContent = new NotificationContent
            {
                Title = "No Available Entity",
                MessageTextSettings = new Notification.Wpf.Base.TextContentSettings
                {
                    FontSize = 17,
                    FontWeight = FontWeights.SemiBold,
                    TextAlignment = TextAlignment.Center
                },
                TitleTextSettings = new Notification.Wpf.Base.TextContentSettings
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,

                },
                Message = "There is no entity in the system",
                Type = NotificationType.None,
                TrimType = NotificationTextTrimType.NoTrim,
                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),
                CloseOnClick = true,
            };

            return notificationContent;
        }
        #endregion

        #region GraphCircles
        private GraphCircle firstCircle;
        private GraphCircle secondCircle;
        private GraphCircle thirdCircle;
        private GraphCircle fourthCircle;
        private GraphCircle fifthCircle;

        public GraphCircle FirstCircle { get => firstCircle; set => firstCircle = value; }
        public GraphCircle SecondCircle { get => secondCircle; set => secondCircle = value; }
        public GraphCircle ThirdCircle { get => thirdCircle; set => thirdCircle = value; }
        public GraphCircle FourthCircle { get => fourthCircle; set => fourthCircle = value; }
        public GraphCircle FifthCircle { get => fifthCircle; set => fifthCircle = value; }
        #endregion

        #region Constructors
        public MeasurementGraphViewModel(ObservableCollection<Server> servers)
        {
            Servers = servers;
            FirstCircle = new GraphCircle();
            SecondCircle = new GraphCircle();
            ThirdCircle = new GraphCircle();
            FourthCircle = new GraphCircle();
            FifthCircle = new GraphCircle();

            ComboBoxCheck = new MyICommand(checkServer);

            Messenger.Default.Register<Server>(this, updateTheGraph);
        }
        #endregion

        #region UpdateTheGraph
        private void updateTheGraph(Server server)
        {
            if (server != null)
            {
                if (Servers.Contains(server))
                {

                    if (server == selectedServer)
                    {
                        readValuesFromTxt(server.Identificator.ToString());
                        OnPropertyChanged(nameof(FirstCircle));
                        OnPropertyChanged(nameof(SecondCircle));
                        OnPropertyChanged(nameof(ThirdCircle));
                        OnPropertyChanged(nameof(FourthCircle));
                        OnPropertyChanged(nameof(FifthCircle));
                    }
                }
                else
                {
                    if (Servers.Count == 0) { 
                    
                        resetTheGraph();
                    }
                }
            }
        }
        #endregion

        #region ResetTheGraph
        private void resetTheGraph()
        {
            FirstCircle = new GraphCircle();
            SecondCircle = new GraphCircle();
            ThirdCircle = new GraphCircle();
            FourthCircle = new GraphCircle();
            FifthCircle = new GraphCircle();

            OnPropertyChanged(nameof(FirstCircle));
            OnPropertyChanged(nameof(SecondCircle));
            OnPropertyChanged(nameof(ThirdCircle));
            OnPropertyChanged(nameof(FourthCircle));
            OnPropertyChanged(nameof(FifthCircle));      
        }
        #endregion

        #region ReadValuesFromTxt
        private void readValuesFromTxt(string id)
        {
            string[] lines = File.ReadAllLines("Log.txt");

            int findedValue = 0;

            foreach (string s in lines.Reverse())
            {
                string[] parts=s.Split(' ');
                string[] timeIdPartupdate = parts[1].Split('|');

                if (timeIdPartupdate[1].Equals(id))
                {

                    if (timeIdPartupdate[3].Equals("DELETE"))
                    {
                        break;
                    }

                    GraphCircle circle = new GraphCircle();

                    circle.Value = timeIdPartupdate[2] + "%";
                    circle.Time = timeIdPartupdate[0];
                    circle.Radius = (double)int.Parse(timeIdPartupdate[2]) * 0.56;
                    circle.Visibility = Visibility.Visible;

                    if (int.Parse(timeIdPartupdate[2])<=45 || int.Parse(timeIdPartupdate[2]) >= 75)
                    {
                        circle.Warning = true;
                    }

                    if (findedValue == 0)
                    {
                        FirstCircle = circle;
                    } 
                    else if (findedValue == 1)
                    {
                        SecondCircle = FirstCircle;
                        FirstCircle = circle;                        
                    } 
                    else if (findedValue == 2)
                    {
                        ThirdCircle = SecondCircle;
                        SecondCircle = FirstCircle;
                        FirstCircle = circle;                        
                    }
                    else if (findedValue == 3)
                    {
                        FourthCircle = ThirdCircle;
                        ThirdCircle = SecondCircle;
                        SecondCircle = FirstCircle;
                        FirstCircle = circle;
                    }
                    else if(findedValue==4)
                    {
                        FifthCircle = FourthCircle;
                        FourthCircle = ThirdCircle;
                        ThirdCircle = SecondCircle;
                        SecondCircle = FirstCircle;
                        FirstCircle = circle;
                        break;
                    }

                    findedValue++;

                    if (findedValue == 5)
                    {
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
