using System;
using System.Threading.Tasks;
using T1._PR2._Threads_i_Tasks___Asteroid.Models;

namespace T1._PR2._Threads_i_Tasks___Asteroid
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.CursorVisible = false;
                Game.Instance.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in game: {ex.Message}");
            }
            finally
            {
                Console.CursorVisible = true;
                Console.Clear();
            }
        }
    }
}
