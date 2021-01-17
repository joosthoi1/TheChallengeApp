using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace App1
{
    public class Registratie
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int idRegistratie { get; set; }
        [MaxLength(45)]
        public string Voornaam { get; set; }
        [MaxLength(45)]
        public string Achternaam { get; set; }
        [MaxLength(12)]
        public string Postcode { get; set; }
        [MaxLength(45)]
        public string Woonplaats { get; set; }
        public DateTime Geboortedatum { get; set; }
        [Unique, NotNull]
        public string Emailadres { get; set; }
        [NotNull]
        public int Gebruikerscode { get; set; }
    }
    public class Database
    {
        readonly SQLiteAsyncConnection _database;
        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Registratie>().Wait();
        }
        public Task<int> RegisterPerson(Registratie user)
        {
            return _database.InsertAsync(user);
        }
        public Task<List<Registratie>> GetPeopleAsync()
        {
            return _database.Table<Registratie>().ToListAsync();
        }
    }
}