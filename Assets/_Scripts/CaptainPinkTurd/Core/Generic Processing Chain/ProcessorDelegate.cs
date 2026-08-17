namespace CaptainPinkTurd.Core.ProcessingChain
{
    public delegate TOut ProcessorDelegate<in TIn, out TOut>(TIn input);
}