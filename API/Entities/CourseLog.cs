using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class CourseLog
    {
        public int Id { get; set; }
        public DateTime InclusionDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
        
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}