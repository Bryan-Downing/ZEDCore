using System.Net.Sockets;
using System.Net;
using ZED.Common;
using rpi_rgb_led_matrix_sharp;
using System.Runtime.Serialization.Formatters.Binary;
using ZED;
using System.IO.Compression;

namespace RemoteLEDMatrixReceiver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var driver = new MainDriver(args);
            driver.Run();
        }
    }

    public class MainDriver : ZEDProgram
    {
        public MainDriver(string[] args) : base(args)
        {

        }

        public void Run()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 9201);
            server.Start();

            try
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                var options = ZEDMatrixOptionsExtensions.Deserialize(stream);

                stream.WriteByte(255); // TODO: Better handshake.

                using (RGBLedMatrix matrix = new RGBLedMatrix(options.ToRGBLedMatrixOptions()))
                {
                    RGBLedCanvas canvas = matrix.CreateOffscreenCanvas();

                    int width = options.Cols * options.ChainLength;
                    int height = options.Rows * options.Parallel;

                    byte[] buffer = new byte[width * height * 3];
                    int totalBytesRead = 0;

                    Console.WriteLine($"Press Q to quit.");

                    while (client.Connected)
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                        {
                            break;
                        }

                        totalBytesRead = 0;

                        while (totalBytesRead < buffer.Length)
                        {
                            int bytesRead = 0;

                            if (stream.DataAvailable)
                            {
                                bytesRead = stream.Read(buffer, totalBytesRead, Math.Min(8192, buffer.Length - totalBytesRead));

                                totalBytesRead += bytesRead;
                            }
                        }

                        canvas.Clear();

                        int i = 0;
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                canvas.SetPixel(x, y, new Color(buffer[i], buffer[i + 1], buffer[i + 2]));

                                i += 3;
                            }
                        }

                        // TODO: Save the last frame so we can keep drawing it until we get the next frame.
                        matrix.SwapOnVsync(canvas);
                    }
                }

                // Shutdown and end connection
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        protected override void HandleArguments(string[] args)
        {
            return;
        }
    }
}