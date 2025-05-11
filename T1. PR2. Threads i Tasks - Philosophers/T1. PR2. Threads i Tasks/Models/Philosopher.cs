using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks.Models
{
    /// <summary>
    /// Esta clase representa a un filósofo en nuestra simulación.
    /// Cada filósofo es un hilo que piensa y come.
    /// </summary>
    public class Philosopher
    {
        // Random compartido para todos los filósofos
        private static readonly Random random = new Random();
        // Random específico para cada hilo para evitar colisiones
        private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(() => new Random(random.Next()));

        // Cosas básicas que necesita cada filósofo
        public int Id { get; set; }
        public string Name { get; set; }
        public Thread Thread { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.Green;
        public List<Stick> Sticks { get; set; } = new List<Stick>();

        // Cuánto tiempo hace cada cosa
        public float minThinkingTime { get; set; } = 0.5f;  // Tiempo mínimo pensando
        public float maxThinkingTime { get; set; } = 1.5f;  // Tiempo máximo pensando
        public float minEatingTime { get; set; } = 0.5f;    // Tiempo mínimo comiendo
        public float maxEatingTime { get; set; } = 1.5f;    // Tiempo máximo comiendo

        // Constantes que usaremos
        private static readonly int TimeUntilStarving = 15000;  // Si no come en 15 segundos, se muere
        private static readonly int TimeUntilNextAttempt = 1000;  // Espera 1 segundo entre intentos

        // Estado del filósofo
        private bool isRunning = true;  // Si sigue vivo
        private int mealsEaten = 0;     // Cuántas veces ha comido
        private int failedAttempts = 0; // Cuántas veces no pudo comer
        private string currentState = "Starting...";  // Qué está haciendo ahora
        private DateTime lastStateChange = DateTime.Now;  // Cuándo cambió de estado

        // Esto es para que no se mezclen los mensajes en pantalla
        private readonly object consoleLock;

        /// <summary>
        /// Constructor: aquí creamos un nuevo filósofo
        /// </summary>
        public Philosopher(
            int id, 
            string name, 
            Stick leftStick, 
            Stick rightStick, 
            ConsoleColor color, 
            float minThink, 
            float maxThink, 
            float minEat, 
            float maxEat,
            object consoleLock
        )
        {
            Id = id;
            Name = name;
            Sticks.Add(leftStick);  // El palillo de la izquierda
            Sticks.Add(rightStick); // El palillo de la derecha
            Color = color;
            this.consoleLock = consoleLock;
            Thread = new Thread(StartSimulation);
            Thread.Name = name;
            minThinkingTime = minThink;
            maxThinkingTime = maxThink;
            minEatingTime = minEat;
            maxEatingTime = maxEat;
        }

        /// <summary>
        /// Detiene al filósofo (cuando presionamos Q)
        /// </summary>
        public void Stop()
        {
            isRunning = false;
        }

        /// <summary>
        /// Actualiza lo que está haciendo el filósofo
        /// </summary>
        private void UpdateStatus(string state)
        {
            currentState = state;
            lastStateChange = DateTime.Now;
            UpdateStatusLine();
        }

        /// <summary>
        /// Actualiza la línea en pantalla donde se muestra el filósofo
        /// </summary>
        private void UpdateStatusLine()
        {
            lock (consoleLock)
            {
                int currentLine = Console.CursorTop;
                int currentColumn = Console.CursorLeft;

                // Posición base: 15 (después del header, configuración y encabezado de la tabla)
                Console.SetCursorPosition(0, 15 + Id);
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.SetCursorPosition(0, 15 + Id);

                Console.ForegroundColor = Color;
                Console.Write($"║ {Name,-12} │ {currentState,-30} │ total meals {mealsEaten,5} │ total failed attempts {failedAttempts,8} ║");

                Console.ResetColor();
                Console.SetCursorPosition(currentColumn, currentLine);
            }
        }

        /// <summary>
        /// Este es el método principal que hace que el filósofo funcione
        /// </summary>
        public void StartSimulation()
        {
            UpdateStatus("Starting");
            while (isRunning)
            {
                try
                {
                    if (!isRunning) break;
                    // El ciclo de vida del filósofo:
                    // 1. Piensa
                    // 2. Intenta coger los palillos
                    // 3. Come
                    // 4. Suelta los palillos
                    Think(minThinkingTime, maxThinkingTime);

                    // Intenta agarrar los palillos, si no lo logra en menos de 15 segundos se muere de hambre
                    if (!TryTakeSticksWithTimeout())
                    {
                        UpdateStatus("Starved");  // Se murió de hambre
                        isRunning = false;
                        break;
                    }
                    // Al lograr tomar los dos palillos, comerá
                    Eat(minEatingTime, maxEatingTime);
                    // Una vez finaliza de comer, devuelve los palillos (los deja libres)
                    PutSticks();
                    mealsEaten++;
                }
                catch (Exception ex)
                {
                    UpdateStatus("Error");
                    break;
                }
            }
            UpdateStatus("Finished");
        }

        /// <summary>
        /// El filósofo piensa un rato
        /// </summary>
        private void Think(float min, float max)
        {
            float thinkingTime = threadRandom.Value.NextSingle() * (max - min) + min;
            UpdateStatus($"Thinking ({thinkingTime:F1}s)");
            Thread.Sleep((int)(thinkingTime * 1000));
        }

        /// <summary>
        /// Intenta coger los palillos, pero si no puede en 15 segundos, se muere
        /// </summary>
        private bool TryTakeSticksWithTimeout()
        {
            DateTime startTime = DateTime.Now;
            bool hasBothSticks = false;

            UpdateStatus("Trying to take sticks");

            while (!hasBothSticks && isRunning && 
                   (DateTime.Now - startTime).TotalMilliseconds < TimeUntilStarving)
            {
                if (TryTakeBothSticks())
                {
                    hasBothSticks = true;
                    break;
                }

                PutSticks();
                failedAttempts++;
                double waitTime = (DateTime.Now - startTime).TotalMilliseconds / 1000.0;
                UpdateStatus($"Waiting for sticks ({waitTime:F1}s)");

                Thread.Sleep(TimeUntilNextAttempt);
            }

            if (!hasBothSticks)
            {
                UpdateStatus("Failed to get sticks");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Intenta coger los dos palillos
        /// Importante: cogemos primero el palillo con ID menor para evitar deadlocks
        /// </summary>
        private bool TryTakeBothSticks()
        {
            // Tomamos primero el palillo con ID menor para evitar deadlocks
            Stick firstStick = Sticks[0].Id < Sticks[1].Id ? Sticks[0] : Sticks[1];
            Stick secondStick = Sticks[0].Id < Sticks[1].Id ? Sticks[1] : Sticks[0];

            // Intentamos tomar el primer palillo
            if (firstStick.Take(Id))
            {
                UpdateStatus($"Took stick {firstStick.Id}");

                // Intentamos tomar el segundo palillo
                if (secondStick.Take(Id))
                {
                    UpdateStatus($"Took stick {secondStick.Id}");
                    return true;
                }
                else
                {
                    // Si no podemos tomar el segundo, soltamos el primero
                    firstStick.Put();
                    UpdateStatus($"Released stick {firstStick.Id}");
                }
            }
            return false;
        }

        /// <summary>
        /// El filósofo come un rato
        /// </summary>
        private void Eat(float min, float max)
        {
            float eatingTime = threadRandom.Value.NextSingle() * (max - min) + min;
            UpdateStatus($"Eating ({eatingTime:F1}s)");
            Thread.Sleep((int)(eatingTime * 1000));
        }

        /// <summary>
        /// Suelta los palillos que está usando
        /// </summary>
        private void PutSticks()
        {
            foreach (var stick in Sticks)
            {
                if (stick.IsUsedBy(Id))
                {
                    stick.Put();
                    UpdateStatus($"Released stick {stick.Id}");
                }
            }
        }
    }
}
