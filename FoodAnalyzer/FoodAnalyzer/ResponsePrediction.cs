namespace FoodAnalyzer
{
    public class ResponsePrediction
    {
        public string TagName { get; set; } = string.Empty;
        public string TagId { get; set; } = string.Empty;
        public double Probability { get; set; }
    }
}
