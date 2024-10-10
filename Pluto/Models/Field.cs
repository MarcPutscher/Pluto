using MvvmHelpers;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    public class Field : INotifyPropertyChanged, ICloneable
    {
        public Field() 
        {
            Possebilities.CollectionChanged += Possebilities_CollectionChanged;
            //Denails.CollectionChanged += Denails_CollectionChanged;
        }

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
        int real_grid_number = 0;
        public int Real_Grid_Number
        {
            get { return real_grid_number; }
            set
            {
                if (Real_Grid_Number == value)
                    return;
                real_grid_number = value; OnPropertyChanged(nameof(Real_Grid_Number));
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
        bool is_clearly = false;
        public bool Is_Clearly
        {
            get { return is_clearly; }
            set
            {
                if (Is_Clearly == value)
                    return;
                is_clearly = value; OnPropertyChanged(nameof(Is_Clearly));
            }
        }
        bool is_semi_clearly = false;
        public bool Is_Semi_Clearly
        {
            get { return is_semi_clearly; }
            set
            {
                if (Is_Semi_Clearly == value)
                    return;
                is_semi_clearly = value; OnPropertyChanged(nameof(Is_Semi_Clearly));
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

        public ObservableCollection<int> Possebilities = new ObservableCollection<int>();

        private void Possebilities_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender == null)
                return;

            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset || (sender as IEnumerable).Cast<int>().Count() == 0)
            {
                Is_1 = false;

                Is_2 = false;

                Is_3 = false;

                Is_4 = false;

                Is_5 = false;

                Is_6 = false;

                Is_7 = false;

                Is_8 = false;

                Is_9 = false;

                return;
            }

            if (sender is IEnumerable)
            {
                int number = (sender as IEnumerable).Cast<int>().Last();

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (number == 1)
                        Is_1 = true;

                    if (number == 2)
                        Is_2 = true;

                    if (number == 3)
                        Is_3 = true;

                    if (number == 4)
                        Is_4 = true;

                    if (number == 5)
                        Is_5 = true;

                    if (number == 6)
                        Is_6 = true;

                    if (number == 7)
                        Is_7 = true;

                    if (number == 8)
                        Is_8 = true;

                    if (number == 9)
                        Is_9 = true;
                }
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    for ( int i = 1; i < 10; i++)
                    {
                        if(!(sender as IEnumerable).Cast<int>().Contains(i))
                        {
                            if (i == 1)
                                Is_1 = false;

                            if (i == 2)
                                Is_2 = false;

                            if (i == 3)
                                Is_3 = false;

                            if (i == 4)
                                Is_4 = false;

                            if (i == 5)
                                Is_5 = false;

                            if (i == 6)
                                Is_6 = false;

                            if (i == 7)
                                Is_7 = false;

                            if (i == 8)
                                Is_8 = false;

                            if (i == 9)
                                Is_9 = false;
                        }
                    }
                }
            }
        }

        //private void Denails_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (sender == null)
        //        return;

        //    if (sender is IEnumerable)
        //    {
        //        if ((sender as IEnumerable).Cast<int>().Count() == 0)
        //            return;

        //        int number = (sender as IEnumerable).Cast<int>().Last();

        //        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        //        {
        //            if (number == 1)
        //                Is_1 = true;

        //            if (number == 2)
        //                Is_2 = true;

        //            if (number == 3)
        //                Is_3 = true;

        //            if (number == 4)
        //                Is_4 = true;

        //            if (number == 5)
        //                Is_5 = true;

        //            if (number == 6)
        //                Is_6 = true;

        //            if (number == 7)
        //                Is_7 = true;

        //            if (number == 8)
        //                Is_8 = true;

        //            if (number == 9)
        //                Is_9 = true;
        //        }
        //        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        //        {
        //            if (number == 1)
        //                Is_1 = false;

        //            if (number == 2)
        //                Is_2 = false;

        //            if (number == 3)
        //                Is_3 = false;

        //            if (number == 4)
        //                Is_4 = false;

        //            if (number == 5)
        //                Is_5 = false;

        //            if (number == 6)
        //                Is_6 = false;

        //            if (number == 7)
        //                Is_7 = false;

        //            if (number == 8)
        //                Is_8 = false;

        //            if (number == 9)
        //                Is_9 = false;
        //        }
        //    }
        //}

        bool is_1 = false;
        public bool Is_1
        {
            get { return is_1; }
            set
            {
                if (Is_1 == value)
                    return;
                is_1 = value; OnPropertyChanged(nameof(Is_1));
            }
        }
        bool is_2 = false;
        public bool Is_2
        {
            get { return is_2; }
            set
            {
                if (Is_2 == value)
                    return;
                is_2 = value; OnPropertyChanged(nameof(Is_2));
            }
        }
        bool is_3 = false;
        public bool Is_3
        {
            get { return is_3; }
            set
            {
                if (Is_3 == value)
                    return;
                is_3 = value; OnPropertyChanged(nameof(Is_3));
            }
        }
        bool is_4 = false;
        public bool Is_4
        {
            get { return is_4; }
            set
            {
                if (Is_4 == value)
                    return;
                is_4 = value; OnPropertyChanged(nameof(Is_4));
            }
        }
        bool is_5 = false;
        public bool Is_5
        {
            get { return is_5; }
            set
            {
                if (Is_5 == value)
                    return;
                is_5 = value; OnPropertyChanged(nameof(Is_5));
            }
        }
        bool is_6 = false;
        public bool Is_6
        {
            get { return is_6; }
            set
            {
                if (Is_6 == value)
                    return;
                is_6 = value; OnPropertyChanged(nameof(Is_6));
            }
        }
        bool is_7 = false;
        public bool Is_7
        {
            get { return is_7; }
            set
            {
                if (Is_7 == value)
                    return;
                is_7 = value; OnPropertyChanged(nameof(Is_7));
            }
        }
        bool is_8 = false;
        public bool Is_8
        {
            get { return is_8; }
            set
            {
                if (Is_8 == value)
                    return;
                is_8 = value; OnPropertyChanged(nameof(Is_8));
            }
        }
        bool is_9 = false;
        public bool Is_9
        {
            get { return is_9; }
            set
            {
                if (Is_9 == value)
                    return;
                is_9 = value; OnPropertyChanged(nameof(Is_9));
            }
        }

        //public ObservableCollection<int> Denails = new ObservableCollection<int>();

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
