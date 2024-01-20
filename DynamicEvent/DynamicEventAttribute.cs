using System;

namespace ET
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DynamicEventAttribute: BaseAttribute
    {
        public long Type {get; }

        public DynamicEventAttribute(long type)
        {
            this.Type = type;
        }
    }
}