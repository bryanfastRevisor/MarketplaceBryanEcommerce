﻿namespace MarketplaceBryanEcommerce.Client.Services.ProductService
{
    public interface IProductService
    {

        event Action ProductsChanged;
        string Message { get; set; }
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        string LastSearch { get; set; }
        List<Product> Products { get; set; }
        Task GetProducts(string? categoryUrl=null);
        Task <ServiceResponse<Product>> GetProduct(int productId);
        Task SearchProducts(string searchText,int page);
        Task<List<string>> GetSearchProductSuggestion(string searchText);

    }
}
