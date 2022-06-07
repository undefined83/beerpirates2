using System.Collections.Generic;
namespace Beerpirates.Functions {
    public class RecommendationModel
    {
        public int Id {get; set;}
        public string ProductName {get; set;}
        public string ProductBrand {get; set;}
        public string ProductImageUri {get; set;}        
        public string ProductCategory {get; set;}
        public List<string> ProductTags {get; set;}
        public decimal ProductPrice {get; set;}
        public string ProductDetails {get; set;}

    }
}