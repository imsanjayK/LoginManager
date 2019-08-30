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
using Newtonsoft.Json.Linq;
using System;

namespace LoginManagerApp.ApiFunction
{
    public static class ApiFunction
    {
        [FunctionName("CreateDatabase")]
        public static async Task<HttpResponseMessage> CreateDatabase([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Database/{db_id}")]HttpRequestMessage req, TraceWriter log, string db_id)
        {
            log.Info("CreateDatabase method started");
            try
            {
                CosmoDBConnection dBConnection = new CosmoDBConnection();
                await dBConnection.CreateDatabase(db_id);
                var database = new JObject();
                database.Add("Database", db_id);
                return Response(database);
            }
            catch (WebException wex)
            {
                return req.CreateResponse(HttpStatusCode.OK, "Database creation fail due to " + wex);
            }
        }

        [FunctionName("CreateCollection")]
        public static async Task<HttpResponseMessage> CreateCollection([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{db_id}/Collection/{collection_id}")]HttpRequestMessage req, TraceWriter log, string db_id, string collection_id)
        {
            log.Info("CreateCollection method started");
            try
            {
                CosmoDBConnection dBConnection = new CosmoDBConnection();
                await dBConnection.CreateCollection(db_id, collection_id);
                var collection = new JObject();
                collection.Add("Database", db_id);
                collection.Add("Collection", collection_id);
                return Response(collection);

            }
            catch (WebException wex)
            {
                return req.CreateResponse(HttpStatusCode.OK, "Collection creation fail due to " + wex);
            }
        }

        [FunctionName("CreateDocument")]
        public static async Task<HttpResponseMessage> CreateDocument([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{db_id}/User/{collection_id}")]HttpRequestMessage req, TraceWriter log, string db_id, string collection_id)
        {
            log.Info("--------------------------------------------------------------");
            log.Info("CreateDocument method started");
            System.Console.WriteLine();
            var profile = JsonConvert.DeserializeObject<UserAccount>(req.Content.ReadAsStringAsync().Result);

            CosmoDBConnection dBConnection = new CosmoDBConnection();

            var UserId = await dBConnection.CreateDocumentIfNotExists(profile, db_id, collection_id, log);

            log.Info("CreateDocument method Ended");
            log.Info("--------------------------------------------------------------");
            if (Guid.TryParse(UserId, out Guid userId))
            {
                var collection = new JObject();
                collection.Add("Database", db_id);
                collection.Add("User", collection_id);
                collection.Add("Document id", userId);
                return Response(collection);
            }
            var response = new JObject();
            response.Add("StatusCode", 409);
            response.Add("Error", "Document Exist");
            return Response(response);
        }

        [FunctionName("GetUserDocument")]
        public static  Task<HttpResponseMessage> GetUserDocument([HttpTrigger(AuthorizationLevel.Function, "get", Route = "{db_id}/User/{collection_id}")]HttpRequestMessage req, TraceWriter log, string db_id, string collection_id)
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
            IQueryable<UserAccount> deviceQuery =  client.CreateDocumentQuery<UserAccount>(UriFactory.CreateDocumentCollectionUri(db_id, collection_id));

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

        private static HttpResponseMessage Response(JObject response) => new HttpResponseMessage()
        {
            Content = new StringContent(JsonConvert.SerializeObject(response))
        };



    }
}
