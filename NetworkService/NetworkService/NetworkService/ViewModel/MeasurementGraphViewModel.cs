using NetworkService.Assets;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
                }
            }
        }

        #endregion

        #region Constructors

        public MeasurementGraphViewModel(ObservableCollection<Server> servers)
        {
            Servers = servers;
        }

        #endregion
    }
}
