using Microsoft.Data.SqlClient;

namespace ParkingPermits
{
    public class PermitRepository
    {
        private readonly string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ParkingPermitsDb;" +
            "Integrated Security=True;TrustServerCertificate=True";

        // Puts permits in three groups: 0 = can be used now, 1 = waiting for payment, 2 = expired.
        // Used by the "Expiring" sorts, so permits that already expired are not shown as "expiring".
        private const string UsableGroup =
            "CASE WHEN ValidUntil < CAST(GETDATE() AS date) THEN 2 " +
            "     WHEN IsPaid = 0 THEN 1 " +
            "     ELSE 0 END";

        // The ORDER BY for each choice in the "Sort by" box, in the same order.
        // A column name cannot be a parameter, so it comes from this fixed list, never from the user.
        private readonly string[] sortOrders =
        {
            "PlateNumber",
            "OwnerName, PlateNumber",
            UsableGroup + ", ValidUntil, PlateNumber",          // Expiring first: usable permits ending soonest on top
            UsableGroup + ", ValidUntil DESC, PlateNumber"      // Expiring last: usable permits ending latest on top
        };

        // text: searched in PlateNumber and OwnerName; empty means "everything"
        // permitType: Student / Staff / Guest; empty means "all types"
        // sortIndex: SelectedIndex of the "Sort by" box
        public List<Permit> Search(string text, string permitType, int sortIndex)
        {
            if (sortIndex < 0 || sortIndex >= sortOrders.Length)
                sortIndex = 0;

            List<Permit> permits = new List<Permit>();
            string sql =
                "SELECT Id, PlateNumber, OwnerName, PermitType, Zone, ValidUntil, IsPaid " +
                "FROM Permits " +
                "WHERE (@text = N'' OR PlateNumber LIKE @pattern OR OwnerName LIKE @pattern) " +
                "  AND (@permitType = N'' OR PermitType = @permitType) " +
                "ORDER BY " + sortOrders[sortIndex];

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@text", text);
                command.Parameters.AddWithValue("@pattern", "%" + EscapeLikeText(text) + "%");
                command.Parameters.AddWithValue("@permitType", permitType);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Permit permit = new Permit();
                        permit.Id = (int)reader["Id"];
                        permit.PlateNumber = (string)reader["PlateNumber"];
                        permit.OwnerName = (string)reader["OwnerName"];
                        permit.PermitType = (string)reader["PermitType"];
                        permit.Zone = (string)reader["Zone"];
                        permit.ValidUntil = (DateTime)reader["ValidUntil"];
                        permit.IsPaid = (bool)reader["IsPaid"];
                        permits.Add(permit);
                    }
                }
            }
            return permits;
        }

        // true if another permit already has this plate number;
        // excludeId is the permit being edited (0 when adding), so it does not match itself
        public bool PlateExists(string plateNumber, int excludeId)
        {
            string sql = "SELECT COUNT(*) FROM Permits WHERE PlateNumber = @plateNumber AND Id <> @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@plateNumber", plateNumber);
                command.Parameters.AddWithValue("@id", excludeId);
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
        }

        // In LIKE, % _ and [ have special meanings ("anything", "any one character", "one of").
        // Wrapping them in [ ] makes SQL Server treat them as ordinary characters,
        // so searching for "%" finds a "%" instead of every row.
        private static string EscapeLikeText(string text)
        {
            return text.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
        }

        // how many spots in a zone are held by permits that have not expired;
        // excludeId is the permit being edited (0 when adding), so it does not count itself
        public int CountTakenSpots(string zone, int excludeId)
        {
            string sql =
                "SELECT COUNT(*) FROM Permits " +
                "WHERE Zone = @zone AND Id <> @id AND ValidUntil >= CAST(GETDATE() AS date)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@zone", zone);
                command.Parameters.AddWithValue("@id", excludeId);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        public void Add(Permit permit)
        {
            string sql =
                "INSERT INTO Permits (PlateNumber, OwnerName, PermitType, Zone, ValidUntil, IsPaid) " +
                "VALUES (@plateNumber, @ownerName, @permitType, @zone, @validUntil, @isPaid)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                AddPermitParameters(command, permit);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Update(Permit permit)
        {
            string sql =
                "UPDATE Permits SET PlateNumber = @plateNumber, OwnerName = @ownerName, " +
                "PermitType = @permitType, Zone = @zone, ValidUntil = @validUntil, IsPaid = @isPaid " +
                "WHERE Id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                AddPermitParameters(command, permit);
                command.Parameters.AddWithValue("@id", permit.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("DELETE FROM Permits WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Add and Update send the same six values
        private void AddPermitParameters(SqlCommand command, Permit permit)
        {
            command.Parameters.AddWithValue("@plateNumber", permit.PlateNumber);
            command.Parameters.AddWithValue("@ownerName", permit.OwnerName);
            command.Parameters.AddWithValue("@permitType", permit.PermitType);
            command.Parameters.AddWithValue("@zone", permit.Zone);
            command.Parameters.AddWithValue("@validUntil", permit.ValidUntil.Date);
            command.Parameters.AddWithValue("@isPaid", permit.IsPaid);
        }
    }
}
