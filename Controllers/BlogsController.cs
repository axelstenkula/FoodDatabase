using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FoodDatabase.Models;

namespace FoodDatabase.Controllers
{
    [Route("api/test")]
    public class BlogsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            // var blogs = SampleData.Blogs();
return NoContent();
            // return Ok(blogs);
        }
    }
}   