using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EventManager : MonoBehaviour
{
    // ╫л╠шео
    List<Event> _events = new List<Event>();
    GameObject _eventPrefab;
    Shuffle _shuffle;
    Reset _reset;
    ScoreManager _scoreManager;

    int _eventScore;
    int _eventScoreAmount = 2000;
    int _resetScore;
    int _resetScoreAmount = 5000;
    int _lastEventIndex;

    public List<Event> Events { get => _events; }
    public Shuffle Shuffle { get => _shuffle; set => _shuffle = value; }
    public Reset Reset { set => _reset = value; }

    public void Init()
    {
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _events.Clear();
        _eventScore = 1000;
        _resetScore = 5000;
        _lastEventIndex = (int)EEventType.Max;
        CreateEvent();
    }

    void CreateEvent()
    {
        _eventPrefab = Resources.Load("Prefabs/Event") as GameObject;
        Instantiate(_eventPrefab, transform.position, Quaternion.identity);
    }

    public void OnEvent()
    {
        if (_resetScore <= _scoreManager.Score)
        {
            _reset.EventEffect();
            _resetScore += _resetScoreAmount;
            if (_resetScore == _eventScore)
                _eventScore += _eventScoreAmount;
        }
        else if (_eventScore <= _scoreManager.Score)
        {
            int randomIndex = Random.Range(0, _events.Count);
            int iteration = 0;
            while (randomIndex == _lastEventIndex && iteration <= 100)
            {
                randomIndex = Random.Range(0, _events.Count);
                iteration++;
            }
            _events[randomIndex].EventEffect();
            _lastEventIndex = randomIndex;
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