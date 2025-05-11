using System.Xml.Linq;
using T1._PR2._Threads_i_Tasks.Models;

namespace T1._PR2._Threads_i_Tasks
{
    internal class Program
    {
        // Variables que necesitamos para la simulación

        public static List<Stick> Sticks = [];  // Los palillos que usan los filósofos
        public static float minThinkingTime = 0.5f;  // Tiempo mínimo que piensan
        public static float maxThinkingTime = 2.0f;  // Tiempo máximo que piensan
        public static float minEatingTime = 0.5f;    // Tiempo mínimo que comen
        public static float maxEatingTime = 1.0f;    // Tiempo máximo que comen
        public static int TimeUntilStarving = 15000; // Si no come en 15 segundos, se muere de hambre
        public static int TimeUntilNextAttempt = 1000; // Espera 1 segundo entre intentos

        // Variables para controlar la simulación
        private static List<Philosopher> philosophers = new List<Philosopher>();  // Lista de filósofos
        private static bool isRunning = true;  // Para saber si la simulación sigue corriendo
        private static DateTime startTime;  // Para saber cuándo empezó
        private static readonly object consoleLock = new object();  // Esto es para que no se mezclen los mensajes en pantalla

        static void Main(string[] args)
        {
            // Primero configuramos la ventana de la consola
            Console.Title = "Dining Philosophers Simulation";
            Console.Clear();
            Console.WindowHeight = 40;
            Console.BufferHeight = 40;

            // Ahora empezamos la simulación
            DrawHeader();  // Dibujamos el título y la configuración
            SetupControls();  // Configuramos los controles (Q para salir)
            InitializeSimulation();  // Creamos los filósofos y los palillos
            RunSimulation();  // Empezamos la simulación
        }

        // Este método dibuja la interfaz en la consola
        private static void DrawHeader()
        {
            lock (consoleLock)  // Esto es para que no se mezclen los mensajes
            {
                // Primero el título
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      DINING PHILOSOPHERS SIMULATION                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();

                // Luego la configuración
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║ CONFIGURATION                                                              ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║ • Thinking time:   {minThinkingTime,4:F1} - {maxThinkingTime,4:F1} seconds");
                Console.WriteLine($"║ • Eating time:     {minEatingTime,4:F1} - {maxEatingTime,4:F1} seconds");
                Console.WriteLine($"║ • Starving time:   {TimeUntilStarving/1000,4} seconds");
                Console.WriteLine($"║ • Next attempt:    {TimeUntilNextAttempt/1000,4} seconds");
                Console.WriteLine("║ • Ctrl+C to stop the simulation and show the summary");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");

                // Añadimos una línea en blanco al final para separar de futuros mensajes
                Console.WriteLine();
            }
        }

        // Configuramos los controles (Ctrl+C)
        private static void SetupControls()
        {
            // Esto es para que cuando presionemos Ctrl+C no se cierre el programa
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                StopSimulation();
            };
        }

        // Creamos los filósofos y los palillos
        private static void InitializeSimulation()
        {
            startTime = DateTime.Now;

            // Primero creamos los 5 palillos
            for (int i = 0; i < 5; i++)
            {
                Sticks.Add(new Stick(i));
            }

            // Luego creamos los 5 filósofos
            // Cada filósofo tiene dos palillos: el de su izquierda y el de su derecha
            philosophers.Add(new Philosopher(0, "Platon", Sticks[0], Sticks[1], ConsoleColor.Blue, minThinkingTime, maxThinkingTime, minEatingTime, maxEatingTime, consoleLock));
            philosophers.Add(new Philosopher(1, "Aristoteles", Sticks[1], Sticks[2], ConsoleColor.Red, minThinkingTime, maxThinkingTime, minEatingTime, maxEatingTime, consoleLock));
            philosophers.Add(new Philosopher(2, "Sokrates", Sticks[2], Sticks[3], ConsoleColor.Green, minThinkingTime, maxThinkingTime, minEatingTime, maxEatingTime, consoleLock));
            philosophers.Add(new Philosopher(3, "Kant", Sticks[3], Sticks[4], ConsoleColor.Yellow, minThinkingTime, maxThinkingTime, minEatingTime, maxEatingTime, consoleLock));
            philosophers.Add(new Philosopher(4, "Hegel", Sticks[4], Sticks[0], ConsoleColor.Cyan, minThinkingTime, maxThinkingTime, minEatingTime, maxEatingTime, consoleLock));
        }

        // Empezamos la simulación
        private static void RunSimulation()
        {
            // Iniciamos cada filósofo en su propio hilo
            foreach (var philosopher in philosophers)
            {
                philosopher.Thread.Start();
            }

            // Esperamos a que todos los filósofos terminen
            foreach (var philosopher in philosophers)
            {
                philosopher.Thread.Join();
            }

        }

        // Detenemos la simulación
        private static void StopSimulation()
        {
            isRunning = false;
            foreach (var philosopher in philosophers)
            {
                philosopher.Stop();
            }
            ShowSimulationSummary();
        }

        // Mostramos un resumen al final
        private static void ShowSimulationSummary()
        {
            var duration = DateTime.Now - startTime;
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(0, 20);
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.SetCursorPosition(0, 20);
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║ SIMULATION SUMMARY                                                         ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║ Total duration: {duration.TotalSeconds:F1} seconds");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
        }
    }
}
