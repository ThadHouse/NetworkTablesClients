// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;

Console.WriteLine("Hello, World!");

var socket = await SocketConnector.ConnectAsync("localhost", 5810, false, "HelloWorld");

//socket.WriteChannel.Complete();

//await socket.DisposeAsync();

while (true) {
    using var m = await socket.ReadChannel.ReadAsync();
    Console.WriteLine(m.MessageType);
    if (m.MessageType == ReceiveMessageType.Disconnected) {
        break;
    }
}

await socket.DisposeAsync();
