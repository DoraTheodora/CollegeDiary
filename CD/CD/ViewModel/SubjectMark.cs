
using System.Collections.Generic;
using CD.Models;

namespace CD.ViewModel
{
    public class SubjectMark
    {
        public Subject subjectL { get; set; }
        public List<Mark> MarkListL { get; set; }
        public SubjectMark(Subject s, List<Mark> markList)
        {
            this.subjectL = s;
            this.MarkListL = markList;
        }
    }
}



