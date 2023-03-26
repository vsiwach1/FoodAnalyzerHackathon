using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace FoodAnalyzer.Pages
{
    public class AnalyzeModel : PageModel
    {
        [Display(Name = "File")]
        public IFormFile? FormFile { get; set; }
        private HttpClient _httpClientAnalyzer;
        private HttpClient _httpClientDetector;
        private HttpClient _httpClientNutrition;
        public string? FoodType { get; set; } = string.Empty;
        public string? DetectedFood { get; set; } = string.Empty;
        public dynamic UploadedFile { get; set; } = string.Empty;
        public FoodNutrition FoodNutrition { get; set; } = new();
        public AnalyzeModel(IHttpClientFactory factory)
        {

            _httpClientAnalyzer = factory.CreateClient("FoodAnalyzer");
            _httpClientDetector = factory.CreateClient("FoodDectector");
            _httpClientNutrition = factory.CreateClient("FoodNutrition");
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            await ProcessImageUsingHttp();
            return Page();
        }
        private async Task ProcessImageUsingHttp()
        {
            try
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new MemoryStream())
                {
                    if (FormFile == null)
                    {
                        ModelState.AddModelError("FormFile", "Please upload file");
                        return;
                    }
                    await FormFile.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var byteData = stream.ToArray();
                    string base64Data = Convert.ToBase64String(byteData);
                    UploadedFile = string.Format("data:image/png;base64,{0}", base64Data);
                    stream.Seek(0, SeekOrigin.Begin);
                    var predictionResult = await GetApiResponse(_httpClientAnalyzer, stream);
                    FoodType = predictionResult?.Predictions?.OrderByDescending(x => x.Probability)?.FirstOrDefault()?.TagName;

                    if (FoodType == "Red")
                        return;
                    stream.Seek(0, SeekOrigin.Begin);
                    predictionResult = await GetApiResponse(_httpClientDetector, stream);
                    DetectedFood = predictionResult?.Predictions?.OrderByDescending(x => x.Probability)?.FirstOrDefault()?.TagName;
                    if (string.IsNullOrEmpty(DetectedFood))
                        return;
                    var nutritionData = await GetNutritions(DetectedFood);
                    FoodNutrition = nutritionData?.Foods.FirstOrDefault(x => DetectedFood.Equals(x.Food_Name, StringComparison.OrdinalIgnoreCase)) ?? new();
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FormFile", "Some error occured");
            }


        }

        private async Task<NutritionAPIResponse?> GetNutritions(string foodName)
        {
            var requestContent = new NutritionApiInputData() { 
                query = foodName, timezone = "US/Eastern"
            };            
            var response = await _httpClientNutrition.PostAsJsonAsync("", requestContent);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStreamAsync();
            var predictionResult = await JsonSerializer.DeserializeAsync<NutritionAPIResponse>(responseContent, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            return predictionResult;
        }

        private async Task<APIResponse?> GetApiResponse(HttpClient client, MemoryStream stream)
        {
            var requestContent = new StreamContent(stream);
            var requestMessage = new HttpRequestMessage()
            {
                Content = requestContent,
                Method = HttpMethod.Post
            };
            requestMessage.Content.Headers.Add("Content-Type", "application/octet-stream");
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var predictionResult = await JsonSerializer.DeserializeAsync<APIResponse>(content, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            return predictionResult;
        }
    }
}
