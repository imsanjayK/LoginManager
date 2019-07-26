using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LoginManagerApp.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Documents.Client;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoginManagerApp.ApiFunction
{
    public static class ApiFunction
    {
        [FunctionName("CreateDatabase")]
        public static async Task<HttpResponseMessage> CreateDatabase([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Database/{db_id}")]HttpRequestMessage req, TraceWriter log, string db_id)
        {
            log.Info("CreateDatabase method started");
            CosmoDBConnection dBConnection = new CosmoDBConnection();
            await dBConnection.CreateDatabase(db_id);
            return req.CreateResponse(HttpStatusCode.OK, "Database created");
        }

        [FunctionName("CreateCollection")]
        public static async Task<HttpResponseMessage> CreateCollection([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Collection/{collection_id}")]HttpRequestMessage req, TraceWriter log,string collection_id)
        {
            log.Info("CreateCollection method started");
            CosmoDBConnection dBConnection = new CosmoDBConnection();
            await dBConnection.CreateCollection(collection_id);
            return req.CreateResponse(HttpStatusCode.OK, "Collection created");
        }

        [FunctionName("CreateDocument")]
        public static async Task<HttpResponseMessage> CreateDocument([HttpTrigger(AuthorizationLevel.Function,"post", Route = "User/{collection_id}")]HttpRequestMessage req, TraceWriter log, string collection_id)
        {
            log.Info("--------------------------------------------------------------");
            log.Info("CreateDocument method started");
            System.Console.WriteLine();
            var profile = JsonConvert.DeserializeObject<UserAccount>(req.Content.ReadAsStringAsync().Result);
           
            CosmoDBConnection dBConnection = new CosmoDBConnection();
            
            await dBConnection.CreateDocumentIfNotExists(profile, collection_id, log);

            log.Info("CreateDocument method Ended");
            log.Info("--------------------------------------------------------------");
           
            return req.CreateResponse(HttpStatusCode.OK, "Collection creted");
        }

        [FunctionName("Document")]
        public static async Task<HttpResponseMessage> Document([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Document/read")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            //string name = req.GetQueryNameValuePairs()
            //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            //    .Value;

            //if (name == null)
            //{
            //    // Get request body
            //    dynamic data = await req.Content.ReadAsAsync<object>();
            //    name = data?.name;
            //}
            //return name == null
            //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);

            CosmoDBConnection dBConnection = new CosmoDBConnection();
            DocumentClient client = dBConnection.GetClient();
            IQueryable<UserAccount> deviceQuery = client.CreateDocumentQuery<UserAccount>(UriFactory.CreateDocumentCollectionUri(dBConnection.GetDataBase(), dBConnection.GetCollection()));

            List<UserAccount> list = new List<UserAccount>();

            foreach (var device in deviceQuery)
            {
                list.Add(device);
            }


            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8, "application/json")
            };
        }

        //[FunctionName("fun")]
        //public static async Task<HttpResponseMessage> fun([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        //{
        //    log.Info("C# HTTP trigger function processed a request.");

        //    parse query parameter
        //    string name = req.GetQueryNameValuePairs()
        //        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        //        .Value;

        //    if (name == null)
        //    {
        //        Get request body
        //       dynamic data = await req.Content.ReadAsAsync<object>();
        //        name = data?.fck;
        //    }
        //    return name == null
        //        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        //        : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);


        //}
    }
}
