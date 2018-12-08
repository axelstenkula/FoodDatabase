using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FoodDatabase.Models;
using Api;

namespace FoodDatabase.Controllers
{

    [Route("api/food")]
    public class FoodController : Controller
    {
        protected IFoodService Service;

        public FoodController(IFoodService service)
        {
            Service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var res = await this.Service.GetAllAsync();
            // var blogs = SampleData.Blogs();
            string a = "aa";
// return NoContent();
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FoodViewModel viewModel) {
            var dto = this.DtoFromViewModel(viewModel);

            var result = await this.Service.CreateAsync(dto);
            if (result.IsError())
            {
                return result.Error.ToActionResult();
            }

            return Created("", result.Value);
        }

        protected FoodDto DtoFromViewModel(FoodViewModel viewmodel)
        {
            return viewmodel.ToDto();
        }
    }

    public static class IErrorExtensions
    {
        public static ActionResult ToActionResult(this IError error)
        {
            if (error == null)
            {
                return new NoContentResult();
            }

            ApiError apiError = ApiError.NewFromIError(error);

            ObjectResult result = new ObjectResult(apiError)
            {
                StatusCode = (int)apiError.HttpStatusCode
            };

            return result;
        }
    }
}   