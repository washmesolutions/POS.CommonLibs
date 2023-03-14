using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POS.Product.Model.Product;
using POS.Product.Service.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IProductDetails _productDetails;
        public ProductController(ILoggerFactory logger, IProductDetails productDetails)
        {
            _loggerFactory = logger;
            _logger = logger.CreateLogger<ProductController>();
            _productDetails = productDetails;
        }

        [Route("[action]")]
        [ActionName("GetProductDetails")]
        [HttpGet]
        [Authorize]
        public async Task<List<ProductDetailModel>> GetProductDetails()
        {
            return  _productDetails.GetProductDetails();
        }
    }
}
