using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EventManager : MonoBehaviour
{
    // �̱���
    List<Event> _events = new List<Event>();
    GameObject _eventPrefab;
    Shuffle _shuffle;
    Reset _reset;

    ScoreManager _scoreManager;

    public List<Event> Events { get => _events; }
    public Shuffle Shuffle { get => _shuffle; set => _shuffle = value; }
    public Reset Reset { set =>_reset = value;}

    int _eventScore;
    int _eventScoreAmount = 2000;
    int _resetScore;
    int _resetScoreAmount = 5000;
    int _lastEventIndex = (int)EEventType.Max;

    public void Init()
    {
        _eventScore = 1000;
        _resetScore = 5000;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        CreateEvent();
    }

    void CreateEvent()
    {
        _eventPrefab = Resources.Load("Prefabs/Event") as GameObject;
        Instantiate(_eventPrefab, transform.position, Quaternion.identity);
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
        else if(_eventScore <= _scoreManager.Score)
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
    Heat,
    MarketDay,
    Typhoon,
    Volcano,
    Max,
}