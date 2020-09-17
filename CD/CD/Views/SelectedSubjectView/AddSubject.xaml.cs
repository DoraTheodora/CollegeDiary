using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CD.Helper;
using CD.Models;
using CD.Views;
using System.Text.RegularExpressions;
using CD.Views.ErrorAndEmpty;
using CD.Views.Login;

namespace CD.Views.SelectedSubjectView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddSubject : ContentPage
    {
        readonly FireBaseHelperSubject fireBaseHelper = new FireBaseHelperSubject();

        public AddSubject() // So here is where the code will go for the validation?
        {
            InitializeComponent();// Loading this page
        }

        private async void Save_Subject(object sender, EventArgs e)
        {
            save_subject_button.IsEnabled = false;
            if (App.CheckConnection())
            {
                if (await App.userExists(App.UserUID))
                {
                    busyindicator.IsVisible = true;
                    try
                    {
                        bool validate = true;
                        bool validateSubjectName = true;

                        NameEntryError.IsVisible = false;
                        CAError.IsVisible = false;
                        FEError.IsVisible = false;
                        CA_FE_Error.IsVisible = false;
                        CA_FE_Decimal.IsVisible = false;
                        EmailError.IsVisible = false;
                        NameAlreadyExists.IsVisible = false;

                        // Checking if all the fields are filled
                        if (string.IsNullOrEmpty(this.subjectName.Text) || string.IsNullOrWhiteSpace(this.subjectName.Text))
                        {
                            busyindicator.IsVisible = false;
                            NameEntryError.IsVisible = true;
                            validate = false;

                        }
                        if (validate)
                        {
                            if (string.IsNullOrEmpty(this.CA.Text) || string.IsNullOrWhiteSpace(this.CA.Text) && validate)
                            {
                                busyindicator.IsVisible = false;
                                validate = false;
                                CAError.IsVisible = true;
                            }
                        }
                        if (validate)
                        {
                            if (string.IsNullOrEmpty(this.finalExam.Text) || string.IsNullOrWhiteSpace(this.finalExam.Text) && validate)
                            {
                                busyindicator.IsVisible = false;
                                FEError.IsVisible = true;
                                validate = false;
                            }
                        }
                        if (validate && !string.IsNullOrEmpty(lecturerEmail.Text))
                        {
                            string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
                            string lecEmail = lecturerEmail.Text.Trim();
                            if (!Regex.IsMatch(lecEmail, pattern) && validate)
                            {
                                busyindicator.IsVisible = false;
                                EmailError.IsVisible = true;
                                validate = false;
                            }
                        }

                        // cheking if the subject already exists in the database
                        var allSubjects = await fireBaseHelper.GetAllSubjects();
                        string newSubjectName = subjectName.Text.Trim();

                        foreach (Subject listS in allSubjects)
                        {
                            if (string.Equals(listS.SubjectName, newSubjectName, StringComparison.OrdinalIgnoreCase))
                            {
                                validateSubjectName = false;
                            }
                        }
                        if (!validateSubjectName)
                        {
                            busyindicator.IsVisible = false;
                            NameAlreadyExists.IsVisible = true;
                            validate = false;
                        }

                        if (validate)
                        {
                            int CA = Int32.Parse(this.CA.Text);
                            int FinalExam = Int32.Parse(this.finalExam.Text);

                            // checking id the weights of the exams add to 100
                            if (CA + FinalExam == 100 && CA >= 0 && FinalExam >= 0)
                            {
                                var subject = await fireBaseHelper.GetSubject(subjectName.Text);
                                
                                await fireBaseHelper.AddSubject(newSubjectName, lecturerName.Text, lecturerEmail.Text, CA, FinalExam);
                                busyindicator.IsVisible = false;
                                await DisplayAlert("Subject Added", $"{this.subjectName.Text}", "OK");
                                MainPage.Instance.toListSubjects();
                                await Navigation.PopToRootAsync();
                            }
                            else
                            {
                                busyindicator.IsVisible = false;
                                CA_FE_Error.IsVisible = true; ;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        CA_FE_Decimal.IsVisible = true;
                        busyindicator.IsVisible = false;
                    }
                    save_subject_button.IsEnabled = true;
                }
                else
                {
                    await DisplayAlert("Something went wrong...", "", "OK");
                    App.Current.MainPage = new NavigationPage(new LogIn());
                }
            }
            else
            {
                await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
            }          
        }

        private async void Cancel_Subject(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            //MainPage.Instance.toFirstTab();
        }

        private async void BackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void info(object sender, EventArgs e)
        {
            if (hiden.IsVisible)
            {
                hiden.IsVisible = false;
            }
            else
            {
                hiden.IsVisible = true;
            }
        }

        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }
    }
}