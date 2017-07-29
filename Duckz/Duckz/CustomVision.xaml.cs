using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Duckz.Model;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace Duckz
{ 
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });


            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "d8715acccea74200966cfba3d0d8392a");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/f73b2383-7270-4f60-afcc-f4c145998811/image";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    JObject rss = JObject.Parse(responseString);
                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);
                    double max = responseModel.Predictions.Max(m => m.Probability);
                    TagLabel.Text = (max >= 0.5) ? "DUCK! \n" : "Not duck \n";

                    var Probability = from p in rss["Predictions"] select (string)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];

                    foreach (var item in Tag)
                    {
                        TagLabel.Text += item + ": \n";
                    }

                    foreach (var item in Probability)
                    {
                        PredictionLabel.Text += "\n" + item + "%";
                    }




                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
    }
}