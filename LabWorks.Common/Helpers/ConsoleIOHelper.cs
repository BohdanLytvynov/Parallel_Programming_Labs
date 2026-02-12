namespace LabWorks.Common.Helpers
{
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
    }
}
