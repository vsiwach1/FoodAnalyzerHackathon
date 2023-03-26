using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FoodAnalyzer
{
    public class FoodNutrition
    {
        public string? Food_Name { get; set; } = string.Empty;
        [Display(Name = "Total Calories")]
        public double Nf_Calories { get; set; }
        [Display(Name = "Total Fat")]
        public double Nf_Total_Fat { get; set; }
        [Display(Name = "Saturated Fat")]
        public double Nf_Saturated_Fat { get; set; }
        [Display(Name = "Cholesterol")]
        public double Nf_Cholesterol { get; set; }
        [Display(Name = "Sodium")]
        public double Nf_Sodium { get; set; }
        [Display(Name = "Carbohydrate")]
        public double Nf_Total_Carbohydrate { get; set; }
        [Display(Name = "Fiber")]
        public double Nf_Dietary_Fiber { get; set; }
        [Display(Name = "Sugar")]
        public double Nf_Sugars { get; set; }
        [Display(Name = "Protein")]
        public double Nf_Protein { get; set; }
        [Display(Name = "Potassium")]
        public double Nf_Potassium { get; set; }

    }
}
