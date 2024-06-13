# ET-DynamicEvent
ET8.1的动态事件扩展

## 优点：

1.使用泛型参数

2.没有使用委托，支持热重载

## 使用：

1.将DynamicEvent目录放入ET的Codes/Model/Share/Module目录下

2.定义事件处理类
```csharp
[DynamicEvent]
public class Test_DynamicEvent: ADynamicEvent<TestEntity, DynamicEventType.Test>
{
    protected override async ETTask Run(TestEntity self, DynamicEventType.Test arg)
    {
        //to do something
    }
}
```

3.注册和反注册需要监听的实体

- 1注册实体
```csharp
testEntity.AddComponent<DynamicEventComponent>();
```

- 2反注册实体
```csharp
testEntity.RemoveComponent<DynamicEventComponent>();
```

4.通知事件
```csharp
DynamicEventSystem.Instance.Publish<DynamicEventType.Test>(new DynamicEventType.Test());
```
或
```csharp
await DynamicEventSystem.Instance.PublishAsync<DynamicEventType.Test>(new DynamicEventType.Test());
```
