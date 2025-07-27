using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    // 싱글턴
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
    /// Addressables 시스템을 사용해 지정된 주소에서 에셋을 비동기로 로드하고 반환
    /// </summary>
    public async Task<T> GetAddressableAsset<T>(string address)
    {
        // TaskCompletionSource를 만들어 비동기 작업 완료 신호를 대기
        TaskCompletionSource<T> _loadCompletionSource = new TaskCompletionSource<T>();

        // LoadAssetAsync 함수 호출해 주소로부터 에셋 로드 시작
        await LoadAssetAsync(address, _loadCompletionSource);

        // 에셋 로드가 완료되면 결과를 반환
        return await _loadCompletionSource.Task;
    }

    public void Release<T>(T target)
    {
        Addressables.Release(target);
    }
}