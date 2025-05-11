using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public class Game
    {
        public static Game instance;
        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Game();
                }
                return instance;
            }
        }

        public float DrawTime { get; set; } = 50f;
        public float MoveTime { get; set; } = 20f;
        public bool isPlaying { get; private set; }
        public GameBuffer GameBuffer => GameBuffer.Instance;
        private readonly object _gameLock = new object();
        public Task drawTask { get; private set; }
        public Task moveTask { get; private set; }
        public Ship ship { get; private set; }
        private List<Asteroid> asteroids = new List<Asteroid>();
        private DateTime gameStartTime;
        private const int INITIAL_ASTEROIDS = 5;
        private const int MAX_ASTEROIDS = 100;
        private const int ASTEROID_INCREASE_INTERVAL = 5000; // 5 segundos
        private const int ASTEROIDS_PER_INCREASE = 5;

        public enum GameState
        {
            StartScreen,
            Running,
            GameOver
        }

        private GameState _state = GameState.StartScreen;

        public GameState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
            }
        }

        public Game()
        {
            State = GameState.StartScreen;
        }

        public void Start()
        {
            try
            {
                Console.CursorVisible = false;
                State = GameState.StartScreen;

                // Start input handling
                var inputTask = Task.Run(() => InputSystem.Instance.StartInputTask());

                // Main game loop
                while (true)
                {
                    if (State == GameState.StartScreen)
                    {
                        ShowStartScreen();
                        State = GameState.Running;
                        InitializeGame();
                        StartGameTasks();
                    }
                    else if (State == GameState.GameOver)
                    {
                        ShowGameOverScreen();
                        State = GameState.Running;
                        ResetGame();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in game: {ex.Message}");
            }
            finally
            {
                Cleanup();
            }
        }

        private void InitializeGame()
        {
            gameStartTime = DateTime.Now;
            asteroids.Clear();

            // Crear los primeros 5 asteroides
            for (int i = 0; i < INITIAL_ASTEROIDS; i++)
            {
                CreateNewAsteroid();
            }

            // Initialize ship
            ship = new Ship(1, "Ship", 2, Console.WindowWidth / 2, Console.WindowHeight - 1, 0, '^');
            ship.Spawn();

            isPlaying = true;
        }

        private void CreateNewAsteroid()
        {
            if (asteroids.Count >= MAX_ASTEROIDS) return;

            var random = new Random();
            var asteroid = new Asteroid(
                asteroids.Count + 1,
                "Asteroid" + (asteroids.Count + 1),
                1,
                random.Next(0, Console.WindowWidth),
                random.Next(0, Console.WindowHeight / 2),
                random.Next(-1, 2),
                1,
                '@'
            );
            asteroid.Spawn();
            asteroids.Add(asteroid);
        }

        private void StartGameTasks()
        {
            isPlaying = true;
            drawTask = Task.Run(async() =>
            {
                try
                {
                    while (isPlaying)
                    {
                        GameBuffer.Clear();
                        
                        lock (_gameLock)
                        {
                            ship?.Draw();

                            foreach (var asteroid in asteroids.ToArray())
                            {
                                asteroid.Draw();
                            }
                        }
                        
                        GameBuffer.Draw();
                        await Task.Delay((int)(DrawTime));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in draw task: {ex.Message}");
                    State = GameState.GameOver;
                }
            });

            moveTask = Task.Run(async() =>
            {
                try
                {
                    while (isPlaying)
                    {
                        lock (_gameLock)
                        {
                            ship?.Move();

                            foreach (var asteroid in asteroids.ToArray())
                            {
                                asteroid.Move();
                            }
                            Update();
                        }

                        await Task.Delay((int)(MoveTime));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in move task: {ex.Message}");
                    State = GameState.GameOver;
                }
            });
        }

        public void Update()
        {
            if (InputSystem.Instance.TryGetInput(out ConsoleKey key))
            {
                if (key == ConsoleKey.Escape)
                {
                    State = GameState.GameOver;
                }
            }
            if (State != GameState.Running) return;

            // Verificar si es tiempo de aumentar los asteroides
            var elapsedTime = (DateTime.Now - gameStartTime).TotalMilliseconds;
            var expectedAsteroids = INITIAL_ASTEROIDS + 
                (int)(elapsedTime / ASTEROID_INCREASE_INTERVAL) * ASTEROIDS_PER_INCREASE;

            // Añadir nuevos asteroides si es necesario
            while (asteroids.Count < expectedAsteroids && asteroids.Count < MAX_ASTEROIDS)
            {
                CreateNewAsteroid();
            }

            // Verificar colisiones antes de actualizar
            if (ship != null && ship.CheckCollision())
            {
                State = GameState.GameOver;
            }
        }

        private void Cleanup()
        {
            lock (_gameLock)
            {
                isPlaying = false;
            }

            // Wait for tasks to complete
            const int CLEANUP_TIMEOUT = 1000;
            if (drawTask != null)
            {
                try { drawTask.Wait(CLEANUP_TIMEOUT); } catch { }
            }
            if (moveTask != null)
            {
                try { moveTask.Wait(CLEANUP_TIMEOUT); } catch { }
            }

            // Clean up ship and asteroids
            lock (_gameLock)
            {
                ship?.ClearDraw();

                foreach (var asteroid in asteroids)
                {
                    if (asteroid != null)
                    {
                        asteroid.ClearDraw();
                    }
                }
                asteroids.Clear();
            }

            // Clean up input system
            InputSystem.Instance.Stop();

            Console.CursorVisible = true;
            Console.Clear();
        }

        public List<Asteroid> GetAsteroids() => asteroids;
        public object GetGameLock() => _gameLock;

        private void ShowStartScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            
            // Título centrado
            string title = "ASTEROID GAME";
            string subtitle = "Press any key to start...";
            string instructions = "Use A and D keys to move";
            
            int centerX = Console.WindowWidth / 2;
            int centerY = Console.WindowHeight / 2;
            
            Console.SetCursorPosition(centerX - (title.Length / 2), centerY - 2);
            Console.WriteLine(title);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(centerX - (subtitle.Length / 2), centerY);
            Console.WriteLine(subtitle);
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(centerX - (instructions.Length / 2), centerY + 2);
            Console.WriteLine(instructions);
            
            Console.ReadKey();
        }

        private void ShowGameOverScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            
            string gameOver = "GAME OVER";
            string pressKey = "Press any key to restart...";
            
            int centerX = Console.WindowWidth / 2;
            int centerY = Console.WindowHeight / 2;
            
            Console.SetCursorPosition(centerX - (gameOver.Length / 2), centerY - 1);
            Console.WriteLine(gameOver);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(centerX - (pressKey.Length / 2), centerY + 1);
            Console.WriteLine(pressKey);
            
            Console.ReadKey();
            GameBuffer.ForceRedraw();
            Console.Clear();
        }

        private void ResetGame()
        {
            lock (_gameLock)
            {
                // Limpiar el estado actual
                ship?.ClearDraw();
                foreach (var asteroid in asteroids)
                {
                    asteroid?.ClearDraw();
                }
                asteroids.Clear();

                // Reinicializar el juego
                gameStartTime = DateTime.Now;
                
                // Crear nuevos asteroides
                for (int i = 0; i < INITIAL_ASTEROIDS; i++)
                {
                    CreateNewAsteroid();
                }
                ship.Spawn();
            }
        }
    }
}
