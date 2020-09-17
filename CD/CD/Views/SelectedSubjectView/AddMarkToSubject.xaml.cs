using System;
using CD.Helper;
using CD.Models;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace CD.Views.SelectedSubjectView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMarkToSubject
    {
        private Subject _subject;
        readonly FireBaseHelperMark fireBaseHelper = new FireBaseHelperMark();

        public AddMarkToSubject(Subject subject)
        {
            _subject = subject;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
        }

        // check if the weight of the current CA is not exceeding the overall weight of the CA
        [Obsolete]
        public async Task<bool> Check_CA_Weight(Subject subject, double weight)
        {
            bool connection = true;
            double total_CA_all_Marks = 0;
            List<Mark> marks_belonging_to_subject = new List<Mark>();
            try
            {
                marks_belonging_to_subject = await fireBaseHelper.GetMarksForSubject(subject.SubjectID);
            }
            catch (Exception)
            {
                connection = false;
            }

            if (connection)
            {
                foreach (Mark m in marks_belonging_to_subject)
                {
                    if (m.Category.Equals("Continuous Assessment"))
                    {
                        total_CA_all_Marks += m.Weight;
                    }
                }
                if (total_CA_all_Marks + weight > subject.CA)
                {
                    ErrorWeightCA.IsVisible = true;
                    busyindicator.IsVisible = false;
                    return false;
                }
                return true;
            }
            else
            {
                busyindicator.IsVisible = false;
                await DisplayAlert("Something went wrong...", "Please check your interner connection", "OK");
                await PopupNavigation.RemovePageAsync(this);
                return false;
            }
        }

        [Obsolete]
        private async void Save_Mark(object sender, EventArgs e)
        {
            busyindicator.IsVisible = true;
            ErrorWeightCA.IsVisible = false;
            ErrorResultCA.IsVisible = false;
            ErrorNameCA.IsVisible = false;
            EmptyWeightCA.IsVisible = false;
            EmptyResultCA.IsVisible = false;
            HigherThanZeroWeight.IsVisible = false;
            WholeDecimalWight.IsVisible = false;

            save_ca_button.IsEnabled = false;
            bool validate = true;
            bool less = true;
            double result = 0;
            double weight = 0;
            bool connection = true;

            // check all the entries are filled in 
            if (string.IsNullOrEmpty(this.mark_name.Text) || string.IsNullOrWhiteSpace(this.mark_name.Text))
            {
                validate = false;
                less = false;
                ErrorNameCA.IsVisible = true;
                busyindicator.IsVisible = false;
            }
            if (validate)
            {
                if (string.IsNullOrEmpty(this.weight.Text) || string.IsNullOrWhiteSpace(this.weight.Text))
                {
                    validate = false;
                    less = false;
                    EmptyWeightCA.IsVisible = true;
                    busyindicator.IsVisible = false;
                }
            }
            if (validate)
            {
                try
                {
                    weight = double.Parse(this.weight.Text);
                }
                catch (Exception)
                {
                    WholeDecimalWight.IsVisible = true;
                    validate = false;
                    busyindicator.IsVisible = false;
                }
            }
            if (validate)
            {
                if (string.IsNullOrEmpty(this.result.Text) || string.IsNullOrWhiteSpace(this.result.Text))
                {
                    validate = false;
                    less = false;
                    EmptyResultCA.IsVisible = true;
                    busyindicator.IsVisible = false;
                }
            }
            // check if the weight of the current CA is not exceeding the overall weight of the CA
            if (validate)
            {
                try
                {
                    result = Double.Parse(this.result.Text);
                    weight = Double.Parse(this.weight.Text);
                    validate = await Check_CA_Weight(_subject, weight);
                }
                catch (Exception)
                {
                    ErrorResultCA.IsVisible = true;
                    busyindicator.IsVisible = false;
                    validate = false;
                }
            }
            // check if the exam weight is higher than 0
            if (validate)
            {
                validate = weight > 0;
                if (!validate) 
                { 
                    HigherThanZeroWeight.IsVisible = true;
                    busyindicator.IsVisible = false;
                }
            }
            // check the mark is not over 100 or negative
            if (validate && result > 100 || result < 0)
            {
                ErrorResultCA.IsVisible = true;
                busyindicator.IsVisible = false;
                validate = false;
                less = false;
                busyindicator.IsVisible = false;
            }
            // if the mark is valid and less than 100
            if (validate && less)
            {
                try
                {
                    var mark = await fireBaseHelper.GetMark(mark_name.Text);
                    await fireBaseHelper.AddMark(_subject.SubjectID, mark_name.Text, result, weight, "Continuous Assessment");
                }
                catch (Exception)
                {
                    connection = false;
                }
                if(!connection)
                {
                    busyindicator.IsVisible = false;
                    await DisplayAlert("SOmething went wrong...", "Please check your internet connection", "OK");
                }

                // refresh the page to show the added mark to the subject
                await Navigation.PushAsync(new SubjectSelected(_subject), false);
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                await PopupNavigation.RemovePageAsync(this);
            }
            save_ca_button.IsEnabled = true;
        }

        [Obsolete]
        private void Cancel_Mark(object sender, EventArgs e)
        {
            PopupNavigation.RemovePageAsync(this);
        }
    }
}