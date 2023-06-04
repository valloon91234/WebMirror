public class Logger
{
    public string LogDirectoryName { get; set; }

    public string LogFilename { get; set; }

    public Logger(string filename, string directoryName = "log")
    {
        this.LogFilename = filename;
        this.LogDirectoryName = directoryName;
    }

    public void WriteLine(string? text = null, ConsoleColor color = ConsoleColor.White, bool writeFile = true)
    {
        if (text == null)
        {
            Console.WriteLine();
            return;
        }
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.White;
        if (writeFile) WriteFile(text);
    }

    public bool ExistFile()
    {
        DirectoryInfo logDirectoryInfo = new(LogDirectoryName);
        if (!logDirectoryInfo.Exists) return false;
        string logFilename = Path.Combine(LogDirectoryName, LogFilename + ".txt");
        return File.Exists(logFilename);
    }

    public void WriteFile(string text)
    {
        try
        {
            DirectoryInfo logDirectoryInfo = new(LogDirectoryName);
            if (!logDirectoryInfo.Exists) logDirectoryInfo.Create();
            string logFilename = Path.Combine(LogDirectoryName, LogFilename + ".txt");
            using var streamWriter = new StreamWriter(logFilename, true);
            streamWriter.WriteLine(text);
        }
        catch (Exception ex)
        {
            WriteLine("Cannot write log file : " + ex.Message, ConsoleColor.Red, false);
        }
    }

    public static void WriteLine(string? text = null, ConsoleColor? color = null)
    {
        if (text == null)
        {
            Console.WriteLine();
            return;
        }
        if (color == null)
        {
            Console.WriteLine(text);
        }
        else
        {
            Console.ForegroundColor = color.Value;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public static void WriteWait(string text, int seconds, int intervalSeconds = 1, ConsoleColor color = ConsoleColor.DarkGray)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        for (int i = 0; i < seconds; i += intervalSeconds)
        {
            Console.Write('.');
            Thread.Sleep(intervalSeconds * 1000);
        }
        Thread.Sleep((seconds % intervalSeconds) * 1000);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
    }

}
