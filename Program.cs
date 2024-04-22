using System.Net;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // Elasticsearch endpoint and index name
        string elasticsearchEndpoint = "http://localhost:9200";
        string indexName = "index1";


        Console.Write("Search in Elasticsearch index: ");
        string search = Console.ReadLine();


        string queryJson = $@"
                {{
                    ""query"": {{
                        ""match"": {{
                            ""name"": {{
                                ""query"": ""{search}"",
                                ""fuzziness"": ""AUTO""
                            }}
                        }}
                    }}
                }}";




        // Set up the request
        var request = WebRequest.Create($"{elasticsearchEndpoint}/{indexName}/_search");
        request.Method = "POST";
        request.ContentType = "application/json";

        // Write the query JSON to the request body
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(queryJson);
            streamWriter.Flush();
            streamWriter.Close();
        }

        // Send the request and get the response
        var response = request.GetResponse();
        var streamReader = new StreamReader(response.GetResponseStream());

        var result = streamReader.ReadToEnd();
        var resultObject = JsonDocument.Parse(result);
        var hitsArray = resultObject.RootElement.GetProperty("hits").GetProperty("hits");


        foreach (var hit in hitsArray.EnumerateArray())
        {
            var source = hit.GetProperty("_source");
            var id = source.GetProperty("id").GetString();
            var name = source.GetProperty("name").GetString();
            var description = source.GetProperty("description").GetString();


            Console.WriteLine("| Id | Name | Description |");
            Console.WriteLine($"| {id} | {name} | {description} |");

        }


    }
}
