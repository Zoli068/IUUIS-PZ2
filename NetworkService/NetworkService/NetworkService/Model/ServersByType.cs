using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class ServersByType
    {
        public string serverType {  get; set; }
        public ObservableCollection<Server> Servers { get; set; }

        public ServersByType(string serverType) 
        {
            this.serverType = serverType;
            Servers = new ObservableCollection<Server>();
        }
    }
}
