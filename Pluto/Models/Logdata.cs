using MvvmHelpers;
using Pluto.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Models
{
    public class Logdata : INotifyPropertyChanged
    {
        public int Logindex;

        int id = 0;
        public int ID
        {
            get { return id; }
            set
            {
                if (ID == value)
                    return;
                id = value; OnPropertyChanged(nameof(ID));
            }
        }

        int grid = 0;
        public int Grid
        {
            get { return grid; }
            set
            {
                if (Grid == value)
                    return;
                grid = value; OnPropertyChanged(nameof(Grid));
            }
        }

        int row = 0;
        public int Row
        {
            get { return row; }
            set
            {
                if (Row == value)
                    return;
                row = value; OnPropertyChanged(nameof(Row));
            }
        }

        int column = 0;
        public int Column
        {
            get { return column; }
            set
            {
                if (Column == value)
                    return;
                column = value; OnPropertyChanged(nameof(Column));
            }
        }

        string text = string.Empty;
        public string Text
        {
            get { return text; }
            set
            {
                if (Text == value)
                    return;
                text = value; OnPropertyChanged(nameof(Text));
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

        public List<Field> Recorded_Playground = new List<Field>();

        public class Strategies
        {
            public const string
                Naked_Single = "Naked Single",
                Hidden_Single = "Hidden Singel",
                Naked_Pair = "Naked Pair",
                Hidden_Pair = "Hidden Pair",
                Naked_Trible = "Naked Trible",
                Hidden_Trible = "Hidden Trible",
                Locked_Candidates_Typ1 = "Locked Candidates Typ 1",
                Locked_Candidates_Typ2 = "Locked Candidates Typ 2";
        }

        public Logdata Removed_Market_Number_From_Field(int number,List<Field> fields, string strategies)
        {
            //Setzt den Text in der UI
            Text = strategies+" | "+number+" als Möglichkeit entfernt im Feld " + id + "" ;

            //Setzt den Logindex
            Logindex = MainPage.Logs.Count;

            //Klont alle Felder
            foreach (var item in (IEnumerable)fields)
            {
                Field cloneable = item as Field;

                Recorded_Playground.Add(cloneable.Clone() as Field);
            }

            //Setzt die Hintergrundfarbe des Feldes wo es eine Änderung gibt
            Recorded_Playground[ID].Number_Background_Color = Color.FromArgb("#94520E");

            //Gibt diese Logdata zurück
            return this;
        }
        public Logdata Add_Market_Number_To_Field(int number, List<Field> fields, string strategies)
        {
            //Setzt den Text in der UI
            Text = strategies + " | " + number + " als Möglichkeit hinzugefügt im Feld " + id + "";

            //Setzt den Logindex
            Logindex = MainPage.Logs.Count;

            //Klont alle Felder
            foreach (var item in (IEnumerable)fields)
            {
                Field cloneable = item as Field;

                Recorded_Playground.Add(cloneable.Clone() as Field);
            }

            //Setzt die Hintergrundfarbe des Feldes wo es eine Änderung gibt
            Recorded_Playground[ID].Number_Background_Color = Color.FromArgb("#94520E");

            //Gibt diese Logdata zurück
            return this;
        }
        public Logdata Place_Number_In_Field(int number, List<Field> fields, string strategies)
        {
            //Setzt den Text in der UI
            Text = strategies + " | " + number + " als Eindeutig eingetragen im Feld " + id + "";

            //Setzt den Logindex
            Logindex = MainPage.Logs.Count;

            //Klont alle Felder
            foreach (var item in (IEnumerable)fields)
            {
                Field cloneable = item as Field;

                Recorded_Playground.Add(cloneable.Clone() as Field);
            }

            //Setzt die Hintergrundfarbe des Feldes wo es eine Änderung gibt
            Recorded_Playground[ID].Number_Background_Color = Color.FromArgb("#94520E");

            //Gibt diese Logdata zurück
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
