using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    public class Playground : INotifyPropertyChanged
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


        string field_numbers = null;
        public string Field_Numbers
        {
            get { return field_numbers; }
            set
            {
                if (Field_Numbers == value)
                    return;
                field_numbers = value; OnPropertyChanged(nameof(Field_Numbers));
            }
        }
        string difficulty = "Leicht";
        public string Difficulty
        {
            get { return difficulty; }
            set
            {
                if (Field_Numbers == value || value == null)
                    return;
                difficulty = value; OnPropertyChanged(nameof(Difficulty));
            }
        }

        int field_with_numbers_count = 0;
        public int Field_With_Numbers_Count
        {
            get { return field_with_numbers_count; }
            set
            {
                if (Field_With_Numbers_Count == value)
                    return;
                field_with_numbers_count = value; OnPropertyChanged(nameof(Field_With_Numbers_Count));
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

    public class Playground_Field_Numbers_Converter
    {
        public string Serialize(List<int> input)
        {
            string s = string.Empty;

            foreach (int i in input)
            {
                s += i + ":";
            }
            return s.Remove(s.Length - 1);
        }
        public List<int> Deserialize(string input)
        {
            List<int> result = new List<int>();

            string s = input;

            while (s.Length > 1)
            {
                result.Add(int.Parse("" + s[s.Length - 1]));
                s = s.Remove(s.Length - 2);
            }
            result.Add(int.Parse(s));

            return result.Reverse<int>().ToList();
        }
    }
}
