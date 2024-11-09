using System;
using UnityEngine;

/// <summary>
/// Class implementation of the EventSubscriber interface.
/// Because Unity will ignore these methods for Some Reason, this next line is added to silence Rider.
///
/// ReSharper disable MemberHidesInterfaceMemberWithDefaultImplementation
/// </summary>
public class EventSubscriber : MonoBehaviour, IEventSubscriber {
    private IEventSubscriber Super => this;
    protected virtual void OnEnable() => Super.OnEnable();
    protected virtual void Start() => Super.Start();
    protected virtual void OnDisable() => Super.OnDisable();
}
