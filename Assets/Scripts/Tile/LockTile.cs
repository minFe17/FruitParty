using UnityEngine;

public class LockTile : MonoBehaviour
{
    public void DestroyTile()
    {
        //tileManager 구현 후 lockTiles 배열에서 제거
        Destroy(this.gameObject);
    }
}
