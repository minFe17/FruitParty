using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EventManager : MonoBehaviour
{
    // ╫л╠шео
    List<Event> _events = new List<Event>();
    Shuffle _shuffle;
    Reset _reset;

    ScoreManager _scoreManager;

    public Shuffle Shuffle { get => _shuffle; }

    int _eventScore;
    int _eventScoreAmount = 2000;
    int _resetScore;
    int _resetScoreAmount = 5000;
    int _lastEventIndex;

    public void Init()
    {
        _eventScore = 1000;
        _resetScore = 5000;
        SetEvent();
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
    }

    void SetEvent()
    {
        AddEvent();
        _shuffle = new Shuffle();
        _reset = new Reset();
    }

    void AddEvent()
    {
        _events.Add(new EarthQuake());
    }

    public void OnEvent()
    {
        if(_resetScore <= _scoreManager.Score)
        {
            _reset.EventEffect();
            _resetScore += _resetScoreAmount;
            if (_resetScore == _eventScore)
                _eventScore += _eventScoreAmount;
        }
        if(_eventScore <= _scoreManager.Score)
        {
            int randomIndex = Random.Range(0, _events.Count);
            if(randomIndex != _lastEventIndex)
            {
                _events[randomIndex].EventEffect();
                _lastEventIndex = randomIndex;
            }
            _eventScore += _eventScoreAmount;
        }
    }
}

public enum EEventType
{
    Shuffle,
    Reset,
    EarthQuake,
    Max,
}