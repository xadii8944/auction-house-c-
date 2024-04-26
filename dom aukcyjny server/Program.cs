using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

class Program
{
    static List<string> properties = new List<string>();

    static void Main(string[] args)
    {
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
                    foreach (string property in properties)
                    {
                        writer.WriteLine(property);
                    }
                }
                else if (request.StartsWith("ADD"))
                {
                    // Add property
                    string property = request.Substring(4);
                    properties.Add(property);
                    Console.WriteLine($"Property added: {property}");
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
}
