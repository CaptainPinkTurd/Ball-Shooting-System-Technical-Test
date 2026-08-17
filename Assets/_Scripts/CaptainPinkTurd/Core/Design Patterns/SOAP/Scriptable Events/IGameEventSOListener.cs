namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    public interface IGameEventSOListener<in T>
    {
        void OnEventRaised(T data);
    }
}