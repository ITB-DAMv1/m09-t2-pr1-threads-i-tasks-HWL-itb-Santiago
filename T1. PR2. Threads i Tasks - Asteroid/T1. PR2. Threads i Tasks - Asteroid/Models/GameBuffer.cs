using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public class GameBuffer
    {
        private static readonly object _lock = new object();
        private static GameBuffer _instance;
        private readonly char[,] _buffer;
        private readonly ConsoleColor[,] _colorBuffer;
        private readonly char[,] _prevBuffer;
        private readonly ConsoleColor[,] _prevColorBuffer;
        private readonly int _width;
        private readonly int _height;
        private readonly StringBuilder _sb;
        private bool _needsRedraw = true;

        public static GameBuffer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new GameBuffer(Console.WindowWidth, Console.WindowHeight);
                    }
                }
                return _instance;
            }
        }

        private GameBuffer(int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = new char[width, height];
            _colorBuffer = new ConsoleColor[width, height];
            _prevBuffer = new char[width, height];
            _prevColorBuffer = new ConsoleColor[width, height];
            _sb = new StringBuilder(width * height + height); // Pre-allocate space for the entire screen
            Clear();
        }

        public void Clear()
        {
            lock (_lock)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        _buffer[x, y] = ' ';
                        _colorBuffer[x, y] = ConsoleColor.White;
                    }
                }
                _needsRedraw = true;
            }
        }

        public void SetPixel(int x, int y, char c, ConsoleColor color)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                lock (_lock)
                {
                    if (_buffer[x, y] != c || _colorBuffer[x, y] != color)
                    {
                        _buffer[x, y] = c;
                        _colorBuffer[x, y] = color;
                        _needsRedraw = true;
                    }
                }
            }
        }

        public void Draw()
        {
            lock (_lock)
            {
                ConsoleColor currentColor = Console.ForegroundColor;

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        char currentChar = _buffer[x, y];
                        ConsoleColor currentPixelColor = _colorBuffer[x, y];

                        // Solo dibujar si algo cambió
                        if (_prevBuffer[x, y] != currentChar || _prevColorBuffer[x, y] != currentPixelColor)
                        {
                            Console.SetCursorPosition(x, y);

                            if (currentPixelColor != currentColor)
                            {
                                Console.ForegroundColor = currentPixelColor;
                                currentColor = currentPixelColor;
                            }

                            Console.Write(currentChar);

                            // Guardar en el buffer anterior
                            _prevBuffer[x, y] = currentChar;
                            _prevColorBuffer[x, y] = currentPixelColor;
                        }
                    }
                }

                Console.ResetColor();
                _needsRedraw = false;
            }
        }

        public void ForceRedraw()
        {
            _needsRedraw = true;
        }
    }
} 