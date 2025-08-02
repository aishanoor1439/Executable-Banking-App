using System;
using System.Data.SqlClient;

namespace ExecutableBankingApp
{
    internal class Program
    {
        static string connectionString = "Server=ELITEX840\\MSSQLSERVER01;Database=BankAppDB;Integrated Security=True;";
        static int loggedInUserId = -1;

        static void Register()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@username", username);

                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                {
                    Console.WriteLine("Username already exists.");
                    return;
                }

                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Registration successful!");
        }

        static bool Login()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id FROM Users WHERE Username = @username AND Password = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    loggedInUserId = Convert.ToInt32(result);
                    Console.WriteLine("Login successful!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid credentials.");
                    return false;
                }
            }
        }

        static void LoggedInMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Account Menu ===");
                Console.WriteLine("1. Check Balance");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Cashout");
                Console.WriteLine("4. Logout");
                Console.Write("Enter choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": CheckBalance(); break;
                    case "2": Deposit(); break;
                    case "3": Cashout(); break;
                    case "4": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }

                Pause();
            }
        }

        static double CheckBalance()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Balance FROM Users WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", loggedInUserId);

                double balance = Convert.ToDouble(cmd.ExecuteScalar());
                Console.WriteLine("Your Balance: Rs. " + balance);
                return balance;
            }
        }

        static void Deposit()
        {
            Console.Write("Enter amount to deposit: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            if (amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Users SET Balance = Balance + @amount WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@id", loggedInUserId);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Deposit successful.");
        }

        static void Cashout()
        {
            Console.Write("Enter amount to withdraw: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                double balance = CheckBalance();

                if (amount > 0 && amount <= balance)
                {
                    string query = "UPDATE Users SET Balance = Balance - @amount WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@id", loggedInUserId);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Withdrawal successful.");
                }
                else
                {
                    Console.WriteLine("Insufficient funds or invalid amount.");
                }
            }
        }

        static void Pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Simple Bank App ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Enter choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": Register(); break;
                    case "2":
                        if (Login())
                            LoggedInMenu();
                        break;
                    case "3": return;
                    default: Console.WriteLine("Invalid Choice"); break;
                }

                Pause();
            }
        }
    }
}
