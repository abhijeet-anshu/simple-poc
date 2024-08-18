
namespace AirlineCheckin;
public class Program
{
    public static void Main(string[] args)
    {
        new SeatAllocator().Execute();

        new SeatAllocatorByLock().Execute();

        new SeatAllocatorByLockSkip().Execute();
    }
}