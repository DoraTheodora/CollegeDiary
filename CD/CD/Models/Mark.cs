using System;

namespace CD.Models
{
    public class Mark
    {
        public Guid SubjectID { get; set; }
        public Guid MarkID { get; set; }
        public string MarkName { get; set; }
        public double Result { get; set; }
        public string Category { get; set; }
        public double Weight { get; set; }
    }
}
