using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Margatsni.Helpers
{
    public class PercentHelper:Utils.DependencyBindableBase
    {
        public PercentHelper()
        {
        }
        public static readonly DependencyProperty PercProperty =
                        DependencyProperty.Register("Perc", typeof(double), typeof(PercentHelper), new PropertyMetadata(0.0, new PropertyChangedCallback(PercChanged)));

        public static readonly DependencyProperty MaxProperty =
                        DependencyProperty.Register("Max", typeof(double), typeof(PercentHelper), new PropertyMetadata(0.0, new PropertyChangedCallback(MaxChanged)));


        static void PercChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as PercentHelper).CalcUseValue();
        }
        public double Perc
        {
            get
            {
                return (double)GetValue(PercProperty);
            }
            set
            {
                SetValue(PercProperty, value);
            }
        }

        static void MaxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as PercentHelper).CalcUseValue();
        }
        public double Max
        {
            get
            {
                return (double)GetValue(MaxProperty);
            }
            set
            {
                SetValue(MaxProperty, value);
            }
        }

        private void CalcUseValue()
        {
            current_use_value_ = Perc * Max;
            OnPropertyChanged("UseValue");
        }

        public double UseValue
        {
            get
            {
                return current_use_value_;
            }
        }
        double current_use_value_ = 0.0;
    }
}
