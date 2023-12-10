using System;
using GsyncSwitchCli;

namespace GsyncSwitchCLI
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments provided. Available commands are:");
                Console.WriteLine("  enable-gsync");
                Console.WriteLine("  disable-gsync");
                Console.WriteLine("  toggle-vsync");
                Console.WriteLine("  toggle-framelimiter <maxFPS>");
                Console.WriteLine("  toggle-hdr");
                return;
            }

            string command = args[0].ToLowerInvariant();

            switch (command)
            {
                case "enable-gsync":
                    GsyncSwitchAPI.NVAPIWrapperEnableGsync();
                    Console.WriteLine("G-Sync has been enabled.");
                    break;
                case "disable-gsync":
                    GsyncSwitchAPI.NVAPIWrapperDisableGsync();
                    Console.WriteLine("G-Sync has been disabled.");
                    break;
                case "toggle-vsync":
                    GsyncSwitchAPI.NVAPIWrapperSwitchVsync(true);
                    Console.WriteLine("V-Sync has been toggled.");
                    break;
                case "toggle-framelimiter":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Please provide the max FPS value for the frame limiter.");
                        return;
                    }

                    if (!int.TryParse(args[1], out int maxFPS))
                    {
                        Console.WriteLine("Invalid max FPS value provided.");
                        return;
                    }

                    GsyncSwitchAPI.NVAPIWrapperSwitchFrameLimiter(true, maxFPS);
                    Console.WriteLine($"Frame Limiter has been toggled with max FPS set to {maxFPS}.");
                    break;
                case "toggle-hdr":
                    GsyncSwitchAPI.NVAPIWrapperSwitchHDR(true);
                    Console.WriteLine("HDR has been toggled.");
                    break;
                default:
                    Console.WriteLine("Unknown command: " + command);
                    break;
            }
        }
    }
}
