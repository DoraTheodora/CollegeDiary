
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public static MainPage Instance;
        
        public MainPage()
        {
            Instance = this;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        public void toListSubjects()
        {
            this.CurrentPage = this.Children[1];
        }
        public void toMyAccount()
        {
            this.CurrentPage = this.Children[0];
        }
    }
}