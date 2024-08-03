using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Assets.Common.Scripts
{
    public interface IDataProvider
    {
        Task<IDictionary<string, object>> LoadAsync(IDictionary<string, Type> metadata);
        Task SaveAsync(IDictionary<string, object> data);
    }
}
