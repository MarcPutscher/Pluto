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
    public class Used_Technic : INotifyPropertyChanged
    {
        public int Used_Technikindex;

        string technic = null;
        public string Technic
        {
            get { return technic; }
            set
            {
                if (Technic == value)
                    return;
                technic = value; OnPropertyChanged(nameof(Technic));
                Text = " x " + technic;
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

        int count = 0;
        public int Count
        {
            get { return count; }
            set
            {
                if (Count == value)
                    return;
                count = value; OnPropertyChanged(nameof(Count));
            }
        }

        public class Technics
        {
            public const string
                Naked_Single = "Naked Single",
                Hidden_Single = "Hidden Singel",
                Naked_Pair = "Naked Pair",
                Hidden_Pair = "Hidden Pair",
                Naked_Trible = "Naked Trible",
                Hidden_Trible = "Hidden Trible",
                Locked_Candidates_Typ1 = "Locked Cand. Typ 1",
                Locked_Candidates_Typ2 = "Locked Cand. Typ 2",
                X_Wing = "X-Wing",
                Y_Wing = "Y-Wing";
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
