using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    public class Logdata : INotifyPropertyChanged
    {

        int grid_attempt = 0;
        public int Grid_Attempt
        {
            get { return grid_attempt; }
            set
            {
                if (Grid_Attempt == value)
                    return;
                grid_attempt = value; OnPropertyChanged(nameof(Grid_Attempt));
            }
        }

        int field_attempt = 0;
        public int Field_Attempt
        {
            get { return field_attempt; }
            set
            {
                if (Field_Attempt == value)
                    return;
                field_attempt = value; OnPropertyChanged(nameof(Field_Attempt));
            }
        }

        Field recorded_field = null;
        public Field Recorded_Field
        {
            get { return recorded_field; }
            set
            {
                if (Recorded_Field == value)
                    return;
                recorded_field = value; OnPropertyChanged(nameof(Recorded_Field));
            }
        }

        int grid_number = 0;
        public int Grid_Number
        {
            get { return grid_number; }
            set
            {
                if (Grid_Number == value)
                    return;
                grid_number = value; OnPropertyChanged(nameof(Grid_Number));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
