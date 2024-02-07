using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EventManager : MonoBehaviour
{
    // ╫л╠шео
    List<IEvent> _events = new List<IEvent>();
    GameObject _eventPrefab;
    IEvent _shuffle;
    IEvent _reset;
    ScoreManager _scoreManager;
    AddressableManager _addressableManager;

    int _eventScore;
    int _eventScoreAmount = 2000;
    int _resetScore;
    int _resetScoreAmount = 5000;
    int _lastEventIndex;

    public List<IEvent> Events { get => _events; }
    public IEvent Shuffle { get => _shuffle; set => _shuffle = value; }
    public IEvent Reset { set => _reset = value; }

    public void Init()
    {
        if (_scoreManager == null)
            _scoreManager = GenericSingleton<ScoreManager>.Instance;

        _events.Clear();
        _eventScore = 1000;
        _resetScore = 5000;
        _lastEventIndex = (int)EEventType.Max;
        CreateEvent();
    }

    public async void LoadAsset()
    {
        if (_addressableManager == null)
            _addressableManager = GenericSingleton<AddressableManager>.Instance;
        _eventPrefab = await _addressableManager.GetAddressableAsset<GameObject>("Event");
    }

    void CreateEvent()
    {
        Instantiate(_eventPrefab, transform.position, Quaternion.identity);
    }

    void ResetEvent()
    {
        if (_resetScore == _eventScore)
            _eventScore += _eventScoreAmount;
        _reset.EventEffect();
        _resetScore += _resetScoreAmount;
    }

    void RandomEvent()
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

    public void OnEvent()
    {
        if (_resetScore <= _scoreManager.Score)
            ResetEvent();
        else if (_eventScore <= _scoreManager.Score)
            RandomEvent();
    }
}