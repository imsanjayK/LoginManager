using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Net;
using Microsoft.Azure.WebJobs.Host;

namespace LoginManagerApp.Service
{
    public class CosmoDBConnection
    {
        private  string endpointUrl = ConfigurationManager.AppSettings["EndpointUrl"];
        private  string primaryKey = ConfigurationManager.AppSettings["PrimaryKey"];
        private string dataBase = ConfigurationManager.AppSettings["DataBase"];
        //private string collection = ConfigurationManager.AppSettings["Collection"];
        public string collection { get; set; }

        private DocumentClient client;

        public CosmoDBConnection()
        {
            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
        }

        public async Task CreateDatabase(string db_id)
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = db_id });
        }

        public async Task CreateCollection(string collection_id)
        {
            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(dataBase), new DocumentCollection { Id = collection_id });
        }
        public async Task CreateDocumentIfNotExists(UserAccount profile,string collection_Id, TraceWriter log)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(GetDataBase(),collection_Id, profile.log_Id));
                log.Error($"Login details with {profile.log_Id} exist, can't create document.");
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(GetDataBase(), collection_Id), profile);
                  log.Info($"Created login details {profile.log_Id}");
                }
                else
                {
                    throw;
                }
            }
        }

        public DocumentClient GetClient()
        {
            return client;
        }

        public string GetDataBase()
        {
            return dataBase;
        }

        public string GetCollection()
        {
            return collection;
        }
    }
}
