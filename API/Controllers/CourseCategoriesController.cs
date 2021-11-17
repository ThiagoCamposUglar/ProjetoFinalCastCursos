using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class CourseCategoriesController : BaseApiController
    {
        private readonly DataContext _context;
        public CourseCategoriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseCategory>>> GetCourseCategories()
        {
            return await _context.CourseCategories.ToListAsync();
        }
    }
}