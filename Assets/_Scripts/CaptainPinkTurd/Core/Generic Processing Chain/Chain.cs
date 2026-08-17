namespace CaptainPinkTurd.Core.ProcessingChain
{
    // When This Pattern Is Worth Using
    //
    // Use it when:
    //
    // logic is multi-steps
    // steps can be reused
    // order of operations matters
    // you want clean modular code
    public class Chain<TIn, TOut> 
    {
        readonly IProcessor<TIn, TOut> processor;

        Chain(IProcessor<TIn, TOut> processor) 
        {
            this.processor = processor;
        }

        public static Chain<TIn, TOut> Start(IProcessor<TIn, TOut> processor) 
        {
            return new Chain<TIn, TOut>(processor);
        }

        public Chain<TIn, TNext> Then<TNext>(IProcessor<TOut, TNext> next)
        {
            var combined = new Combined<TIn, TOut, TNext>(processor, next);
            return new Chain<TIn, TNext>(combined);
        }
    
        public TOut Run(TIn input) => processor.Process(input);
        public ProcessorDelegate<TIn, TOut> Compile() => input => processor.Process(input); // Turns the pipeline into a reusable function
    }
}