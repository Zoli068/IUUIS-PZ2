using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FontAwesome5;
using System.Threading.Tasks;
using NetworkService.Assets;
using System.Windows.Data;
using System.IO;
using System.Windows;
using Notification.Wpf;
using System.Windows.Media;
using MVVMLight.Messaging;
using System.ComponentModel;
using System.Windows.Threading;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel:BindableBase
    {
        #region DispatcherTimer

        private DispatcherTimer dispatcherTimer;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            InvalidIdAddServer = "";
            InvalidIdFilter = "";

            AddServer.ValidationErrors["Address"] ="";
            AddServer.ValidationErrors["Name"] = "";
            AddServer.Refresh();
        }

        #endregion

        #region Lists

        public ObservableCollection<Server> Servers { get; set; }
        public ObservableCollection<Server> FilteredServers { get; set; }
        public List<ServerType> ServerTypes { get; set; }
        public List<ServerType> FilterServerTypes { get; set; }
        public List<Server> ToDeleteList { get; set; }

        #endregion

        #region Commands

        public MyICommand ResetFilter { get; set; }
        public MyICommand ShowFilter { get; set; }
        public MyICommand ShowAdd { get; set; }
        public MyICommand HideTheTabs { get; set; }
        public MyICommand ApplyFilter { get; set; } 
        public MyICommand<Server> AddToDelete { get; set; }
        public MyICommand RemoveSelected { get; set; }
        public MyICommand TrueRemoveSelected { get; set; }
        public MyICommand AddEntity { get; set; }
        public MyICommand AbortDelete { get; set; }

        private void AbbortDeleteAttempt()
        {
            OpacityBackGround = false;
            DeleteConfirm = Visibility.Collapsed;
        }

        private void ResetFilterValues()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Start();

            Equal = true;
            Id_Filter = string.Empty;
            ServerTypeFilter = FilterServerTypes.ElementAt(0);

            FilteredServers.Clear();

            foreach(Server s in Servers)
            {
                FilteredServers.Add(s);
            }
            HideTheSideTabs();
            ActiveFilter = Visibility.Hidden;
        }

        private void TrueRemoveList()
        {
            Messenger.Default.Send<NotificationContent>(CreateDeleteToastNotification());

            OpacityBackGround = false;
            DeleteConfirm =Visibility.Collapsed;

            foreach (Server s in ToDeleteList)
            {
                if (Servers.Contains(s))
                {
                    Servers.Remove(s);
                }

                if (FilteredServers.Contains(s))
                {
                    FilteredServers.Remove(s);
                }
            }
            HideTheSideTabs();
            ToDeleteList.Clear();
        }

        private void TableFilter()
        {

            dispatcherTimer.Stop();
            dispatcherTimer.Start();

            int id;
            List<Server> tempServers = new List<Server>();

            FilteredServers.Clear();
            foreach (Server s in Servers)
            {
                FilteredServers.Add(s);
            }


            if (Id_Filter != null)
            {
                if (!id_Filter.Equals(string.Empty))
                {
                    if ( int.TryParse(Id_Filter,out id))
                    {
                        InvalidIdFilter = "";
                        ActiveFilter = Visibility.Visible;
                        if (Equal)
                        {
                            foreach(Server s in Servers)
                            {
                                if (s.Identificator != id)
                                {
                                    FilteredServers.Remove(s);
                                }
                            }
                        }else if (GreaterThan)
                        {
                            foreach (Server s in Servers)
                            {
                                if (s.Identificator <= id)
                                {
                                    FilteredServers.Remove(s);
                                }
                            }
                        }else if (LessThan)
                        {
                            foreach (Server s in Servers)
                            {
                                if (s.Identificator >= id)
                                {
                                    FilteredServers.Remove(s);
                                }
                            }
                        }
                    }
                    else
                    {
                        InvalidIdFilter = "Value must be a number";
                    }
                }
                else
                {
                    InvalidIdFilter = "";
                }

            }

            foreach (Server s in FilteredServers)
            {
                tempServers.Add(s);
            }

            if (!ServerTypeFilter.ServerTypeName.Equals("All Types"))
            {
                ActiveFilter = Visibility.Visible;
                foreach (Server s in FilteredServers)
                {
                    if (!s.Type.ServerTypeName.Equals(ServerTypeFilter.ServerTypeName))
                    {
                        tempServers.Remove(s);
                    }
                }
            }
            FilteredServers.Clear();

            foreach(Server s in tempServers)
            {
                FilteredServers.Add(s);
            }

            if(invalidIdFilter != null) 
            { 
                if(invalidIdFilter.Equals("") )
                {
                    HideTheSideTabs();

                }
            }
            else
            {
                HideTheSideTabs();
            }

        }


        public void AddToDeleteList(Server server)
        {
            if (ToDeleteList.Contains(server))
            {
                ToDeleteList.Remove(server);
            }
            else
            {
                ToDeleteList.Add(server);
            }
        }

        public void RemoveSelectedServers()
        {
            if (ToDeleteList.Count > 0)
            {
                OpacityBackGround = true;
                DeleteConfirm = Visibility.Visible;
            }
            else
            {
                Messenger.Default.Send<NotificationContent>(CreateUnsuccessfullDeleteToastNotification());
            }

        }

        private void OnAdd()
        {
            int id;

            if (IdAddServer==null ||IdAddServer.Equals(string.Empty))
            {
                InvalidIdAddServer = "Identificator is required.";
                id = -1;
            }
            else
            {
                if(int.TryParse(IdAddServer,out id))
                {
                    if (id < 1)
                    {
                        id = -1;
                        InvalidIdAddServer = "Must be a positive number";
                    }
                    else
                    {
                        InvalidIdAddServer = "";

                        if (id > 999)
                        {
                            id = -1;
                            InvalidIdAddServer = "Identificator can't exceed 999";
                        }
                        else
                        {
                            foreach(Server s in Servers)
                            {
                                if (s.Identificator.Equals(id))
                                {
                                    id = -1;
                                    InvalidIdAddServer="Identificator already in use";
                                    break;
                                }
                            } 
                        }

                    }
                }
                else
                {
                    id = -1;
                    InvalidIdAddServer = "Must be a positive number";
                }
            }

            AddServer.Validate();
            if (AddServer.IsValid && id != -1) 
            {
                Server server = new Server();
                server.Identificator = int.Parse(IdAddServer);
                server.Name=AddServer.Name;
                server.IpAddress=AddServer.IpAddress;
                server.Type=AddServer.Type;

                Servers.Add(server);
                TableFilter();

                IdAddServer = "";
                AddServer.Name = "";
                AddServer.IpAddress = "";
                AddServer.Type = ServerTypes.ElementAt(0);

                Messenger.Default.Send<NotificationContent>(CreateSuccessToastNotification());
                HideTheSideTabs();
            }
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        private void HideTheSideTabs()
        {
            AbbortDeleteAttempt();
            OpacityBackGround = false;
            AddTabVisibility = false;            
            FilterTabVisibility = false;
        }

        private void ShowAddTab()
        {
            OpacityBackGround= true;
            AddTabVisibility = true;            
        }

        private void ShowFilterTab()
        {
            OpacityBackGround= true;
            FilterTabVisibility = true;
        }

        #endregion

        #region Values For Delete

        private Visibility deleteConfirm = Visibility.Hidden;

        public Visibility DeleteConfirm
        {
            get
            {
                return deleteConfirm;
            }

            set
            {
                deleteConfirm = value;
                OnPropertyChanged(nameof(DeleteConfirm));
            }
        }

        #endregion

        #region Values For Filter

        private string id_Filter;

        public string Id_Filter
        {
            get 
            {
                return id_Filter;
            }
            set
            {
                if(id_Filter != value)
                {
                    id_Filter = value;
                    OnPropertyChanged(nameof(Id_Filter));
                }
            }

        }

        private Visibility activeFilter = Visibility.Hidden;

        public Visibility ActiveFilter
        {
            get
            {
                return activeFilter;
            }

            set
            {
                if (activeFilter != value)
                {
                    activeFilter = value;
                    OnPropertyChanged(nameof(ActiveFilter));
                }
            }
        }

        private string invalidIdFilter;

        public string InvalidIdFilter
        {
            get
            {
                return invalidIdFilter;
            }
            set
            {
                if (invalidIdFilter != value)
                {
                    invalidIdFilter = value;
                    OnPropertyChanged(nameof(InvalidIdFilter));
                }
            }
        }

        private ServerType serverTypeFilter;

        public ServerType ServerTypeFilter
        {
            get 
            { 
                return serverTypeFilter; 
            }

            set
            {
                if(serverTypeFilter != value)
                {
                    serverTypeFilter = value;
                    OnPropertyChanged(nameof(ServerTypeFilter));
                }
            }

        }

        private bool lessThan;
        private bool equal;
        private bool greaterThan;

        public bool LessThan
        {
            get 
            { 
                return lessThan; 
            }
            set
            {
                if(lessThan != value)
                {
                    lessThan = value;
                    OnPropertyChanged(nameof(LessThan));
                }
            }
        
        }

        public bool Equal
        {
            get
            {
                return equal;
            }
            set
            {
                if (Equal != value)
                {
                    equal = value;
                    OnPropertyChanged(nameof(Equal));
                }
            }

        }

        public bool GreaterThan
        {
            get
            {
                return greaterThan;
            }
            set
            {
                if (greaterThan != value)
                {
                    greaterThan = value;
                    OnPropertyChanged(nameof(GreaterThan));
                }
            }

        }
        #endregion

        #region Values For Add Entity

        private Server addServer = new Server();

        public Server AddServer
        {
            get 
            { 
                return addServer; 
            }
            
            set
            {
                if(addServer!= value)
                {
                    addServer = value;
                    OnPropertyChanged(nameof(AddServer));
                }

            }
        
        }

        private string idAddServer;

        public string IdAddServer
        {
            get
            {
                return idAddServer;
            }
            set
            {
                if(idAddServer!= value)
                {
                    idAddServer = value;
                    OnPropertyChanged(nameof(IdAddServer));
                }
            }
        }

        private string invalidIdAddServer;

        public string InvalidIdAddServer
        {
            get
            {
                return invalidIdAddServer;
            }
            set
            {
                if(invalidIdAddServer != value)
                {
                    invalidIdAddServer = value;
                    OnPropertyChanged(nameof(InvalidIdAddServer));
                }
            }
        }


        #endregion

        #region Values For Hiding Tabs

        private bool opacityBackGround = false;
       
        public bool OpacityBackGround
        {
            get
            {
                return opacityBackGround;
            }

            set
            {
                if(value!=opacityBackGround)
                {
                    opacityBackGround = value;
                    OnPropertyChanged(nameof(OpacityBackGround));
                }
            }
        }

        private bool filterTabVisibility = false;

        public bool FilterTabVisibility
        {
            get
            {
                return filterTabVisibility;
            }

            set
            {
                if (value != filterTabVisibility)
                {
                    filterTabVisibility = value;
                    OnPropertyChanged(nameof(FilterTabVisibility));
                }
            }
        }

        private bool addTabVisibility = false;

        public bool AddTabVisibility
        {
            get
            {
                return addTabVisibility;
            }

            set
            {
                if (value != addTabVisibility)
                {
                    addTabVisibility = value;
                    OnPropertyChanged(nameof(AddTabVisibility));
                }
            }
        }

        #endregion

        #region Constructor

        public NetworkEntitiesViewModel(ObservableCollection<Server> servers,List<ServerType> serverTypes) { 
        
            //Init the lists
            Servers = servers;
            ServerTypes =serverTypes;
            FilterServerTypes = new List<ServerType>();
            FilteredServers = new ObservableCollection<Server>();
            ToDeleteList= new List<Server>();


            //Init the commands
            AddEntity = new MyICommand(OnAdd);
            ShowAdd = new MyICommand(ShowAddTab);
            ShowFilter = new MyICommand(ShowFilterTab);
            ResetFilter = new MyICommand(ResetFilterValues);
            ApplyFilter = new MyICommand(TableFilter);
            TrueRemoveSelected = new MyICommand(TrueRemoveList);
            AddToDelete = new MyICommand<Server>(AddToDeleteList);
            AbortDelete = new MyICommand(AbbortDeleteAttempt);
            HideTheTabs = new MyICommand(HideTheSideTabs);
            RemoveSelected = new MyICommand(RemoveSelectedServers);
            LoadData();
        }

        #endregion

        #region NotificationCreators
        private NotificationContent CreateSuccessToastNotification()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Success",
                MessageTextSettings = new Notification.Wpf.Base.TextContentSettings { 
                                                                                    FontSize = 17,
                                                                                    FontWeight=FontWeights.SemiBold, 
                                                                                    TextAlignment=TextAlignment.Center
                                                                                    },
                TitleTextSettings=new Notification.Wpf.Base.TextContentSettings
                                                                                {
                                                                                FontSize=20  ,  
                                                                                FontWeight=FontWeights.Bold,
                                                                                TextAlignment=TextAlignment.Center,
                                                                                    
                                                                                },
                Message = "Entity successfully added",
                Type = NotificationType.None,
                TrimType = NotificationTextTrimType.NoTrim,
                Background = new SolidColorBrush(Colors.LimeGreen),
                Foreground = new SolidColorBrush(Colors.White),
                CloseOnClick = true,

            };

            return notificationContent;
        }

        private NotificationContent CreateDeleteToastNotification()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Successfully Deleted",
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
                Message = "Selected entities successfully deleted",
                Type = NotificationType.None,
                TrimType = NotificationTextTrimType.NoTrim,
                Background = new SolidColorBrush(Colors.Blue),
                Foreground = new SolidColorBrush(Colors.White),
                CloseOnClick = true,

            };

            return notificationContent;
        }

        private NotificationContent CreateUnsuccessfullDeleteToastNotification()
        {
            var notificationContent = new NotificationContent
            {
                Title = "Unsuccessful delete",
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
                Message = "No entites selected for delete",
                Type = NotificationType.None,
                TrimType = NotificationTextTrimType.NoTrim,
                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),
                CloseOnClick = true,

            };

            return notificationContent;
        }
        #endregion

        #region Loading the ServerTypes

        public void LoadData()
        {

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 6);
            


            Equal = true;

            ServerTypeFilter = new ServerType("All Types", "");
            FilterServerTypes.Add(ServerTypeFilter);

            foreach(ServerType serverType in ServerTypes)
            {
                FilterServerTypes.Add(serverType);
            }

            foreach(Server s in Servers)
            {
                FilteredServers.Add(s);
            }
            
        }

        #endregion
    }
}
