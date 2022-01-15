namespace Client
{
    using Grpc.Net.Client;
    using Service;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            //Ignore this for now
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            SayHello();
            await SubscribeToStream();

        }

        private static void SayHello()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5002");

            // Greeter service is defined in hello.proto
            // <service-name>.<service-name>Client is auto-created
            var client = new Greeter.GreeterClient(channel);

            // HelloRequest is defined in hello.proto
            var request = new HelloRequest();
            request.Name = "Brian!";

            // SayHello method is defined in hello.proto
            var response = client.SayHello(request);

            // HelloResponse.Message is defined in hello.proto
            Console.WriteLine(response.Message);
        }

        private static async Task SubscribeToStream()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5002");

            var client = new Pricing.PricingClient(channel);
            var request = new PriceRequest { Uic = "211", AssetType = "Vacation Days" };

            var streamReader = client.Subscribe(request).ResponseStream;

            while (await streamReader.MoveNext(new CancellationToken()))
            {
                Console.WriteLine($"Received: {streamReader.Current}");
            }

            Console.WriteLine("Gracefully ended.");
        }
    }
}
