using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    public class Number : INotifyPropertyChanged
    {
        string visible_number = string.Empty;
        public string Visible_Number
        {
            get { return visible_number; }
            set
            {
                if (Visible_Number == value)
                    return;
                visible_number = value; OnPropertyChanged(nameof(Visible_Number));
            }
        }

        bool is_select = false;
        public bool Is_Select
        {
            get { return is_select; }
            set
            {
                if (Is_Select == value)
                    return;
                is_select = value; OnPropertyChanged(nameof(Is_Select));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
