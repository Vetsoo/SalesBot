using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace SalesBot.Azure.CosmosDb
{
    public abstract class CosmosDbRepository<T>
    {
        protected readonly DocumentClient _client;
        protected Database _database = null;
        protected string _collectionName = string.Empty;
        protected Uri _collectionUri = null;
        protected DocumentCollection _collection = null;

        public CosmosDbRepository(ICosmosDbSettings documentDbSettings)
        {
            if(documentDbSettings == null) throw new ArgumentNullException("documentDbSettings should not be null");
            _client = new DocumentClient(new Uri(documentDbSettings.EndpointUrl), documentDbSettings.AuthorizationKey);

            _client.OpenAsync();
            InitializeAsync(documentDbSettings).Wait();
        }

        public async Task InitializeAsync(ICosmosDbSettings documentDbSettings)
        {
            if(documentDbSettings.CollectionName == null) throw new ArgumentNullException("documentDbSettings.CollectionName should not be null");
            _collectionName = documentDbSettings.CollectionName;
            if(documentDbSettings.DatabaseName == null) throw new ArgumentNullException("documentDbSettings.DatabaseName should not be null");
            if(_client == null) throw new ArgumentNullException("DocumentClient should be initialized");

            _database = _client.CreateDatabaseIfNotExistsAsync(new Database { Id = documentDbSettings.DatabaseName }).Result;
            if (_collection == null) await GetOrCreateCollectionAsync();

            _collectionUri = UriFactory.CreateDocumentCollectionUri(documentDbSettings.DatabaseName, documentDbSettings.CollectionName);
        }

        protected async Task<IEnumerable<U>> ExecuteDocumentQuery<U>(IDocumentQuery<U> query, string source)
        {
            double requestCharge = 0.0;
            var result = Enumerable.Empty<U>().ToList();
            while (query.HasMoreResults)
            {
                var nextResponse = await query.ExecuteNextAsync<U>();
                requestCharge += nextResponse.RequestCharge;
                result.AddRange(nextResponse);
            }

            return result;
        }

        private async Task<DocumentCollection> GetOrCreateCollectionAsync()
        {
            _collection = _client.CreateDocumentCollectionQuery(_database.SelfLink).Where(c => c.Id == _collectionName).ToArray().FirstOrDefault();

            if (_collection == null)
            {
                _collection = new DocumentCollection { Id = _collectionName };

                _collection = await _client.CreateDocumentCollectionAsync(_database.SelfLink, _collection);
            }

            return _collection;
        }

        public async Task<int> CountAsync()
        {
            return await _client.CreateDocumentQuery<T>(_collectionUri).CountAsync();
        }

        public int CountAsync(Expression<Func<T, bool>> predicate)
        {
            return _client.CreateDocumentQuery<T>(_collectionUri).Where(predicate).Count();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            double requestCharge = 0.0;
            var result = Enumerable.Empty<T>().ToList();

            var queryable = _client.CreateDocumentQuery<T>(_collectionUri).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                var nextResponse = await queryable.ExecuteNextAsync<T>();
                requestCharge += nextResponse.RequestCharge;
                result.AddRange(nextResponse);
            }

            return result;
        }


        public T FirstOrDefaultAsync(Func<T, bool> predicate)
        {
            return
                _client.CreateDocumentQuery<T>(_collectionUri)
                    .Where(predicate)
                    .AsEnumerable()
                    .FirstOrDefault();
        }

        public async Task<T> AddOrUpdateAsync(T entity, RequestOptions requestOptions = null)
        {
            T upsertedEntity;

            var upsertedDoc = await _client.UpsertDocumentAsync(_collectionUri, entity, requestOptions);
            upsertedEntity = JsonConvert.DeserializeObject<T>(upsertedDoc.Resource.ToString());

            return upsertedEntity;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var queryable = _client.CreateDocumentQuery<Document>(_collectionUri).Where(d => d.Id.ToUpper() == id.ToUpper()).AsDocumentQuery();
            var feedResponse = await queryable.ExecuteNextAsync<T>();

            return feedResponse.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate, string partitionKey = "")
        {
            FeedOptions feedOptions = null;
            if (!string.IsNullOrEmpty(partitionKey)) feedOptions = new FeedOptions { PartitionKey = new PartitionKey(partitionKey) };

            var queryable = _client.CreateDocumentQuery<T>(_collectionUri, feedOptions).Where(predicate).AsDocumentQuery();

            return await ExecuteDocumentQuery(queryable, "Where");
        }

        public async Task<bool> RemoveAsync(string id, RequestOptions requestOptions = null)
        {
            bool isSuccess = false;

            var doc = _client.CreateDocumentQuery<Document>(_collectionUri).Where(d => d.Id == id.ToString()).AsEnumerable().FirstOrDefault();

            if (doc != null)
            {
                var result = await _client.DeleteDocumentAsync(doc.SelfLink, requestOptions);
                isSuccess = result.StatusCode == HttpStatusCode.NoContent;
            }

            return isSuccess;
        }

    }
}