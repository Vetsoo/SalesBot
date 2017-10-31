using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBot.Azure.CosmosDb
{
    public class ConsultantInfoRepository : CosmosDbRepository<ConsultantInfo>, IConsultantInfoRepository
    {
        public ConsultantInfoRepository(ICosmosDbSettings documentDbSettings) : base(documentDbSettings)
        {
        }

        public async Task<IEnumerable<ConsultantInfo>> GetAllConsultantInfo()
        {
            return await GetAllAsync();
        }

        public async Task<IEnumerable<ConsultantInfo>> QueryConsultantInfoBySkill(string skillSearchTerm)
        {
            var consultantsWithSkillQueryable = _client.CreateDocumentQuery<ConsultantInfo>(_collectionUri)
                .Where(consultantData => consultantData.Skills.Contains(skillSearchTerm))
                .AsDocumentQuery();

            return await ExecuteDocumentQuery(consultantsWithSkillQueryable, "QueryConsultantInfoBySkill");
        }

        public async Task<IEnumerable<ConsultantInfo>> QueryConsultantInfoByInterest(string interestSearchterm)
        {
            var consultantsInterestedIndQueryable = _client.CreateDocumentQuery<ConsultantInfo>(_collectionUri)
                .Where(consultantData => consultantData.Interests.Contains(interestSearchterm))
                .AsDocumentQuery();

            return await ExecuteDocumentQuery(consultantsInterestedIndQueryable, "QueryConsultantInfoByInterest");
        }
    }

    public interface IConsultantInfoRepository
    {
        Task<IEnumerable<ConsultantInfo>> GetAllConsultantInfo();
        Task<IEnumerable<ConsultantInfo>> QueryConsultantInfoBySkill(string skillSearchTerm);
        Task<IEnumerable<ConsultantInfo>> QueryConsultantInfoByInterest(string skillSearchTerm);
    }
}
