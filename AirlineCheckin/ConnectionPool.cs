using MySqlConnector;
using System;
using System.Collections.Concurrent;

namespace AirlineCheckin
{
    internal class ConnectionPool : IDisposable
    {
        const int connectionPoolSize = 120;

        readonly static string ConnectionString = Environment.GetEnvironmentVariable("mysqlConnection");

        readonly BlockingCollection<MySqlConnection> blockingQueue = new(connectionPoolSize);

        public ConnectionPool()
        {
            for (int i = 0; i < connectionPoolSize; i++)
            {
                blockingQueue.Add(GetMySqlConnection());
            }
        }

        public void ExecuteNonQuery(string sql)
        {
            UseConnection(conn =>
            {
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
            });
        }

        public void UseConnection(Action<MySqlConnection> action)
        {
            MySqlConnection conn = null;
            try
            {
                conn = GetConnection();
                action(conn);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during querying
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    ReturnConnection(conn);
                }
            }
        }

        public T UseConnection<T>(Func<MySqlConnection, T> func)
        {
            MySqlConnection conn = null;
            try
            {
                conn = GetConnection();
                return func(conn);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during querying
                Console.WriteLine("An error occurred: " + ex.Message);
                throw; // Re-throw the exception to allow the caller to handle it
            }
            finally
            {
                if (conn != null)
                {
                    ReturnConnection(conn);
                }
            }
        }

        private MySqlConnection GetConnection()
        {
            return blockingQueue.Take();
        }

        private void ReturnConnection(MySqlConnection connection)
        {
            if (!blockingQueue.IsAddingCompleted)
            {
                blockingQueue.Add(connection);
            }
            else
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void Dispose()
        {
            // Mark the collection as not accepting any more items
            blockingQueue.CompleteAdding();

            // Close all connections in the blocking queue
            foreach (var conn in blockingQueue)
            {
                conn.Close();
                conn.Dispose();
            }

            // Clear the collection
            blockingQueue.Dispose();
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
