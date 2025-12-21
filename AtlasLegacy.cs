using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace NyrionShellLegacy
{
    class Program
    {
        static readonly string NYRION_VERSION = "Atlas LEGACY";
        static readonly string BUILD = "26S1.0-net20";
        static readonly string PREFIX = "QopWarren Legacy Build";

        static List<string> CommandHistory = new List<string>();
        static bool EchoOn = true;
        static DateTime StartTime = DateTime.Now;
        static Random Rng = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine(NYRION_VERSION);
            Console.WriteLine(BUILD);
            Console.WriteLine(PREFIX);
            MainLoop();
        }

        static void MainLoop()
        {
            while (true)
            {
                Console.Write("Nyrion> ");
                string input = Console.ReadLine();
                if (input == null || input.Trim().Length == 0)
                    continue;

                ExecuteCommand(input);
            }
        }

        static void ExecuteCommand(string cmd)
        {
            if (EchoOn)
                Console.WriteLine(cmd);

            CommandHistory.Add(cmd);
            if (CommandHistory.Count > 100)
                CommandHistory.RemoveAt(0);

            string[] parts = cmd.Trim().Split(' ');
            if (parts.Length < 2 || parts[0].ToLower() != "qwe")
            {
                Console.WriteLine("Commands must start with 'qwe'");
                return;
            }

            string command = parts[1].ToLower();
            string[] args = new string[Math.Max(0, parts.Length - 2)];
            if (parts.Length > 2)
                Array.Copy(parts, 2, args, 0, args.Length);

            try
            {
                switch (command)
                {
                    case "help": Help(); break;
                    case "cls": Console.Clear(); break;
                    case "time": Console.WriteLine(DateTime.Now); break;
                    case "date": Console.WriteLine(DateTime.Now.ToShortDateString()); break;
                    case "dir": ListDir(); break;
                    case "cd": ChangeDir(args); break;
                    case "type": TypeFile(args); break;
                    case "echo": Echo(args); break;
                    case "calc": Calculator(); break;
                    case "history": History(); break;
                    case "uptime": Uptime(); break;
                    case "exit": Environment.Exit(0); break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        static void Help()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine(" qwe help");
            Console.WriteLine(" qwe cls");
            Console.WriteLine(" qwe time");
            Console.WriteLine(" qwe date");
            Console.WriteLine(" qwe dir");
            Console.WriteLine(" qwe cd <dir>");
            Console.WriteLine(" qwe type <file>");
            Console.WriteLine(" qwe echo on|off|text");
            Console.WriteLine(" qwe calc");
            Console.WriteLine(" qwe history");
            Console.WriteLine(" qwe uptime");
            Console.WriteLine(" qwe exit");
        }

        static void ListDir()
        {
            string[] entries = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory());
            for (int i = 0; i < entries.Length; i++)
            {
                string name = Path.GetFileName(entries[i]);
                if (Directory.Exists(entries[i]))
                    Console.WriteLine(name + "\\");
                else
                    Console.WriteLine(name);
            }
        }

        static void ChangeDir(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Missing directory.");
                return;
            }
            Directory.SetCurrentDirectory(args[0]);
        }

        static void TypeFile(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Missing filename.");
                return;
            }
            Console.WriteLine(File.ReadAllText(args[0]));
        }

        static void Echo(string[] args)
        {
            if (args.Length == 0)
                return;

            string a = args[0].ToLower();
            if (a == "off") EchoOn = false;
            else if (a == "on") EchoOn = true;
            else Console.WriteLine(string.Join(" ", args));
        }

        static void Calculator()
        {
            Console.WriteLine("Calculator (type exit to quit)");
            while (true)
            {
                Console.Write("calc> ");
                string expr = Console.ReadLine();
                if (expr == null || expr.ToLower() == "exit")
                    break;

                try
                {
                    DataTable dt = new DataTable();
                    Console.WriteLine(dt.Compute(expr, null));
                }
                catch
                {
                    Console.WriteLine("Invalid expression.");
                }
            }
        }

        static void History()
        {
            int start = Math.Max(0, CommandHistory.Count - 5);
            for (int i = start; i < CommandHistory.Count; i++)
                Console.WriteLine(CommandHistory[i]);
        }

        static void Uptime()
        {
            TimeSpan up = DateTime.Now - StartTime;
            Console.WriteLine(
                "Uptime: {0}d {1}h {2}m {3}s",
                up.Days, up.Hours, up.Minutes, up.Seconds);
        }
    }
}
