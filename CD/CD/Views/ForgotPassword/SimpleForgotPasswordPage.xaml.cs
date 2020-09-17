using CD.Helper;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CD.Views.SignUp;
using System.Text.RegularExpressions;
using System;
using CD.Views.Login;
using CD.Views.ErrorAndEmpty;

namespace CD.Views.ForgotPassword
{
    /// <summary>
    /// Page to retrieve the password forgotten.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimpleForgotPasswordPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleForgotPasswordPage" /> class.
        /// </summary>
        IFirebaseForgotPassword auth;
        public SimpleForgotPasswordPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseForgotPassword>();
        }

        private void SignUp(object sender, EventArgs e)
        {
            sign_up_button.IsEnabled = false;
            Navigation.PushAsync(new SignUpPage());
            sign_up_button.IsEnabled = true;
        }

        private async void ForgotPassword(object sender, EventArgs e)
        {
            if (App.CheckConnection())
            {
                send_forgotpassword_button.IsEnabled = false;
                EmailEntry.IsVisible = false;
                bool validate = true;
                string pattern = null;
                pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
                string userEmail = "";

                if (!string.IsNullOrEmpty(ForgotPasswordEmail.Text) && !string.IsNullOrWhiteSpace(ForgotPasswordEmail.Text))
                {
                    userEmail = ForgotPasswordEmail.Text.Trim();
                }
                else
                {
                    EmailEntry.IsVisible = true;
                    validate = false;
                }
                if (!Regex.IsMatch(userEmail, pattern) && validate)
                {
                    EmailEntry.IsVisible = true;
                    validate = false;
                }
                if (validate)
                {
                    busyindicator.IsVisible = true;
                    string forgotPassword = await auth.ForgotPassword(ForgotPasswordEmail.Text);
                    if (forgotPassword == "emailSent")
                    {
                        busyindicator.IsVisible = false;
                        await DisplayAlert("Success", "Please verify your email to reset your password", "OK");
                        await Navigation.PushAsync(new LogIn());
                        await Navigation.PopToRootAsync(true);
                    }
                    else if (forgotPassword == "emailNotFound")
                    {
                        busyindicator.IsVisible = false;
                        await DisplayAlert("Email not found", "There is no account matching this email \nPlease sign up", "OK");
                    }
                    else
                    {
                        busyindicator.IsVisible = false;
                        await DisplayAlert("Error", "Please try again", "OK");
                    }
                }
                send_forgotpassword_button.IsEnabled = true;
            }
            else
            {
                await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
            }           
        }
    }
}