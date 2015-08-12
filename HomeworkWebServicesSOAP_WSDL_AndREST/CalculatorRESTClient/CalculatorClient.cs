using System;
using RestSharp;

namespace CalculatorRESTClient
{
    public class CalculatorClient
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your port(You can find it after starting project \"CalculatorRESTService\"): ");
            string port = Console.ReadLine();
            
            Console.WriteLine("Enter PointOne.X: ");
            int startX = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter PointOne.Y: ");
            int startY = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter PointTwo.X: ");
            int endX = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter PointTwo.Y: ");
            int endY = int.Parse(Console.ReadLine());

            var client = new RestClient("http://localhost:" + port);

            var request = new RestRequest("api/distance", Method.GET);
            request.AddParameter("startX", startX);
            request.AddParameter("startY", startY);
            request.AddParameter("endX", endX);
            request.AddParameter("endY", endY);

            var response = client.Execute(request);
            var statusCode = response.StatusCode;

            if (statusCode == 0)
            {
                Console.WriteLine("Server is not started.");
                Console.WriteLine("Check the port.");
                Console.WriteLine("Run project \"CalculatorRESTService in order to start the server\".");
                return;
            }

            var result = response.Content;
            Console.WriteLine("distance = " + result);
        }
    }
}
