namespace CalculatorSOAPClient
{
    using System;
    using CalculatorSOAPClient.CalculatorService;

    public class CalculatorClient
    {
        static void Main(string[] args)
        {
            using (var calculatorService = new CalculatorServiceClient())
            {
                Point start = new Point() { X = 10, Y = 10 };
                Point end = new Point() { X = 15, Y = 15 };

                Console.WriteLine(calculatorService.CalcDistance(start, end));
            }
        }
    }
}
