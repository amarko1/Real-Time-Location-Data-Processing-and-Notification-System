using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7130/searchHub")
                .WithAutomaticReconnect() 
                .Build();

            connection.On<string>("ReceiveSearchNotification", message =>
            {
                Console.WriteLine($"Received notification: {message}");
            });

            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    Console.WriteLine("Connected on the hub.");
                    break;  
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to the hub: {ex.GetBaseException()}");
                }
            }

            Console.WriteLine("Press Q for exit.");
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                //wait
            }

            if (connection.State == HubConnectionState.Connected)
            {
                await connection.StopAsync();
                Console.WriteLine("Disconnected from the hub");
            }
        }
    }
}
