using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Module1.Data;
using Module1.Models;

namespace Module1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Product2Controller : ControllerBase
    {
        private ProductDbContext productDbContext;

        public Product2Controller(ProductDbContext _productDbContext)
        {
            productDbContext = _productDbContext;
        }

        // GET: api/Product2
        // 1. sorting: sortPrice
        // 2. paging: pageNumber, pageSize
        // 3. searching: searchProduct
        [HttpGet]
        public IEnumerable<Product> Get(int? pageNumber, int? pageSize, string sortPrice, string searchProduct)
        {
            IQueryable<Product> products;
            
            // searching
            products = productDbContext.Products.Where(x => x.ProductName.StartsWith(searchProduct));

            // sorting
            switch(sortPrice)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                //default:
                //    products = productDbContext.Products;
                //    break;

            }

            // paging
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 3;
            var items = products.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).ToList();

            return items;
        }

        // GET: api/Product2/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var product = productDbContext.Products.SingleOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return NotFound("No record found...");
            }
            return Ok(product);
        }

        // POST: api/Product2
        [HttpPost]
        public IActionResult Post([FromBody] Product value)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            productDbContext.Products.Add(value);
            productDbContext.SaveChanges(true);

            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT: api/Product2/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id != value.ProductId)
            {
                return BadRequest();
            }

            productDbContext.Products.Update(value);
            productDbContext.SaveChanges(true);

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = productDbContext.Products.SingleOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return NotFound("No record found...");
            }
            productDbContext.Products.Remove(product);
            productDbContext.SaveChanges(true);
            return Ok("Product Deleted....");
        }
    }
}
