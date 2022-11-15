// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;

Console.WriteLine("Hello, World!");

var socket = await SocketConnector.ConnectAsync("localhost", 5810, false, "HelloWorld");

while (true) {
    var m = await socket.ReadChannel.ReadAsync();
    Console.WriteLine(m.MessageType);
}

Console.WriteLine("Connected!");
