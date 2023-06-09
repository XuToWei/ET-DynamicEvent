# ET-DynamicEvent
ET的动态事件扩展

## 优点：

1.使用泛型参数

2.没有使用委托，支持热重载

## 使用：

1.将DynamicEvent目录放入ET的Codes/Model/Share/Module目录下

2.给Scene添加DynamicEventWatcherComponent
```csharp
Root.Instance.Scene.AddComponent<DynamicEventWatcherComponent>();
```

3.定义事件处理类
```csharp
[DynamicEvent(SceneType.Client)]
public class Test_DynamicEvent:ADynamicEvent<TestEntity,DynamicEventType.Test>
{
    protected override async ETTask Run(Scene scene, TestEntity self,DynamicEventType.Test arg)
    {
        //to do something
    }
}
```

4.注册和反注册需要监听的实体

- 1注册实体
```csharp
testEntity.AddComponent<DynamicEventComponent>();
```
或
```csharp
DynamicEventWatcherComponent.Instance.Register(testEntity);
```

- 2反注册实体
```csharp
testEntity.RemoveComponent<DynamicEventComponent>();
```
或
```csharp
DynamicEventWatcherComponent.Instance.UnRegister(testEntity);
```

5.通知事件
```csharp
DynamicEventWatcherComponent.Instance.Publish<DynamicEventType.Test>(scene, new DynamicEventType.Test())
```
或
```csharp
await DynamicEventWatcherComponent.Instance.PublishAsync<DynamicEventType.Test>(scene, new DynamicEventType.Test())
```

# TODO

 - [ ] 优化筛选大量Entity的性能问题
