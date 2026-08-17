namespace CaptainPinkTurd.Core.Interfaces
{
    public interface IModifiable
    {
        void Modify(object target);
    }
    public interface IModifiable<in T> : IModifiable
    {
        void Modify(T targetType);
    }
}
