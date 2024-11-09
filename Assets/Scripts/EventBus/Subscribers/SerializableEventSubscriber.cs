public class SerializableEventSubscriber<Self> : SerializableScript<Self>, IEventSubscriber
    where Self : SerializableEventSubscriber<Self> {
    private IEventSubscriber Super => this;
    protected virtual void OnEnable() => Super.OnEnable();
    protected virtual void Start() => Super.Start();
    protected virtual void OnDisable() => Super.OnDisable();
}