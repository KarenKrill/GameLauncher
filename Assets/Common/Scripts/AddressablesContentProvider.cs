using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets;

namespace Assets.Common.Scripts
{
    internal class AddressablesContentProvider : IContentProvider
    {
        public event ProgressChangedHandler ProgressChanged;
        public event DownloadCompletedHandler DownloadCompleted;
        public event LoadSceneCompletedHandler LoadSceneCompleted;
        public event UnloadCompletedHandler UnloadCompleted;
        public IEnumerator DownloadContentCoroutine(string contentId)
        {
            var handle = Addressables.DownloadDependenciesAsync(contentId);
            while (!handle.IsDone)
            {
                yield return null;
                if (handle.PercentComplete > 0)
                {
                    ProgressChanged?.Invoke(contentId, handle.PercentComplete);
                }
            }
            OperationResult operationResult = new();
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                operationResult.Succeeded = false;
                operationResult.Exception = handle.OperationException;
            }
            else
            {
                operationResult.Succeeded = true;
                ProgressChanged?.Invoke(contentId, handle.PercentComplete);
            }
            Addressables.Release(handle);
            DownloadCompleted?.Invoke(contentId, operationResult);
        }
        public IEnumerator LoadSceneCoroutine(string contentId, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activeOnLoad = true, int priority = 100)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var handle = Addressables.LoadSceneAsync(contentId, loadSceneMode, activeOnLoad, priority);
            yield return null;
            while (!handle.IsDone)
            {
                yield return null;
                if (handle.PercentComplete > 0)
                {
                    ProgressChanged?.Invoke(contentId, handle.PercentComplete);
                }
            }
            ProgressChanged?.Invoke(contentId, handle.PercentComplete);
            sw.Stop();
            Debug.LogError($"Game loaded about {sw.ElapsedMilliseconds} ms");
            LoadSceneCompleted?.Invoke(contentId, new OperationResult<SceneInstance>() { Result = handle.Result, Exception = handle.OperationException, Succeeded = handle.Status == AsyncOperationStatus.Succeeded });
            Addressables.Release(handle);
        }
        public IEnumerator UnloadContentCoroutine(string contentId)
        {
            //Caching.ClearCache();
            //yield return null;
            var handle = Addressables.ClearDependencyCacheAsync(contentId, false);
            while (!handle.IsDone)
            {
                yield return null;
                if (handle.PercentComplete > 0)
                {
                    ProgressChanged?.Invoke(contentId, 1 - handle.PercentComplete);
                }
            }
            OperationResult operationResult = new();
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                operationResult.Succeeded = false;
                operationResult.Exception = handle.OperationException;
            }
            else
            {
                operationResult.Succeeded = true;
                ProgressChanged?.Invoke(contentId, 1 - handle.PercentComplete);
            }
            Addressables.Release(handle);
            UnloadCompleted?.Invoke(contentId, operationResult);
        }
        public async Task<bool> IsCachedAsync(string contentId)
        {
            var downloadSize = await GetDownloadSizeAsync(contentId);
            return downloadSize == 0;
        }
        public async Task<long> GetDownloadSizeAsync(string contentId)
        {
            long totalDownloadSize = 0;
            AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(contentId);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                totalDownloadSize = handle.Result;
            }
            else
            {
                Debug.LogError($"{nameof(Addressables)}.{nameof(Addressables.GetDownloadSizeAsync)}(\"{contentId}\") failed with {handle.OperationException.GetType()}: {handle.OperationException}");
            }
            Addressables.Release(handle);
            return totalDownloadSize;
        }
    }
}
