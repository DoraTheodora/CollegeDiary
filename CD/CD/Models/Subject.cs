using System;
using CD.ViewModel;

namespace CD.Models
{
    public class Subject 
    {
        public Guid SubjectID { get; set; }
        public String SubjectName { get; set; }
        public String LecturerName { get; set; }
        public String LecturerEmail { get; set; }
        public int CA { get; set; }
        public int FinalExam { get; set; }
        public double TotalCA { get; set; }
        public double TotalFinalExam { get; set; }
    }
}
