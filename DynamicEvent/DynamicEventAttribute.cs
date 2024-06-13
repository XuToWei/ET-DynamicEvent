using System;

namespace ET
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DynamicEventAttribute: BaseAttribute
    {
        public SceneType SceneType {get; }

        public DynamicEventAttribute(SceneType sceneType = SceneType.None)
        {
            this.SceneType = sceneType;
        }
    }
}