using System.Collections.Generic;

namespace CaptainPinkTurd.Core.DesignPattern.Command
{
    public abstract class CommandInvoker<TCollectionType, T> where TCollectionType : IEnumerable<T>
    {
        protected readonly int MAX_HISTORY;
        protected bool enableMaxHistory = false;
        
        protected TCollectionType commandList;
        public IReadOnlyCollection<T> readOnlyCommandList => commandList as IReadOnlyCollection<T>;
        
        public CommandInvoker(bool enableMaxHistory, int maxHistory)
        {
            this.enableMaxHistory = enableMaxHistory;   
            this.MAX_HISTORY = maxHistory;  
        }

        public abstract void AddCommand(ICommand newCommand);
        public abstract void UndoCommand();
        protected abstract void TrimOldestIfNeeded();
    }
    public abstract class CommandInvoker<TCollectionType, TCommandType, T> where TCollectionType : IEnumerable<T>
    {
        protected readonly int MAX_HISTORY;
        protected bool enableMaxHistory = false;
        
        protected TCollectionType commandList;
        public IReadOnlyCollection<T> readOnlyCommandList => commandList as IReadOnlyCollection<T>;
        
        public CommandInvoker(bool enableMaxHistory, int maxHistory)
        {
            this.enableMaxHistory = enableMaxHistory;   
            this.MAX_HISTORY = maxHistory;  
        }

        public abstract void AddCommand(ICommand<TCommandType> newCommand, TCommandType item);
        public abstract void UndoCommand();
        protected abstract void TrimOldestIfNeeded();
    }
}