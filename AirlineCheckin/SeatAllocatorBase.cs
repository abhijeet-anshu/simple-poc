using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineCheckin
{
    internal abstract class SeatAllocatorBase : IDisposable
    {
        protected ConnectionPool connectionPool;

        protected List<string> seatAllottmentLog = new List<string>();

        protected StreamWriter logWriter;

        protected StreamWriter commonWriter;

        public void Execute()
        {
            InitializeSeats();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            AllocateAllSeats();
            stopwatch.Stop();
            PrintSeatMapT(GetSeatMap());
            WriteOutputCommonLine($"Time taken in Allocating: {stopwatch.ElapsedMilliseconds} ms\n\n\n");
            Dispose();
        }

        internal SeatAllocatorBase()
        {
            connectionPool = new ConnectionPool();
            string myClassName = this.GetType().Name;
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Navigate up to the project root directory
            string projectRoot = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            // Define the path for the new output folder
            string outputFolderPath = Path.Combine(projectRoot, "OutputFolder");
            // Ensure the output directory exists
            if (!Directory.Exists(outputFolderPath))
            {

                Directory.CreateDirectory(outputFolderPath);
            }

            string logFilePath = Path.Combine(outputFolderPath, $"seat_allocation_{myClassName}_log.txt");

            string commonFilePath = Path.Combine(outputFolderPath, $"seat_allocation_out.txt");

            logWriter = new StreamWriter(logFilePath, append: false);
            commonWriter = new StreamWriter(commonFilePath, append: true);

            commonWriter.WriteLine($"---------------------{DateTime.UtcNow}-----------------------------");
        }

        internal virtual void AllocateSeat(int userId)
        {

        }

        public void AllocateAllSeats()
        {

            Parallel.For(0, 180, i =>
            {
                AllocateSeat(i + 1);
            });
        }

        char[] columns = { 'A', 'B', 'C', 'D', 'E', 'F' };
        public Dictionary<string, string> GetSeatMap()
        {
            Dictionary<string, string> seatMap = new Dictionary<string, string>();

            char[] columns = { 'A', 'B', 'C', 'D', 'E', 'F' };

            for (int row = 1; row <= 30; row++)
            {
                foreach (char column in columns)
                {
                    string seatId = $"{row}{column}";

                    string query = $"select user_id from seats where seat_name=\'{seatId}\';";

                    object ob = connectionPool.UseConnection(conn =>
                    {
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = query;
                        return command.ExecuteScalar();
                    });

                    if (ob != null && ob != DBNull.Value)
                    {
                        seatMap[seatId] = "X";
                    }
                    else
                    {
                        seatMap[seatId] = ".";
                    }
                }
            }

            return seatMap;
        }

        void EmptySeats()
        {
            string query = $"TRUNCATE TABLE SEATS;";
            connectionPool.ExecuteNonQuery(query);
        }

        public void InsertSeat(string seatId)
        {
            string query = $"INSERT INTO seats (seat_name) VALUES ('{seatId}');";

            connectionPool.ExecuteNonQuery(query);
        }

        public void PrintLog()
        {
            WriteOutputLine(string.Join("\n", seatAllottmentLog));

            WriteOutputLine("--------------------\n\n\n");
        }

        public void PrintSeatMapT(Dictionary<string, string> seatMap)
        {
            WriteOutputCommonLine("Seat Map for " + GetType().Name);

            int unoccupiedSeat = 0, occupiedSeat = 0;
            for (int i = 0; i < columns.Length; i++)
            {
                for (int row = 1; row <= 30; row++)
                {
                    string seatId = $"{row}{columns[i]}";
                    WriteOutputCommon(seatMap[seatId] + " ");
                    if (seatMap[seatId] == ".")
                    {
                        unoccupiedSeat++;
                    }
                    else
                    {
                        occupiedSeat++;
                    }
                }
                WriteOutputCommonLine();
                if ((i + 1) % 3 == 0)
                {
                    WriteOutputCommonLine();
                    WriteOutputCommonLine();
                }
            }
            WriteOutputCommonLine("--------------------");
            WriteOutputCommonLine($"Unoccupied Seats: {unoccupiedSeat}");
            WriteOutputCommonLine($"Occupied Seats: {occupiedSeat}");
            WriteOutputCommonLine("--------------------");
        }

        public void PrintSeatMap(Dictionary<string, string> seatMap)
        {
            for (int row = 1; row <= 30; row++)
            {
                //WriteOutput($"{row} ");
                for (int i = 0; i < columns.Length; i++)
                {
                    string seatId = $"{row}{columns[i]}";

                    WriteOutput(seatMap[seatId]);

                    if (i == 2) // Add extra space after the third column
                    {
                        WriteOutput("    ");
                    }
                    else
                    {
                        WriteOutput(" ");
                    }
                }
                WriteOutputLine();
            }
        }
        public void InitializeSeats()
        {

            //create seats

            List<string> seatMatrix = new List<string>();
            Dictionary<string, string> seatMap = new Dictionary<string, string>();


            EmptySeats();

            for (int row = 1; row <= 30; row++)
            {
                foreach (char column in columns)
                {
                    string seatId = $"{row}{column}";

                    seatMatrix.Add($"{row}{column}");
                    seatMap[$"{row}{column}"] = ".";

                    InsertSeat(seatId);
                }
            }



            string seatMatrixString = string.Join(", ", seatMatrix);

        }

        void WriteOutputCommon(string message)
        {
            Console.Write(message);
            commonWriter.Write(message);
        }

        void WriteOutputCommonLine(string message = "")
        {
            Console.WriteLine(message);
            commonWriter.WriteLine(message);
        }

        void WriteOutput(string message)
        {
            //Console.Write(message);
            logWriter.Write(message);
        }

        void WriteOutputLine(string message = "")
        {
            //Console.WriteLine(message);
            logWriter.WriteLine(message);
        }

        public void Dispose()
        {
            connectionPool.Dispose();
            logWriter.Close();
            commonWriter.Close();
        }
    }
}
