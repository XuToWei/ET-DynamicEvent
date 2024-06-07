using System;

namespace ET
{
    public interface IDynamicEvent
    {
        public Type ArgType { get; }
        public Type EntityType { get; }
    }
    
    public interface IDynamicEvent<in T> : IDynamicEvent where T: struct
    {
        public ETTask Handle(Entity entity, T arg);
    }

    [EnableClass]
    public abstract class ADynamicEvent<A, B> : IDynamicEvent<B> where A : Entity where B : struct
    {
        public Type ArgType => typeof(B);
        public Type EntityType => typeof(A);
        
        protected abstract ETTask Run(A self, B arg);
        
        public async ETTask Handle(Entity self, B arg)
        {
            try
            {
                await Run((A)self, arg);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}