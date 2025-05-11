using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public class Asteroid : IGameObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
        public char Symbol { get; set; } = '@'; // Default symbol for the asteroid
        public bool isPlaying { get; set; } = true; // Flag to control the game loop
        public object _lock { get; set; } = new();
        private List<(int x, int y)> trail = new List<(int x, int y)>();
        private bool isRespawned = false;
        private Game _game;

        public Asteroid(int id, string name, int speed, int positionX, int positionY, int directionX, int directionY, char symbol)
        {
            Id = id;
            Name = name;
            Speed = speed;
            PositionX = positionX;
            PositionY = positionY;
            DirectionX = directionX;
            DirectionY = directionY;
            Symbol = symbol;
            trail.Add((positionX, positionY));
            _game = Game.Instance;
        }

        public void Draw()
        {
            lock (_game.GetGameLock())
            {
                // Si el asteroide necesita respawnear, lo hacemos primero
                if (isRespawned)
                {
                    Spawn();
                    isRespawned = false;
                }

                if (PositionX >= 0 && PositionX < Console.WindowWidth &&
                    PositionY >= 0 && PositionY < Console.WindowHeight)
                {
                    // Limpiamos el rastro anterior
                    ClearTrail();

                    // Dibujamos en la nueva posición
                    GameBuffer.Instance.SetPixel(PositionX, PositionY, Symbol, ConsoleColor.Red);

                    // Actualizamos el rastro
                    UpdateTrail();
                }
            }
        }

        private void UpdateTrail()
        {
            // Mantenemos solo las últimas 3 posiciones para el rastro
            trail.Add((PositionX, PositionY));
            if (trail.Count > 3)
            {
                trail.RemoveAt(0);
            }
        }

        private void ClearTrail()
        {
            foreach (var pos in trail)
            {
                if (pos.x >= 0 && pos.x < Console.WindowWidth &&
                    pos.y >= 0 && pos.y < Console.WindowHeight)
                {
                    GameBuffer.Instance.SetPixel(pos.x, pos.y, ' ', ConsoleColor.White);
                }
            }
        }

        public void Spawn()
        {
            // Limpiamos el rastro antes de respawnear
            ClearTrail();
            trail.Clear();

            // Spawn logic for the asteroid
            PositionX = new Random().Next(1, Console.WindowWidth - 1);
            PositionY = new Random().Next(1, Console.WindowHeight) * -1; // Start at the top of the screen
            DirectionY = 1; // Move downwards
            trail.Add((PositionX, PositionY));
        }

        public void Move()
        {
            PositionY += DirectionY;
            
            // Check for boundary collisions
            if (PositionY >= Console.WindowHeight)
            {
                // En lugar de respawnear inmediatamente, marcamos que necesita respawnear
                isRespawned = true;
                // Limpiamos la posición actual
                ClearTrail();
                trail.Clear();
            }
        }

        public void ClearDraw()
        {
            lock (_game.GetGameLock())
            {
                ClearTrail();
                trail.Clear();
                isRespawned = false;
            }
        }
    }
}
