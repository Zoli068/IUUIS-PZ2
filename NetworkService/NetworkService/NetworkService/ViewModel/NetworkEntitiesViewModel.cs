using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FontAwesome5;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel:BindableBase
    {

        public ObservableCollection<Server> Servers { get; set; }

        public NetworkEntitiesViewModel() { 
        
            Servers = new ObservableCollection<Server>();
            LoadData();
        }

        //TO DELETE after the implementation of ADD function
        public void LoadData()
        {

            Server s1 = new Server();
            s1.Name = "Oracle";
            s1.Identificator = 1;
            s1.IpAddress = "127.0.0.1";
            s1.Type = ServerType.WebServer;

            Server s2 = new Server();
            s2.Name = "Steam";
            s2.Identificator = 2;
            s2.IpAddress = "127.0.0.1";
            s2.Type = ServerType.FileServer;

            Server s3 = new Server();
            s3.Name = "Google";
            s3.Identificator = 3;
            s3.IpAddress = "127.110.220.111";
            s3.Type = ServerType.DataBaseServer;

            Server s4 = new Server();
            s4.Name = "Microsoft";
            s4.Identificator = 4;
            s4.IpAddress = "127.0.0.1";
            s4.Type = ServerType.FileServer;

            Servers.Add(s1);
            Servers.Add(s2);
            Servers.Add(s3);
            Servers.Add(s4);
        }

    }
}
