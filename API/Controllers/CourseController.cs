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
    public class CourseController : BaseApiController
    {
        private readonly DataContext _context;
        public CourseController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<Course>> PostCourse(Course course, int id)
        {
            if(course.StartDate < DateTime.Today)
            {
                return BadRequest("A data de início deve ser maior que a data atual.");
            }
            else if(course.EndDate < course.StartDate)
            {
                return BadRequest("A data de início deve ser menor que a data de término.");
            }
            else if(CourseExistsInTS(course))
            {
                return BadRequest("Existe(m) curso(s) planejados(s) dentro do período informado.");
            }

            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                CourseLog log = new CourseLog
                {
                    InclusionDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now,
                    CourseId = course.Id,
                    EmployeeId = id
                };

                _context.CourseLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {  
                throw;
            }


            return Ok("Curso cadastrado com sucesso");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if(id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            DateTime incDate = DateTime.Now;
            int employeeId = 1;

            foreach (var log in _context.CourseLogs)
            {
                if(log.CourseId == course.Id)
                {
                    incDate = log.InclusionDate;
                    employeeId = log.EmployeeId;
                    break;
                }
            }

            CourseLog newlog = new CourseLog
            {
                InclusionDate = incDate,
                LastUpdateDate = DateTime.Now,
                CourseId = course.Id,
                EmployeeId = employeeId
            };

            _context.CourseLogs.Add(newlog);
            await _context.SaveChangesAsync();

            return Ok("Dados alterados com sucesso.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if(course == null)
            {
                return NotFound();
            }
            else if(course.EndDate <= DateTime.Now)
            {
                return BadRequest("Não é possível excluir cursos já realizados");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok("Curso excluído com sucesso");
        }

        public bool CourseExistsInTS(Course course)
        {
            // var course2 = _context.Courses.Where(x => (x.StartDate <= course.StartDate && x.EndDate <= course.EndDate && x.EndDate >= course.StartDate) ||
            //  (x.StartDate >= course.StartDate && x.StartDate <= course.EndDate) ||
            //   (x.StartDate <= course.StartDate && x.EndDate >= course.EndDate));
            foreach (var course2 in _context.Courses)
            {
                if((course2.StartDate <= course.StartDate && course2.EndDate <= course.EndDate && course2.EndDate >= course.StartDate) || (course2.StartDate >= course.StartDate && course2.StartDate <= course.EndDate) || (course2.StartDate <= course.StartDate && course2.EndDate >= course.EndDate))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}