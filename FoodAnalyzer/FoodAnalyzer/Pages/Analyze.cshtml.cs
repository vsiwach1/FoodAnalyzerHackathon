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

namespace FoodAnalyzer.Pages
{
    public class AnalyzeModel : PageModel
    {
        [Display(Name = "File")]
        public IFormFile? FormFile { get; set; }
        private HttpClient _httpClient;
        public string FoodType { get; set; } = string.Empty;
        public dynamic UploadedFile { get; set; } = string.Empty;
        public AnalyzeModel(IHttpClientFactory factory)
        {
           
            _httpClient = factory.CreateClient("FoodAnalyzer");
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
                    string imreBase64Data = Convert.ToBase64String(byteData);
                    UploadedFile = string.Format("data:image/png;base64,{0}", imreBase64Data);
                    stream.Seek(0, SeekOrigin.Begin);
                    var requestContent = new StreamContent(stream);
                    var requestMessage = new HttpRequestMessage()
                    {
                        Content = requestContent,
                        Method = HttpMethod.Post
                    };
                    requestMessage.Content.Headers.Add("Content-Type", "application/octet-stream");
                    var response = await _httpClient.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStreamAsync();
                    var predictionResult = await JsonSerializer.DeserializeAsync<APIResponse>(content, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    FoodType = predictionResult?.Predictions?.OrderByDescending(x => x.Probability)?.FirstOrDefault()?.TagName;
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("FormFile", "Some error occured");
            }


        }
    }
}
