using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BCrypt.Net;
using vc.Models;

namespace vc.Database
{
    public class DatabaseHelper
    {
        private string connectionString;
        private string dbPath;

        public DatabaseHelper()
        {



            // Определяем путь к базе данных в папке приложения
            string appDataPath = Path.Combine(Application.StartupPath, "Data");
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            dbPath = Path.Combine(appDataPath, "VetClinic.db");

            // Строка подключения к SQLite
            connectionString = $"Data Source={dbPath};Version=3;Pooling=True;Max Pool Size=100;";

            // Инициализируем базу данных при первом запуске
            InitializeDatabase();
        }


        /// Создает базу данных и таблицы, если они не существуют
     
        /// 





        private void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    CreateTables();

                    // Добавляем тестового администратора
                    CreateDefaultAdmin();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации БД: {ex.Message}");
                MessageBox.Show($"Ошибка создания базы данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }






        /// Создает таблицы в базе данных

        private void CreateTables()
        {
            string[] createTableQueries = {
                // Таблица пользователей
                @"CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    FullName TEXT NOT NULL,
                    Phone TEXT,
                    Role TEXT DEFAULT 'User',
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );",

                // Таблица животных
                @"CREATE TABLE Pets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Species TEXT NOT NULL,
                    Breed TEXT,
                    Age INTEGER,
                    OwnerId INTEGER,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
                );",

                // Таблица записей на прием
                @"CREATE TABLE Appointments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PetId INTEGER,
                    OwnerId INTEGER,
                    AppointmentDate DATETIME NOT NULL,
                    Reason TEXT,
                    Status TEXT DEFAULT 'Scheduled',
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (PetId) REFERENCES Pets(Id) ON DELETE CASCADE,
                    FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
                );",

                // Таблица медицинских записей
                @"CREATE TABLE MedicalRecords (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PetId INTEGER,
                    AppointmentId INTEGER,
                    Diagnosis TEXT,
                    Treatment TEXT,
                    Prescription TEXT,
                    RecordDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    DoctorId INTEGER,
                    FOREIGN KEY (PetId) REFERENCES Pets(Id) ON DELETE CASCADE,
                    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id),
                    FOREIGN KEY (DoctorId) REFERENCES Users(Id)
                );",

                // Таблица лекарств
                @"CREATE TABLE Medicines (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Quantity INTEGER DEFAULT 0,
                    Price DECIMAL(10, 2),
                    ExpiryDate DATE
                );",

                // Индексы для улучшения производительности
                "CREATE INDEX idx_pets_owner ON Pets(OwnerId);",
                "CREATE INDEX idx_appointments_pet ON Appointments(PetId);",
                "CREATE INDEX idx_appointments_date ON Appointments(AppointmentDate);",
                "CREATE INDEX idx_medicalrecords_pet ON MedicalRecords(PetId);"
            };




            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var query in createTableQueries)
                        {
                            using (var cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                        System.Diagnostics.Debug.WriteLine("Таблицы успешно созданы");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine($"Ошибка создания таблиц: {ex.Message}");
                    }
                }
            }
        }

        









        /// Создает тестового администратора
        

        private void CreateDefaultAdmin()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");

            string query = @"INSERT INTO Users (Username, Password, Email, FullName, Role) 
                           VALUES ('admin', @password, 'admin@vetclinic.com', 'System Administrator', 'Admin')";

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    








        /// Тестовый метод для проверки подключения
        

        public bool TestConnection()
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"✅ Подключение к SQLite успешно! База данных: {dbPath}");

                    // Проверяем наличие таблиц
                    string query = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        bool exists = result != null;
                        System.Diagnostics.Debug.WriteLine($"Таблица Users существует: {exists}");

                        if (exists)
                        {
                            string countQuery = "SELECT COUNT(*) FROM Users";
                            using (var countCmd = new SQLiteCommand(countQuery, conn))
                            {
                                long count = (long)countCmd.ExecuteScalar();
                                System.Diagnostics.Debug.WriteLine($"Количество пользователей: {count}");
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка подключения: {ex.Message}");
                return false;
            }
        }








       
       // АВТОРИЗАЦИЯ И ПОЛЬЗОВАТЕЛИ 


        public User Login(string username, string password)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Users WHERE Username = @username";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    Id = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    FullName = reader.GetString(4),
                                    Phone = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Role = reader.GetString(6),
                                    CreatedAt = reader.GetDateTime(7)
                                };

                                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                                {
                                    return user;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в Login: {ex.Message}");
            }
            return null;
        }










        public bool RegisterUser(User user)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем существование пользователя
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username OR Email = @email";
                    using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", user.Username);
                        checkCmd.Parameters.AddWithValue("@email", user.Email);
                        long count = (long)checkCmd.ExecuteScalar();

                        if (count > 0)
                            return false;
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    string query = @"INSERT INTO Users (Username, Password, Email, FullName, Phone, Role) 
                                   VALUES (@username, @password, @email, @fullname, @phone, 'User')";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@fullname", user.FullName);
                        cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в RegisterUser: {ex.Message}");
                return false;
            }
        }









        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Username, Email, FullName, Phone, Role, CreatedAt FROM Users ORDER BY Id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = "",
                                Email = reader.GetString(2),
                                FullName = reader.GetString(3),
                                Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Role = reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetAllUsers: {ex.Message}");
            }
            return users;
        }











        public User GetUserById(int userId)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Users WHERE Id = @userId";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    FullName = reader.GetString(4),
                                    Phone = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Role = reader.GetString(6),
                                    CreatedAt = reader.GetDateTime(7)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetUserById: {ex.Message}");
            }
            return null;
        }

       









        // ПИТОМЦЫ
        // 


        public List<Pet> GetUserPets(int userId)
        {
            var pets = new List<Pet>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Pets WHERE OwnerId = @userId ORDER BY Name";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pets.Add(new Pet
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Species = reader.GetString(2),
                                    Breed = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Age = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                    OwnerId = reader.GetInt32(5),
                                    CreatedAt = reader.GetDateTime(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetUserPets: {ex.Message}");
            }
            return pets;
        }












        public List<Pet> GetAllPets()
        {
            var pets = new List<Pet>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT p.*, u.FullName as OwnerName 
                                   FROM Pets p 
                                   JOIN Users u ON p.OwnerId = u.Id 
                                   ORDER BY p.Id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pets.Add(new Pet
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Species = reader.GetString(2),
                                Breed = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Age = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                OwnerId = reader.GetInt32(5),
                                OwnerName = reader.GetString(7),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetAllPets: {ex.Message}");
            }
            return pets;
        }















        public Pet GetPetById(int petId)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT p.*, u.FullName as OwnerName 
                                   FROM Pets p 
                                   JOIN Users u ON p.OwnerId = u.Id 
                                   WHERE p.Id = @petId";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@petId", petId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Pet
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Species = reader.GetString(2),
                                    Breed = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Age = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                    OwnerId = reader.GetInt32(5),
                                    OwnerName = reader.GetString(7),
                                    CreatedAt = reader.GetDateTime(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetPetById: {ex.Message}");
            }
            return null;
        }










        public bool AddPet(Pet pet)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Pets (Name, Species, Breed, Age, OwnerId) 
                                   VALUES (@name, @species, @breed, @age, @ownerId)";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", pet.Name);
                        cmd.Parameters.AddWithValue("@species", pet.Species);
                        cmd.Parameters.AddWithValue("@breed", pet.Breed ?? "");

                        cmd.Parameters.AddWithValue("@age", pet.Age);
                        cmd.Parameters.AddWithValue("@ownerId", pet.OwnerId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в AddPet: {ex.Message}");
                return false;
            }
        }













        // ЗАПИСИ НА ПРИЕМ 

        public List<AvailableTimeSlot> GetAvailableTimeSlots(DateTime date)
        {
            var availableSlots = new List<AvailableTimeSlot>();
            try
            {
                var startTime = new DateTime(date.Year, date.Month, date.Day, 8, 0, 0);
                var endTime = new DateTime(date.Year, date.Month, date.Day, 20, 0, 0);

                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT AppointmentDate FROM Appointments 
                                   WHERE DATE(AppointmentDate) = @date 
                                   AND Status != 'Cancelled'";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date.ToString("yyyy-MM-dd"));

                        using (var reader = cmd.ExecuteReader())
                        {
                            var bookedTimes = new List<DateTime>();
                            while (reader.Read())
                            {
                                bookedTimes.Add(reader.GetDateTime(0));
                            }

                            for (var time = startTime; time < endTime; time = time.AddHours(1))
                            {
                                bool isBooked = bookedTimes.Any(bt =>
                                    bt.Hour == time.Hour && bt.Date == time.Date);

                                availableSlots.Add(new AvailableTimeSlot
                                {
                                    DateTime = time,
                                    IsAvailable = !isBooked
                                });
                            }


                        }
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetAvailableTimeSlots: {ex.Message}");
            }
            return availableSlots;
        }









        public List<Appointment> GetUserAppointments(int userId)
        {



            var appointments = new List<Appointment>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT a.*, p.Name as PetName, u.FullName as OwnerName 
                                   FROM Appointments a
                                   JOIN Pets p ON a.PetId = p.Id
                                   JOIN Users u ON a.OwnerId = u.Id
                                   WHERE a.OwnerId = @userId 
                                   ORDER BY a.AppointmentDate DESC";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                appointments.Add(new Appointment
                                {
                                    Id = reader.GetInt32(0),
                                    PetId = reader.GetInt32(1),
                                    PetName = reader.GetString(7),
                                    OwnerId = reader.GetInt32(2),
                                    OwnerName = reader.GetString(8),
                                    AppointmentDate = reader.GetDateTime(3),

                                    Reason = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Status = reader.GetString(5),
                                    CreatedAt = reader.GetDateTime(6)
                                });
                            }
                        }



                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetUserAppointments: {ex.Message}");
            }
            return appointments;
        }







        public List<Appointment> GetAllAppointments()
        {
            var appointments = new List<Appointment>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT a.*, p.Name as PetName, u.FullName as OwnerName 
                                   FROM Appointments a
                                   JOIN Pets p ON a.PetId = p.Id
                                   JOIN Users u ON a.OwnerId = u.Id
                                   ORDER BY a.AppointmentDate DESC";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            appointments.Add(new Appointment
                            {
                                Id = reader.GetInt32(0),
                                PetId = reader.GetInt32(1),
                                PetName = reader.GetString(7),
                                OwnerId = reader.GetInt32(2),
                                OwnerName = reader.GetString(8),
                                AppointmentDate = reader.GetDateTime(3),
                                Reason = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Status = reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetAllAppointments: {ex.Message}");
            }
            return appointments;
        }









        public List<Appointment> GetUpcomingAppointments(int days = 7)
        {
            var appointments = new List<Appointment>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT a.*, p.Name as PetName, u.FullName as OwnerName 
                                   FROM Appointments a
                                   JOIN Pets p ON a.PetId = p.Id
                                   JOIN Users u ON a.OwnerId = u.Id
                                   WHERE a.AppointmentDate BETWEEN datetime('now') AND datetime('now', '+' || @days || ' days')
                                   AND a.Status = 'Scheduled'
                                   ORDER BY a.AppointmentDate";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@days", days);





                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                appointments.Add(new Appointment
                                {
                                    Id = reader.GetInt32(0),
                                    PetId = reader.GetInt32(1),
                                    PetName = reader.GetString(7),
                                    OwnerId = reader.GetInt32(2),
                                    OwnerName = reader.GetString(8),
                                    AppointmentDate = reader.GetDateTime(3),
                                    Reason = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Status = reader.GetString(5),
                                    CreatedAt = reader.GetDateTime(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetUpcomingAppointments: {ex.Message}");
            }
            return appointments;
        }












        public bool CreateAppointment(Appointment appointment)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string checkQuery = @"SELECT COUNT(*) FROM Appointments 
                                        WHERE AppointmentDate = @date 
                                        AND Status != 'Cancelled'";

                    using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@date", appointment.AppointmentDate);
                        long count = (long)checkCmd.ExecuteScalar();

                        if (count > 0)
                            return false;
                    }

                    string query = @"INSERT INTO Appointments (PetId, OwnerId, AppointmentDate, Reason, Status) 
                                   VALUES (@petId, @ownerId, @date, @reason, 'Scheduled')";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@petId", appointment.PetId);
                        cmd.Parameters.AddWithValue("@ownerId", appointment.OwnerId);
                        cmd.Parameters.AddWithValue("@date", appointment.AppointmentDate);
                        cmd.Parameters.AddWithValue("@reason", appointment.Reason ?? "");

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в CreateAppointment: {ex.Message}");
                return false;
            }
        }







        public bool UpdateAppointmentStatus(int appointmentId, string status)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Appointments SET Status = @status WHERE Id = @id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@id", appointmentId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в UpdateAppointmentStatus: {ex.Message}");
                return false;
            }
        }

















        // МЕДИЦИНСКИЕ ЗАПИСИ

        public List<MedicalRecord> GetPetMedicalHistory(int petId)
        {
            var records = new List<MedicalRecord>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT m.*, p.Name as PetName, u.FullName as DoctorName 
                                   FROM MedicalRecords m
                                   JOIN Pets p ON m.PetId = p.Id
                                   LEFT JOIN Users u ON m.DoctorId = u.Id
                                   WHERE m.PetId = @petId 
                                   ORDER BY m.RecordDate DESC";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {


                        cmd.Parameters.AddWithValue("@petId", petId);




                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                records.Add(new MedicalRecord
                                {
                                    Id = reader.GetInt32(0),
                                    PetId = reader.GetInt32(1),
                                    PetName = reader.GetString(8),
                                    AppointmentId = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2),
                                    Diagnosis = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Treatment = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Prescription = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    RecordDate = reader.GetDateTime(6),
                                    DoctorId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                    DoctorName = reader.IsDBNull(9) ? "" : reader.GetString(9)
                                });
                            }
                        }




                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetPetMedicalHistory: {ex.Message}");
            }
            return records;
        }







        public bool AddMedicalRecord(MedicalRecord record)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO MedicalRecords 
                                   (PetId, AppointmentId, Diagnosis, Treatment, Prescription, DoctorId) 
                                   VALUES (@petId, @appointmentId, @diagnosis, @treatment, @prescription, @doctorId)";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@petId", record.PetId);
                        cmd.Parameters.AddWithValue("@appointmentId", record.AppointmentId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@diagnosis", record.Diagnosis ?? "");
                        cmd.Parameters.AddWithValue("@treatment", record.Treatment ?? "");
                        cmd.Parameters.AddWithValue("@prescription", record.Prescription ?? "");
                        cmd.Parameters.AddWithValue("@doctorId", record.DoctorId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в AddMedicalRecord: {ex.Message}");
                return false;
            }
        }









        // АПТЕКА

        public List<Medicine> GetAllMedicines()
        {
            var medicines = new List<Medicine>();
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Medicines ORDER BY Name";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicines.Add(new Medicine
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Quantity = reader.GetInt32(3),
                                Price = reader.GetDecimal(4),
                                ExpiryDate = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в GetAllMedicines: {ex.Message}");
            }
            return medicines;
        }















        public bool AddMedicine(Medicine medicine)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Medicines (Name, Description, Quantity, Price, ExpiryDate) 
                                   VALUES (@name, @description, @quantity, @price, @expiryDate)";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", medicine.Name);
                        cmd.Parameters.AddWithValue("@description", medicine.Description ?? "");
                        cmd.Parameters.AddWithValue("@quantity", medicine.Quantity);
                        cmd.Parameters.AddWithValue("@price", medicine.Price);
                        cmd.Parameters.AddWithValue("@expiryDate", medicine.ExpiryDate);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в AddMedicine: {ex.Message}");
                return false;
            }
        }

















        public bool UpdateMedicine(Medicine medicine)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Medicines 
                                   SET Name = @name, Description = @description, 
                                       Quantity = @quantity, Price = @price, ExpiryDate = @expiryDate 
                                   WHERE Id = @id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", medicine.Id);
                        cmd.Parameters.AddWithValue("@name", medicine.Name);
                        cmd.Parameters.AddWithValue("@description", medicine.Description ?? "");
                        cmd.Parameters.AddWithValue("@quantity", medicine.Quantity);
                        cmd.Parameters.AddWithValue("@price", medicine.Price);
                        cmd.Parameters.AddWithValue("@expiryDate", medicine.ExpiryDate);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в UpdateMedicine: {ex.Message}");
                return false;
            }
        }














        public bool UpdateMedicineQuantity(int medicineId, int newQuantity)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Medicines SET Quantity = @quantity WHERE Id = @id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@quantity", newQuantity);
                        cmd.Parameters.AddWithValue("@id", medicineId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в UpdateMedicineQuantity: {ex.Message}");
                return false;
            }
        }

















        public bool DeleteMedicine(int medicineId)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Medicines WHERE Id = @id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", medicineId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в DeleteMedicine: {ex.Message}");
                return false;
            }
        }
    }
}