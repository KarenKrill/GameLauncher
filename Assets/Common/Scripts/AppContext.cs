using Assets.Common.Scripts.Utils;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.Util;

namespace Assets.Common.Scripts
{
    internal class AppContext : MonoBehaviour
    {
        private static bool _isInited = false;
        private static AppContext _instance = null;
        private async Task InitUnityCloudDataProvider()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public async Task Init()
        {
            if (_isInited)
            {
                ContentProvider = _instance.ContentProvider;
            }
            else
            {
                ContentProvider = new AddressablesContentProvider();
                await InitUnityCloudDataProvider();
                _isInited = true;
            }
        }
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }
        [NonSerialized]
        public IContentProvider ContentProvider;
    }
}