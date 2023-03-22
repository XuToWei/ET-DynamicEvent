using System;
using System.Collections.Generic;

namespace ET
{
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public class DynamicEventWatcherComponent : Entity, IAwake, IDestroy, ILoad
    {
        [StaticField] public static DynamicEventWatcherComponent Instance;

        private class DynamicEventInfo
        {
            public SceneType SceneType { get; }
            public IDynamicEvent DynamicEvent { get; }

            public DynamicEventInfo(SceneType sceneType, IDynamicEvent iDynamicEvent)
            {
                this.SceneType = sceneType;
                this.DynamicEvent = iDynamicEvent;
            }
        }

        /// <summary>
        /// 参数Type：{Entity的Type：DynamicEventInfo}
        /// </summary>
        private readonly Dictionary<Type, ListComponent<DynamicEventInfo>> allDynamicEventInfos;

        private readonly HashSet<long> registeredEntityIds = new HashSet<long>();
        
        public void Register(Entity component)
        {
            this.registeredEntityIds.Add(component.InstanceId);
        }
        
        public void Register(long instanceId)
        {
            this.registeredEntityIds.Add(instanceId);
        }

        public void UnRegister(Entity component)
        {
            this.registeredEntityIds.Remove(component.InstanceId);
        }
        
        public void UnRegister(long instanceId)
        {
            this.registeredEntityIds.Remove(instanceId);
        }

        internal void Clear()
        {
            this.allDynamicEventInfos.Clear();
            this.registeredEntityIds.Clear();
        }

        internal void Init()
        {
            this.allDynamicEventInfos.Clear();
            HashSet<Type> types = EventSystem.Instance.GetTypes(typeof(DynamicEventAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(DynamicEventAttribute), false);

                foreach (object attr in attrs)
                {
                    DynamicEventAttribute dynamicEventAttribute = (DynamicEventAttribute)attr;
                    IDynamicEvent obj = (IDynamicEvent)Activator.CreateInstance(type);
                    DynamicEventInfo dynamicEventInfo = new DynamicEventInfo(dynamicEventAttribute.SceneType, obj);
                    if (!this.allDynamicEventInfos.TryGetValue(dynamicEventInfo.DynamicEvent.ArgType, out ListComponent<DynamicEventInfo> dynamicEventInfos))
                    {
                        dynamicEventInfos = new ListComponent<DynamicEventInfo>();
                        this.allDynamicEventInfos.Add(dynamicEventInfo.DynamicEvent.ArgType, dynamicEventInfos);
                    }
                    dynamicEventInfos.Add(dynamicEventInfo);
                }
            }
        }

        public void Publish<A>(Scene scene, A arg) where A : struct
        {
            SceneType domainSceneType = scene.SceneType;
            Type argType = typeof(A);
            if (this.allDynamicEventInfos.TryGetValue(argType, out ListComponent<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (dynamicEventInfo.SceneType != domainSceneType && dynamicEventInfo.SceneType != SceneType.None)
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    foreach (long instanceId in this.registeredEntityIds)
                    {
                        Entity entity = Root.Instance.Get(instanceId);
                        if (entity != null && dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
                        {
                            dynamicEvent.Handle(scene, entity, arg).Coroutine();
                        }
                    }
                }
            }
        }

        public async ETTask PublishAsync<A>(Scene scene, A arg) where A : struct
        {
            using ListComponent<ETTask> taskList = ListComponent<ETTask>.Create();

            SceneType domainSceneType = scene.SceneType;

            Type argType = typeof(A);
            if (this.allDynamicEventInfos.TryGetValue(argType, out ListComponent<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (dynamicEventInfo.SceneType != domainSceneType && dynamicEventInfo.SceneType != SceneType.None)
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    foreach (long instanceId in this.registeredEntityIds)
                    {
                        Entity entity = Root.Instance.Get(instanceId);
                        if (entity != null && dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
                        {
                            taskList.Add(dynamicEvent.Handle(scene, entity, arg));
                        }
                    }
                }
            }

            try
            {
                await ETTaskHelper.WaitAll(taskList);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
    
    [FriendOf(typeof(DynamicEventWatcherComponent))]
    public static class DynamicEventWatcherSystem
    {
        [ObjectSystem]
        public class DynamicEventWatcherAwakeSystem : AwakeSystem<DynamicEventWatcherComponent>
        {
            protected override void Awake(DynamicEventWatcherComponent self)
            {
                DynamicEventWatcherComponent.Instance = self;
                self.Init();
            }
        }
        
        [ObjectSystem]
        public class DynamicEventWatcherDestroySystem : DestroySystem<DynamicEventWatcherComponent>
        {
            protected override void Destroy(DynamicEventWatcherComponent self)
            {
                self.Clear();
                DynamicEventWatcherComponent.Instance = null;
            }
        }

        [ObjectSystem]
        public class DynamicEventWatcherLoadSystem : LoadSystem<DynamicEventWatcherComponent>
        {
            protected override void Load(DynamicEventWatcherComponent self)
            {
                self.Init();
            }
        }
    }
}