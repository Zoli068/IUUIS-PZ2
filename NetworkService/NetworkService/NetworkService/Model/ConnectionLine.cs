using NetworkService.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class ConnectionLine:BindableBase
    {
        private double x1;
        private double x2;
        private double y1;
        private double y2;

        public ConnectionLine(double x1, double x2, double y1, double y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        public double X1 
        {
            get 
            { 
                return x1; 
            } 
            set 
            {
                if (value != x1)
                {
                    OnPropertyChanged(nameof(X1));
                    x1 = value; 
                }
            } 
        }

        public double X2 
        {
            get 
            { 
                return x2; 
            } 
            set 
            {
                if (value != x2)
                {
                    OnPropertyChanged(nameof(X2));
                    x2 = value; 
                }
            } 
        }

        public double Y1 
        {
            get 
            { 
                return y1; 
            } 
            set 
            {
                if (value != y1)
                {
                    OnPropertyChanged(nameof(Y1));
                    y1 = value; 
                }
            } 
        }

        public double Y2 
        {
            get 
            { 
                return y2; 
            } 
            set 
            {
                if (value != y2)
                {
                    OnPropertyChanged(nameof(Y2));
                    y2 = value; 
                }
            } 
        }

    }
}
