using System.Configuration;

namespace SalesBot.Azure.CosmosDb
{
    public class DefaultCosmosDbSettings : ICosmosDbSettings
    {
        public DefaultCosmosDbSettings()
        {}

        public virtual string DatabaseName => ConfigurationManager.AppSettings["DocDb_DatabaseName"];

        public virtual string CollectionName { get; set; }

        public virtual string EndpointUrl => ConfigurationManager.AppSettings["DocDb_EndpointUrl"];

        public virtual string AuthorizationKey => ConfigurationManager.AppSettings["DocDb_AuthorizationKey"];
    }

    public class ConsultantCosmosDbSettings : DefaultCosmosDbSettings
    {
        public override string CollectionName => "consultantData";
    }
}
