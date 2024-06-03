using NetworkService.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.Model
{
    public class ServerDisplayWrapper : BindableBase
    {
        private int index;
        private Server server;
        private Visibility alarmVisibility;
        private Visibility serverVisibility;

        public ServerDisplayWrapper(int index)
        {
            this.alarmVisibility = Visibility.Hidden;
            this.serverVisibility = Visibility.Hidden; 
            this.index = index;
        }

        #region Properties
        public int Index
        {
            get 
            { 
                return index; 
            }
        }

        public Server Server
        {
            get
            {
                return server;
            }
            set
            {
                if (value != server)
                {
                    server = value;
                    OnPropertyChanged(nameof(Server));
                }
            }
        }

        public Visibility AlarmVisibility
        {
            get
            {
                return alarmVisibility;
            }
            set
            {
                if (value != alarmVisibility)
                {
                    alarmVisibility = value;
                    OnPropertyChanged(nameof(AlarmVisibility));
                }
            }
        }

        public Visibility ServerVisibility
        {
            get
            {
                return serverVisibility;
            }
            set
            {
                if (value != serverVisibility)
                {
                    serverVisibility = value;
                    OnPropertyChanged(nameof(ServerVisibility));
                }
            }
        }
        #endregion
    }
}
