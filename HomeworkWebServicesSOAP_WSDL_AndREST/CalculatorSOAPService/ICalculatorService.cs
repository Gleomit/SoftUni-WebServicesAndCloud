namespace CalculatorSOAPService
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ICalculatorService
    {
        [OperationContract]
        double CalcDistance(Point start, Point end);
    }
}
