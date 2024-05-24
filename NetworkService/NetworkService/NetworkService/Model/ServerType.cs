using NetworkService.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public  class ServerType:BindableBase
    {
        private string _serverTypeName;
        private string _path;


        public ServerType(string serverTypeName,string path)
        {
            this._serverTypeName = serverTypeName;
            this._path = path;
        } 

        public string ServerTypeName
        {
            get
            {
                return _serverTypeName;
            }
            set
            {
                if(value !=_serverTypeName) 
                { 
                    _serverTypeName = value;
                    OnPropertyChanged(nameof(ServerTypeName));
                }
            }
        }

        public string Path {
            get
            {
                return _path;
            }
            set
            {
                if (value != _path)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }
    }
}
