using NetworkService.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum ServerType 
    {
        Web_Server,
        File_Server,
        Database_Server
    }

    public class Server:BindableBase
    {
        private int _identificator;
        private string _name;
        private ServerType _type;
        private string _ipAddress;
        private int _usage;

        public int Identificator
        {
            get
            {
                return _identificator;
            }
            set
            {
                if(_identificator != value)
                {
                    _identificator = value;
                    OnPropertyChanged(nameof(Identificator));
                }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ServerType Type
        {
            get
            {
                return _type;

            }
            set
            {
                if( _type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                if ( _ipAddress != value)
                {
                    _ipAddress = value;
                    OnPropertyChanged(nameof(IpAddress));
                }
            }
        }

        public int Usage
        {
            get
            {
                return _usage;
            }

            set
            {
                if(_usage != value)
                {
                    _usage = value;
                    OnPropertyChanged(nameof(Usage));
                }
            }
        }
    }
}
