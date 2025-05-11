using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public class Ship : IGameObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int DirectionX { get; set; } = 0;
        public int DirectionY { get; set; } = 0;
        public char Symbol { get; set; } = 'S'; // Default symbol for the ship
        public object _lock { get; set; } = new();
        private int oldX = 0;
        private Game _game;

        public Ship(int id, string name, int speed, int positionX, int positionY, int directionX, char symbol)
        {
            Id = id;
            Name = name;
            Speed = speed;
            PositionX = positionX;
            PositionY = positionY;
            DirectionX = directionX;
            Symbol = symbol;
            _game = Game.Instance;
        }

        public void Move()
        {
            if (InputSystem.Instance.TryGetInput(out ConsoleKey key))
            {
                if (key == ConsoleKey.A)
                {
                    DirectionX = -1;
                }
                else if (key == ConsoleKey.D)
                {
                    DirectionX = 1;
                }
                else
                {
                    DirectionX = 0;
                }
                oldX = PositionX;
                PositionX += DirectionX * Speed;
                if (PositionX <= 0)
                {
                    PositionX = 1;
                }
                else if (PositionX >= Console.WindowWidth - Speed)
                {
                    PositionX = Console.WindowWidth - Speed - 1;
                }
            }
        }

        public void Draw()
        {
            lock (_game.GetGameLock())
            {
                GameBuffer.Instance.SetPixel(PositionX, PositionY, Symbol, ConsoleColor.Green);
            }
        }

        public void ClearDraw()
        {
            lock (_game.GetGameLock())
            {
                GameBuffer.Instance.SetPixel(PositionX, PositionY, ' ', ConsoleColor.White);
            }
        }

        public void Spawn()
        {
            // Spawn logic for the ship
            PositionX = Console.WindowWidth / 2;
            PositionY = Console.WindowHeight - 1; // Start at the bottom of the screen
            DirectionX = 0; // No initial movement

        }

        public bool CheckCollision()
        {
            lock (_game.GetGameLock())
            {
                var asteroidsCopy = _game.GetAsteroids().ToArray();
                foreach (var asteroid in asteroidsCopy)
                {
                    if (CollisionSystem.Instance.CheckCollision(this, asteroid))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
