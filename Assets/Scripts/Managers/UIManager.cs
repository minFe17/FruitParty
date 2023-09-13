using UnityEngine;

public class UIManager : MonoBehaviour
{
    // ╫л╠шео
    UI _ui;

    public UI UI { get => _ui; }

    public void CreateUI()
    {
        GameObject temp = Resources.Load("Prefabs/UI") as GameObject;
        GameObject ui = Instantiate(temp, transform.position, Quaternion.identity);
        _ui = ui.GetComponent<UI>();
    }
}