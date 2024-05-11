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
            s1.Usage = 79;
            s1.Type = ServerType.Web_Server;

            Server s2 = new Server();
            s2.Name = "Steam";
            s2.Identificator = 2;
            s1.Usage = 100;
            s2.IpAddress = "127.0.0.1";
            s2.Type = ServerType.File_Server;

            Server s3 = new Server();
            s3.Name = "www wwww23wwww";
            s3.Identificator = 123;
            s3.Usage = 2;
            s3.IpAddress = "127.110.220.111";
            s3.Type = ServerType.Database_Server;

            Server s4 = new Server();
            s4.Name = "Microsoft";
            s4.Identificator = 14;
            s4.Usage = 50;
            s4.IpAddress = "127.0.0.1";
            s4.Type = ServerType.File_Server;

            Server s5 = new Server();
            s5.Name = "Ubisoft";
            s5.Identificator = 13;
            s4.Usage = 50;
            s5.IpAddress = "192.0.0.1";
            s5.Type = ServerType.File_Server;

            Server s6 = new Server();
            s6.Name = "Riot games";
            s6.Identificator = 7;
            s6.Usage = 50;
            s6.IpAddress = "127.0.120.1";
            s6.Type = ServerType.Database_Server;

            Servers.Add(s1);
            Servers.Add(s2);
            Servers.Add(s3);
            Servers.Add(s4);
            Servers.Add(s5);
            Servers.Add(s6);
        }


    }
}
