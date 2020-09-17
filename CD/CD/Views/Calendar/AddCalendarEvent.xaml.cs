using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using System;
using CD.Helper;
using System.Drawing;
using Syncfusion.XForms.Buttons;
using CD.Views.ErrorAndEmpty;

namespace CD.Views.Calendar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCalendarEvent
    {
        private DateTime start_Date;
        private DateTime end_Date;
        readonly FireBaseHelperCalendarEvents fireBaseHelper = new FireBaseHelperCalendarEvents();
        private int color = 0;

        public AddCalendarEvent(DateTime selectedDate)
        {
            InitializeComponent();
            string[] theDate = SimplePage.parseDate(selectedDate);

            // date selected displayed in the pop-up form
            startDate.Date = selectedDate;
            endDate.Date = selectedDate;
            start_Date = selectedDate;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = Color.FromArgb(200, 0, 0, 0);
            // tap a color on the selection line 
            segmentedControl.SelectionChanged += Handle_SelectionChanged;
        }

        [Obsolete]
        private async void Save_Event(object sender, System.EventArgs e)
        {
            save_button.IsEnabled = false;
            ErrorName.IsVisible = false;
            busyindicator.IsVisible = true;

            string name = event_name.Text;
            string desc = event_description.Text;
            start_Date = new DateTime(startDate.Date.Year, startDate.Date.Month, startDate.Date.Day, startTimePicker.Time.Hours, startTimePicker.Time.Minutes, startTimePicker.Time.Seconds);
            end_Date = new DateTime(endDate.Date.Year, endDate.Date.Month, endDate.Date.Day, endTimePicker.Time.Hours, endTimePicker.Time.Minutes, endTimePicker.Time.Seconds);
            checkDates(start_Date, end_Date);
            Color colorEvent = colorSelected(color);
            bool connection = true;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    await fireBaseHelper.AddEvent(name, desc, start_Date, end_Date, colorEvent);
                    connection = true;
                }
                catch (Exception)
                {
                    connection = false;
                    
                }
                if (connection)
                {
                    busyindicator.IsVisible = false;
                    save_button.IsEnabled = true;
                    await PopupNavigation.RemovePageAsync(this);
                }
                else
                {
                    busyindicator.IsVisible = false;
                    save_button.IsEnabled = true;
                    await Navigation.PushAsync(new NoInternetConnectionPage("notApp"));
                    await PopupNavigation.RemovePageAsync(this);
                }
            }
            else
            {
                busyindicator.IsVisible = false;
                save_button.IsEnabled = true;
                ErrorName.IsVisible = true;
            }

            // repopulating the calendar
            await SimplePage.Instance.refreshCalendar();
        }

        [Obsolete]
        private async void Cancel_Event(object sender, System.EventArgs e)
        {
            cancel_button.IsEnabled = false;
            await PopupNavigation.RemovePageAsync(this);
            cancel_button.IsEnabled = true;
        }

        private void OnTimePickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void Handle_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private Color colorSelected(int theColor)
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

        private void BackgroundGradient_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            MyAccount.setGradientWallpaper(e);
        }
    }
}