using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Assets.Common.Scripts
{
    public class OperationResult
    {
        public bool Succeeded;
        public Exception Exception;
    }
    public class OperationResult<T> : OperationResult
    {
        public T Result;
    }
    public delegate void ProgressChangedHandler(string contentId, float progress);
    public delegate void DownloadCompletedHandler(string contentId, OperationResult result);
    public delegate void LoadSceneCompletedHandler(string contentId, OperationResult<SceneInstance> result);
    public delegate void UnloadCompletedHandler(string contentId, OperationResult result);
    public interface IContentProvider
    {
        event ProgressChangedHandler ProgressChanged;
        event DownloadCompletedHandler DownloadCompleted;
        event LoadSceneCompletedHandler LoadSceneCompleted;
        event UnloadCompletedHandler UnloadCompleted;
        Task<bool> IsCachedAsync(string contentId);
        Task<long> GetDownloadSizeAsync(string contentId);
        IEnumerator DownloadContentCoroutine(string contentId);
        IEnumerator LoadSceneCoroutine(string contentId, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activeOnLoad = true, int priority = 100);
        IEnumerator UnloadContentCoroutine(string contentId);
    }
}
