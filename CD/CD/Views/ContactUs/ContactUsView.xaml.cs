using CD.Views.ErrorAndEmpty;
using CD.Views.Login;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CD.Views.ContactUs
{
    /// <summary>
    /// Page to show Contact profile page
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactUsView
    {
        private CD.Models.Student _student;
        public ContactUsView(CD.Models.Student student)
        {
            InitializeComponent();
            _student = student;
        }
        protected override async void OnAppearing()
        {
            if (App.CheckConnection())
            {
                if (await App.userExists(App.UserUID))
                {
                    studentName.Text = _student.StudentName;
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

        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }

        [Obsolete]
        private void contactus_email_button(object sender, System.EventArgs e)
        {
            contact_button.IsEnabled = false;
            Device.OpenUri(new Uri("mailto:" + contact_button.Text));
            contact_button.IsEnabled = true;
        }

        [Obsolete]
        private void go_to_facebook(object sender, EventArgs e)
        {
            facebook_button.IsEnabled = false;
            Device.OpenUri(new Uri("fb://page/222530839180059"));
            facebook_button.IsEnabled = true;
        }
    }
}