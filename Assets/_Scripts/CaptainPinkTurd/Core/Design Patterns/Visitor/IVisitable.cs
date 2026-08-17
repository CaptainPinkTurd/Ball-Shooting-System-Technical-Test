namespace CaptainPinkTurd.Core.DesignPattern.Visitor
{
    public interface IVisitable<in TVisitor>
    {
        void Accept(TVisitor visitor);
    }
}