namespace LabWorks.Common.Helpers
{
    public delegate Tout ValueConverter<Tout>(string value, out bool convResult, out string error);
    public delegate bool ValueValidator<Tin>(Tin value, out string error);

    public static class ConsoleIOHelper
    {
        public static void Print(string msg, 
            ConsoleColor foreground, 
            ConsoleColor background = ConsoleColor.Black)
        { 
            if(msg == null) throw new ArgumentNullException(nameof(msg));

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintCenter(string msg,
            ConsoleColor foreground,
            ConsoleColor background = ConsoleColor.Black)
        { 
            if(msg == null) throw new ArgumentNullException(nameof(msg));

            var msgLength = msg.Length;
            var winWidth = Console.WindowWidth;

            var left = (winWidth / 2) - (msgLength / 2);
            var top = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(left, top);
            Print(msg, foreground, background);
        }

        public static bool KeyPressed(ConsoleKey key) =>
            Console.ReadKey().Key == key;

        public static bool KeyPressed(ConsoleKey key, string msg, ConsoleColor color)
        {
            Print(msg, color);
            return KeyPressed(key);
        }

        public static string Input(string msg, ConsoleColor color)
        {
            Print(msg, color);
            return Console.ReadLine();
        }

        public static bool Input<Tout>(string msg, ConsoleColor color, out Tout result,
            ValueConverter<Tout> converter, ValueValidator<Tout> validator = null)
        {
            Print(msg, color);
            string inp = Console.ReadLine() ?? string.Empty;
            string error = string.Empty;
            bool operResult = false;
            result = converter(inp, out operResult, out error);

            if (!operResult)
            {
                Print(error, ConsoleColor.Red);
                return false;
            }

            if(validator == null)
                return true;

            operResult = false;
            error = string.Empty;

            if(validator(result, out error))
                return true;

            Print(error, ConsoleColor.Red);
            return false;
        }

        public static Tout DoInputWhileOk<Tout>(string msg, ConsoleColor color,
            ValueConverter<Tout> converter, ValueValidator<Tout> validator = null)
        {
            Tout res = default;

            while (!Input<Tout>(msg, color, out res, converter, validator))
            { 
            
            }

            return res;
        }
    }
}
