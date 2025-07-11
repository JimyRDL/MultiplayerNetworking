
using FishNet.Connection;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Events
{
    public static StartCountdownEvent startCountdownEvent = new StartCountdownEvent();
    public static CountdownFinishedEvent countdownFinishedEvent = new CountdownFinishedEvent();
}

public class StartCountdownEvent : GameEvent
{
}
public class CountdownFinishedEvent : GameEvent
{
}

public class DieEvent : GameEvent
{
    public GameObject playerDead;
    public GameObject playerShooter;
    public NetworkConnection connection; 
}

