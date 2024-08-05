using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace Assets.Common.Scripts
{
    internal class CloudSaveDataProvider : IRemoteDataProvider
    {
        #region IRemoteDataProvider
        public bool IsSignInNeeded => true;
        public bool IsSignedIn => UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn;
        private async Task InitServicesIfUninited()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
        }
        public async Task SignUpAsync(string login, string password)
        {
            await InitServicesIfUninited();
            if (!IsSignedIn)
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(login, password);
            }
        }
        public async Task SignInAsync(string login, string password)
        {
            await InitServicesIfUninited();
            if (!IsSignedIn)
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(login, password);
            }
        }
        public async Task SignInAnonymouslyAsync()
        {
            try
            {
                await InitServicesIfUninited();
                if (!IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public Task SignOutAsync()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized && IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }
            return Task.CompletedTask;
        }
        #endregion

        #region IDataProvider
        public async Task<IDictionary<string, object>> LoadAsync(IDictionary<string, Type> metadata)
        {
            await InitServicesIfUninited();
            var dataItems = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>(metadata.Keys.AsEnumerable()));
            Dictionary<string, object> result = new();
            var getAsTypeMethodInfo = typeof(Unity.Services.CloudSave.Internal.Http.IDeserializable).GetMethod(nameof(Unity.Services.CloudSave.Internal.Http.IDeserializable.GetAs));
            foreach (var itemMetadata in metadata)
            {
                if (dataItems.TryGetValue(itemMetadata.Key, out var item))
                {
                    var getAsGenericTypeMethodInfo = getAsTypeMethodInfo.MakeGenericMethod(itemMetadata.Value);
                    var obj = getAsGenericTypeMethodInfo?.Invoke(item.Value, new object[1] { null });
                    result[item.Key] = obj;
                }
            }
            return result;
        }
        public async Task SaveAsync(IDictionary<string, object> data)
        {
            await InitServicesIfUninited();
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        #endregion
    }
}
