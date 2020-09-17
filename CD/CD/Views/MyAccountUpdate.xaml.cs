using System;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;
using CD.Helper;
using CD.Models;

namespace CD.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyAccountUpdate 
    {
        Student user;
        readonly FireBaseHelperStudent fireBaseHelperStudent = new FireBaseHelperStudent();
        public MyAccountUpdate(Student User)
        {
            InitializeComponent();
            user = User;
            userName.Text = user.StudentName;
            userInstitute.Text = user.Institute;
            userEmail.Text = user.StudentEmail;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
        }
        [Obsolete]
        private async void Save_Account(object sender, EventArgs e)
        {
            busyindicator.IsVisible = true;
            save_profile_button.IsEnabled = false;
            bool validate = true;

            ErrorName.IsVisible = false;
            ErrorInstite.IsVisible = false;

            if (string.IsNullOrEmpty(userName.Text) || string.IsNullOrWhiteSpace(userName.Text))
            {
                validate = false;
                ErrorName.IsVisible = true;
                busyindicator.IsVisible = false;
            }
            if (validate)
            {
                if(string.IsNullOrWhiteSpace(userInstitute.Text) || string.IsNullOrEmpty(userInstitute.Text))
                {
                    validate = false;
                    ErrorInstite.IsVisible = true;
                    busyindicator.IsVisible = false;
                }
            }
            if (validate)
            {
                bool connection = true;
                try
                {
                    var studentToEdit = await fireBaseHelperStudent.GetStudent(user.StudentID);
                }
                catch (Exception)
                {
                    connection = false;
                }
                try
                {
                    await fireBaseHelperStudent.UpdateAccount(user.StudentID, userName.Text, userInstitute.Text);
                }
                catch (Exception)
                {
                    connection = false;
                }
                if(connection)
                { 
                    busyindicator.IsVisible = false;
                    await Navigation.PushAsync(new MyAccount(), false);
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                    await PopupNavigation.RemovePageAsync(this);
                }
                else
                {
                    busyindicator.IsVisible = false;
                    await DisplayAlert("Something went wrong...", "Please check your interner connection", "OK");
                }

            }
            save_profile_button.IsEnabled = true;
        }

        [Obsolete]
        private async void Cancel_Update(object sender, EventArgs e)
        {
            await PopupNavigation.RemovePageAsync(this);
        }
    }
}