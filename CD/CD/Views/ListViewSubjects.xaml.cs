using System;
using System.Threading.Tasks;
using CD.Helper;
using CD.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CD.Views.List;
using Rg.Plugins.Popup.Services;
using CD.Views.SelectedSubjectView;
using CD.Views.ErrorAndEmpty;
using CD.Views.Login;
using System.Collections.Generic;

namespace CD.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ListViewSubjects : ContentPage
    {
        readonly FireBaseHelperSubject fireBaseHelper = new FireBaseHelperSubject();

        public ListViewSubjects()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (App.CheckConnection())
            {
                if (await App.userExists(App.UserUID))
                {
                    try
                    {
                        busyindicator.IsVisible = true;                       
                        await FetchAllSubjects();
                        busyindicator.IsVisible = false;
                    }
                    catch (Exception)
                    {
                        await Navigation.PushAsync(new NoInternetConnectionPage("MyAccount"));
                    }
                }
                else
                {
                    await DisplayAlert("Something went wrong...", "Please check you internet connection", "OK");
                    App.Current.MainPage = new NavigationPage(new LogIn());
                }
            }
            else
            {
                await Navigation.PushAsync(new NoInternetConnectionPage("MyAccount"));
            }
        }
        private async Task FetchAllSubjects()
        {
            bool connection = true;
            List<Subject> allSubjects = new List<Subject>();
            try
            {
                allSubjects = await fireBaseHelper.GetAllSubjects();
            }
            catch (Exception)
            {
                connection = false;
            }
            if(connection)
            { 
                LstSubjects.ItemsSource = allSubjects;
                if (allSubjects.Count == 0)
                {
                    Subject_text.IsVisible = true;
                    //add_Subject_Arrow.IsVisible = true;
                }
                else
                {
                    Subject_text.IsVisible = false;
                    //add_Subject_Arrow.IsVisible = false;
                }
            }
            else
            {
                await Navigation.PushAsync(new NoInternetConnectionPage("noApp"));
            }


        }

        private void BackButton(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void LstSubjects_ItemSelected(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            if (!busyindicator.IsVisible)
            {
                if (App.CheckConnection())
                {
                    if (await App.userExists(App.UserUID))
                    {
                        LstSubjects.IsEnabled = false;
                        add_subject.IsEnabled = false;
                        bool connextion = true;
                        Subject subject = new Subject();
                        try
                        {
                            subject = await fireBaseHelper.GetSubject((e.ItemData as Subject).SubjectID);
                        }
                        catch (Exception)
                        {
                            connextion = false;
                            await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
                        }
                        if (connextion)
                        {
                            await Navigation.PushAsync(new SubjectSelected(subject));
                            LstSubjects.IsEnabled = true;
                            add_subject.IsEnabled = true;
                        }
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
        }

        private void BackToTitle_Clicked(object sender, EventArgs e)
        {
            this.SearchButton.IsVisible = true;
            add_subject.IsVisible = true;
            if (this.TitleView != null)
            {
                double opacity;

                // Animating Width of the search box, from full width to 0 before it removed from view.
                var shrinkAnimation = new Animation(property =>
                {
                    Search.WidthRequest = property;
                    opacity = property / TitleView.Width;
                    Search.Opacity = opacity;
                },
                TitleView.Width, 0, Easing.Linear);
                shrinkAnimation.Commit(Search, "Shrink", 16, 250, Easing.Linear, (p, q) => this.SearchBoxAnimationCompleted());
            }

            SearchEntry.Text = string.Empty;
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            add_subject.IsVisible = false;
            this.Search.IsVisible = true;
            this.TitleSubjectView.IsVisible = false;
            this.SearchButton.IsVisible = false;

            if (this.TitleView != null)
            {
                double opacity;

                // Animating Width of the search box, from 0 to full width when it added to the view.
                var expandAnimation = new Animation(
                    property =>
                    {
                        Search.WidthRequest = property;
                        opacity = property / TitleView.Width;
                        Search.Opacity = opacity;
                    }, 0, TitleView.Width, Easing.Linear);
                expandAnimation.Commit(Search, "Expand", 16, 250, Easing.Linear, (p, q) => this.SearchExpandAnimationCompleted());
            }
        }
        private void SearchBoxAnimationCompleted()
        {
            this.Search.IsVisible = false;
            this.TitleSubjectView.IsVisible = true;
        }

        
        private void SearchExpandAnimationCompleted()
        {
            this.SearchEntry.Focus();
        }

        private async void load_add_subject(object sender, EventArgs e)
        {
            add_subject.IsEnabled = false;
            LstSubjects.IsEnabled = false;
            await Navigation.PushAsync(new AddSubject());
            add_subject.IsEnabled = true;
            LstSubjects.IsEnabled = true;
        }

        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }
    }
}
