using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Plugin.Geolocator;

namespace Duckz
{
    public partial class AzureTable : ContentPage
    {

        public AzureTable()
        {
            InitializeComponent();

        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<DuckModel> DuckInformation = await AzureManager.AzureManagerInstance.GetDuckInformation();

            DuckList.ItemsSource = DuckInformation;
            await PostLocationAsync();
        }
        private async Task PostLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

            DuckModel model = new DuckModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude

            };

            await AzureManager.AzureManagerInstance.PostDuckInformation(model);
        }

    }
}