using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace lab1.DB
{
    public static class Queries
    {
        public static string UpdateClientQuery = "UPDATE client SET @Field = @NewValue WHERE id = @ClientId";
        public static string DeleteClientQuery = "DELETE FROM client WHERE id = @ClientId";

        public static string GetAllClientsQuery = @"
                SELECT 
                    c.id,
                    c.name,
                    c.surname,
                    c.lastname,
                    c.dateOfBirth,
                    c.gender,
                    c.passportSeries,
                    c.passportNumber,
                    c.passportIssuedBy,
                    c.passportDateOfIssue,
                    c.passportId,
                    c.birthPlace,
                    c.address,
                    c.phoneNumber,
                    c.stationaryPhoneNumber,
                    c.email,
                    c.placeOfWork,
                    c.jobTitle,
                    c.isRetired,
                    c.monthlyIncome,
                    c.conscript,
                    cor.name AS CityOfResidence,
                    fs.status AS FamilyStatus,
                    ci.citizenship,
                    d.type AS Disability
                FROM 
                    client c
                JOIN 
                    cityOfResidence cor ON c.cityOfResidence_id = cor.id
                JOIN 
                    familyStatus fs ON c.familyStatus_id = fs.id
                JOIN 
                    citizenship ci ON c.citizenship_id = ci.id
                JOIN 
                    disability d ON c.disability_id = d.id;";
        public static DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }

            return dataTable;
        }
        public static void UpdateClient(int clientId, string field, object NewValue)
        {
            using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
            {
                connection.Open();

                string query = "UPDATE client SET " + field + " = @NewValue WHERE id = @ClientId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewValue", NewValue);
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    command.ExecuteNonQuery();
                }
            }
        }
        public static void DeleteClientById(int clientId)
        {
            string deleteQuery = "DELETE FROM client WHERE id = @ClientId";

            using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Клиент с id {clientId} успешно удален из базы данных.");
                        }
                        else
                        {
                            Console.WriteLine($"Клиент с id {clientId} не найден в базе данных.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при удалении клиента: {ex.Message}");
                    }
                }
            }
        }
        public static int InsertClient(string name, string surname, string lastname, DateTime dateOfBirth, int gender,
                                string passportSeries, string passportNumber, string passportIssuedBy,
                                DateTime passportDateOfIssue, string passportID, string birthPlace, string address,
                                string phoneNumber, string stationaryPhoneNumber, string email, string placeOfWork,
                                string jobTitle, int isRetired, double? monthlyIncome, int conscript,
                                int cityOfResidenceId, int familyStatusId, int citizenshipId, int disabilityId)
        {
            using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
            {
                connection.Open();

                // Замените "YourTableName" на имя вашей таблицы
                //string query = "INSERT INTO client (Name, Surname, Lastname, DateOfBirth, Gender, PassportSeries, " +
                //               "PassportNumber, PassportIssuedBy, PassportDateOfIssue, PassportID, BirthPlace, Address, " +
                //               "PhoneNumber, StationaryPhoneNumber, Email, PlaceOfWork, JobTitle, IsRetired, monthlyIncome, " +
                //               "Conscript, CityOfResidence_id, FamilyStatus_id, Citizenship_id, Disability_id) " +
                //               "VALUES (@Name, @Surname, @Lastname, @DateOfBirth, @Gender, @PassportSeries, @PassportNumber, " +
                //               "@PassportIssuedBy, @PassportDateOfIssue, @PassportID, @BirthPlace, @Address, @PhoneNumber, " +
                //               "@StationaryPhoneNumber, @Email, @PlaceOfWork, @JobTitle, @IsRetired, @MonthlyIncome, " +
                //               "@Conscript, @CityOfResidence_id, @FamilyStatus_id, @Citizenship_id, @Disability_id)";
                string queryStart = "INSERT INTO client (Name, Surname, Lastname, DateOfBirth, Gender, PassportSeries, " +
                               "PassportNumber, PassportIssuedBy, PassportDateOfIssue, PassportID, BirthPlace, Address, " +
                               "IsRetired, " +
                               "Conscript, CityOfResidence_id, FamilyStatus_id, Citizenship_id, Disability_id";

                string queryEnd = "VALUES(@Name, @Surname, @Lastname, @DateOfBirth, @Gender, @PassportSeries, @PassportNumber, " +
                               "@PassportIssuedBy, @PassportDateOfIssue, @PassportID, @BirthPlace, @Address, @IsRetired, " +
                               "@Conscript, @CityOfResidence_id, @FamilyStatus_id, @Citizenship_id, @Disability_id";
                string[] additions = { "PhoneNumber", "StationaryPhoneNumber", "Email", "PlaceOfWork", "JobTitle", "monthlyIncome" };
                string commaAndSpace = ", ";
                char at = '@';

                if (phoneNumber != "")
                {
                    queryStart += commaAndSpace + additions[0];
                    queryEnd += commaAndSpace + at + additions[0];
                }

                if (stationaryPhoneNumber != "")
                {
                    queryStart += commaAndSpace + additions[1];
                    queryEnd += commaAndSpace + at + additions[1];
                }
                if (email != "")
                {
                    queryStart += commaAndSpace + additions[2];
                    queryEnd += commaAndSpace + at + additions[2];
                }
                if (placeOfWork != "")
                {
                    queryStart += commaAndSpace + additions[3];
                    queryEnd += commaAndSpace + at + additions[3];
                }
                if (jobTitle != "")
                {
                    queryStart += commaAndSpace + additions[4];
                    queryEnd += commaAndSpace + at + additions[4];
                }
                if (monthlyIncome != null)
                {
                    queryStart += commaAndSpace + additions[5];
                    queryEnd += commaAndSpace + at + additions[5];
                }

                queryStart += ")";
                queryEnd += ")";

                string query = queryStart + queryEnd;
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@Lastname", lastname);
                    command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@PassportSeries", passportSeries);
                    command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                    command.Parameters.AddWithValue("@PassportIssuedBy", passportIssuedBy);
                    command.Parameters.AddWithValue("@PassportDateOfIssue", passportDateOfIssue);
                    command.Parameters.AddWithValue("@PassportID", passportID);
                    command.Parameters.AddWithValue("@BirthPlace", birthPlace);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@StationaryPhoneNumber", stationaryPhoneNumber);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PlaceOfWork", placeOfWork);
                    command.Parameters.AddWithValue("@JobTitle", jobTitle);
                    command.Parameters.AddWithValue("@IsRetired", isRetired);
                    command.Parameters.AddWithValue("@MonthlyIncome", monthlyIncome);
                    command.Parameters.AddWithValue("@Conscript", conscript);
                    command.Parameters.AddWithValue("@CityOfResidence_id", cityOfResidenceId);
                    command.Parameters.AddWithValue("@FamilyStatus_id", familyStatusId);
                    command.Parameters.AddWithValue("@Citizenship_id", citizenshipId);
                    command.Parameters.AddWithValue("@Disability_id", disabilityId);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static bool IsInDB(string tableName, string columnName, string valueToCheck, int? excludedId)
        {
            using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
            {
                connection.Open();

                string query = $"SELECT * FROM {tableName} WHERE {columnName} = @ValueToCheck";
                if (excludedId != null)
                {
                    query = $"SELECT * FROM {tableName} WHERE {columnName} = @ValueToCheck AND Id != @ExcludedId";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ValueToCheck", valueToCheck);
                    if (excludedId != null)
                    {
                        command.Parameters.AddWithValue("@ExcludedId", excludedId);
                    }

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Если есть хотя бы одна запись, то значение уже существует в базе данных
                        return reader.HasRows;
                    }
                }
            }
        }


        public static int GetIdByValue(string tableName, string columnName, string columnValue)
        {
            using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
            {
                connection.Open();

                string query = $"SELECT id FROM {tableName} WHERE {columnName} = @{columnName}";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue($"@{columnName}", columnValue);

                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        public static bool IsPassportExists(string passportSeries, string passportNumber, int? excludedId)
        {
            using (MySqlConnection connection = new MySqlConnection(Connection.ConnectionString))
            {
                connection.Open();

                string query;
                if (excludedId != null)
                {
                    query = $"SELECT COUNT(*) FROM client WHERE PassportSeries = '{passportSeries}' AND PassportNumber = '{passportNumber}' AND Id != {excludedId}";
                }
                else
                {
                    query = $"SELECT COUNT(*) FROM client WHERE PassportSeries = '{passportSeries}' AND PassportNumber = '{passportNumber}'";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }





    }
}
