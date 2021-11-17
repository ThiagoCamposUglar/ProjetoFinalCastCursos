using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? StudentsQuantity { get; set; }

        public int? CourseCategoryId { get; set; }
        public CourseCategory CourseCategory { get; set; }

    }
}