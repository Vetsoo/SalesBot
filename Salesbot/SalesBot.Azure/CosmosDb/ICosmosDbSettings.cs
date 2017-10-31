using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBot.Azure.CosmosDb
{
    public interface ICosmosDbSettings
    {
        string DatabaseName { get; }

        string CollectionName { get; }

        string EndpointUrl { get; }

        string AuthorizationKey { get; }
    }
}