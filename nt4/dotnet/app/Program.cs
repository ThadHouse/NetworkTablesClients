// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");



void localFunc() {

    Console.WriteLine("WAT");

    var s = "[{\"method\": \"Hello!!!\", \"params\": {}}, {}]"u8;

    Utf8JsonReader reader = new(s);

    using var doc = JsonDocument.ParseValue(ref reader);
    Console.WriteLine(doc.RootElement.ValueKind);
    foreach (var item in doc.RootElement.EnumerateArray()) {
        foreach (var item2 in item.EnumerateObject()) {
            Console.WriteLine(item2);
        }
        Console.WriteLine(item);
    }

    var arr = JsonSerializer.Deserialize<MethodThing[]>(s, new JsonSerializerOptions() {
    });

    //Console.WriteLine(arr!.Length);
    Console.WriteLine(arr[0].Method);
    Console.WriteLine(arr[0].Parameters);

    throw new InvalidOperationException();
}

localFunc();
//socket.WriteChannel.Complete();

var socket = await SocketConnector.ConnectAsync("localhost", 5810, false, "HelloWorld");

//await socket.DisposeAsync();

while (true) {
    using var m = await socket.ReadChannel.ReadAsync();
    Console.WriteLine(m.MessageType);
    if (m.MessageType == ReceiveMessageType.Disconnected) {
        break;
    }
}

await socket.DisposeAsync();

public readonly struct MethodThing {
    [JsonConstructor]
    public MethodThing(string? method, JsonDocument? parameters)
    {
        Method = method;
        Parameters = parameters;
    }

    [JsonPropertyName("method")]
    public string? Method {get;}
    [JsonPropertyName("params")]
    public JsonDocument? Parameters {get;}
}
