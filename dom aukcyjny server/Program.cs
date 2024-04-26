using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

class Program
{
    static List<(string, double, DateTime)> properties = new List<(string, double, DateTime)>();

    static void Main(string[] args)
    {
        LoadPropertiesFromJson();

        TcpListener server = null;
        try
        {
            int port = 8888;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);

            server.Start();

            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("Client connected.");

                NetworkStream stream = client.GetStream();

                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                string request = reader.ReadLine();

                if (request == "GET")
                {
                    // Send properties to client
                    foreach (var property in properties)
                    {
                        writer.WriteLine(property);
                    }
                }
                else if (request.StartsWith("ADD"))
                {
                    // Add property
                    string[] data = request.Substring(4).Split(',');
                    if (data.Length == 3 && double.TryParse(data[1], out double price))
                    {
                        AddProperty(data[0], price);
                        Console.WriteLine($"Property added: {data[0]}, Price: {price}, Added on: {DateTime.Now}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid data format. Format: ADD <property>, price, date");
                    }
                }

                client.Close();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }
        finally
        {
            server.Stop();
        }

        Console.WriteLine("\nServer stopped.");
        Console.ReadLine();
    }

    static void AddProperty(string name, double price)
    {
        properties.Add((name, price, DateTime.Now));
        SavePropertiesToJson();
    }

    static void LoadPropertiesFromJson()
    {
        if (File.Exists("properties.json"))
        {
            string json = File.ReadAllText("properties.json");
            properties = JsonConvert.DeserializeObject<List<(string, double, DateTime)>>(json);
        }
    }

    static void SavePropertiesToJson()
    {
        string json = JsonConvert.SerializeObject(properties);
        File.WriteAllText("properties.json", json);
    }
}
