interface IEventSubscriber
{
    void OnEnable() {
        if (EventBus.Instance != null) EventBus.Instance.AddListeners(this);
    }

    void Start() {
        EventBus.Instance.AddListeners(this);       
    }

    void OnDisable() {
        EventBus.Instance.RemoveListeners(this);
    }
}
