using System;
using System.Threading.Tasks;

using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace Assets.Common.Scripts
{
    internal class AppContext : MonoBehaviour
    {
        private static bool _isInited = false;
        private static AppContext _instance = null;
        public async Task Init()
        {
            if (_isInited)
            {
                ContentProvider = _instance.ContentProvider;
                DataProvider = _instance.DataProvider;
            }
            else
            {
                ContentProvider = new AddressablesContentProvider();
                DataProvider = new CloudSaveDataProvider();
                if (DataProvider is IRemoteDataProvider remoteDataProvider)
                {
                    if (remoteDataProvider.IsSignInNeeded)
                    {
                        await remoteDataProvider.SignInAnonymouslyAsync();
                    }
                }
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
        [NonSerialized]
        public IDataProvider DataProvider;
    }
}