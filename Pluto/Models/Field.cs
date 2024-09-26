using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    public class Field : INotifyPropertyChanged
    {
        int id;
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set
            {
                if (Id == value)
                    return;
                id = value; OnPropertyChanged(nameof(Id));
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
        int row_number = 0;
        public int Row_Number
        {
            get { return row_number; }
            set
            {
                if (Row_Number == value)
                    return;
                row_number = value; OnPropertyChanged(nameof(Row_Number));
            }
        }
        int column_number = 0;
        public int Column_Number
        {
            get { return column_number; }
            set
            {
                if (Column_Number == value)
                    return;
                column_number = value; OnPropertyChanged(nameof(Column_Number));
            }
        }


        int number = 0;
        public int Number
        {
            get { return number; }
            set
            {
                if (Number == value)
                    return;
                if (Is_Locked == true)
                {
                    return;
                }
                number = value; OnPropertyChanged(nameof(Number));
                if (number != 0)
                {
                    Visible_Number = number.ToString();
                }
                else
                {
                    Visible_Number = string.Empty;
                }
            }
        }
        int skips = 0;
        public int Skips
        {
            get { return skips; }
            set
            {
                if (Skips == value)
                    return;
                if (Is_Locked == true)
                {
                    return;
                }
                skips = value; OnPropertyChanged(nameof(Skips));
                if (skips != 0)
                {
                    Visible_Skips = skips.ToString();
                }
                else
                {
                    Visible_Skips = string.Empty;
                }
            }
        }
        public string visible_skips = null;
        public string Visible_Skips
        {
            get { return visible_skips; }
            set
            {
                if (Visible_Skips == value)
                    return;
                if (Is_Locked == true)
                {
                    return;
                }
                visible_skips = value; OnPropertyChanged(nameof(Visible_Skips));
            }
        }
        public string visible_number = null;
        public string Visible_Number
        {
            get { return visible_number; }
            set
            {
                if (Visible_Number == value)
                    return;
                if (Is_Locked == true)
                {
                    return;
                }
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
                if(Is_Locked == true)
                {
                    return;
                }
                is_select = value; OnPropertyChanged(nameof(Is_Select));
            }
        }
        bool is_locked = false;
        public bool Is_Locked
        {
            get { return is_locked; }
            set
            {
                if (Is_Locked == value)
                    return;
                is_locked = value; OnPropertyChanged(nameof(Is_Locked));
                if(value == true)
                {
                    Number_Background_Color = Color.FromArgb("#FF2B2B2F");
                }
                else
                {
                    Number_Background_Color = Color.FromArgb("#07060D");
                }
            }
        }
        bool was_manuel_select = false;
        public bool Was_Manuel_Select
        {
            get { return was_manuel_select; }
            set
            {
                if (Was_Manuel_Select == value)
                    return;
                was_manuel_select = value; OnPropertyChanged(nameof(Was_Manuel_Select));
            }
        }
        bool is_fault = false;
        public bool Is_Fault
        {
            get { return is_fault; }
            set
            {
                if (Is_Fault == value)
                    return;
                is_fault = value; OnPropertyChanged(nameof(Is_Fault));
                if(is_fault == true)
                {
                    Number_Color = Color.FromArgb("#FFA53337");
                }
                else
                {
                    Number_Color = Color.FromArgb("#278A15");
                }
            }
        }
        bool is_saturated = false;
        public bool Is_Saturated
        {
            get { return is_saturated; }
            set
            {
                if (Is_Saturated == value)
                    return;
                if (Is_Locked == true)
                {
                    return;
                }
                is_saturated = value; OnPropertyChanged(nameof(Is_Saturated));
            }
        }

        Color number_color = Color.FromArgb("#278A15");
        public Color Number_Color
        {
            get { return number_color; }
            set
            {
                if (Number_Color == value)
                    return;
                number_color = value; OnPropertyChanged(nameof(Number_Color));
            }
        }
        Color number_background_color = Color.FromArgb("#07060D");
        public Color Number_Background_Color
        {
            get { return number_background_color; }
            set
            {
                if (Number_Background_Color == value)
                    return;
                number_background_color = value; OnPropertyChanged(nameof(Number_Background_Color));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
