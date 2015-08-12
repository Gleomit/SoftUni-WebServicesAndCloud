namespace CalculatorSOAPService
{
    using System;

    public class CalculatorService : ICalculatorService
    {
        public double CalcDistance(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow((double)(end.X - start.X), 2) + Math.Pow((double)(end.Y - start.Y), 2));
        }
    }
}
