using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using CD.Helper;
using Syncfusion.SfSchedule.XForms;
using CD.Models.Calendar;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Connectivity;
using CD.Views.ErrorAndEmpty;
using CD.Views.Login;

namespace CD.Views.Calendar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimplePage : ContentPage
    {
        readonly FireBaseHelperCalendarEvents fireBaseHelperEvents = new FireBaseHelperCalendarEvents();
        private List<EventModel> listEvents;
        public static SimplePage Instance;

        [Obsolete]
        public SimplePage()
        {
            Instance = this;
            InitializeComponent();

            // tapping an appointment
            schedule.MonthInlineAppointmentTapped += Schedule_MonthInlineAppointmentTapped;
            async void Schedule_MonthInlineAppointmentTapped(object sender, MonthInlineAppointmentTappedEventArgs args)
            {           
                if (args.Appointment != null)
                {
                    add_calendar_event_button.IsEnabled = false;
                    var appointment = (args.Appointment as ScheduleAppointment);
                    await PopupNavigation.PushAsync(new EventSelected(appointment, "SimplePage"));
                    add_calendar_event_button.IsEnabled = true;
                }
            }
            refreshCalendar();           ;
        }
        protected override async void OnAppearing()
        {
            if (App.CheckConnection())
            {
                if (await App.userExists(App.UserUID))
                {
                    base.OnAppearing();
                    List<EventModel> theListOfEvents = new List<EventModel>();
                    try
                    {
                        theListOfEvents = await fireBaseHelperEvents.GetAllEvents();
                    }
                    catch
                    {
                        Navigation.PushAsync(new NoInternetConnectionPage("noApp"));
                    }
                    listEvents = theListOfEvents;
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
        public async Task refreshCalendar()
        {
            bool connection = true;
            try
            {
                listEvents = await fireBaseHelperEvents.GetAllEvents();
                
            }
            catch (Exception)
            {
                connection = false;
            }
            ScheduleAppointmentCollection scheduleAppointmentCollection = new ScheduleAppointmentCollection();
            if (connection)
            {
                if (listEvents.Count == 0)
                {
                    schedule.DataSource = scheduleAppointmentCollection;
                }
                else
                {
                    foreach (EventModel ev in listEvents)
                    {
                        DateTime startDate = Convert.ToDateTime(ev.StartEventDate.ToString());
                        DateTime endDate = Convert.ToDateTime(ev.EndEventDate.ToString());

                        //Console.WriteLine("---------------------- ev->" + ev.EventDate.ToString() + "  -------------------- start_Date->" + start_Date.ToString());
                        scheduleAppointmentCollection.Add(new ScheduleAppointment()
                        {
                            BindingContext = this,
                            StartTime = startDate,
                            EndTime = endDate,
                            Subject = ev.Name,
                            Notes = ev.Description,
                            Color = ev.Color,
                        });
                        schedule.DataSource = scheduleAppointmentCollection;
                    }
                }
            }
        }

        [Obsolete]
        private async void AddEvent(object sender, EventArgs e)
        {
            add_calendar_event_button.IsEnabled = false;
            DateTime dateSelected = Convert.ToDateTime(string.IsNullOrEmpty(schedule.SelectedDate.ToString()) ? DateTime.Now.ToString(): schedule.SelectedDate.ToString());
            await PopupNavigation.PushAsync(new AddCalendarEvent(dateSelected));
            add_calendar_event_button.IsEnabled = true;
        }

        public static string[] parseDate(DateTime date)
        {
            var day = date.Day.ToString();
            var month = date.Month.ToString();
            var year = date.Year.ToString();
            string[] parsedDate = { day, month, year };
            return parsedDate;
        }

        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }
    }
}