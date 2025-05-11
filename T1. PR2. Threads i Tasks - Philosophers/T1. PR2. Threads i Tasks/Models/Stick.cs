using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks.Models
{
    /// <summary>
    /// Esta clase representa un palillo en nuestra simulación.
    /// Los palillos son los recursos que comparten los filósofos.
    /// </summary>
    public class Stick
    {
        // Cosas básicas que necesita cada palillo
        public int Id { get; set; }  // Su número
        public bool isFree = true;  // Si está libre o no
        private int currentPhilosopherId = -1;  // Qué filósofo lo está usando (-1 = nadie)
        private DateTime lastTaken;  // Cuándo fue tomado por última vez

        // Objeto para sincronización
        private readonly object stickLock = new object();

        /// <summary>
        /// Constructor: aquí creamos un nuevo palillo
        /// </summary>
        public Stick(int id)
        {
            Id = id;
            lastTaken = DateTime.MinValue;
        }

        /// <summary>
        /// Un filósofo intenta coger el palillo
        /// </summary>
        public bool Take(int philosopherId)
        {
            lock (stickLock)
            {
                if (!isFree)
                {
                    return false;
                }

                isFree = false;
                currentPhilosopherId = philosopherId;
                lastTaken = DateTime.Now;
                return true;
            }
        }

        /// <summary>
        /// Un filósofo suelta el palillo
        /// </summary>
        public void Put()
        {
            lock (stickLock)
            {
                isFree = true;
                currentPhilosopherId = -1;
            }
        }

        /// <summary>
        /// Obtiene el tiempo que lleva el palillo en uso
        /// </summary>
        public TimeSpan GetUsageTime()
        {
            lock (stickLock)
            {
                if (isFree)
                {
                    return TimeSpan.Zero;
                }
                return DateTime.Now - lastTaken;
            }
        }

        /// <summary>
        /// Verifica si el palillo está siendo usado por un filósofo específico
        /// </summary>
        public bool IsUsedBy(int philosopherId)
        {
            lock (stickLock)
            {
                return !isFree && currentPhilosopherId == philosopherId;
            }
        }
    }
}
