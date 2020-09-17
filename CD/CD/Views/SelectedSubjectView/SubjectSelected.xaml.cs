using System;
using CD.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CD.Helper;
using CD.ViewModel;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Syncfusion.XForms.ProgressBar;
using CD.Views.ErrorAndEmpty;
using CD.Views.Login;
using System.Threading.Tasks;

namespace CD.Views.SelectedSubjectView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SubjectSelected : ContentPage
    {
        private Subject _subject;
        private SubjectMark subjectMark;
        readonly FireBaseHelperSubject fireBaseHelperSubject = new FireBaseHelperSubject();
        readonly FireBaseHelperMark fireBaseHelperMark = new FireBaseHelperMark();
        bool ready = false;

        public SubjectSelected(Subject subject)
        {
            ready = false;
            _subject = subject;
            InitializeComponent();
            busyindicator.IsVisible = true;
            ready = true;
        }
        protected override async void OnAppearing()
        {
            busyindicator.IsVisible = true;
            if (App.CheckConnection())
            {
                if (await App.userExists(App.UserUID))
                {

                    ready = false;
                    base.OnAppearing();
                    List<Mark> listMarks = new List<Mark>();
                    bool connection = true;
                    try
                    {
                        listMarks = await fireBaseHelperMark.GetMarksForSubject(_subject.SubjectID);
                        await status_bars();
                        double remCA = await fireBaseHelperSubject.remainigCA(_subject.SubjectID);
                        remainingCA.Text = remCA.ToString("F2") + "%";
                        busyindicator.IsVisible = false;
                    }
                    catch (Exception)
                    {
                        connection = false;
                    }
                    if (connection)
                    {
                        subjectMark = new SubjectMark(_subject, listMarks);
                        if (listMarks.Count == 0)
                        {
                            results_text.IsVisible = true;
                        }
                        else
                        {
                            results_text.IsVisible = false;
                        }
                        this.BindingContext = subjectMark; //!!!
                        ready = true;
                    }
                    else
                    {
                        busyindicator.IsVisible = false;
                        await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
                    }

                }
                else
                {
                    busyindicator.IsVisible = true;
                    await DisplayAlert("Something went wrong...", "", "OK");
                    App.Current.MainPage = new NavigationPage(new LogIn());
                }
            }
            else
            {
                busyindicator.IsVisible = true;
                await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
            }
        }
        private async Task status_bars()
        {
            ready = false;
            bool connection = true;
            double CAProgress = 0;
            double FinalExamProgress = 0;
            double GPA = 0;
            try
            {
                Tuple<Double,Double> result = await fireBaseHelperSubject.GetGPA_CA_GPA_FE(_subject.SubjectID);
                CAProgress = result.Item1;
                FinalExamProgress = result.Item2;
                GPA = CAProgress + FinalExamProgress;
            }
            catch (Exception)
            {
                connection = false;
            }
            if (connection)
            {
                statusCA.Progress = CAProgress;
                colorTheStatusBars(CAProgress, statusCA, "CA");
                statusFinalExam.Progress = FinalExamProgress;
                colorTheStatusBars(FinalExamProgress, statusFinalExam, "FE");
                statusSubjectGPA.Progress = GPA;
                colorTheStatusBars(GPA, statusSubjectGPA, "GPA");

                Ca_StatusBar.Text = CAProgress.ToString("F2");
                Fe_StatusBar.Text = FinalExamProgress.ToString("F2");
                gpa_StatusBar.Text = GPA.ToString("F2");
                ready = true;
            }
            else
            {
                busyindicator.IsVisible = false;
                await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
            }
        }

        public void colorTheStatusBars(double process, SfLinearProgressBar bar, string type)
        {
            ready = false;
            double segments = 0;
            double grade = 0;
            if (type == "CA")
            {
                segments = _subject.CA / 3;
                grade = _subject.CA;
            }
            else if (type == "FE")
            {
                segments = _subject.FinalExam / 3;
                grade = _subject.FinalExam;
            }
            else if (type == "GPA")
            {
                segments = 100 / 3;
                grade = 100;
            }

            RangeColorCollection rangeColors = new RangeColorCollection();
            if (type == "CA" || type == "FE")
            {
                if (process <= grade / 3)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#ffcccb"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Red, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if ((process > grade / 3) && (process < grade / 3 * 2))
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#FDE8D3"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Orange, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if (process >= grade / 3 * 2)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#d2f8d2"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Green, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
            }
            else if (type == "GPA")
            {
                if (process < 40)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#ffcccb"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Red, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if ((process >= 40) && (process < 70))
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#FDE8D3"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Orange, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if (process > 70)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#d2f8d2"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#287c37"), IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
            }
            ready = true;
        }

        [Obsolete]
        private async void add_new_mark(object sender, EventArgs e)
        {
            add_ca_button.IsEnabled = false;
            add_fe_button.IsEnabled = false;
            await PopupNavigation.PushAsync(new AddMarkToSubject(_subject));
            add_ca_button.IsEnabled = true;
            add_fe_button.IsEnabled = true;
        }

        [Obsolete]
        private async void add_final_exam(object sender, EventArgs e)
        {
            add_fe_button.IsEnabled = false;
            add_ca_button.IsEnabled = false;
            await PopupNavigation.PushAsync(new AddFinalExamToSubject(_subject));
            add_fe_button.IsEnabled = true;
            add_ca_button.IsEnabled = true;
        }

        [Obsolete]
        private async void edit_subject(object sender, EventArgs e)
        {
            if (!busyindicator.IsVisible)
            {
                await PopupNavigation.PushAsync(new EditDeleteSubject(_subject));
            }
        }

        private async void delete_subject(object sender, EventArgs e)
        {
            bool connection = true;
            if (!busyindicator.IsVisible)
            {
                var result = await DisplayAlert("Are you sure you want to delete this subject?", "Subject name: " + _subject.SubjectName, "Yes", "No");
                
                if (result) // YES
                {
                    busyindicator.IsVisible = true;
                    try
                    {
                        await fireBaseHelperSubject.DeleteSubject(_subject.SubjectID);
                    }
                    catch (Exception)
                    {
                        connection = false;
                    }
                    try
                    {
                        await fireBaseHelperMark.DeleteMarks(_subject.SubjectID);
                    }
                    catch (Exception)
                    {
                        connection = false;
                    }
                    busyindicator.IsVisible = false;
                    if (connection)
                    {
                        await DisplayAlert("Success", _subject.SubjectName + " was deleted", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Something went wrong...", "Please check your interner connection", "OK");
                    }

                }
            }
        }

        [Obsolete]
        private async void delete_mark(object sender, Syncfusion.ListView.XForms.ItemHoldingEventArgs e)
        {
            var thisMark = e.ItemData as Mark;
            await PopupNavigation.PushAsync(new DeleteMark(thisMark, _subject));
        }

        private void tips(object sender, EventArgs e)
        {
            if (!hidenSubjectDetailsHelp.IsVisible)
            {
                hidenSubjectDetailsHelp.IsVisible = true;
                hidenYourResultsHelp.IsVisible = false;
                hidenResultsDetails.IsVisible = false;
            }
            else
            {
                hidenYourResultsHelp.IsVisible = false;
                hidenResultsDetails.IsVisible = false;
                hidenSubjectDetailsHelp.IsVisible = false;
            }

        }

        private void tipsYourResults(object sender, EventArgs e)
        {
            if (!hidenYourResultsHelp.IsVisible)
            {
                hidenYourResultsHelp.IsVisible = true;
                hidenSubjectDetailsHelp.IsVisible = false;
                hidenResultsDetails.IsVisible = false;
            }
            else
            {
                hidenYourResultsHelp.IsVisible = false;
                hidenResultsDetails.IsVisible = false;
                hidenYourResultsHelp.IsVisible = false;
            }
            
        }

        private void moreDetailsExpans(object sender, EventArgs e)
        {
            if (moreSubjectDetails.IsVisible == false)
            {
                moreSubjectDetails.IsVisible = true;
                moreDetails.Text = "↑";
            }
            else if (moreSubjectDetails.IsVisible == true)
            {
                moreSubjectDetails.IsVisible = false;
                moreDetails.Text = "↓";
            }
        }

        [Obsolete]
        private void SendEmail(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(lecturerEmail.Text) || !string.IsNullOrWhiteSpace(lecturerEmail.Text))
            {
                Device.OpenUri(new Uri("mailto:" + lecturerEmail.Text));
            }
        }

        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }

        private void tipsResults(object sender, EventArgs e)
        {
            if (hidenResultsDetails.IsVisible == false)
            {
                hidenResultsDetails.IsVisible = true;
                hidenSubjectDetailsHelp.IsVisible = false;
                hidenYourResultsHelp.IsVisible = false;
            }
            else if (hidenResultsDetails.IsVisible == true)
            {
                hidenResultsDetails.IsVisible = false;
                hidenSubjectDetailsHelp.IsVisible = false;
                hidenYourResultsHelp.IsVisible = false;
            }
        }
    }
}