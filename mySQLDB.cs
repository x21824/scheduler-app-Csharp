using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows;
using MySql.Data.MySqlClient;

namespace scheduler
{
    public class mySQLDB
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string pass;
        public static int loggedInUserID;
        public static string loggedInUserName;
        public static int appointmentID;

        public mySQLDB()
        {
            Initialize();
        }

        private void Initialize()
        {
            server = "localhost";
            database = "client_schedule";
            uid = "sqlUser";
            pass = "Passw0rd!";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + pass + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server :( ");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username and/or password, try again.");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void InsertIntoCustomer()
        {
            string query = "INSERT INTO client_schedule.customer VALUES(5, 'Hehe Mari', 3, 1,'2024-07-14 00:00:00','test','2024-07-14 00:00:00','test')";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public void Update()
        {

        }

        public void Delete()
        {

        }

        public int LoginDBUser(string user, string pass)
        {
            string query = "SELECT COUNT(*) FROM client_schedule.user WHERE userName='" + user + "' AND password='" + pass + "';";
            int count = -1;
            int loggedInUserID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                count = int.Parse(cmd.ExecuteScalar() + "");
                if (count == 1)
                {
                    string subQuery = "SELECT userID FROM client_schedule.user WHERE userName = '" + user + "' AND password = '" + pass + "'; ";
                    MySqlCommand subCmd = new MySqlCommand(subQuery, connection);
                    loggedInUserID = int.Parse(subCmd.ExecuteScalar() + "");
                    SetLoggedInUID(loggedInUserID);
                    SetLoggedInUName(user);
                    this.CloseConnection();
                    return loggedInUserID;
                }
                else
                {
                    this.CloseConnection();
                    return loggedInUserID;
                }
            }
            else
            {
                return loggedInUserID;
            }
        }

        public static void SetLoggedInUID(int currentUID)
        {
            loggedInUserID = currentUID;
        }

        public static int GetLoggedInUID()
        {
            return loggedInUserID;
        }
        public static void SetAppointmentID(int currentAID)
        {
            appointmentID = currentAID;
        }

        public static int GetAppointmentID()
        {
            return appointmentID;
        }

        public static void SetLoggedInUName(string uname)
        {
            loggedInUserName = uname;
        }

        public static string GetLoggedInUName()
        {
            return loggedInUserName;
        }
        public string GetConsultantNameFromUserID(int userID)
        {
            string consultantName = "Error: No Customer Found";
            string query = "SELECT userName FROM client_schedule.user WHERE userId = " + userID + ";";
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                consultantName = (cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return consultantName;
            }
            else
            {
                return consultantName;
            }
        }

        public List<string> SelectAppointmentColumns()
        {
            string query = "SELECT COLUMN_NAME FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = 'appointment';";

            List<string> columnList = new List<string>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    columnList.Add(dataReader["COLUMN_NAME"] + "");
                }

                dataReader.Close();

                this.CloseConnection();

                return columnList;
            }
            else
            {
                return columnList;
            }
        }

        public List<Appointment> SelectAllAppointments()
        {
            string query = "SELECT * FROM client_schedule.appointment";

            var list = new List<Appointment>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    list.Add(new Appointment
                    {
                        Id = ((int)dataReader["appointmentId"]),
                        apptCustID = ((int)dataReader["customerId"]),
                        apptUserID = ((int)dataReader["userId"]),
                        apptTitle = dataReader["title"].ToString(),
                        apptDescription = dataReader["Description"].ToString(),
                        apptLocation = dataReader["location"].ToString(),
                        apptContact = dataReader["contact"].ToString(),
                        apptType = dataReader["type"].ToString(),
                        apptURL = dataReader["url"].ToString(),
                        startDateTime = (DateTime)dataReader["start"],
                        endDateTime = (DateTime)dataReader["end"],
                        Created = (DateTime)dataReader["createDate"],
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = (DateTime)dataReader["lastUpdate"],
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();

                return list;
            }
            else
            {
                return list;
            }
        }

        public List<Appointment> SelectCurrentUIDAppointments()
        {
            string query = "SELECT * FROM client_schedule.appointment WHERE userId = " + GetLoggedInUID() + ";";

            var list = new List<Appointment>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    list.Add(new Appointment
                    {
                        Id = ((int)dataReader["appointmentId"]),
                        apptCustID = ((int)dataReader["customerId"]),
                        apptUserID = ((int)dataReader["userId"]),
                        apptTitle = dataReader["title"].ToString(),
                        apptDescription = dataReader["Description"].ToString(),
                        apptLocation = dataReader["location"].ToString(),
                        apptContact = dataReader["contact"].ToString(),
                        apptType = dataReader["type"].ToString(),
                        apptURL = dataReader["url"].ToString(),
                        startDateTime = (DateTime)dataReader["start"],
                        endDateTime = (DateTime)dataReader["end"],
                        Created = (DateTime)dataReader["createDate"],
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = (DateTime)dataReader["lastUpdate"],
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();

                return list;
            }
            else
            {
                return list;
            }
        }

        public int CheckForApptsAtTime(DateTime start, DateTime end)
        {
            string startDT = start.ToString("yyyy-MM-dd HH:mm:ss");
            string endDT = end.ToString("yyyy-MM-dd HH:mm:ss");
            string query = "SELECT Count(*) FROM client_schedule.appointment WHERE (start = '" + startDT + "') && (end = '" + endDT + "') && (userId = " + GetLoggedInUID() + ");";
            int count = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                count = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return count;
            }
            else
            {
                return count;
            }
        }

        public int GetMaxAppointmentID()
        {
            int maxAppointmentID = 0;

            string query = "SELECT MAX(appointmentId) FROM appointment";

            string connectionString = "Server=127.0.0.1;Port=3306;Database=client_schedule;User ID=sqlUser;Password=Passw0rd!;";

            try
            {
                MySqlConnection dbConnection = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(query, dbConnection);
                dbConnection.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    maxAppointmentID = Convert.ToInt32(result);
                }
                else
                {
                    maxAppointmentID = 0;
                }

                dbConnection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return maxAppointmentID;

        }



        public bool InsertNewAppointment(int custID, int userID, string title, string description, string location, string contact, string type, string url, DateTime start, DateTime end, DateTime create, string createBy, DateTime last, string lastBy)
        {
            string formatStart = start.ToString("yyyy-MM-dd HH:mm:ss");
            string formatEnd = end.ToString("yyyy-MM-dd HH:mm:ss");
            string formatCreate = create.ToString("yyyy-MM-dd HH:mm:ss");
            string formatLast = last.ToString("yyyy-MM-dd HH:mm:ss");
            string query = "INSERT INTO client_schedule.appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES(" + custID + "," + userID + ",'" + title + "','" + description + "','" + location + "','" + contact + "','" + type + "','" + url + "','" + formatStart + "','" + formatEnd + "','" + formatCreate + "','" + createBy + "','" + formatLast + "','" + lastBy + "');";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            else
            {
                this.CloseConnection();
                return false;
            }

        }

        public bool UpdateAppointment(int apptID, int custID, string title, string description, string location, string contact, string type, string url, DateTime start, DateTime end, DateTime last, string lastBy)
        {
            string formatStart = start.ToString("yyyy-MM-dd HH:mm:ss");
            string formatEnd = end.ToString("yyyy-MM-dd HH:mm:ss");
            string formatLast = last.ToString("yyyy-MM-dd HH:mm:ss");
            string query = "UPDATE client_schedule.appointment SET customerId = " + custID + ", title = '" + title + "', description = '" + description + "', location = '" + location + "', contact = '" + contact + "', type = '" + type + "', url = '" + url + "', start = '" + formatStart + "', end = '" + formatEnd + "', lastUpdate = '" + formatLast + "', lastUpdateBy = '" + lastBy + "' WHERE appointmentId = " + apptID + ";";
            Console.WriteLine(query);
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            else
            {
                this.CloseConnection();
                return false;
            }
        }
        public void DeleteAppointment(int apptID)
        {
            string query = "DELETE FROM client_schedule.appointment WHERE appointmentId = " + apptID + ";";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
            else
            {
                this.CloseConnection();
            }
        }

        public void DeleteCustomerAppointments(int customerID)
        {
            string query = "DELETE FROM client_schedule.appointment WHERE customerId = " + customerID + ";";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
            else
            {
                this.CloseConnection();
            }
        }

        public int GetMaxCustomerID()
        {
            string query = "SELECT MAX(customerId) FROM client_schedule.customer;";
            int maxCustID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    maxCustID = int.Parse(result.ToString());
                }
                else
                {
                    maxCustID = 1; // If no records exist, start with ID 1
                }

                this.CloseConnection();
                return maxCustID + 1;
            }
            else
            {
                return maxCustID;
            }

        }


        public List<Customer> SelectAllCustomers()
        {
            string query = "SELECT * FROM client_schedule.customer";

            var custList = new List<Customer>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    custList.Add(new Customer
                    {
                        Id = (int)dataReader["customerId"],
                        customerName = dataReader["customerName"].ToString(),
                        customerAddressID = (int)dataReader["addressId"],
                        customerActive = Convert.ToByte(dataReader["active"]),
                        Created = ((DateTime)dataReader["createDate"]).ToUniversalTime(),
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = ((DateTime)dataReader["lastUpdate"]).ToUniversalTime(),
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();

                return custList;
            }
            else
            {
                return custList;
            }
        }
        public List<Customer> SelectAllCustomersAndAddressData()
        {
            string query = "SELECT customer.*, address.address, address.address2, address.postalCode, address.phone, city.city, country.country FROM client_schedule.customer INNER JOIN address ON customer.addressId = address.addressId INNER JOIN city ON address.cityId = city.cityId INNER JOIN country ON city.countryId = country.countryId;";

            var custList = new List<Customer>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    custList.Add(new Customer
                    {
                        Id = (int)dataReader["customerId"],
                        customerName = dataReader["customerName"].ToString(),
                        customerAddressID = (int)dataReader["addressId"],
                        customerActive = Convert.ToByte(dataReader["active"]),
                        Created = ((DateTime)dataReader["createDate"]).ToUniversalTime(),
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = ((DateTime)dataReader["lastUpdate"]).ToUniversalTime(),
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                        address = dataReader["address"].ToString(),
                        address2 = dataReader["address2"].ToString(),
                        postal = dataReader["postalCode"].ToString(),
                        phone = dataReader["phone"].ToString(),
                        city = dataReader["city"].ToString(),
                        country = dataReader["country"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();
                return custList;
            }
            else
            {
                return custList;
            }
        }
        public int GetCustomerIDFromCustomerName(string custName)
        {
            string customerName = custName;
            int customerID = -1;
            List<Customer> customers = new List<Customer>();
            mySQLDB mySQLDB = new mySQLDB();
            customers = SelectAllCustomers();
            foreach (var customer in customers)
            {
                if (customerName == customer.customerName)
                {
                    customerID = customer.Id;
                    return customerID;
                }
            }
            return customerID;
        }

        public string GetCustomerNameFromCustomerID(int custID)
        {
            string customerName = "Error: No Customer Found";
            string query = "SELECT customerName FROM client_schedule.customer WHERE customerId = " + custID + ";";
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                customerName = (cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return customerName;
            }
            else
            {
                return customerName;
            }
        }

        public bool InsertCustomer(string customerName, int addressID)
        {
            int customerID = GetMaxCustomerID();
            if (customerID == -1)
            {
                MessageBox.Show("Exception has occurred. Unable to add Customer, the CustomerID is invalid.", "scheduler - Customer Error");
                return false;
            }
            DateTime current = DateTime.UtcNow;
            string currentF = current.ToString("yyyy-MM-dd HH:mm:ss");
            string uname = GetLoggedInUName();
            string query = "INSERT INTO client_schedule.customer VALUES(" + customerID + ",'" + customerName + "'," + addressID + "," + 1 + ",'" + currentF + "','" + uname + "','" + currentF + "','" + uname + "');";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateCustomer(int customerID, string customerName, int addressID, string address, string phone)
        {
            DateTime current = DateTime.UtcNow;
            string currentF = current.ToString("yyyy-MM-dd HH:mm:ss");
            string uname = GetLoggedInUName();

            if (this.OpenConnection() == true)
            {
                // Update customer table
                string query = "UPDATE client_schedule.customer SET customerName = '" + customerName + "', addressId = " + addressID + ", lastUpdate = '" + currentF + "', lastUpdateBy = '" + uname + "' WHERE customerId = " + customerID + ";";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                // Update address table
                query = "UPDATE client_schedule.address SET address = '" + address + "', phone = '" + phone + "', lastUpdate = '" + currentF + "', lastUpdateBy = '" + uname + "' WHERE addressId = " + addressID + ";";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteCustomer(int customerID)
        {
            DeleteCustomerAppointments(customerID); // Delete related appointments first
            string query = "DELETE FROM client_schedule.customer WHERE customerId = " + customerID + ";";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
            else
            {
                this.CloseConnection();
            }
        }

        public int GetMaxAddressID()
        {
            string query = "SELECT MAX(addressId) FROM client_schedule.address;";
            int maxAddressID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                maxAddressID = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return maxAddressID + 1;
            }
            else
            {
                return maxAddressID;
            }
        }
        public int GetAddressIDFromAddress(string address, string address2, int cityID)
        {
            string countQuery = "SELECT COUNT(addressId) FROM client_schedule.address WHERE (address = '" + address + "') && (address2 = '" + address2 + "') && (cityId = " + cityID + ");";
            string query = "SELECT addressId FROM client_schedule.address WHERE (address = '" + address + "') && (address2 = '" + address2 + "') && (cityId = " + cityID + ");";
            int countAddressID = -1;
            int addressID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand countCmd = new MySqlCommand(countQuery, connection);
                countAddressID = int.Parse(countCmd.ExecuteScalar() + "");
                if (countAddressID > 1)
                {
                    return addressID;
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    addressID = int.Parse(cmd.ExecuteScalar() + "");
                    this.CloseConnection();
                    return addressID;
                }
            }
            else
            {
                return addressID;
            }
        }

        public void InsertAddress(string cityName, string address, string address2, string postal, string phone)
        {
            int addressID = GetMaxAddressID();
            if (addressID == -1)
            {
                MessageBox.Show("Exception has occurred. Unable to add Address, the AddressID is invalid.", "scheduler - Address Error");
                return;
            }
            int cityID = GetCityIDFromCityName(cityName);
            DateTime current = DateTime.UtcNow;
            string currentF = current.ToString("yyyy-MM-dd HH:mm:ss");
            string uname = GetLoggedInUName();
            string query = "INSERT INTO client_schedule.address VALUES(" + addressID + ",'" + address + "','" + address2 + "'," + cityID + ",'" + postal + "','" + phone + "','" + currentF + "','" + uname + "','" + currentF + "','" + uname + "');";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public List<Address> SelectAllAddresses()
        {
            string query = "SELECT * FROM client_schedule.address";

            var addressList = new List<Address>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    addressList.Add(new Address
                    {
                        Id = (int)dataReader["addressId"],
                        address = dataReader["address"].ToString(),
                        address2 = dataReader["address2"].ToString(),
                        cityID = (int)dataReader["cityId"],
                        postalCode = dataReader["postalCode"].ToString(),
                        phone = dataReader["phone"].ToString(),
                        Created = ((DateTime)dataReader["createDate"]).ToLocalTime(),
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = ((DateTime)dataReader["lastUpdate"]).ToLocalTime(),
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();

                return addressList;
            }
            else
            {
                return addressList;
            }
        }

        public int GetMaxCityID()
        {
            string query = "SELECT MAX(cityId) FROM client_schedule.city;";
            int maxCityID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                maxCityID = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return maxCityID + 1;
            }
            else
            {
                return maxCityID;
            }
        }
        public int GetCountryIDFromCityName(string cityName)
        {
            string cityNormalized = (char.ToUpper(cityName[0]) + cityName.Substring(1));
            string query = "SELECT countryId FROM client_schedule.city WHERE city = '" + cityNormalized + "';";
            int countryID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                countryID = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return countryID;
            }
            else
            {
                return countryID;
            }
        }

        public int GetCityIDFromCityName(string cityName)
        {
            string cityNormalized = (char.ToUpper(cityName[0]) + cityName.Substring(1));
            string query = "SELECT cityId FROM client_schedule.city WHERE city = '" + cityNormalized + "';";
            int cityID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cityID = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return cityID;
            }
            else
            {
                return cityID;
            }
        }
        public string GetCityNameFromCityID(int cityID)
        {
            string query = "SELECT city FROM client_schedule.city WHERE cityId = '" + cityID + "';";
            string cityName = "";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cityName = (cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return cityName;
            }
            else
            {
                return cityName;
            }
        }

        public void InsertCity(string countryName, string cityName)
        {
            int countryID = GetCountryIDFromCountryName(countryName);
            int cityID = GetMaxCityID();
            if (cityID == -1)
            {
                MessageBox.Show("Exception has occurred. Unable to add City, the CityID is invalid.", "scheduler - City Error");
                return;
            }
            string cityNameNormalized = (char.ToUpper(cityName[0]) + cityName.Substring(1));
            DateTime current = DateTime.UtcNow;
            string uname = GetLoggedInUName();

            string query = "INSERT INTO client_schedule.city VALUES(" + cityID + ",'" + cityNameNormalized + "'," + countryID + ",'" + current + "','" + uname + "','" + current + "','" + uname + ");";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        public List<City> SelectAllCities()
        {
            string query = "SELECT * FROM client_schedule.city";

            var cityList = new List<City>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    cityList.Add(new City
                    {
                        Id = (int)dataReader["cityId"],
                        city = dataReader["city"].ToString(),
                        countryID = (int)dataReader["countryId"],
                        Created = ((DateTime)dataReader["createDate"]).ToUniversalTime(),
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = ((DateTime)dataReader["lastUpdate"]).ToUniversalTime(),
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();

                return cityList;
            }
            else
            {
                return cityList;
            }
        }

        public int GetMaxCountryID()
        {
            string query = "SELECT MAX(countryId) FROM client_schedule.country;";
            int maxCountryID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                maxCountryID = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return maxCountryID + 1;
            }
            else
            {
                return maxCountryID;
            }
        }
        public int GetCountryIDFromCountryName(string countryName)
        {
            string countryNormalized = (char.ToUpper(countryName[0]) + countryName.Substring(1));
            string query = "SELECT countryId FROM client_schedule.country WHERE country = '" + countryNormalized + "';";
            int countryID = -1;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                countryID = int.Parse(cmd.ExecuteScalar() + "");
                this.CloseConnection();
                return countryID;
            }
            else
            {
                return countryID;
            }
        }

        public void InsertNewCountry(string country)
        {
            int cid = GetMaxCountryID();
            if (cid == -1)
            {
                MessageBox.Show("Exception has occurred. Unable to add Country, the CountryID is invalid.", "scheduler - Country Error");
                return;
            }
            string countryNormalized = (char.ToUpper(country[0]) + country.Substring(1));
            DateTime current = DateTime.UtcNow;
            string uname = GetLoggedInUName();
            string query = "INSERT INTO client_schedule.country VALUES(" + cid + ",'" + countryNormalized + "','" + current + "','" + uname + "','" + current + "','" + uname + ");";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        public List<Country> SelectAllCountries()
        {
            string query = "SELECT * FROM client_schedule.country";

            var countryList = new List<Country>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    countryList.Add(new Country
                    {
                        Id = (int)dataReader["countryId"],
                        country = dataReader["country"].ToString(),
                        Created = ((DateTime)dataReader["createDate"]).ToUniversalTime(),
                        CreatedBy = dataReader["createdBy"].ToString(),
                        Modified = ((DateTime)dataReader["lastUpdate"]).ToUniversalTime(),
                        ModifiedBy = dataReader["lastUpdateBy"].ToString(),
                    });

                }

                dataReader.Close();

                this.CloseConnection();
                return countryList;
            }
            else
            {
                return countryList;
            }
        }

        public List<NumberAppointmentTypesByMonth> Report_NumAppointmentTypesByMonth()
        {
            List<NumberAppointmentTypesByMonth> reportList = new List<NumberAppointmentTypesByMonth>();

            string query = "SELECT EXTRACT(YEAR_MONTH FROM start) appointmentMonth, type, COUNT(*) AS 'Count' FROM appointment GROUP BY appointmentMonth, type";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    reportList.Add(new NumberAppointmentTypesByMonth
                    {
                        appointmentMonth = dataReader["appointmentMonth"].ToString(),
                        appointmentType = dataReader["type"].ToString(),
                        appointmentCount = dataReader["Count"].ToString()
                    });

                }

                dataReader.Close();
                this.CloseConnection();
                return reportList;
            }
            else
            {
                return reportList;
            }

        }
        public List<ConsultantSchedule> Report_ConsultantSchedules()
        {
            List<ConsultantSchedule> reportList = new List<ConsultantSchedule>();

            string query = "SELECT userId, customerId, title, type, start, end FROM appointment ORDER BY userId, start";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    reportList.Add(new ConsultantSchedule
                    {
                        consultantName = dataReader["userId"].ToString(),
                        customerName = dataReader["customerId"].ToString(),
                        apptTitle = dataReader["title"].ToString(),
                        apptType = dataReader["type"].ToString(),
                        startTime = ((DateTime)dataReader["start"]).ToLocalTime(),
                        endTime = ((DateTime)dataReader["end"]).ToLocalTime()
                    });

                }

                dataReader.Close();
                this.CloseConnection();
                return reportList;
            }
            else
            {
                return reportList;
            }

        }
        public List<NumberCustomersAndAppointmentsByCity> Report_NumberCustomersAndAppointmentsByCity()
        {
            List<NumberCustomersAndAppointmentsByCity> reportList = new List<NumberCustomersAndAppointmentsByCity>();

            string query = "SELECT city.city, COUNT(DISTINCT appointment.appointmentId), COUNT(DISTINCT customer.customerId) FROM appointment RIGHT JOIN customer on appointment.customerId = customer.customerId JOIN address on customer.addressId = address.addressId JOIN city on address.cityId = city.cityId GROUP BY city";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    reportList.Add(new NumberCustomersAndAppointmentsByCity
                    {
                        cityName = dataReader["city"].ToString(),
                        countAppointments = dataReader["COUNT(DISTINCT appointment.appointmentId)"].ToString(),
                        countCustomers = dataReader["COUNT(DISTINCT customer.customerId)"].ToString(),
                    });

                }

                dataReader.Close();
                this.CloseConnection();
                return reportList;
            }
            else
            {
                return reportList;
            }

        }
    }
}
