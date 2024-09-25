using Pluto.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Service
{
    /// <summary>
    /// Ist eine Serviceklasse für die Spielfelder.
    /// </summary>
    public class PlaygroundService
    {

        public static SQLiteAsyncConnection db;

        /// <summary>
        /// Erstellt eine Verbindung zur Datenbank her.
        /// </summary>
        /// <returns></returns>
        public static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Pluto.db");

            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<Playground>();
        }

        /// <summary>
        /// Fügt ein Spielfeld hinzu.
        ///  0 = Erfolgreich | 1 = Input war null
        /// </summary>
        /// <param name="input">Das Spielfeld das in die Datenbank hinzugefügt werden soll.</param>
        /// <returns></returns>
        public static async Task<int> Add_Playground(Playground input)
        {
            if (input == null)
            {
                return 1;
            }
            else
            {
                await db.InsertAsync(input);

                return 0;
            }
        }

        /// <summary>
        /// Entfernt ein spezifisches Spielfeld.
        /// true == noch Spielfelder da | false == kein Spielfeld mehr vorhanden
        /// </summary>
        /// <param name="item">Das Spielfeld das entfernt werden soll.</param>
        /// <returns></returns>
        public static async Task<bool> Remove_Playground(Playground item)
        {
            await Init();

            await db.DeleteAsync<Playground>(item.Id);

            var playground = await Get_all_Playgrounds();

            if(playground.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Bearbeitet ein Spielfeld.
        /// </summary>
        /// <param name="item">Das Spielfeld das in der Datenbank verändert werden soll.</param>
        /// <returns></returns>
        public static async Task Edit_Playground(Playground item)
        {
            await Init();

            await db.UpdateAsync(item);
        }

        /// <summary>
        /// Gibt alle Spielfelder in der Datenbank zurück.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Playground>> Get_all_Playgrounds()
        {
            await Init();

            try
            {
                var playgrounds = await db.Table<Playground>().ToListAsync();

                return playgrounds.Reverse<Playground>().ToList();
            }
            catch
            {
                return new List<Playground>();
            }
        }

        /// <summary>
        /// Gibt alle Spielfelder in der Datenbank zurück in einer ObservableCollection.
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Playground>> Get_all_Playgrounds_in_ObservableCollection()
        {
            await Init();

            try
            {
                ObservableCollection<Playground> playgrounds = new ObservableCollection<Playground>();

                var playgrounds1 = await db.Table<Playground>().ToListAsync();

                playgrounds1.Reverse<Playground>().ToList();

                if(playgrounds1.Count != 0)
                {
                    foreach (var playground in playgrounds1)
                    {
                        playgrounds.Add(playground);
                    }
                }

                return playgrounds;
            }
            catch
            {
                return new ObservableCollection<Playground>();
            }
        }
    }
}
