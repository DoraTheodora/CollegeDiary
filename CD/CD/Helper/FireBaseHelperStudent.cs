using CD.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CD.Helper
{
	class FireBaseHelperStudent
	{
		private readonly string Student_Name = "Student";
		private readonly string UserUID = App.UserUID;
		readonly FirebaseClient firebase = new FirebaseClient(App.conf.firebase);
		readonly FireBaseHelperSubject fireBaseHelperSubject = new FireBaseHelperSubject();

		public async Task<List<Student>> GetAllStudents()
		{
			return (await firebase.Child(UserUID).Child(Student_Name).OnceAsync<Student>()).Select(item => new Student
			{
				StudentName = item.Object.StudentName,
				StudentID = item.Object.StudentID,
				StudentEmail = item.Object.StudentEmail,
				Institute = item.Object.Institute,
				FinalGPA = item.Object.FinalGPA,
			}).ToList();
		}

		public async Task AddStudent(string UID, string studentName, string UC, string studentEmail)
		{
			await firebase.Child(UID).Child(Student_Name).PostAsync(new Student()
			{
				StudentID = UID,
				StudentName = studentName,
				StudentEmail = studentEmail,
				Institute = UC,
				FinalGPA = 0,
			}); 
		}

		public async Task<Student> GetStudent(string studentID)
		{
			var allStudents = await GetAllStudents();
			await firebase.Child(UserUID).Child(Student_Name).OnceAsync<Student>();
			return allStudents.FirstOrDefault(a => a.StudentID == studentID);
		}

		public async Task DeleteStudent(string studentID)
		{
			var toDeleteStudent = (await firebase.Child(UserUID).Child(Student_Name).OnceAsync<Student>()).FirstOrDefault
				(a => a.Object.StudentID == studentID);
			await firebase.Child(UserUID).Child(Student_Name).Child(toDeleteStudent.Key).DeleteAsync();
			await firebase.Child(UserUID).DeleteAsync();
		}

		public async Task AddGPA(string StudentID)
		{
			var subjects = await fireBaseHelperSubject.GetAllSubjects();
			double finalGPA = 0;
			if (subjects.Count != 0)
			{
				foreach (Subject sub in subjects)
				{
					finalGPA += sub.TotalCA + sub.TotalFinalExam;
				}
				finalGPA = finalGPA / subjects.Count;
				var userToUpdate = (await firebase.Child(UserUID).Child(Student_Name)
				.OnceAsync<Student>())
				.FirstOrDefault
				(a => a.Object.StudentID == StudentID);
				await firebase.Child(UserUID).Child(Student_Name).Child(userToUpdate.Key).Child("FinalGPA").PutAsync(finalGPA);
			}
			else if (subjects.Count == 0)
			{
				var userToUpdate = (await firebase.Child(UserUID).Child(Student_Name)
				.OnceAsync<Student>())
				.FirstOrDefault
				(a => a.Object.StudentID == StudentID);
				await firebase.Child(UserUID).Child(Student_Name).Child(userToUpdate.Key).Child("FinalGPA").PutAsync(finalGPA);
			}
		}

		public async Task UpdateAccount(string userID, String userName, String institute)
		{
			var toUpdateSubject = (await firebase.Child(UserUID).Child(Student_Name)
				.OnceAsync<Student>())
				.FirstOrDefault(a => a.Object.StudentID == userID);
			await firebase.Child(UserUID).Child(Student_Name).Child(toUpdateSubject.Key)
				.PutAsync(new Student()
				{
					StudentID = toUpdateSubject.Object.StudentID,
					StudentName = userName,
					StudentEmail = toUpdateSubject.Object.StudentEmail,
					Institute = institute,
					FinalGPA = toUpdateSubject.Object.FinalGPA,
				});
		}
	}
}
