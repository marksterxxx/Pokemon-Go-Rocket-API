using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Exceptions;

namespace PokemonGo.RocketAPI.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.SetLogger(new ConsoleLogger(LogLevel.Info));

            Task.Run(() =>
            {
                try
                {
                    var xSettings = new Settings();

                    if(xSettings.DefaultLatitude != double.NaN && xSettings.DefaultLongitude != double.NaN)
                    {
                        Logger.Write($"Default coordinates Lat: {xSettings.DefaultLatitude} , Lng: {xSettings.DefaultLongitude}");
                    }
                  

                    new Logic.Logic(xSettings).Execute().Wait();
                    
                }
                catch (PtcOfflineException)
                {
                    Logger.Write("PTC Servers are probably down OR your credentials are wrong. Try google", LogLevel.Error);
                }
                catch (Exception ex)
                {
                    Logger.Write($"Unhandled exception: {ex}", LogLevel.Error);
                }
            });
             System.Console.ReadLine();
        }
    }
}