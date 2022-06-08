using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MySqlConnector;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Linq;

namespace Beerpirates.Functions
{
    public static class GetBeerRecommendations
    {
        [FunctionName("GetBeerRecommendations")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "beerrecommendations")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);
            return await GetDate(log);
        }

        private static async Task<HttpResponseMessage> GetDate(ILogger log)
        {
            List<RecommendationModel> reccomendations = new List<RecommendationModel>();
            using (var conn = new MySqlConnection(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")))
            {
                try
                {
                    Console.WriteLine("Opening connection");
                    await conn.OpenAsync();

                    var query = "SELECT * FROM reccomendations;";

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = query;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var priceString = reader["ProductPrice"].ToString();

                                Decimal.TryParse(priceString, out decimal priceDecimal);

                                var reccomendation = new RecommendationModel()
                                {
                                    //Id = (int)reader["Id"].ToString(),
                                    ProductName = reader["ProductName"].ToString(),
                                    ProductBrand = reader["ProductBrand"].ToString(),
                                    ProductImageUri = reader["ProductImageUri"].ToString(),
                                    ProductCategory = reader["ProductCategory"].ToString(),
                                    ProductTags = (reader["ProductTags"].ToString()).Split(";").ToList<string>(),
                                    ProductPrice = priceDecimal,
                                    ProductDetails = reader["ProductDetails"].ToString(),
                                };

                                reccomendations.Add(reccomendation);
                            }
                        }
                    }

                    Console.WriteLine("Closing connection");
                }

                catch (Exception e)
                {
                    log.LogError(e.ToString());
                }
                if (reccomendations.Count > 0)
                {
                    var jsonToReturn = JsonConvert.SerializeObject(reccomendations);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

            }

        }
    }
}
