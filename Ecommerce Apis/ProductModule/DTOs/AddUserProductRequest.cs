﻿using Ecommerce_Apis.ProductModule.Models;

namespace Ecommerce_Apis.ProductModule.DTOs
{
    public class AddUserProductRequest
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string ProductURL { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
