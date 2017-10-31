using System.Web.Http;
using Autofac;
using SalesBot.Azure.CosmosDb;

namespace Salesbot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static IContainer _container;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();
            builder.Register((p) => new ConsultantCosmosDbSettings()).As<ICosmosDbSettings>();
            builder.RegisterType<ConsultantInfoRepository>().As<IConsultantInfoRepository>();
            _container = builder.Build();
        }
    }
}
