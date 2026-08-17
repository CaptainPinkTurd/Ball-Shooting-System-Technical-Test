using System;

namespace CaptainPinkTurd.Core.CustomDataStructure
{
    public class OneTimeSettable<T>
    {
        private T value;
        private bool isSet = false;

        public T Value
        {
            get => value;
            set
            {
                if (isSet)
                {
                    throw new InvalidOperationException("Value can only be set once until reset.");
                }

                this.value = value;
                isSet = true;
            }
        }

        public void Reset()
        {
            value = default;
            isSet = false;
        }

        public bool IsSet => isSet;
    }
}
