using UnityEngine;

public class LockTile : MonoBehaviour
{
    public void DestroyTile()
    {
        //tileManager ���� �� lockTiles �迭���� ����
        Destroy(this.gameObject);
    }
}
