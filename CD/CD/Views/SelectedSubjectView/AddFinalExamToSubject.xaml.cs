using System;
using System.Threading.Tasks;
using CD.Models;
using CD.Helper;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CD.Views.SelectedSubjectView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddFinalExamToSubject
    {
        private Subject _subject;
        readonly FireBaseHelperMark fireBaseHelper = new FireBaseHelperMark();
        public AddFinalExamToSubject(Subject subject)
        {
            _subject = subject;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
        }
        public async Task<string> Check_FinalExam_Weight(Subject subject)
        {
            List<Mark> marks_belonging_to_subject = new List<Mark>();
            bool connection = true;
            try
            {
                marks_belonging_to_subject = await fireBaseHelper.GetMarksForSubject(subject.SubjectID);
            }
            catch (Exception)
            {
                connection = false;
            }
            if (connection)
            {
                foreach (Mark m in marks_belonging_to_subject)
                {
                    if (m.Category.Equals("Final Exam"))
                    {
                        return "existing";
                    }
                }
                return "notExisting";
            }
            return "noConnection";
        }

        [Obsolete]
        private async void Save_Exam(object sender, EventArgs e)
        {
            save_exam_button.IsEnabled = false;
            busyindicator.IsVisible = true;
            bool validate = true;
            bool less = true;
            string existing = "";
            bool connection = true;

            Error1.IsVisible = false;
            Error2.IsVisible = false;

            // chekc if the result entry is not empty
            if (string.IsNullOrEmpty(this.result.Text) || string.IsNullOrWhiteSpace(this.result.Text))
            {
                validate = false;
                Error1.IsVisible = true;
                busyindicator.IsVisible = false;
            }

            // check if the result is not higher than 100
            if (validate)
            {
                try
                {
                    if (Decimal.Parse(this.result.Text) > 100 || Decimal.Parse(this.result.Text) < 0)
                    {
                        Error1.IsVisible = true;
                        busyindicator.IsVisible = false;
                        less = false;
                        validate = false;
                    }
                }
                catch (Exception)
                {
                    Error2.IsVisible = true;
                    busyindicator.IsVisible = false;
                    less = false;
                    validate = false;
                }
            }
            // check if there is already a final exam recorded
            if (validate)
            {
                existing = await Check_FinalExam_Weight(_subject);
                validate = (existing == "notExisting");
            }

            // if the mark is less than 100 and valid
            if (validate && less)
            {
                try
                {
                    double result = Double.Parse(this.result.Text);
                    await fireBaseHelper.AddMark(_subject.SubjectID, "Final Exam", result, _subject.FinalExam, "Final Exam");
                }
                catch (Exception)
                {
                    connection = false;
                }
                if(connection)
                {
                    
                    busyindicator.IsVisible = false;
                    await Navigation.PushAsync(new SubjectSelected(_subject), false);
                }
                else
                {
                    busyindicator.IsVisible = false;
                    await DisplayAlert("Something went wrong...", "Please check your internet connection", "OK");
                }
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                await PopupNavigation.RemovePageAsync(this);

            }
            // if the mark is less than 100 but not valid
            if (existing == "existing")
            {
                busyindicator.IsVisible = false;
                await DisplayAlert("Result not added", "The final exam can be added just once \nIf you need to make changes tap long on the result you want to change and delete it. \nThen you can add a new Final Exam result", "OK");
                await PopupNavigation.RemovePageAsync(this);
            }
            else if(existing == "noConnection")
            {
                busyindicator.IsVisible = false;
                await DisplayAlert("Something went wrong...","Please check your internet connection", "OK");
            }

            save_exam_button.IsEnabled = true;

        }

        [Obsolete]
        private void Cancel_Exam(object sender, EventArgs e)
        {
            PopupNavigation.RemovePageAsync(this);
        }
    }
}