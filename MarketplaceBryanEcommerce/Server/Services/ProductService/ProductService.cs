﻿namespace MarketplaceBryanEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                .Where(p => p.Featured)
                .Include(p => p.Variants)
                .ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response=new ServiceResponse<Product>();
            var product = await _context.Products
                .Include(p => p.Variants)
                .ThenInclude(v => v.ProductType)
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                response.Success = false;
                response.Message = "Lo Sentimos, pero el producto no existe.";
            }
            else
            {
                response.Data = product;
            }
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductListAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products.Include(p=>p.Variants).ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                .Where(p => p.Category!.Url.ToLower().Equals(categoryUrl.ToLower()))
                .Include(p=>p.Variants)
                .ToListAsync()
            };
            return response;
        }
        // Implementacion de Busquedas
        public async Task<ServiceResponse<List<string>>> GetSearchProductSuggestion(string searchText)
        {
            var response = await SearchProductsFunction(searchText);
            List<string> result = new List<string>();
            foreach(var product in response)
            {
                    if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(product.Title);
                    }
                if (product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                        .Distinct().ToArray();
                    var words = product.Description.Split()
                        .Select(s => s.Trim(punctuation));
                    foreach(var word in words)
                    {
                        if(word.Contains(searchText,StringComparison.OrdinalIgnoreCase)&& !result.Contains(word)){
                            result.Add(word);
                        }
                    }
                }
            }
            return new ServiceResponse<List<String>> { Data=result };
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText,int page)
        {
            var pageResults = 2f;
            var pageCount = Math.Ceiling((await SearchProductsFunction(searchText)).Count/pageResults);
            var products= await _context.Products
                    .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                    ||
                    p.Description.ToLower().Contains(searchText.ToLower()))
                    .Include(p => p.Variants)
                    .Skip((page-1)*(int)pageResults)
                    .Take((int)pageResults)
                    .ToListAsync();
            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage=page,
                    Pages=(int)pageCount
                }
            };
            return response;
        }

        private async Task<List<Product>> SearchProductsFunction(string searchText)
        {
            return await _context.Products
                    .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                    ||
                    p.Description.ToLower().Contains(searchText.ToLower()))
                    .Include(p => p.Variants)
                    .ToListAsync();
        }
    }
}
