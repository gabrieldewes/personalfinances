using MySql.Data.MySqlClient;
using System;

namespace MyApp.Database
{
    public class DBConnection {
        MySqlConnection connection;
        MySqlDataReader dataReader;
        const String URL = "Server=localhost;Port=3306;Database=lpc_g1_app;Uid=root;Pwd=root;SslMode=None;";

        public DBConnection() {
            connection = new MySqlConnection(URL);
        }

        private void GetConnection() {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
        }

        public void CloseConnection() {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public void ExecuteCommand(MySqlCommand command) {
            GetConnection();
            command.Connection = connection;
            command.ExecuteNonQuery();
            CloseConnection();
        }

        public ulong ExecuteCommandScalar(MySqlCommand command) {
            GetConnection();
            command.Connection = connection;
            var id = (ulong) command.ExecuteScalar();
            CloseConnection();
            return id;
        }

        public MySqlDataReader GetDataReader(MySqlCommand command) {
            GetConnection();
            command.Connection = connection;
            var data = dataReader = command.ExecuteReader();
            return data;
        }
    }
}
