public class Reset : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Reset;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        ResetEvent();
    }

    void ResetEvent()
    {

    }
}
