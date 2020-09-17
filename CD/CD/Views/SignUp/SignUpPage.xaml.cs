using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CD.Views.Login;
using CD.Helper;
using Xamarin.Forms;
using System;
using System.Text.RegularExpressions;
using CD.Views.ErrorAndEmpty;

namespace CD.Views.SignUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage: ContentPage
    {
        IFirebaseRegister auth;
        readonly FireBaseHelperStudent firebaseStudent = new FireBaseHelperStudent();

        public SignUpPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseRegister>();
        }
        private void LoginPage(object sender, System.EventArgs e)
        {
            // not allowing the user to use the back button from the phone
            App.Current.MainPage = new NavigationPage(new LogIn());
            //await Navigation.PopToRootAsync(true);
        }
        private async void RegiterNewUser(object sender, EventArgs e)
        {
            if (App.CheckConnection())
            {
                signup_button.IsEnabled = false;
                bool validate = true;
                string pattern = null;
                pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

                NameError.IsVisible = false;
                InstituteError.IsVisible = false;
                EmailError.IsVisible = false;
                PasswordErrorNotMatching.IsVisible = false;
                PasswordErrorTooShort.IsVisible = false;
                PasswordEmpty.IsVisible = false;

                if (string.IsNullOrEmpty(NameEntry.Text) && validate)
                {
                    NameError.IsVisible = true;
                    validate = false;
                }
                if (string.IsNullOrEmpty(College_University.Text) && validate)
                {
                    InstituteError.IsVisible = true;
                    validate = false;
                }
                // cheking if  the email is valid
                string userEmail = "";
                if (validate)
                {
                    if (!string.IsNullOrEmpty(SignUpEmailEntry.Text) && !string.IsNullOrWhiteSpace(SignUpEmailEntry.Text))
                    {
                        userEmail = SignUpEmailEntry.Text.Trim();
                    }
                    else
                    {
                        validate = false;
                        EmailError.IsVisible = true;
                    }
                }

                if (validate)
                {
                    if (!Regex.IsMatch(userEmail, pattern))
                    {
                        EmailError.IsVisible = true;
                        validate = false;
                    }
                }
                if (validate)
                {
                    if (string.IsNullOrEmpty(PasswordEntry.Text) && string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
                    {
                        PasswordEmpty.IsVisible = true;
                        validate = false;
                    }
                    if (!passwordMatch(PasswordEntry.Text, ConfirmPasswordEntry.Text) && validate)
                    {
                        PasswordErrorNotMatching.IsVisible = true;
                        validate = false;
                    }
                    if (validate && !string.IsNullOrEmpty(PasswordEntry.Text) && PasswordEntry.Text.Length < 6)
                    {
                        PasswordErrorTooShort.IsVisible = true;
                        validate = false;
                    }
                }

                if (validate)
                {
                    //TODO: check this busy indicator
                    //System.Console.WriteLine("=====================================" + SignUpEmailEntry.Text + " " + PasswordEntry.Text);
                    busyindicator.IsVisible = true;
                    bool connection = true;
                    string Token = "";
                    try
                    {
                        Token = await auth.RegisterWithEmailAndPassword(userEmail, PasswordEntry.Text);
                    }
                    catch (Exception)
                    {
                        connection = false;
                    }
                    if (connection)
                    {
                        if (!string.IsNullOrEmpty(Token) && Token != "existing")
                        {
                            busyindicator.IsVisible = false;                          
                            AddUserDetails(NameEntry.Text, College_University.Text, SignUpEmailEntry.Text);
                            App.UserUID = "";
                            App.Current.Properties["App.UserUID"] = "";
                            await App.Current.SavePropertiesAsync();
                            await DisplayAlert("Congratulations", "Your account has been created", "OK");
                            App.Current.MainPage = new NavigationPage(new LogIn());
                        }
                        else if (Token == "existing")
                        {
                            busyindicator.IsVisible = false;
                            await DisplayAlert("Attention", "An account using this email already exists", "OK");
                        }
                        else
                        {
                            busyindicator.IsVisible = false;
                            await DisplayAlert("Error", "Please try again", "OK");
                        }
                    }
                }
                signup_button.IsEnabled = true;
            }
            else
            {
                await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
            }         
        }

        private bool passwordMatch(string password1, string password2)
        {
            return password1 == password2;
        }

        private async void AddUserDetails(string Name, string UC, string Email)
        {
            await firebaseStudent.AddStudent(App.UserUID, Name, UC, Email);
        }
    }
}