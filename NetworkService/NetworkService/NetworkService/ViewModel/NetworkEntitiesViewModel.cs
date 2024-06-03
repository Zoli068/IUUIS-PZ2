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
using System.Reflection;
using System.Xml.Linq;
using System.Threading;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel:BindableBase
    {       
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
        public MyICommand ApplyFilter { get; set; } 
        public MyICommand AddEntity { get; set; }
        public MyICommand ShowAdd { get; set; }
        public MyICommand HideTheTabs { get; set; }
        public MyICommand<Server> AddToDelete { get; set; }
        public MyICommand RemoveSelected { get; set; }
        public MyICommand AbortDelete { get; set; }
        public MyICommand TrueRemoveSelected { get; set; }
        public MyICommand<string> KeyboardButtonPress { get; set; }
        public MyICommand HideKeyboard { get; set; }
        public MyICommand<string> FocusTextBox { get; set; }
        #endregion

        #region Command Functions

        #region Keyboard Typing
        private bool lastShift = false;

        private void TypingWithKeyboard(string value)
        {
            PropertyInfo propertyInfo=null;
            string oldValue;

            if (!PropertyNameToType.Equals("AddServer.IpAddress") && !PropertyNameToType.Equals("AddServer.Name"))
            {
                 propertyInfo = GetType().GetProperty(PropertyNameToType);
                 oldValue = propertyInfo.GetValue(this) as string;
            }
            else
            {
                if (PropertyNameToType.Equals("AddServer.IpAddress"))
                {
                    oldValue = addServer.IpAddress;
                }
                else
                {
                    oldValue = addServer.Name;
                }
            }

            if (value.Equals("SPACE"))
            {
                oldValue += " ";
                lastShift = false;
            }
            else if (value.Equals("ENTER"))
            {
                
            }
            else if (value.Equals("BACKSPACE"))
            {
                if (oldValue.Length > 0)
                {
                    oldValue=oldValue.Remove(oldValue.Length - 1);
                }
                lastShift = false;
            }
            else if (value.Equals("Shift"))
            {
                    if(LowerCaseKeyboardVisibility==Visibility.Visible && UpperCaseKeyboardVisibility == Visibility.Hidden)
                    {
                            LowerCaseKeyboardVisibility = Visibility.Hidden;
                            UpperCaseKeyboardVisibility = Visibility.Visible;                    
                    }
                    else 
                    {
                            LowerCaseKeyboardVisibility = Visibility.Visible;
                            UpperCaseKeyboardVisibility = Visibility.Hidden;
                    }

                    if (lastShift)
                    {
                        lastShift = false;
                    }
                    else
                    {
                        lastShift = true;
                    }
            }
            else if (value.Equals("CAPS_LOCK"))
            {
                lastShift = false;
                if (CapsLockIndiciator == Visibility.Visible)
                {
                    CapsLockIndiciator = Visibility.Hidden;
                    LowerCaseKeyboardVisibility = Visibility.Visible;
                    UpperCaseKeyboardVisibility = Visibility.Hidden;                    
                }
                else
                {
                    CapsLockIndiciator=Visibility.Visible;
                    LowerCaseKeyboardVisibility = Visibility.Hidden;
                    UpperCaseKeyboardVisibility = Visibility.Visible;
                }
            }
            else
            {           
                oldValue += value;

                if (lastShift)
                {
                    if (LowerCaseKeyboardVisibility == Visibility.Visible && UpperCaseKeyboardVisibility == Visibility.Hidden)
                    {
                        LowerCaseKeyboardVisibility = Visibility.Hidden;
                        UpperCaseKeyboardVisibility = Visibility.Visible;
                    }
                    else
                    {
                        LowerCaseKeyboardVisibility = Visibility.Visible;
                        UpperCaseKeyboardVisibility = Visibility.Hidden;
                    }
                    lastShift = false;
                }           
            }

            if (!PropertyNameToType.Equals("AddServer.IpAddress") && !PropertyNameToType.Equals("AddServer.Name"))
            {
                propertyInfo.SetValue(this, oldValue);
            }
            else
            {
                if (PropertyNameToType.Equals("AddServer.IpAddress"))
                {
                    addServer.IpAddress = oldValue;
                }
                else
                {
                    addServer.Name = oldValue;
                }
            }

            if (value.Equals("ENTER"))
            {
                HideKeyboardFunc();
                lastShift = false;
            }
        }

        private void HideKeyboardFunc()
        {
            KeyBoardVisibility = false;
            LowerCaseKeyboardVisibility = Visibility.Hidden; 
            UpperCaseKeyboardVisibility=Visibility.Hidden;
            CapsLockIndiciator = Visibility.Hidden;
            PropertyNameToType = "";
        }

        private void FocusTheTextBox(string propertyName) 
        {
            PropertyNameToType = propertyName;
            KeyBoardVisibility = true;
            LowerCaseKeyboardVisibility = Visibility.Visible;
            UpperCaseKeyboardVisibility=Visibility.Hidden;
        }
        #endregion

        #region Filter Functions
        private void ResetFilterValues()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
            dispatcherTimer_Tick(null,null);

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

        private void TableFilter()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Start();

            int id;

            List<Server> tempServers = new List<Server>();
            List<Server> tempServers2 = new List<Server>();

            foreach (Server s in FilteredServers)
            {
                tempServers2.Add(s);
            }

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
            else
            {
                if (id_Filter != null)
                {
                    if (Id_Filter.Equals(""))
                    {
                        ActiveFilter = Visibility.Hidden;
                    }
                }
            }

            FilteredServers.Clear();

            foreach(Server s in tempServers)
            {
                FilteredServers.Add(s);
            }

            int pom;
            if (Id_Filter != null)
            {
                if (!Id_Filter.Equals(""))
                {
                    if(!int.TryParse(Id_Filter,out pom))
                    {
                        ActiveFilter = Visibility.Hidden;

                        FilteredServers.Clear();

                        foreach (Server s in tempServers2)
                        {
                            FilteredServers.Add(s);
                        }
                    }
                }
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
        #endregion

        #region Delete Functions
        private void AbbortDeleteAttempt()
        {
            OpacityBackGround = false;
            DeleteConfirm = Visibility.Collapsed;
        }

        private void TrueRemoveList()
        {
            OpacityBackGround = false;
            DeleteConfirm = Visibility.Collapsed;

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
                LogWriter(s);
                Messenger.Default.Send<Server>(s);
            }
            HideTheSideTabs();
            ToDeleteList.Clear();
            Messenger.Default.Send<NotificationContent>(CreateDeleteToastNotification());
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
        #endregion

        #region Add Functions     
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
        #endregion

        #region Hide/Show Tabs Functions
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

        #region Changing The View State
        private void ChangingTheViewState(string updateMessage)
        {
            if (updateMessage.Equals("DefaultEntityTable"))
            {
                HideTheSideTabs();
                HideKeyboardFunc();
            }
            else if (updateMessage.Equals("AddEntityTable"))
            {
                HideTheSideTabs();
                ShowAddTab();
            }else if (updateMessage.Equals("FilterEntityTable")){
                HideTheSideTabs();
                ShowFilterTab();
            }
        }
        #endregion

        #endregion

        #region Values For KeyBoard
        private Visibility lowerCaseKeyboardVisibility = Visibility.Hidden;
        private Visibility upperCaseKeyboardVisibility = Visibility.Hidden;
        private Visibility capsLockIndiciator = Visibility.Hidden;
        private string propertyNameToType;
        private bool keyBoardVisibility=false;

        public bool KeyBoardVisibility
        {
            get { return keyBoardVisibility; }
            set
            {
                if (value != keyBoardVisibility)
                {
                    keyBoardVisibility = value;
                    OnPropertyChanged(nameof(KeyBoardVisibility));
                }
            }
        }

        public string PropertyNameToType
        {
            get
            {
                return propertyNameToType;
            }
            set
            {
                if (value != propertyNameToType)
                {
                    propertyNameToType = value;
                    OnPropertyChanged(nameof(PropertyNameToType));
                }
            }
        }
        
        public Visibility LowerCaseKeyboardVisibility
        {
            get
            {
                return lowerCaseKeyboardVisibility;
            }
            set
            {
                if (value != lowerCaseKeyboardVisibility)
                {
                    lowerCaseKeyboardVisibility = value;
                    OnPropertyChanged(nameof(LowerCaseKeyboardVisibility));
                }
            }
        }

        public Visibility UpperCaseKeyboardVisibility
        {
            get
            {
                return upperCaseKeyboardVisibility;
            }
            set
            {
                if (value != upperCaseKeyboardVisibility)
                {
                    upperCaseKeyboardVisibility = value;
                    OnPropertyChanged(nameof(UpperCaseKeyboardVisibility));
                }
            }
        }

        public Visibility CapsLockIndiciator
        {
            get
            {
                return capsLockIndiciator; 
            }
            set
            {
                if(value!= capsLockIndiciator)
                {
                    capsLockIndiciator = value;
                    OnPropertyChanged(nameof(CapsLockIndiciator));
                }
            }
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
        private string id_Filter = "";
        private string invalidIdFilter;
        private Visibility activeFilter = Visibility.Hidden;
        private bool lessThan;
        private bool equal;
        private bool greaterThan;
        private ServerType serverTypeFilter;

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
        private string idAddServer="";
        private string invalidIdAddServer="";

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
        private bool addTabVisibility = false;
        private bool filterTabVisibility = false;
       
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

            Servers = servers;
            ServerTypes =serverTypes;
            FilterServerTypes = new List<ServerType>();
            FilteredServers = new ObservableCollection<Server>();
            ToDeleteList= new List<Server>();

            AddEntity = new MyICommand(OnAdd);
            ShowAdd = new MyICommand(ShowAddTab);
            ShowFilter = new MyICommand(ShowFilterTab);
            ResetFilter = new MyICommand(ResetFilterValues);
            ApplyFilter = new MyICommand(TableFilter);
            TrueRemoveSelected = new MyICommand(TrueRemoveList);
            AddToDelete = new MyICommand<Server>(AddToDeleteList);
            AbortDelete = new MyICommand(AbbortDeleteAttempt);
            RemoveSelected = new MyICommand(RemoveSelectedServers);
            HideTheTabs = new MyICommand(HideTheSideTabs);
            KeyboardButtonPress = new MyICommand<string>(TypingWithKeyboard);
            FocusTextBox = new MyICommand<string>(FocusTheTextBox);
            HideKeyboard = new MyICommand(HideKeyboardFunc);

            LoadData();

            Messenger.Default.Register<string>(this, ChangingTheViewState);
        }
        #endregion

        #region DispatcherTimer
        private DispatcherTimer dispatcherTimer;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            InvalidIdAddServer = "";
            InvalidIdFilter = "";

            AddServer.ValidationErrors["Address"] = "";
            AddServer.ValidationErrors["Name"] = "";
            AddServer.Refresh();
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
                TitleTextSettings=new Notification.Wpf.Base.TextContentSettings {
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
                Message = "No entities selected for delete",
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

        #region LogWriter
        private void LogWriter(Server server)
        {
            using (StreamWriter sr = File.AppendText("Log.txt"))
            {
                DateTime dateTime = DateTime.Now;
                sr.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}|{server.Identificator}|{server.Usage}|DELETE|");
            }
        }
        #endregion
    }
}
