using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using MyApp.Database;
using MyApp.Models;
using MySql.Data.MySqlClient;

namespace MyApp.Repositories {
    public class AccountRepository : IAccountRepository {
        DBConnection conn;

        public AccountRepository() {
            if (conn == null) conn = new DBConnection();
        }

        public ClaimsPrincipal Get(string username) {
            return null;
        }

        public List<User> GetAll() {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT * FROM lpc_g1_app.users u");
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            List<User> users = new List<User>();
            while (dataReader.Read()) {
                users.Add(
                    new User {
                        id        = (long)  dataReader["id_user"],
                        fullname  = (string) dataReader["fullname"],
                        email     = (string) dataReader["email"],
                        username  = (string) dataReader["username"],
                        password  = (string) dataReader["password"],
                        role      = (string) dataReader["role"]
                    }
                );
            }
            dataReader.Close();
            return users;
        }
        public long Add(User u) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("INSERT INTO users(fullname, username, password, email, role) ")
              .Append("values (@fullname, @username, @password, @email, @role); ")
              .Append("SELECT last_insert_id(); ");
            command.Parameters.AddWithValue("@fullname", u.fullname);
            command.Parameters.AddWithValue("@username", u.username);
            command.Parameters.AddWithValue("@password", u.password);
            command.Parameters.AddWithValue("@email", u.email);
            command.Parameters.AddWithValue("@role", u.role);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }
        public User GetById(long id) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT * FROM lpc_g1_app.users u ");
            sb.Append("WHERE u.id_user = @id");
            command.Parameters.AddWithValue("@id", id);
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            User u = null;
            if (dataReader.Read()) {
                u = new User {
                    id       = (long)   dataReader["id_user"],
                    fullname = (string) dataReader["fullname"],
                    username = (string) dataReader["username"],
                    password = (string) dataReader["password"],
                    email    = (string) dataReader["email"],
                    role     = (string) dataReader["role"]
                };
            }
            dataReader.Close();
            return u;
        }

        public User GetByUsername(string username) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT * FROM lpc_g1_app.users u ");
            sb.Append("WHERE u.username = @username; ");
            command.Parameters.AddWithValue("@username", username);
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            User u=null;
            if (dataReader.Read()) {
                u = new User {
                    id       = (long)   dataReader["id_user"],
                    fullname = (string) dataReader["fullname"],
                    username = (string) dataReader["username"],
                    password = (string) dataReader["password"],
                    email    = (string) dataReader["email"],
                    role     = (string) dataReader["role"]
                };
            }
            dataReader.Close();
            return u;
        }
        public long Update(User u) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("UPDATE users u SET fullname=@fullname, username=@username, password=@password, email=@email ")
              .Append("WHERE u.id_user = @id; ")
              .Append("SELECT CAST(ROW_COUNT() AS UNSIGNED INTEGER); ");
            command.Parameters.AddWithValue("@fullname", u.fullname);
            command.Parameters.AddWithValue("@username", u.username);
            command.Parameters.AddWithValue("@password", u.password);
            command.Parameters.AddWithValue("@email", u.email);
            command.Parameters.AddWithValue("@id", u.id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }
        public long Delete(long id) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("DELETE FROM lpc_g1_app.users u WHERE u.id_user = @id; ")
              .Append("SELECT CAST(ROW_COUNT() AS UNSIGNED INTEGER); ");
            command.Parameters.AddWithValue("@id", id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }
        public bool UserExists(long id) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT CAST(COUNT(id_user) AS UNSIGNED INTEGER) FROM users WHERE id_user = @id; ");
            command.Parameters.AddWithValue("@id", id);
            command.CommandText = sb.ToString();
            return (ulong) conn.ExecuteCommandScalar(command) == 0L;
        }

        public bool FieldAlreadyInUse(string field, string value) {
            return true;
        }
    }
}