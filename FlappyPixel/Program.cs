using System;

namespace FlappyPixel
{
    class Program
    {
        static System.Timers.Timer _timer;

        static int _origX;
        static int _origY;

        static Tube[] _tubes = new Tube[25];
        static Pixel _pixel = new Pixel();

        static int _windowWidth;
        static int _windowHeight;

        const ConsoleColor DEFAULT_BACKGROUND_COLOR = ConsoleColor.Black;
        const ConsoleColor DEFAULT_FOREGROUND_COLOR = ConsoleColor.Green;
        const ConsoleColor EXPLOSION_COLOR = ConsoleColor.Yellow;

        const string SPACE = " ";
        const string TUBE = "#";
        const string PIXEL = ">";
        const string EXPLOSION = "*";

        static bool _gameOver = false;

        static void Main(string[] args)
        {
            Initialize();
            InitializePixel();
            InitializeTubes();

            ReadKey();

            Console.ReadLine();
        }

        static void Initialize()
        {
            try
            {
                Console.Clear();
                Console.CursorVisible = false;

                Console.ForegroundColor = DEFAULT_FOREGROUND_COLOR;
                Console.BackgroundColor = DEFAULT_BACKGROUND_COLOR;
                Console.Clear();

                _origY = Console.CursorTop;
                _origX = Console.CursorLeft;

                _windowWidth = Console.WindowWidth;
                _windowHeight = Console.WindowHeight;

                _timer = new System.Timers.Timer(250);
                _timer.Elapsed += (sender, e) => MoveTubes();
                _timer.Elapsed += (sender, e) => DropPixel();
                _timer.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void InitializePixel()
        {
            _pixel.X = 10;
            _pixel.Y = _windowHeight / 2;

            WriteAt(PIXEL, _pixel.X, _pixel.Y);
        }

        static void InitializeTubes()
        {
            var random = new Random();

            for (int i = 0; i < _tubes.Length; i++)
            {
                var x = random.Next(0, _windowWidth) + _windowWidth;
                var y = _windowHeight;
                var height = random.Next(5, 18);

                _tubes[i] = new Tube{ X = x, Y = y, Height = height};

                for (int j = _windowHeight; j >= _windowHeight - _tubes[i].Height ; j--)
                {
                    WriteAt(TUBE, _tubes[i].X, j);
                }
            }
        }

        static void WriteAt(string s, int x, int y,
                            ConsoleColor foregroundColor = DEFAULT_FOREGROUND_COLOR,
                            ConsoleColor backgroundColor = DEFAULT_BACKGROUND_COLOR)
        {
            try
            {
                if (_origX + x > _windowWidth || _origX + x < 0
                    || _origY + y > _windowHeight || _origY + y < 0)
                {
                    return;
                }

                Console.ForegroundColor = foregroundColor;
                Console.SetCursorPosition(_origX + x, _origY + y);

                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        static void ClearPosition(int x, int y)
        {
            WriteAt(SPACE, x, y, DEFAULT_BACKGROUND_COLOR, DEFAULT_FOREGROUND_COLOR);
        }

        static void MoveTubes()
        {
            CheckIfPixelCollidedWithTubes();

            for (int i = 0; i < _tubes.Length; i++)
            {
                // move part of the tube to the left
                var newX = _tubes[i].X - 2;

                for (int j = _windowHeight; j >= _windowHeight - _tubes[i].Height; j--)
                {
                    // clear part of the tube
                    WriteAt(SPACE, _tubes[i].X, j);

                    // redraw part of the tube
                    WriteAt(TUBE, newX, j);
                }

                _tubes[i].X = newX;
            }

            CheckIfPixelCollidedWithTubes();
        }

        static void DropPixel()
        {
            CheckIfPixelCollidedWithTubes();

            WriteAt(SPACE, _pixel.X, _pixel.Y);

            _pixel.Y += 1;

            WriteAt(PIXEL, _pixel.X, _pixel.Y);

            CheckIfPixelCollidedWithTubes();
        }

        static void RisePixel()
        {
            CheckIfPixelCollidedWithTubes();

            WriteAt(SPACE, _pixel.X, _pixel.Y);

            _pixel.Y -= 1;

            WriteAt(PIXEL, _pixel.X, _pixel.Y);

            CheckIfPixelCollidedWithTubes();
        }

        static void CheckIfPixelCollidedWithTubes()
        {
            for (int i = 0; i < _tubes.Length; i++)
            {
                if (_tubes[i].X == _pixel.X && _windowHeight - _tubes[i].Height <= _pixel.Y)
                {
                    WriteAt(EXPLOSION, _pixel.X, _pixel.Y, EXPLOSION_COLOR);
                    PrintMissionFailed();
                }
            }
        }

        static void PrintMissionFailed()
        {
            Console.Clear();

            var message = "Mission failed!!!";

            WriteAt(message, (_windowWidth / 2) - message.Length / 2, _windowHeight / 2, ConsoleColor.Red);

            _gameOver = true;
            _timer.Stop();
        }

        static void PrintMissionAccomplished()
        {
            Console.Clear();

            var message = "Mission accomplished!!!";

            WriteAt(message, (_windowWidth / 2) - message.Length / 2, _windowHeight / 2, ConsoleColor.Yellow);

            _gameOver = true;
            _timer.Stop();
        }

        static void ReadKey()
        {
            ConsoleKeyInfo keyInfo;

            try
            {
                while (!_gameOver && (keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
                {
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Spacebar:
                            RisePixel();
                            break;
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
