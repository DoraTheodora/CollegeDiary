using System.Collections.Generic;
using CD.Models;
using CD.Models.Calendar;

namespace CD.ViewModel
{
    public class StudentCalendar
    {
        public Student studentSelected { get; set; }
        public List<EventModel> studentEvents {get; set;}

        public StudentCalendar(Student s, List<EventModel> studentEventss)
        {
            this.studentSelected = s;
            this.studentEvents = studentEventss;
        }
        
    }
}
