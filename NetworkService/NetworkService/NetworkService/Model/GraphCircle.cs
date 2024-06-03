using NetworkService.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.Model
{
    public class GraphCircle : BindableBase
    {
        private string _value;
        private string _time;
        private double _radius;
        private bool _warning;
        private Visibility _visibility;

        public GraphCircle()
        {
            _value = "";
            _time = "";
            _radius= 0;
            _warning = false;
            _visibility = Visibility.Hidden;
        }

        #region Properties
        public string Value { 
            get 
            { 
                return _value;
            }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public string Time { 
            get 
            { 
                return _time;
            }
            set
            {
                if (value != _time)
                {
                    _time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                if(value != _radius)
                {
                    _radius = value;
                    OnPropertyChanged(nameof(Radius));
                }
            }
        }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                if (value != _visibility)
                {
                    _visibility = value;
                    OnPropertyChanged(nameof(Visibility));
                }
            }
        }

        public bool Warning
        {
            get
            {
                return _warning;
            }
            set
            {
                if (value != _warning)
                {
                    _warning = value;
                    OnPropertyChanged(nameof(Warning));
                }
            }
        }
        #endregion
    }
}
