using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public class InputSystem
    {
        private static readonly object _lock = new object();
        private static InputSystem _instance;
        private readonly ConcurrentQueue<ConsoleKey> _inputQueue;
        private readonly CancellationTokenSource _cts;
        private Task _inputTask;

        public static InputSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new InputSystem();
                    }
                }
                return _instance;
            }
        }

        private InputSystem()
        {
            _inputQueue = new ConcurrentQueue<ConsoleKey>();
            _cts = new CancellationTokenSource();
            StartInputTask();
        }

        public void StartInputTask()
        {
            _inputTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        _inputQueue.Enqueue(key);
                    }
                    await Task.Delay(100, _cts.Token);
                }
            }, _cts.Token);
        }

        public bool TryGetInput(out ConsoleKey key)
        {
            return _inputQueue.TryDequeue(out key);
        }

        public void Stop()
        {
            _cts.Cancel();
            _inputTask?.Wait(1000);
        }
    }
} 