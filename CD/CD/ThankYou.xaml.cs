using CD.Views;
using Xamarin.Forms;

namespace CD
{
    public partial class ThankYou : ContentPage
    {
        public ThankYou()
        {
            InitializeComponent();
        }

        void BackgroundGradient_PaintSurface(System.Object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }
    }
}
