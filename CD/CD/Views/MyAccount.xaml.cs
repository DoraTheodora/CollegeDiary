using System;
using CD.Views.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CD.Models;
using CD.Helper;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using CD.Models.Calendar;
using System.Collections.Generic;
using CD.ViewModel;
using System.Linq;
using Syncfusion.SfSchedule.XForms;
using CD.Views.Calendar;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using CD.Views.ContactUs;
using CD.Views.PrivatePolicy;
using CD.Views.ErrorAndEmpty;

namespace CD.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyAccount : ContentPage
    {
        string userID = "";
        readonly FireBaseHelperStudent fireBaseHelperStudent = new FireBaseHelperStudent();
        readonly FireBaseHelperCalendarEvents fireBaseHelperCalendar = new FireBaseHelperCalendarEvents();
        
        IFirebaseDeleteAccount authDeleteAccount;
        IFirebaseSignOut authSignOut;
        Student user;
        protected override bool OnBackButtonPressed() => false;
        bool allowPopup = true;


        public MyAccount()
        {
            InitializeComponent();
            userID = App.UserUID;
            authDeleteAccount = DependencyService.Get<IFirebaseDeleteAccount>();
            authSignOut = DependencyService.Get<IFirebaseSignOut>();
            //RunHourlyTasks();
        }
        protected override async void OnAppearing()
        {
            if (App.CheckConnection())
            {
                if (await App.userExists(App.UserUID))
                {
                    busyindicator.IsVisible = true;
                    bool connection = true;
                    List<EventModel> allEvents = new List<EventModel>();
                    List<EventModel> listEvents =new List<EventModel>();
                    try
                    {
                        await fireBaseHelperStudent.AddGPA(userID);
                        user = await fireBaseHelperStudent.GetStudent(userID);
                        allEvents = await fireBaseHelperCalendar.GetAllEvents();
                        listEvents = next7DaysEvents(allEvents);
                    }
                    catch (Exception)
                    {
                        connection = false;   
                    }
                    if (connection)
                    {
                        // creating a new model from 2 classes - student and calendar
                        StudentCalendar studentCalendar = new StudentCalendar(user, listEvents);
                        this.BindingContext = studentCalendar;

                        // progress bar
                        totalGPA.Progress = Convert.ToDouble(user.FinalGPA);
                        busyindicator.IsVisible = false;
                    }
                    else
                    {
                        await Navigation.PushAsync(new NoInternetConnectionPage("noApp"));
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
                await Navigation.PushAsync(new NoInternetConnectionPage("noApp"));
            }
        }
        private void GPA_Changed(object sender, Syncfusion.XForms.ProgressBar.ProgressValueEventArgs e)
        {
            busyindicator.IsVisible = true;
            if (e.Progress < 40)
            {
                totalGPA.ProgressColor = Color.PaleVioletRed;
            }
            else if (e.Progress >= 40 && e.Progress < 70)
            {
                totalGPA.ProgressColor = Color.FromHex("F8C371") ;
            }
            else if (e.Progress > 70)
            {
                totalGPA.ProgressColor = Color.PaleGreen;
            }
            busyindicator.IsVisible = false;
        }

        [Obsolete]
        private async void edit_account(object sender, EventArgs e)
        {
            if (!busyindicator.IsVisible && allowPopup)
            {
                allowPopup = false;
                Student student = new Student();
                bool connection = true;
                try
                {
                    student = await fireBaseHelperStudent.GetStudent(userID);
                }
                catch (Exception)
                {
                    connection = false;
                }
                if (connection)
                {
                    await PopupNavigation.PushAsync(new MyAccountUpdate(student));
                }
                else
                {
                    await DisplayAlert("Something went wrong...", "Please check your interner connection", "OK");
                }
                allowPopup = true;
            }
        }

        private async void delete_account(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Are you sure you want to delete your account?",
                "If you delete your account all your information will be permanently deleted.", "Yes", "No");
            if (!busyindicator.IsVisible && allowPopup)
            {
                allowPopup = false;
                if (!App.loggedInNow && result)
                {
                    await DisplayAlert("For security reasons", "Please log in again before deleting your account!", "OK");
                    App.UserUID = "";
                    App.Current.Properties.Remove("App.UserUID");
                    await App.Current.SavePropertiesAsync();
                    App.Current.MainPage = new NavigationPage(new LogIn());
                    OnBackButtonPressed();
                }
                if (result && App.loggedInNow) // if it's equal to Yes
                {
                    bool validate = true;
                    try
                    {
                        await authDeleteAccount.DeleteAccount();
                    }
                    catch (Exception)
                    {
                        validate = false;
                    }
                    if (validate)
                    {
                        try
                        {
                            await fireBaseHelperStudent.DeleteStudent(App.UserUID);
                        }
                        catch (Exception)
                        {
                            validate = false;
                        }
                    }
                    if (validate)
                    {
                        App.UserUID = "";
                        App.Current.MainPage = new NavigationPage(new LogIn());
                        await DisplayAlert("Account deleted", "To use the application again please sign up", "OK");
                        App.Current.Properties.Remove("App.UserUID");
                        await App.Current.SavePropertiesAsync();
                        OnBackButtonPressed();
                    }
                    else
                    {
                        await DisplayAlert("Something went wrong...", "Please check your interner connection", "OK");
                    }
                }
                allowPopup = true;
            }
        }

        private async void sign_out(object sender, EventArgs e)
        {
            if (!busyindicator.IsVisible && allowPopup)
            {
                allowPopup = false;
                App.UserUID = "";
                await authSignOut.SignOut();
                App.Current.MainPage = new NavigationPage(new LogIn());
                App.Current.Properties.Remove("App.UserUID");
                await App.Current.SavePropertiesAsync();
                //Console.WriteLine("---------------------------------" + App.Current.Properties.Count());
                // back button disabled
                OnBackButtonPressed();
                allowPopup = false; 
            }
        }

        private void help(object sender, EventArgs e)
        {
            if (helpMyAccount.IsVisible)
            {
                helpMyAccount.IsVisible = false;
            }
            else 
            {
                helpMyAccount.IsVisible = true;
            }
        }

        //public static async void RunHourlyTasks(params Action[] tasks)
        //{
        //    DateTime runHour = DateTime.Now.AddHours(1.0);
        //    TimeSpan ts = new TimeSpan(runHour.Hour, 0, 0);
        //    runHour = runHour.Date + ts;


        //    Console.WriteLine("next run will be at: {0} and current hour is: {1}", runHour, DateTime.Now);
        //    while (true)
        //    {
        //        TimeSpan duration = runHour.Subtract(DateTime.Now);
        //        if (duration.TotalMilliseconds <= 0.0)
        //        {
        //            Parallel.Invoke(tasks);
        //            Console.WriteLine("It is the run time as shown before to be: {0} confirmed with system time, that is: {1}", runHour, DateTime.Now);
        //            runHour = DateTime.Now.AddHours(1.0);
        //            Console.WriteLine("next run will be at: {0} and current hour is: {1}", runHour, DateTime.Now);
        //            await App.Current.MainPage.DisplayAlert("ONE HOUR!", DateTime.Now.ToLongDateString(), "OK");
        //            continue;
        //        }
        //        int delay = (int)(duration.TotalMilliseconds / 2);
        //        await Task.Delay(30000);  // 30 seconds
        //    }
        //}
        private List<EventModel> next7DaysEvents(List<EventModel> listEvents)
        {
            busyindicator.IsVisible = true;
            List<EventModel> next7Days = new List<EventModel>();
            DateTime today = DateTime.Today;
            DateTime timeWeek = today.AddDays(8);
            if (listEvents.Count > 0)
            {
                Calendar_View_Text.IsVisible = false;
                foreach (EventModel e in listEvents)
                {
                    DateTime startDate = Convert.ToDateTime(e.StartEventDate.ToString());
                    int laterThanToday = DateTime.Compare(startDate, today);
                    int inThisWeek = DateTime.Compare(startDate, timeWeek);
                    if ((laterThanToday >= 0) && inThisWeek <= 0)
                    {
                        next7Days.Add(e);
                    }
                }
                busyindicator.IsVisible = false;
            }
            // order the list by dates
            next7Days = next7Days.OrderBy(x => x.StartEventDate).ToList();
            if (next7Days.Count == 0)
            {
                Calendar_View_Text.IsVisible = true;
            }
            else 
            {
                Calendar_View_Text.IsVisible = false;
            }
            return next7Days;
        }

        [Obsolete]
        private async void view_event(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            LstEvents.IsEnabled = false;
            if (e != null)
            {
                ScheduleAppointment appointment = new ScheduleAppointment();
                appointment.Subject = (e.ItemData as EventModel).Name;
                appointment.Notes = (e.ItemData as EventModel).Description;
                appointment.StartTime = Convert.ToDateTime((e.ItemData as EventModel).StartEventDate);
                appointment.EndTime = Convert.ToDateTime((e.ItemData as EventModel).EndEventDate);
                appointment.Color = (e.ItemData as EventModel).Color;

                await PopupNavigation.PushAsync(new EventSelected(appointment, "MyAccount"));
            }
            LstEvents.IsEnabled = true;          
        }

        // background brush
        static SKPaint backgroundBrush = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = Color.Red.ToSKColor()
        };
        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            setGradientWallpaper(e);
        }

        public static void setGradientWallpaper(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // get the brush based on the theme
            SKColor gradientStart = ((Color)Application.Current.Resources["BackgroundGradientStartColor"]).ToSKColor();
            SKColor gradientMid = ((Color)Application.Current.Resources["BackgroundGradientMidColor"]).ToSKColor();
            SKColor gradientEnd = ((Color)Application.Current.Resources["BackgroundGradientEndColor"]).ToSKColor();

            // gradient backround
            backgroundBrush.Shader = SKShader.CreateRadialGradient
                (new SKPoint(0, info.Height * .8f),
                info.Height * .8f,
                new SKColor[] { gradientStart, gradientMid, gradientEnd },
                new float[] { 0, .5f, 1 },
                SKShaderTileMode.Clamp);

            //backgroundBrush.Shader = SKShader.CreateLinearGradient(
            //                              new SKPoint(0, 0),
            //                              new SKPoint(info.Width, info.Height),
            //                              new SKColor[] {
            //                                  gradientStart, gradientEnd },
            //                              new float[] { 0, 1 },
            //                              SKShaderTileMode.Clamp);
            SKRect backgroundBounds = new SKRect(0, 0, info.Width, info.Height);
            canvas.DrawRect(backgroundBounds, backgroundBrush);
        }

        private void contact_us(object sender, EventArgs e)
        {
            if(!busyindicator.IsVisible && allowPopup)
            {
                allowPopup = false;
                Navigation.PushAsync(new ContactUsView(user));
                allowPopup = true;
            }
        }
    }
}