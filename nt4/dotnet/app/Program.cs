// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;

Console.WriteLine("Hello, World!");

var ws = new ClientWebSocket();
ws.Options.AddSubProtocol("networktables.first.wpi.edu");
ws.Options.SetBuffer(2 * 1024 * 1024, 2 * 1024 * 1024);

await ws.ConnectAsync(new Uri("ws://localhost:5810/nt/hello"), default);
Console.WriteLine(ws.SubProtocol);

Console.WriteLine(ws.State);

while (true) {
    byte[] data = new byte[0xFFFF];
    try {
    var r = await ws.ReceiveAsync(data, default);
    Console.WriteLine("Received");
    } catch (Exception e) {
        Console.WriteLine(e);
        Console.WriteLine(ws.CloseStatus);
        Console.WriteLine(ws.CloseStatusDescription);
        break;
    }
}

Console.WriteLine("Connected!");
