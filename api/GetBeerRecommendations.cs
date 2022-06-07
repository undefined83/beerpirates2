using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MySqlConnector;

namespace Beerpirates.Functions
{
    public static class GetBeerRecommendations
    {
        [FunctionName("GetBeerRecommendations")]
        public static async Task<IActionResult> Run(
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

                // to be refactored to get from config file
        private static MySqlConnectionStringBuilder GetMySqlConn()
        {
            return new MySqlConnectionStringBuilder
            {
                Server = "beerpirates.mysql.database.azure.com",
                Database = "beerpirates",
                UserID = "agrujic",
                Password = "-Beerpirates1",
                SslMode = MySqlSslMode.None,
            };
        }

                private static async Task<IActionResult> GetDate(ILogger log)
        {

            List<RecommendationModel> reccomendations = new List<RecommendationModel>();
            using (var conn = new MySqlConnection(GetMySqlConn().ConnectionString))
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
                                    ProdcutName = reader["ProductName"].ToString(),
                                    ProdcutBrand = reader["ProductBrand"].ToString(),
                                    ProductPrice = priceDecimal,
                                    ProductDetails = reader["ProductDetails"].ToString()
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
                    return new OkObjectResult(reccomendations);
                }
                else
                {
                    return new NotFoundResult();
                }

            }

        }
    }
}
