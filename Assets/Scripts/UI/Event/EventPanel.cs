using System.Collections.Generic;
using UnityEngine;

public class EventPanel : MonoBehaviour
{
    [SerializeField] EventUIBase _shuffle;
    Dictionary<string, EventUIBase> _eventUI = new Dictionary<string, EventUIBase>();

}
