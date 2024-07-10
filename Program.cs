using System.Net;
using System.Text;

Main();

class HttpServer
{
    int Port = 8080;
    HttpListener? _listener;
    void ListenerCallback(IAsyncResult result)
    {
        if (_listener!.IsListening)
        {
            HttpListenerContext context = _listener.EndGetContext(result);
            HttpListenerRequest? request = context.Request;
            Console.WriteLine($"{request.Url}");

            HttpListenerResponse response = context.Response;
            response.ContentType = "text/plain";

            string responseString = "connected";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            Receive();
        }
    }
    void Receive()
    {
        _listener!.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }
    public void Stop()
    {
        _listener?.Stop();
    }
    public string Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{Port}/");
        _listener.Start();
        Receive();
        return $"Server started at http://localhost:{Port}/";
    }
}

partial class Program
{
    private static bool _keepRunning = true;
    static void Main()
    {
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _keepRunning = false;
        };

        Console.WriteLine("Starting HTTP listener...");

        var httpServer = new HttpServer();
        httpServer.Start();

        while (_keepRunning) { }

        httpServer.Stop();
    }
}
