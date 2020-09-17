using CD.Helper;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using CD.Models;
using Xamarin.Forms.Xaml;
using System;

namespace CD.Views.SelectedSubjectView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeleteMark 
    {
        Mark _mark;
        Subject _subject;
        readonly FireBaseHelperMark fireBaseHelperMark = new FireBaseHelperMark();
        public DeleteMark(Mark thisMark, Subject thisSubject)
        {
            InitializeComponent();
            _mark = thisMark;
            _subject = thisSubject;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
            MarkName.Text = _mark.MarkName;
            MarkWeight.Text = _mark.Weight + "%";
            MarkResult.Text = _mark.Result + "%";
        }

        [Obsolete]
        private async void delete_mark(object sender, System.EventArgs e)
        {
            delete_button.IsEnabled = false;
            busyindicator.IsVisible = true;
            bool connection = true;
            try
            {
                await fireBaseHelperMark.DeleteMark(_mark.MarkID);
            }
            catch (Exception)
            {
                connection = false;
            }
            if(connection)
            { 
                await Navigation.PushAsync(new SubjectSelected(_subject), false);
                busyindicator.IsVisible = false;
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                await PopupNavigation.RemovePageAsync(this);
            }
            else
            {
                busyindicator.IsVisible = false;
                await DisplayAlert("Something went wrong...", "Please check your interner connection", "OK");
                PopupNavigation.RemovePageAsync(this);
            }
        }

        [Obsolete]
        private async void cancel(object sender, System.EventArgs e)
        {
            await PopupNavigation.RemovePageAsync(this);
        }
    }
}