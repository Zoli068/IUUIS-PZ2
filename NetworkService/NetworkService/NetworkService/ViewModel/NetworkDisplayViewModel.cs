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
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel:BindableBase
    {
        #region Lists
        public ObservableCollection<Server> Servers { get; set; }
        public ObservableCollection<ServersByType> ServersByTypes { get; set; }
        public ObservableCollection<ServerDisplayWrapper> ServersAtDisplay { get; set; }
        private List<Server> toDeleteList;
        private List<Point> DisplayEntityPoints;
        public ObservableCollection<ConnectionLine> Lines { get; set; }
        #endregion

        #region Commands
        public MyICommand<Server> MouseLeftButtonDown { get; set; }
        public MyICommand<string> DropDown {  get; set; }
        public MyICommand<string> LineMouseLeftButtonDown {  get; set; }
        public MyICommand<string> LineMouseLeftButtonUp {  get; set; }
        public MyICommand<ServerDisplayWrapper> DeleteFromDisplay {  get; set; }
        public MyICommand AbbortDrag {  get; set; }

        #region Command Functions
        private void DragStarted(Server s)
        {
            DraggedServer= s;
            OpacityForDrag = 1;
            IsDragging = true;
            startIndex = -1;
            endIndex = -1;

            foreach (ServerDisplayWrapper serverDisplay in ServersAtDisplay)
            {
                if (serverDisplay.Server == DraggedServer)
                {
                    OpacityForDrag = 0;
                    break;
                }
            }
        }

        private void TryToDropDown(string value)
        {

            startIndex = -1;
            endIndex = -1;
            int num = int.Parse(value);
            ServerDisplayWrapper sdw=null;
            ServerDisplayWrapper oldWrapper = null;

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

            foreach (ServerDisplayWrapper sd in ServersAtDisplay)
            {
                if (DraggedServer == sd.Server)
                {
                    oldWrapper = sd;
                    break;
                }
            }

            if (oldWrapper==null)
            {
                if (sdw.Server != null)
                {
                    DeleteServerFromDisplay(sdw);
                }
            }
            else
            {
                Point point1 = DisplayEntityPoints.ElementAt(num);
                Point point2 = DisplayEntityPoints.ElementAt(oldWrapper.Index);

                if (ServersAtDisplay.ElementAt(num).Server == null)
                {
                    foreach(ConnectionLine line in Lines)
                    {
                        if(line.X1==point2.X && line.Y1==point2.Y)
                        {
                            line.X1 = point1.X;
                            line.Y1 = point1.Y;
                        }
                        else if(line.X2==point2.X && line.Y2 == point2.Y)
                        {
                            line.X2 = point1.X;
                            line.Y2 = point1.Y;
                        }
                        OnPropertyChanged(nameof(Lines));
                    }

                    //stupid thing but other way won't normally update
                    List<ConnectionLine> tempLines = new List<ConnectionLine>();

                    foreach(ConnectionLine cl in Lines)
                    {
                        tempLines.Add(cl);
                    }

                    Lines.Clear();

                    foreach(ConnectionLine cl in tempLines)
                    {
                        Lines.Add(cl);
                    }

                    ServerDisplayWrapper sdFirst1 = new ServerDisplayWrapper(-1);
                    sdFirst1.Server = oldWrapper.Server;
                    sdFirst1.AlarmVisibility = oldWrapper.AlarmVisibility;
                    sdFirst1.ServerVisibility = oldWrapper.ServerVisibility;

                    oldWrapper.Server = sdw.Server;
                    oldWrapper.AlarmVisibility = sdw.AlarmVisibility;
                    oldWrapper.ServerVisibility = sdw.ServerVisibility;

                    sdw.Server = sdFirst1.Server;
                    sdw.ServerVisibility = sdFirst1.ServerVisibility;
                    sdw.AlarmVisibility = sdFirst1.AlarmVisibility;

                    return;
                }

                foreach(ConnectionLine line in Lines)
                {
                    if(line.X1==point1.X && line.Y1 == point1.Y && line.X2==point2.X && line.Y2==point2.Y)
                    {
                        line.X1 = point2.X;
                        line.Y1 = point2.Y;
                        line.X2 = point1.X;
                        line.Y2 = point1.Y;
                    }
                    else if(line.X1 == point2.X && line.Y1 == point2.Y && line.X2 == point1.X && line.Y2 == point1.Y)
                    {
                        line.X1 = point1.X;
                        line.Y1 = point1.Y;
                        line.X2 = point2.X;
                        line.Y2 = point2.Y;
                    }
                    else if(line.X2 == point1.X && line.Y2 == point1.Y)
                    {
                        line.X2 = point2.X;
                        line.Y2 = point2.Y;
                    }
                    else if(line.X2 == point2.X && line.Y2 == point2.Y)
                    {
                        line.X2 = point1.X;
                        line.Y2 = point1.Y;
                    }
                    else if (line.X1 == point1.X && line.Y1 == point1.Y)
                    {
                        line.X1 = point2.X;
                        line.Y1 = point2.Y;
                    }
                    else if (line.X1 == point2.X && line.Y1 == point2.Y)
                    {
                        line.X1 = point1.X;
                        line.Y1 = point1.Y;
                    }
                }

                //stupid thing but other way won't normally update
                List<ConnectionLine> tempLines2 = new List<ConnectionLine>();

                foreach (ConnectionLine cl in Lines)
                {
                    tempLines2.Add(cl);
                }

                Lines.Clear();

                foreach (ConnectionLine cl in tempLines2)
                {
                    Lines.Add(cl);
                }

                ServerDisplayWrapper sdFirst = new ServerDisplayWrapper(-1);
                sdFirst.Server = oldWrapper.Server;
                sdFirst.AlarmVisibility = oldWrapper.AlarmVisibility;
                sdFirst.ServerVisibility = oldWrapper.ServerVisibility;

                oldWrapper.Server = sdw.Server;
                oldWrapper.AlarmVisibility = sdw.AlarmVisibility;
                oldWrapper.ServerVisibility = sdw.ServerVisibility;

                sdw.Server = sdFirst.Server;
                sdw.ServerVisibility=sdFirst.ServerVisibility;
                sdw.AlarmVisibility = sdFirst.AlarmVisibility;
            }

            sdw.Server = DraggedServer;
            RemoveServerFromList(DraggedServer);
            sdw.ServerVisibility=Visibility.Visible;

            if (DraggedServer.Usage < 45 || DraggedServer.Usage > 75)
            {
                sdw.AlarmVisibility = Visibility.Visible;
            }
            else
            {
                sdw.AlarmVisibility = Visibility.Hidden;
            }

            OnPropertyChanged(nameof(ServerDisplayWrapper));
            DraggedServer = null;

            return;
        }

        private void ResetTheDropValues() {
            OpacityForDrag = 0;
            IsDragging = false;
            startIndex = -1;
            endIndex = -1;
        }


        private void DeleteServerFromDisplay(ServerDisplayWrapper wrapper)
        {
            AddServerToList(wrapper.Server);

            wrapper.AlarmVisibility = Visibility.Hidden;
            wrapper.Server = null;
            wrapper.ServerVisibility = Visibility.Hidden;

            List<ConnectionLine> toDeleteLines = new List<ConnectionLine>();

            foreach(ConnectionLine line in Lines)
            {
                if(line.Y1==DisplayEntityPoints.ElementAt(wrapper.Index).Y && line.X1== DisplayEntityPoints.ElementAt(wrapper.Index).X 
                    || line.Y2 == DisplayEntityPoints.ElementAt(wrapper.Index).Y && line.X2 == DisplayEntityPoints.ElementAt(wrapper.Index).X){

                    toDeleteLines.Add(line);
                }
            }

            foreach(ConnectionLine line in toDeleteLines)
            {
                Lines.Remove(line);
            }
        }

        private void FinishDrawingTheLine(string index)
        {
            endIndex = int.Parse(index);

            if (startIndex == endIndex)
            {
                startIndex = -1;
                endIndex = -1;
                return;
            }
            draggedServer = null;
            drawTheLine();
        }

        private void StartDrawingTheLine(string index)
        {
            startIndex = int.Parse(index);
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

        #region LineDrawing
        private int startIndex;
        private int endIndex;

        private void drawTheLine()
        {
            if(startIndex==-1 || endIndex == -1)
            {
                return;
            }

            Point startPoint = DisplayEntityPoints.ElementAt(startIndex);
            Point endPoint = DisplayEntityPoints.ElementAt(endIndex);
            startIndex = -1;
            endIndex = -1;

            ConnectionLine conLine = null;

            foreach(ConnectionLine cl in Lines)
            {
                if(cl.Y1==startPoint.Y && cl.X1==startPoint.X &&cl.X2==endPoint.X && cl.Y2 == endPoint.Y)
                {
                    conLine = cl;
                    break;
                }
                else if(cl.Y1 == endPoint.Y && cl.X1 == endPoint.X && cl.X2 == startPoint.X && cl.Y2 == startPoint.Y){
                    conLine = cl;
                    break;
                }
            }

            if (conLine != null)
            {
                Lines.Remove(conLine);
                return;
            }

            conLine = new ConnectionLine(startPoint.X, endPoint.X, startPoint.Y, endPoint.Y);
            Lines.Add(conLine);
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
                        postoji=true;
                        break;
                    }
                }

                if (postoji)
                {
                    postoji = false;
                    continue;
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

        private void updateAlArms(Server server)
        {
            foreach (ServerDisplayWrapper sd in ServersAtDisplay)
            {
                if (sd.Server == server)
                {
                    if (server.Usage < 45 || server.Usage > 75)
                    {
                        sd.AlarmVisibility = Visibility.Visible;
                    }
                    else
                    {
                        sd.AlarmVisibility = Visibility.Hidden;
                    }
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

        private void RemoveServerFromList(Server s)
        {
            if (s.Type.ServerTypeName.Equals("Web Server"))
            {
                ServersByTypes[0].Servers.Remove(s);
            }
            else if (s.Type.ServerTypeName.Equals("File Server"))
            {
                ServersByTypes[1].Servers.Remove(s);
            }
            else
            {
                ServersByTypes[2].Servers.Remove(s);
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

                        List<ConnectionLine> toDeleteLines = new List<ConnectionLine>();

                        foreach (ConnectionLine line in Lines)
                        {
                            if (line.Y1 == DisplayEntityPoints.ElementAt(sd.Index).Y && line.X1 == DisplayEntityPoints.ElementAt(sd.Index).X
                                || line.Y2 == DisplayEntityPoints.ElementAt(sd.Index).Y && line.X2 == DisplayEntityPoints.ElementAt(sd.Index).X)
                            {
                                toDeleteLines.Add(line);
                            }
                        }

                        foreach (ConnectionLine line in toDeleteLines)
                        {
                            Lines.Remove(line);
                        }
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
            DisplayEntityPoints = new List<Point>();
            Lines=new ObservableCollection<ConnectionLine>();

            ServersByTypes.Add(new ServersByType("Web Servers"));
            ServersByTypes.Add(new ServersByType("File Servers"));
            ServersByTypes.Add(new ServersByType("Database Servers"));

            toDeleteList = new List<Server>();

            //TODO delete at end bcs, at begining we wont have no server in system
            //foreach (Server s in Servers)
            //{
            //  AddServerToList(s);
            //}

            for (int i = 0; i < 6; i++)
            {
                for(int j= 0; j < 2; j++){
                    DisplayEntityPoints.Add(new Point(24.6 + j * 95, 17.5 + i * 94.16));
                }
            }

            for(int i = 0; i < 12; i++)
            {
                ServersAtDisplay.Add(new ServerDisplayWrapper(i));
            }

            startIndex = -1;
            endIndex = -1;
            OpacityForDrag = 0;

            MouseLeftButtonDown = new MyICommand<Server>(DragStarted);
            DropDown = new MyICommand<string>(TryToDropDown);
            AbbortDrag = new MyICommand(ResetTheDropValues);
            DeleteFromDisplay = new MyICommand<ServerDisplayWrapper>(DeleteServerFromDisplay);
            LineMouseLeftButtonDown = new MyICommand<string>(StartDrawingTheLine);
            LineMouseLeftButtonUp = new MyICommand<string>(FinishDrawingTheLine);

            Messenger.Default.Register<NotificationContent>(this,UpdateTheLists);
            Messenger.Default.Register<Server>(this, updateAlArms);
        }
        #endregion
    }
}
