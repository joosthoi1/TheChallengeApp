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
    public class Schakelaar
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int DeviceId { get; set; }
        [MaxLength(36), NotNull]
        public string DeviceGUID { get; set; }
        [NotNull]
        public int UserId { get; set; }
        public string Name { get; set; }
    }
    public class Timers
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int TimerId { get; set; }
        [NotNull]
        public TimeSpan Start {get;set;}
        [NotNull]
        public TimeSpan Stop { get; set; }
        [NotNull]
        public bool StaatAan { get; set; }
        [NotNull]
        public int DeviceId { get; set; }
        [NotNull]
        public int UserId { get; set; }
    }
    public class Database
    {
        readonly SQLiteAsyncConnection _database;
        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Registratie>().Wait();
            _database.CreateTableAsync<Schakelaar>().Wait();
            _database.CreateTableAsync<Timers>().Wait();
        }
        public Task<int> UpdateTimer(Timers timer)
        {
            return _database.UpdateAsync(timer);
        }
        public Task<List<Schakelaar>> GetUserDevices(int userId)
        {
            return _database.QueryAsync<Schakelaar>($"SELECT * from Schakelaar where UserId = '{userId}'");
        }
        public Task<Timers> GetTimerByPK(int primaryKey)
        {
            return _database.Table<Timers>().Where(i => i.TimerId == primaryKey).FirstOrDefaultAsync();
        }
        public Task<Schakelaar> GetSchakelaarByPK(int primaryKey)
        {
            return _database.Table<Schakelaar>().Where(i => i.DeviceId == primaryKey).FirstOrDefaultAsync();
        }
        public Task<List<Timers>> GetDeviceTimers(int userId, int DeviceId)
        {
            return _database.Table<Timers>().Where(i => i.UserId == userId && i.DeviceId == DeviceId).ToListAsync();
        }
        public async Task<bool> ValidateLogin(string email, int code)
        {
            System.Diagnostics.Debug.WriteLine("TEST");
            System.Diagnostics.Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(await RegisterQuerry(email)));
            return (await RegisterQuerry(email)).Gebruikerscode == code;
        }
        public Task<Registratie> RegisterQuerry(string email)
        {
            return _database.Table<Registratie>().Where(i => i.Emailadres == email.ToLower()).FirstOrDefaultAsync();
            //return _database.QueryAsync<Registratie>($"SELECT * from Registratie where Emailadres = '{email.ToLower()}'");
        }
        public Task<int> RegisterPerson(Registratie user)
        {
            return _database.InsertAsync(user);
        }
        public Task<int> AddDevice(Schakelaar device)
        {
            return _database.InsertAsync(device);
        }
        public Task<List<Registratie>> GetPeopleAsync()
        {
            return _database.Table<Registratie>().ToListAsync();
        }
        public Task<int> AddTimer(Timers timer)
        {
            return _database.InsertAsync(timer);
        }

    }
}