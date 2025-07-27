using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    // �̱���
    void LoadAsset<T>(string address, Action<T> callback)
    {
        Addressables.LoadAssetAsync<T>(address).Completed += handle => OnLoadDone(handle, callback);
    }

    void OnLoadDone<T>(AsyncOperationHandle<T> handle, Action<T> callback)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
            callback.Invoke(handle.Result);
    }

    Task LoadAssetAsync<T>(string address, TaskCompletionSource<T> completionSource)
    {
        Action<T> callback = spriteAtlas => { completionSource.SetResult(spriteAtlas); };
        LoadAsset(address, callback);

        return completionSource.Task;
    }

    /// <summary>
    /// Addressables �ý����� ����� ������ �ּҿ��� ������ �񵿱�� �ε��ϰ� ��ȯ
    /// </summary>
    public async Task<T> GetAddressableAsset<T>(string address)
    {
        // TaskCompletionSource�� ����� �񵿱� �۾� �Ϸ� ��ȣ�� ���
        TaskCompletionSource<T> _loadCompletionSource = new TaskCompletionSource<T>();

        // LoadAssetAsync �Լ� ȣ���� �ּҷκ��� ���� �ε� ����
        await LoadAssetAsync(address, _loadCompletionSource);

        // ���� �ε尡 �Ϸ�Ǹ� ����� ��ȯ
        return await _loadCompletionSource.Task;
    }

    public void Release<T>(T target)
    {
        Addressables.Release(target);
    }
}