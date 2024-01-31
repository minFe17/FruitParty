using UnityEngine;
using Utils;

public class Groundwork : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    void Start()
    {
        CreateUI();
    }

    void CreateUI()
    {
        GenericSingleton<UIManager>.Instance.CreateUI(_mainCamera);
    }
}