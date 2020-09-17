using CD.Views.Login;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CD.Views.ErrorAndEmpty
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [Preserve(AllMembers = true)]
    public partial class NoInternetConnectionPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoInternetConnectionPage" /> class.
        /// </summary>
        private string page;
        public NoInternetConnectionPage(string previousPage)
        {
            InitializeComponent();
            page = previousPage;
        }

        /// <summary>
        /// Invoked when view size is changed.
        /// </summary>
        /// <param name="width">The Width</param>
        /// <param name="height">The Height</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height)
            {
                if (Device.Idiom == TargetIdiom.Phone)
                {
                    ErrorImage.IsVisible = false;
                }
            }
            else
            {
                ErrorImage.IsVisible = true;
            }
        }

        private async void GoBack(object sender, System.EventArgs e)
        {
            if (page == "App")
            {
                App.Current.MainPage = new NavigationPage(new LogIn());
                App.Current.Properties.Remove("App.UserUID");
                await App.Current.SavePropertiesAsync();
                // back button disabled
                OnBackButtonPressed();
            }
            else 
            {
                await Navigation.PopAsync();
            }
        }
    }
}