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
        //private string dataBase = ConfigurationManager.AppSettings["DataBase"];
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

        public async Task CreateCollection(string db_id, string collection_id)
        {
            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(db_id), new DocumentCollection { Id = collection_id });
        }

        public async Task<string> CreateDocumentIfNotExists(UserAccount profile,string db_id, string collection_Id, TraceWriter log)
        {
            //try
            //{
            //    await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(db_id,collection_Id, profile.log_Id));
            //    log.Error($"Login details with {profile.log_Id} exist, can't create document.");
            //}
            //catch (DocumentClientException de)
            //{
            //    if (de.StatusCode == HttpStatusCode.NotFound)
            //    {

            IQueryable<UserAccount> deviceQuery = client.CreateDocumentQuery<UserAccount>(UriFactory.CreateDocumentCollectionUri(db_id, collection_Id));

            List<UserAccount> list = new List<UserAccount>();

            foreach (var device in deviceQuery)
            {
                if (profile.Equals(device))
                {
                    log.Error($"Login details with {device.log_Id} exist, can't create document.");
                    return $"Login details with {device.log_Id} exist, can't create document."; 
                }
            }

            profile.log_Id = Guid.NewGuid().ToString();
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(db_id, collection_Id), profile);
                  log.Info($"Created login details {profile.log_Id}");
            return profile.log_Id;
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            //catch (Exception de)
            //{

            //}
        }

        public DocumentClient GetClient()
        {
            return client;
        }

        //public string GetDataBase()
        //{
        //    return dataBase;
        //}

        //public string GetCollection()
        //{
        //    return collection;
        //}
    }
}
