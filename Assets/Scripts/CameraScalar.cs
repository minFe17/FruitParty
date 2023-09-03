using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    [SerializeField] int _cameraOffset;
    [SerializeField] int _displayWidth;
    [SerializeField] int _displawyHeight;
    [SerializeField] float _padding;

    float _aspectRatio;

    public void SettingCamera(int width, int height)
    {
        Vector3 position = new Vector3(width / 2, height / 2, _cameraOffset);
        transform.position = position;
        _aspectRatio = _displawyHeight / (float)_displayWidth;
        if (width >= height)
            Camera.main.orthographicSize = (width / 2 + _padding) / _aspectRatio;
        else
            Camera.main.orthographicSize = height / 2 + _padding;
    }
}
