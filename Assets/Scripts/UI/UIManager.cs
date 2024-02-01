using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class UIManager : MonoBehaviour
{
    // ╫л╠шео
    GameObject _uiPrefab;
    GameObject _lobbyUIPrefab;
    AddressableManager _addressableManager;

    public UI UI { get; set; }
    public GameUIPanel GameUIPanel { get; set; }


    public async Task LoadAsset()
    {
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        _uiPrefab = await _addressableManager.GetAddressableAsset<GameObject>("UI");
        _lobbyUIPrefab = await _addressableManager.GetAddressableAsset<GameObject>("LobbyUI");
    }

    public void CreateUI(Camera camera)
    {
        GameObject ui = Instantiate(_uiPrefab, transform.position, Quaternion.identity);
        ui.GetComponent<UI>().Init();
        ui.GetComponent<Canvas>().worldCamera = camera;
        ui.GetComponent<Canvas>().sortingLayerName = "UI";
    }

    public void CreateLobbyUI(Camera camera)
    {
        GameObject temp = Instantiate(_lobbyUIPrefab);
        temp.GetComponent<Canvas>().worldCamera = camera;
    }
}