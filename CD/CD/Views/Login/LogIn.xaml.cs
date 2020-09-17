
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using CD.Views.SignUp;
using CD.Views.ForgotPassword;
using System.Linq;
using CD.Helper;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Plugin.Connectivity;
using CD.Views.ErrorAndEmpty;
using CD.Views.PrivatePolicy;

namespace CD.Views.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogIn : ContentPage
	{
		protected override bool OnBackButtonPressed() => false;
		IFirebaseAuthenticator auth;
		public ICommand LoginCmd { get; }
		public LogIn()
		{
			InitializeComponent();
			auth = DependencyService.Get<IFirebaseAuthenticator>();		
		}

		public async void Login(object sender, EventArgs e)
		{
			if (!App.CheckConnection())
			{
				await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
			}
			else
			{
				PasswordError.IsVisible = false;
				EmailError.IsVisible = false;
				Login_Button.IsEnabled = false;
				busyindicator.IsVisible = true;
				bool validate = true;
				string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
				string userEmail = "";
				if (!string.IsNullOrEmpty(EmailEntry.Text) && !string.IsNullOrWhiteSpace(EmailEntry.Text) && validate)
				{
					userEmail = EmailEntry.Text.Trim();
					if (!Regex.IsMatch(userEmail, pattern))
					{
						busyindicator.IsVisible = false;
						EmailError.IsVisible = true;
						validate = false;
					}
				}
				else
				{
					busyindicator.IsVisible = false;
					EmailError.IsVisible = true;
					validate = false;
				}

				if (validate)
				{
					if (string.IsNullOrEmpty(PasswordEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
					{
						busyindicator.IsVisible = false;
						PasswordError.IsVisible = true;
						validate = false;
					}
				}
				if (validate)
				{
					try
					{
						App.UserUID = (Application.Current as App).AuthToken = await auth.LoginWithEmailPassword(userEmail, PasswordEntry.Text);

						if (auth.IsSignedIn() && !string.IsNullOrWhiteSpace(App.UserUID) && !string.IsNullOrEmpty(App.UserUID) && await App.userExists(App.UserUID))
						{
							App.Current.MainPage = new NavigationPage(new MainPage());
							Application.Current.Properties["App.UserUID"] = App.UserUID;
							await App.Current.SavePropertiesAsync();
							App.loggedInNow = true;
							busyindicator.IsVisible = false;
						}
						else
						{
							App.UserUID = "";
							App.Current.Properties["App.UserUID"] = "";
							await App.Current.SavePropertiesAsync();
							busyindicator.IsVisible = false;
							await DisplayAlert("Login Failed", "Invalid e-mail or password", "OK");
						}
					}

					catch (Exception)
					{
						App.UserUID = "";
						App.Current.Properties["App.UserUID"] = "";
						await App.Current.SavePropertiesAsync();
						busyindicator.IsVisible = false;
						await DisplayAlert("Login Failed", "Something went wrong, \nCheck you internet connection", "OK");
					}
				}
				Login_Button.IsEnabled = true;
			}
		}

		private async void SignUpPage(object sender, EventArgs e)
		{
			sign_up_button.IsEnabled = false;
			await Navigation.PushAsync(new SignUpPage());
			sign_up_button.IsEnabled = true;
		}

		private async void ForgotPasswordPage(object sender, EventArgs e)
		{
			ForgotPasswordLabel.IsEnabled = false;
			await Navigation.PushAsync(new SimpleForgotPasswordPage());
			ForgotPasswordLabel.IsEnabled = true;
		}

		private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
		{
			MyAccount.setGradientWallpaper(e);
		}

		private void Privacy_policy(object sender, EventArgs e)
		{
			Navigation.PushAsync(new PrivacyPolicyView());
		}
	}
}