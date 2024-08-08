using MySQLConnectionPool;
using MySqlConnector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionPoolPlay
{
    public class Program
    {
        readonly static string ConnectionString = Environment.GetEnvironmentVariable("mysqlConnection");

        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                Action();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Uber level exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            sw.Stop();
            Console.WriteLine("Time taken: " + sw.ElapsedMilliseconds + "milliSeconds");
            Console.WriteLine("Time taken: " + sw.ElapsedMilliseconds / 1000 + "seconds");
        }

        static void Action()
        {
            ExecuteWithPooledConnection(20040);
            //ExecuteWithSingleConnectionEveryTime(100); //crashes in ~150 
        }


        static void ExecuteWithPooledConnection(int n)
        {
            BlockingCollection<MySqlConnection> blockingQueue = new BlockingCollection<MySqlConnection>(100);
            for (int i = 0; i < 100; i++)
            {
                blockingQueue.Add(GetMySqlConnection());
            }
            Parallel.For(0, n, i =>
            {
                MySqlConnection conn = blockingQueue.Take();
                RunQuery(conn);
                blockingQueue.Add(conn);
            });
        }

        static void ExecuteWithSingleConnectionEveryTime(int n)
        {
            List<MySqlConnection> mySqlConnections = new List<MySqlConnection>();
            Parallel.For(0, n, i =>
            {
                MySqlConnection conn = GetMySqlConnection();
                RunQuery(conn);
                mySqlConnections.Add(conn);
            });
        }

        private static void RunQuery(MySqlConnection mySqlConnection)
        {
            try
            {
                var _ = RunQueryHelper(mySqlConnection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static string RunQueryHelper(MySqlConnection mySqlConnection)
        {
            // Create a command object
            MySqlCommand command = mySqlConnection.CreateCommand();

            // Set the command text to select 1 from dual
            command.CommandText = "SELECT 100 FROM dual";

            // Execute the command and get the result
            long result = (long)command.ExecuteScalar();

            // Do something with the result
            return result.ToString();

            // Do some database operations here
        }

        private static MySqlConnection GetMySqlConnection()
        {
            string myConnectionString = ConnectionString + ";Pooling=False;";
            MySqlConnection conn = new MySqlConnection(myConnectionString);

            conn.Open();
            return conn;
        }
    }
}