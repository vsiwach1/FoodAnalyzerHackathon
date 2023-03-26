namespace FoodAnalyzer
{
    public class APIResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string Iteration { get; set; } = string.Empty;
        public DateTime Created { get; set; }

        public List<ResponsePrediction> Predictions { get; set; } = new List<ResponsePrediction>();

    }
}
