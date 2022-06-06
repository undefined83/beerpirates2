using System.Collections.Generic;
namespace Beerpirates.Functions {
    public class RecommendationModel
    {
        public int Id {get; set;}
        public string ProdcutName {get; set;}
        public string ProdcutBrand {get; set;}
        public string ProcutImageUri {get; set;}        
        public string ProcutCategory {get; set;}
        public List<string> ProductTags {get; set;}
        public decimal ProductPrice {get; set;}
        public string ProductDetails {get; set;}

    }
}