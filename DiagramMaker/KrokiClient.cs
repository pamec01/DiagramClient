using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiagramClient
{
    public class KrokiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://kroki.io/nomnoml/svg";

        public KrokiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateDiagramAsync(string diagramSource)
        {

            string normalizedDiagramSource = diagramSource.Replace("\r\n", "\n").Replace("\r", "\n");

            // Vytvoření objektu JSON, který obsahuje diagram_source
            var jsonPayload = new
            {
                diagram_source = normalizedDiagramSource
            };

            string jsonString = JsonSerializer.Serialize(jsonPayload);
            

            Console.WriteLine("JSON payload:");
            Console.WriteLine(jsonString);

            // Nastavení obsahu požadavku jako JSON string s hlavičkou application/json
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_baseUrl, content);

                response.EnsureSuccessStatusCode();

                string diagramResult = await response.Content.ReadAsStringAsync();
                return diagramResult;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Status Code: {(int)ex.StatusCode}");
                throw;
            }
        }
    }
}
