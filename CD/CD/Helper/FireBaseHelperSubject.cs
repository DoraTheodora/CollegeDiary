using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using CD.Models;
using CD.Helper;

namespace CD.Helper
{
    class FireBaseHelperSubject
    {
        private readonly string UserUID = App.UserUID;
        private readonly string Subject_Name = "Subjects";
        readonly FirebaseClient firebase = new FirebaseClient(App.conf.firebase);

        readonly FireBaseHelperMark fireBaseHelperMark = new FireBaseHelperMark();

        public async Task<List<Subject>> GetAllSubjects()
        {
            return (await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>()).Select(item => new Subject
            {
                SubjectName = item.Object.SubjectName,
                SubjectID = item.Object.SubjectID,
                LecturerName = item.Object.LecturerName,
                LecturerEmail = item.Object.LecturerEmail,
                CA = item.Object.CA,
                FinalExam = item.Object.FinalExam,
                TotalCA = item.Object.TotalCA,
                TotalFinalExam = item.Object.TotalFinalExam
            }).ToList();
        }
        public async Task AddSubject(string subjectName, string lecturerName, string lecturerEmail, int ca, int finalExam)
        {
            await firebase.Child(UserUID).Child(Subject_Name).PostAsync(new Subject()
            {
                SubjectID = Guid.NewGuid(),
                SubjectName = subjectName,
                LecturerName = lecturerName,
                LecturerEmail = lecturerEmail,
                CA = ca,
                FinalExam = finalExam,
                TotalCA = 0,
                TotalFinalExam = 0,
            }) ; 
        }
        public async Task<Subject> GetSubject(Guid subjectID)
        {
            var allSubjects = await GetAllSubjects();
            await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>();
            return allSubjects.FirstOrDefault(a => a.SubjectID == subjectID);
        }

        public async Task<Subject> GetSubject(string subjectname)
        {
            var allSubjects = await GetAllSubjects();
            await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>();
            return allSubjects.FirstOrDefault(a => a.SubjectName == subjectname);
        }

        public async Task DeleteSubject(Guid subjectID)
        {
            var toDeleteSubject = (await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>()).FirstOrDefault
                (a => a.Object.SubjectID == subjectID);
            await firebase.Child(UserUID).Child(Subject_Name).Child(toDeleteSubject.Key).DeleteAsync();
        }

        //public async Task<Double> getTotalCA(Guid SubjectID)
        //{
        //    var marks_belonging_to_subject = await fireBaseHelperMark.GetMarksForSubject(SubjectID);
        //    double total_CA_all_Marks = 0;
        //    foreach (Mark m in marks_belonging_to_subject)
        //    {
        //        if (m.Category.Equals("Continuous Assessment"))
        //        {
        //            double result = m.Result;
        //            double weight = m.Weight / 100;
        //            total_CA_all_Marks += (weight * result);
        //        }
        //    }

        //    Subject this_subject = await GetSubject(SubjectID);

        //    var subjectToUpdate = (await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>()).FirstOrDefault(a => a.Object.SubjectID == SubjectID);
        //    await firebase.Child(UserUID).Child(Subject_Name).Child(subjectToUpdate.Key).Child("TotalCA").PutAsync(total_CA_all_Marks);

        //    return total_CA_all_Marks;
        //}


        public async Task<Tuple<double, double>> GetGPA_CA_GPA_FE(Guid SubjectID)
        {
            var all_marks_belonging_to_subject = await fireBaseHelperMark.GetMarksForSubject(SubjectID);
            double total_CA_all_Marks = 0;
            double finalExam = 0;
            foreach (Mark m in all_marks_belonging_to_subject)
            {
                if (m.Category.Equals("Continuous Assessment"))
                {
                    double result = m.Result;
                    double weight = m.Weight / 100;
                    total_CA_all_Marks += (weight * result);
                }
                else if (m.Category.Equals("Final Exam"))
                {
                    double result = m.Result;
                    double weight = m.Weight / 100;
                    finalExam = result * weight;
                }
            }
            var subjectToUpdate = (await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>()).FirstOrDefault(a => a.Object.SubjectID == SubjectID);
            await firebase.Child(UserUID).Child(Subject_Name).Child(subjectToUpdate.Key).Child("TotalCA").PutAsync(total_CA_all_Marks);
            await firebase.Child(UserUID).Child(Subject_Name).Child(subjectToUpdate.Key).Child("TotalFinalExam").PutAsync(finalExam);
            return Tuple.Create<double, double>(total_CA_all_Marks, finalExam);
        }


        //public async Task<Double> Final_Exam_Progress(Guid SubjectID)
        //{
        //    var marks_belonging_to_subject = await fireBaseHelperMark.GetMarksForSubject(SubjectID);
        //    double finalExam = 0;
        //    foreach (Mark m in marks_belonging_to_subject)
        //    {
        //        if (m.Category.Equals("Final Exam"))
        //        {
        //            double result = m.Result;
        //            double weight = m.Weight/100;
        //            finalExam = result * weight;
        //        }
        //    }

        //    var subjectToUpdate = (await firebase.Child(UserUID).Child(Subject_Name).OnceAsync<Subject>()).FirstOrDefault(a => a.Object.SubjectID == SubjectID);
        //    await firebase.Child(UserUID).Child(Subject_Name).Child(subjectToUpdate.Key).Child("TotalFinalExam").PutAsync(finalExam);

        //    return finalExam;
        //}

        public async Task UpdateSubject(Guid subjectID, String subjectName, String lecturerName, String lecturerEmail)
        {
            var toUpdateSubject = (await firebase.Child(UserUID).Child(Subject_Name)
                .OnceAsync<Subject>())
                .FirstOrDefault(a => a.Object.SubjectID == subjectID);
            await firebase.Child(UserUID).Child(Subject_Name).Child(toUpdateSubject.Key)
                .PutAsync(new Subject() { SubjectID=subjectID, SubjectName = subjectName, LecturerName = lecturerName, LecturerEmail = lecturerEmail,
                    CA= toUpdateSubject.Object.CA, FinalExam = toUpdateSubject.Object.FinalExam, TotalCA = toUpdateSubject.Object.TotalCA, TotalFinalExam = toUpdateSubject.Object.TotalFinalExam  });
        }

        public async Task<Double> remainigCA(Guid subjectID)
        {
            try
            {
                double result = 0;
                Subject subject = await GetSubject(subjectID);
                var marks_belonging_to_subject = await fireBaseHelperMark.GetMarksForSubject(subjectID);
                foreach (Mark m in marks_belonging_to_subject)
                {
                    if (m.Category.Equals("Continuous Assessment"))
                    {
                        result += m.Weight;
                    }
                }
                double remainingCA = subject.CA - result;
                return remainingCA;
            }
            catch
            {
                return 0;
            }
        }
    }
}
