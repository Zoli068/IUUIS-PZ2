using MVVMLight.Messaging;
using NetworkService.Assets;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel:BindableBase
    {

        #region Lists

        public ObservableCollection<Server> Servers { get; set; }

        public ObservableCollection<ServersByType> ServersByTypes { get; set; }

        public ObservableCollection<ServerDisplayWrapper> ServersAtDisplay { get; set; }

        private List<Server> toDeleteList;
        #endregion

        #region Commands

        public MyICommand<Server> MouseLeftButtonDown { get; set; }
        public MyICommand<string> DropDown {  get; set; }
        public MyICommand AbbortDrag {  get; set; }

        #region Command Functions

        private void DragStarted(Server s)
        {
            DraggedServer= s;
            OpacityForDrag = 1;
            IsDragging =true;
        }

        private void TryToDropDown(string value)
        {
            int num = int.Parse(value);
            ServerDisplayWrapper sdw=null;

            if (DraggedServer == null)
            {
                return;
            }

            foreach(ServerDisplayWrapper sd in ServersAtDisplay)
            {
                if(sd.Index== num)
                {
                    sdw = sd;
                    break;
                }
            }

            sdw.Server = DraggedServer;
            sdw.ServerVisibility=Visibility.Visible;
            OnPropertyChanged(nameof(ServerDisplayWrapper));
            DraggedServer = null;

            return;
        }

        private void ResetTheDropValues() {
            OpacityForDrag = 0;
            IsDragging = false;
        }

        #endregion

        #region DragValues

        private Server draggedServer;
        private bool isDragging;
        private int opacityForDrag;

        public Server DraggedServer 
        {
            get 
            { 
                return draggedServer; 
            }
            set
            {
                if (value != draggedServer)
                {
                    draggedServer = value;
                    OnPropertyChanged(nameof(DraggedServer));
                }
            }

        }

        public int OpacityForDrag
        {
            get
            {
                return opacityForDrag;
            }
            set
            {   
                opacityForDrag = value;
                OnPropertyChanged(nameof(OpacityForDrag));   
            }
        }

        public bool IsDragging
        {
            get
            {
                return isDragging;
            }
            set
            {
                if(value != isDragging)
                {
                    isDragging = value;
                    OnPropertyChanged(nameof(IsDragging));
                }
            }
        }

        #endregion


        #endregion

        #region Update/Remove Server From Lists

        private void UpdateTheLists(NotificationContent content)
        {
            if(content.Message.Equals("Selected entities successfully deleted"))
            {
                removeServerFromDisplay();
            }
            else if(content.Message.Equals("Entity successfully added")) 
            {
                UpdateServerToList();
            }
        }

        private void UpdateServerToList()
        {
            bool postoji;
            foreach(Server server in Servers)
            {
                postoji = false;
                foreach(ServerDisplayWrapper sd in ServersAtDisplay)
                {
                    if (sd.Server == server)
                    {
                        continue;
                    }
                }


                foreach (ServersByType serverByType in ServersByTypes)
                {
                    if (serverByType.Servers.Contains(server))
                    {
                        postoji = true;
                        break;
                    }
                }

                if (postoji)
                {
                    postoji = false;
                }
                else
                {
                    AddServerToList(server);

                }

            }
        }

        private void AddServerToList(Server s)
        {
            if (s.Type.ServerTypeName.Equals("Web Server"))
            {
                ServersByTypes[0].Servers.Add(s);
            }
            else if (s.Type.ServerTypeName.Equals("File Server"))
            {
                ServersByTypes[1].Servers.Add(s);    
            }
            else
            {
                ServersByTypes[2].Servers.Add(s);
            }  
        }

        private void removeServerFromDisplay()
        {

            foreach(ServersByType serverByType in ServersByTypes)
            {
                foreach(Server s in serverByType.Servers)
                {
                    if (!Servers.Contains(s))
                    {
                        toDeleteList.Add(s);
                    }
                }
            }

            foreach (ServerDisplayWrapper sd in ServersAtDisplay)
            {
                if (!Servers.Contains(sd.Server))
                    {
                        toDeleteList.Add(sd.Server);
                    }
            }

            foreach(Server s in toDeleteList)
            {
                foreach (ServersByType serverByType in ServersByTypes)
                {
                    serverByType.Servers.Remove(s);
                }

                foreach (ServerDisplayWrapper sd in ServersAtDisplay)
                {
                    if (sd.Server == s)
                    {
                        sd.Server=null;
                        sd.ServerVisibility = Visibility.Hidden;
                        sd.AlarmVisibility=Visibility.Hidden;
                    }
                }
            }
        }

        #endregion

        #region Constructor

        public NetworkDisplayViewModel(ObservableCollection<Server> Servers)
        {
            this.Servers = Servers;

            ServersByTypes = new ObservableCollection<ServersByType>();
            ServersAtDisplay = new ObservableCollection<ServerDisplayWrapper>();

            ServersByTypes.Add(new ServersByType("Web Servers"));
            ServersByTypes.Add(new ServersByType("File Servers"));
            ServersByTypes.Add(new ServersByType("Database Servers"));

            toDeleteList = new List<Server>();
            //TODO delete at end bcs, at begining we wont have no server in system
            foreach(Server s in Servers)
            {
                AddServerToList(s);
            }


            //creating the empty wrappers for servers
            for(int i = 0; i < 12; i++)
            {
                ServersAtDisplay.Add(new ServerDisplayWrapper(i));
            }

            OpacityForDrag = 0;
            MouseLeftButtonDown = new MyICommand<Server>(DragStarted);
            DropDown = new MyICommand<string>(TryToDropDown);
            AbbortDrag = new MyICommand(ResetTheDropValues);
            Messenger.Default.Register<NotificationContent>(this,UpdateTheLists);
        }

        #endregion
    }
}
