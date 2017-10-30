using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using SalesBot.Azure.CosmosDb;
using System.Threading.Tasks;
using Salesbot.Models;
using System.Linq;

namespace Salesbot.Tests
{
    [TestClass]
    public class ConsultantInfoTests
    {
        private static IContainer _container; 

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            var builder = new ContainerBuilder();
            builder.Register((p) => new ConsultantCosmosDbSettings()).As<ICosmosDbSettings>();
            builder.RegisterType<ConsultantInfoRepository>().As<IConsultantInfoRepository>();
            _container = builder.Build();

            var consultantInfoRepository = _container.Resolve<IConsultantInfoRepository>() as CosmosDbRepository<ConsultantInfo>;
            if (consultantInfoRepository.CountAsync().GetAwaiter().GetResult() == 0)
            {
                var consultantInfo = new ConsultantInfo
                {
                    Email = "kris.bulte@axxes.com",
                    Name = "Kris Bulté",
                    Interests = new string[] { "architecture", "containers", "serverless" },
                    Skills = new string[] { "asp.net" }
                };
                consultantInfoRepository.AddOrUpdateAsync(consultantInfo, null).GetAwaiter();
            }
        }

        [TestMethod]
        public async Task QueryAllShouldReturnAllConsultantData()
        {
            var consultantInfoRepository = _container.Resolve<IConsultantInfoRepository>();
            var result = await consultantInfoRepository.GetAllConsultantInfo();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task QueryingOnSkillsShouldReturnFilteredConsultantData()
        {
            var consultantInfoRepository = _container.Resolve<IConsultantInfoRepository>();
            var result = await consultantInfoRepository.QueryConsultantInfoBySkill("asp.net");

            Assert.IsTrue(result.Any(x => x.Skills.Contains("ASP.NET", StringComparer.CurrentCultureIgnoreCase)));
        }

        [TestMethod]
        public async Task QueryingOnInterestsShouldReturnFilteredConsultantData()
        {
            var consultantInfoRepository = _container.Resolve<IConsultantInfoRepository>();
            var result = await consultantInfoRepository.QueryConsultantInfoByInterest("containers");

            Assert.IsTrue(result.Any(x => x.Interests.Contains("containers", StringComparer.CurrentCultureIgnoreCase)));
        }
    }
}
