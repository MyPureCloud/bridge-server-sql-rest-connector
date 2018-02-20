using System.ServiceModel;
[ServiceContract]
public interface IExampleService
{
    [OperationContract]
    string HelloWorld(string message);

    [OperationContract]
    int GetIntegerValue();

    [OperationContract]
    int AddToIntegerValue(int value);

    [OperationContract]
    int SubtractFromIntegerValue(int value);
}