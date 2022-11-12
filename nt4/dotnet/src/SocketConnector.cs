using System.Net.WebSockets;

public class SocketConnector
{
    ClientWebSocket ws;



    public SocketConnector() {
        ws = new ClientWebSocket();
        ws.ReceiveAsync(new byte[2048], default);
    }


}
