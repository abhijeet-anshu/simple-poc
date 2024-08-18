using MySqlConnector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AirlineCheckin
{
    internal class SeatAllocator : SeatAllocatorBase
    {
        internal override void AllocateSeat(int userId)
        {
            string getEmptySeatRowQuery = @"
                    SELECT seat_id, seat_name, user_id
                    FROM seats
                    WHERE user_id IS NULL
                    ORDER BY seat_id asc
                    LIMIT 1;
                ";

            string updateQuery = @"
                    UPDATE seats
                    SET user_id = @userId
                    WHERE seat_id = @seatId;
                ";

            int seatId = -1;
            string seat_name = "";

            connectionPool.UseConnection(conn =>
            {
                MySqlTransaction transaction = conn.BeginTransaction();



                using (var command = new MySqlCommand(getEmptySeatRowQuery, conn))
                {
                    command.Transaction = transaction;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            seatId = reader.GetInt32("seat_id");
                            seat_name = reader.GetString("seat_name").ToString();
                        }
                        else
                        {
                            Console.WriteLine("No more seats available");
                            transaction.Rollback();
                            return;
                        }
                    }
                }

                if (seatId != -1)
                {
                    using (var command = new MySqlCommand(updateQuery, conn))
                    {
                        command.Transaction = transaction;
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@seatId", seatId);
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            });

            seatAllottmentLog.Add($"Seat {seat_name} alloted to user {userId}");
        }
    }
}
