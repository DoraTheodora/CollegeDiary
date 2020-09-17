using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CD.Helper;
using CD.Models.Calendar;
using CD.Views.ErrorAndEmpty;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CD.Views.Calendar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCalendarEvent 
    {
        readonly FireBaseHelperCalendarEvents fireBaseHelper = new FireBaseHelperCalendarEvents();
        private int color = 0;
        private ScheduleAppointment thisAppointment;
        private DateTime start_Date;
        private DateTime end_Date;
        string sourcePage = "";
        public EditCalendarEvent(ScheduleAppointment args, string motiv)
        {
            InitializeComponent();
            event_name.Text = args.Subject;
            event_description.Text = args.Notes;
            startDate.Date = args.StartTime.Date;
            endDate.Date = args.EndTime.Date;
            startTimePicker.Time = args.StartTime.TimeOfDay;
            endTimePicker.Time = args.EndTime.TimeOfDay;
            segmentedControl.SelectionIndicatorSettings.Color = args.Color;
            color = segmentedControl.SelectedIndex = colorSelected(args.Color);
            thisAppointment = args;
            sourcePage = motiv;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
            // tap a color on the selection line 
            segmentedControl.SelectionChanged += Handle_SelectionChanged;
        }

        private void OnTimePickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        [Obsolete]
        private async void Save_Event(object sender, EventArgs e)
        {
            busyindicator.IsVisible = true;
            save_button.IsEnabled = false;
            ErrorName.IsVisible = false;
            bool validate = true;
            bool connection = true;
            EventModel thisEvent = new EventModel();

            try
            {
                thisEvent = await fireBaseHelper.GetEvent(thisAppointment.Subject,
                    thisAppointment.Notes, thisAppointment.StartTime,
                    thisAppointment.EndTime, thisAppointment.Color);
            }
            catch(Exception)
            {
                connection = false;
            }

            start_Date = new DateTime(startDate.Date.Year, startDate.Date.Month, startDate.Date.Day, startTimePicker.Time.Hours, startTimePicker.Time.Minutes, startTimePicker.Time.Seconds);
            end_Date = new DateTime(endDate.Date.Year, endDate.Date.Month, endDate.Date.Day, endTimePicker.Time.Hours, endTimePicker.Time.Minutes, endTimePicker.Time.Seconds);
            checkDates(start_Date, end_Date);
            Color colorEvent = changeColor(color);
            

            if (string.IsNullOrEmpty(event_name.Text) || string.IsNullOrWhiteSpace(event_name.Text))
            {
                validate = false;
                ErrorName.IsVisible = true;
                busyindicator.IsVisible = false;
            }
            if (validate)
            {
                try
                {
                    await fireBaseHelper.UpdateEvent(thisEvent.EventID, event_name.Text, event_description.Text, start_Date, end_Date, colorEvent);
                }
                catch (Exception)
                {
                    connection = false;
                    busyindicator.IsVisible = false;
                }
            }
            if (connection)
            {
                save_button.IsEnabled = true;
                if (validate)
                {
                    if (sourcePage == "SimplePage")
                    {
                        busyindicator.IsVisible = false;
                        await PopupNavigation.RemovePageAsync(this);
                    }
                    else if (sourcePage == "MyAccount")
                    {
                        busyindicator.IsVisible = false;
                        await Navigation.PushAsync(new MyAccount(), false);
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                        await PopupNavigation.RemovePageAsync(this);
                    }
                }
            }
            else 
            {
                await DisplayAlert("Something went wrong....", "Please check your internet connection", "OK");
                busyindicator.IsVisible = false;
                if (sourcePage == "SimplePage")
                {
                    busyindicator.IsVisible = false;
                    await PopupNavigation.RemovePageAsync(this);
                }
                else if (sourcePage == "MyAccount")
                {
                    busyindicator.IsVisible = false;
                    await PopupNavigation.RemovePageAsync(this);
                }
            }
            await SimplePage.Instance.refreshCalendar();
        }

        [Obsolete]
        private async void Cancel_Event(object sender, EventArgs e)
        {
            await PopupNavigation.RemovePageAsync(this);
        }

        private void Handle_SelectionChanged(object sender, Syncfusion.XForms.Buttons.SelectionChangedEventArgs e)
        {
            color = segmentedControl.SelectedIndex;
            if (color == 0)
            {
                segmentedControl.SelectionIndicatorSettings.Color = Color.OrangeRed;
            }
            if (color == 1)
            {
                segmentedControl.SelectionIndicatorSettings.Color = Color.Orange;
            }
            if (color == 2)
            {
                segmentedControl.SelectionIndicatorSettings.Color = Color.DeepPink;
            }
            if (color == 3)
            {
                segmentedControl.SelectionIndicatorSettings.Color = Color.DodgerBlue;
            }
            if (color == 4)
            {
                segmentedControl.SelectionIndicatorSettings.Color = Color.MediumSeaGreen;
            }
            if (color == 5)
            {
                segmentedControl.SelectionIndicatorSettings.Color = Color.BlueViolet;
            }

        }
        private int colorSelected(Color theColor)
        {
            if (theColor.ToString() == Color.OrangeRed.ToString())
                return 0;
            else if (theColor.ToString() == Color.Orange.ToString())
                return 1;
            else if (theColor.ToString() == Color.DeepPink.ToString())
                return 2;
            else if (theColor.ToString() == Color.DodgerBlue.ToString())
                return 3;
            else if (theColor.ToString() == Color.MediumSeaGreen.ToString())
                return 4;
            else if (theColor.ToString() == Color.BlueViolet.ToString())
                return 5;
            else
                return 0;
        }

        private Color changeColor(int theColor)
        {
            if (theColor == 0)
                return Color.OrangeRed;
            else if (theColor == 1)
                return Color.Orange;
            else if (theColor == 2)
                return Color.DeepPink;
            else if (theColor == 3)
                return Color.DodgerBlue;
            else if (theColor == 4)
                return Color.MediumSeaGreen;
            else if (theColor == 5)
                return Color.BlueViolet;
            else
                return Color.Blue;
        }
        private void checkDates(DateTime startDate, DateTime endDate)
        {
            int res = DateTime.Compare(startDate, endDate);
            if (res == 1 || res == 0)
            {
                this.end_Date = new DateTime(startDate.Date.Year, startDate.Date.Month, startDate.Date.Day, startTimePicker.Time.Hours, startTimePicker.Time.Minutes, startTimePicker.Time.Seconds);
                this.end_Date = end_Date.AddMinutes(30);
            }
        }
    }
}