using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MyApp.Database;
using MyApp.Models;
using MySql.Data.MySqlClient;

namespace MyApp.Repositories {

    public class FinancesRepository : IFinancesRepository {
        DBConnection conn;
        public FinancesRepository() {
            if (conn==null) conn = new DBConnection();
        }

        private StringBuilder BuildSearchable(Dictionary<string, object> searchable, StringBuilder sb, MySqlCommand command) {
            if (searchable != null) {
                if (searchable["financeTypeId"] != null && (long) searchable["financeTypeId"] != 0) {
                    sb.Append("AND f.finance_type_id = @financeTypeId ");
                    command.Parameters.AddWithValue("@financeTypeId", (long) searchable["financeTypeId"]);
                }
                if ( searchable["placeId"] != null && (long) searchable["placeId"] != 0) {
                    sb.Append("AND f.place_id = @placeId ");
                    command.Parameters.AddWithValue("@placeId", (long) searchable["placeId"]);
                }
                if ( !(String.IsNullOrEmpty((string)searchable["startDate"]) && 
                       String.IsNullOrEmpty((string)searchable["startDate"]))) 
                {
                    sb.Append("AND f.date BETWEEN @startDate AND @endDate ");
                    command.Parameters.AddWithValue("@startDate", searchable["startDate"]);
                    command.Parameters.AddWithValue("@endDate", searchable["endDate"]);
                }
            }
            return sb;
        }

        private StringBuilder BuildSortable(Dictionary<string, object> sortable, StringBuilder sb) {
            if (sortable != null) {
               if ( !(String.IsNullOrEmpty((string)sortable["sortColumn"]) && 
                      String.IsNullOrEmpty((string)sortable["sortType"])))  {
                    sb.Append("ORDER BY f."+sortable["sortColumn"]+" "+sortable["sortType"]+" ");
                }                
                if (sortable["offset"] != null && sortable["max"] != null) {
                    sb.Append(" LIMIT "+sortable["offset"]+", "+sortable["max"]+"; ");
                }   
            }
            return sb;
            
        }

        public Decimal GetFinancesSumByUserId(long userId, Dictionary<string, object> searchable) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT IFNULL(sumValue, 0) AS sumValue FROM( ")
              .Append("SELECT sum(f.value) AS sumValue from finances f ")
              .Append("WHERE f.user_id = @userId ");
            command.Parameters.AddWithValue("@userId", userId);

            BuildSearchable(searchable, sb, command);

            sb.Append(") q;");   
            command.CommandText = sb.ToString();
            var dataReader = conn.GetDataReader(command);
            var d = 0M;
            if (dataReader.Read()) {
                d = (Decimal) dataReader["sumValue"];
            }
            dataReader.Close();
            return d;
        }

        public Finance GetFinanceById(long id) {
            Finance f = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM finances f ")
              .Append("INNER JOIN finance_types ft ON f.finance_type_id = ft.id_finance_type ")
              .Append("INNER JOIN places p ON f.place_id = p.id_place ")
              .Append("WHERE f.id_finance = @id;");
            MySqlCommand command = new MySqlCommand();
            command.Parameters.AddWithValue("@id", id);
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            if (dataReader.Read()) {
                f = new Finance {
                        id     = (long)     dataReader["id_finance"],
                        date   = (DateTime) dataReader["date"],
                        value  = (Decimal)  dataReader["value"],
                        status = (int)      dataReader["status"],
                        level  = (int)      dataReader["level"],
                         financeType = new FinanceType {
                            id   = (long)   dataReader["id_finance_type"],
                            name = (string) dataReader["name_finance_type"]
                        },
                        place = new Place {
                            id   = (long)   dataReader["id_place"],
                            name = (string) dataReader["name_place"]
                        }
                    };
            }
            dataReader.Close();
            return f;
        }

        public List<Finance> GetAllFinancesByUserId(long userId, 
                                                    Dictionary<string, object> sortable, 
                                                    Dictionary<string, object> searchable) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT * FROM finances f ")
              .Append("INNER JOIN finance_types ft ON f.finance_type_id = ft.id_finance_type ")
              .Append("INNER JOIN places p ON f.place_id = p.id_place ")
              .Append("WHERE f.user_id = @id ");
            command.Parameters.AddWithValue("@id", userId);

            BuildSearchable(searchable, sb, command);
            BuildSortable(sortable, sb);
                        
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            List<Finance> finances = new List<Finance>();
            while (dataReader.Read()) {
                finances.Add(
                    new Finance {
                        id     = (long)     dataReader["id_finance"],
                        date   = (DateTime) dataReader["date"],
                        value  = (Decimal)  dataReader["value"],
                        status = (int)      dataReader["status"],
                        level  = (int)      dataReader["level"],
                         financeType = new FinanceType {
                            id   = (long)   dataReader["id_finance_type"],
                            name = (string) dataReader["name_finance_type"]
                        },
                        place = new Place {
                            id   = (long)   dataReader["id_place"],
                            name = (string) dataReader["name_place"]
                        }
                    }
                );
            }
            dataReader.Close();
            return finances;
        }

        public List<FinanceType> GetAllFinanceTypesByUserId(long userId) {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM lpc_g1_app.finance_types ft ")
              .Append("WHERE ft.user_id = @id; ");
            MySqlCommand command = new MySqlCommand();
            command.Parameters.AddWithValue("@id", userId);
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            List<FinanceType> financeTypes = new List<FinanceType>();
            while(dataReader.Read()) {
                financeTypes.Add(new FinanceType {
                    id = (long) dataReader["id_finance_type"],
                    name = (String) dataReader["name_finance_type"]
                });
            }
            dataReader.Close();
            return financeTypes;
        }
        public List<Place> GetAllPlacesByUserId(long userId) {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM places p ")
              .Append("WHERE p.user_id = @id; ");
            MySqlCommand command = new MySqlCommand();
            command.Parameters.AddWithValue("@id", userId);
            command.CommandText = sb.ToString();
            MySqlDataReader dataReader = conn.GetDataReader(command);
            List<Place> places = new List<Place>();
            while(dataReader.Read()) {
                places.Add(new Place {
                    id = (long) dataReader["id_place"],
                    name = (String) dataReader["name_place"]
                });
            }
            dataReader.Close();
            return places; 
        }

        public long CreateFinance(Finance finance) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("INSERT INTO finances(date, value, level, status, finance_type_id, place_id, user_id) ")
              .Append("values (@date, @value, @level, @status, @finance_type_id, @place_id, @user_id); ")
              .Append("SELECT last_insert_id(); ");
            command.Parameters.AddWithValue("@date", finance.date);
            command.Parameters.AddWithValue("@value", finance.value);
            command.Parameters.AddWithValue("@level", finance.level);
            command.Parameters.AddWithValue("@status", finance.status);
            command.Parameters.AddWithValue("@finance_type_id", finance.financeType.id);
            command.Parameters.AddWithValue("@place_id", finance.place.id);
            command.Parameters.AddWithValue("@user_id", finance.userId);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }

        [HttpPost]
        public long CreateFinanceType(FinanceType financeType) {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO lpc_g1_app.finance_types(name_finance_type, user_id) ")
              .Append("values (@name_finance_type, @user_id); ")
              .Append("SELECT last_insert_id(); ");
            MySqlCommand command = new MySqlCommand();
            command.Parameters.AddWithValue("@name_finance_type", financeType.name);
            command.Parameters.AddWithValue("@user_id", financeType.id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }

        public long DeleteFinance(long id) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("DELETE FROM finances WHERE id_finance = @id_finance; ")
              .Append("SELECT CAST(ROW_COUNT() AS UNSIGNED INTEGER); ");
            command.Parameters.AddWithValue("@id_finance", id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }
        public long DeleteFinanceType(long id) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("DELETE FROM finance_types WHERE id_finance_type = @id_finance_type; ")
              .Append("SELECT CAST(ROW_COUNT() AS UNSIGNED INTEGER); ");
            command.Parameters.AddWithValue("@id_finance_type", id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }

        public long CreatePlace(Place place) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("INSERT INTO places(name_place, user_id) ")
              .Append("values (@name_place, @user_id); ")
              .Append("SELECT last_insert_id(); ");
            command.Parameters.AddWithValue("@name_place", place.name);
            command.Parameters.AddWithValue("@user_id", place.id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }

        public long DeletePlace(long id) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("DELETE FROM places WHERE id_place = @id_place; ")
              .Append("SELECT CAST(ROW_COUNT() AS UNSIGNED INTEGER); ");
            command.Parameters.AddWithValue("@id_place", id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }

        public bool CheckForeign(long id, string table, string column) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("SELECT CAST(COUNT("+column+") AS UNSIGNED INTEGER) FROM "+table+" WHERE "+column+" = @id; ");
            command.Parameters.AddWithValue("@id", id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command) == 0L;
        }

        public long UpdateStatus(long id, int status) {
            StringBuilder sb = new StringBuilder();
            MySqlCommand command = new MySqlCommand();
            sb.Append("UPDATE finances SET status = @status ")
              .Append("WHERE id_finance = @id; ")
              .Append("SELECT CAST(ROW_COUNT() AS UNSIGNED INTEGER); ");
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", id);
            command.CommandText = sb.ToString();
            return (long) conn.ExecuteCommandScalar(command);
        }
        
    }
}