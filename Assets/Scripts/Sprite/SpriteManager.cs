using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using Utils;

public class SpriteManager : MonoBehaviour
{
    // ╫л╠шео
    AddressableManager _addressableManager;

    SpriteAtlas _fruitAtlas;
    SpriteAtlas _bombAtlas;
    SpriteAtlas _tileAtlas;
    SpriteAtlas _uiAtlas;
    SpriteAtlas _eventAtals;

    Sprite _backgroundSprite;

    public SpriteAtlas FruitAtlas { get => _fruitAtlas; }
    public SpriteAtlas BombAtlas { get => _bombAtlas; }
    public SpriteAtlas TileAtlas { get => _tileAtlas; }
    public SpriteAtlas UIAtlas { get => _uiAtlas; }
    public SpriteAtlas EventAtlas { get => _eventAtals; }
    public Sprite BackgroundSprite { get => _backgroundSprite; }
 
    public async Task Init()
    {
        _fruitAtlas = await LoadAsset<SpriteAtlas>("FruitAtlas");
        _bombAtlas = await LoadAsset<SpriteAtlas>("BombAtlas");
        _tileAtlas = await LoadAsset<SpriteAtlas>("TileAtlas");
        _uiAtlas = await LoadAsset<SpriteAtlas>("UIAtlas");
        _eventAtals = await LoadAsset<SpriteAtlas>("EventAtlas");
        _backgroundSprite = await LoadAsset<Sprite>("BackgroundSprite");
    }

    async Task<T> LoadAsset<T>(string address)
    {
        if (_addressableManager == null)
            _addressableManager = GenericSingleton<AddressableManager>.Instance;
        return await _addressableManager.GetAddressableAsset<T>(address);
    }
}