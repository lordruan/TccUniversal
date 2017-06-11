using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TccUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditingPage : Page
    {
        App app = Application.Current as App;
        private WriteableBitmap originalBitmap;
        private Posts post;
        public EditingPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (!e.Parameter.Equals(null))   // I've also used if(data != null) which hasn't worked either
                {
                    post = (Posts)e.Parameter;
                    imgPreview.Source = app.imgTemp;
                }
            }
            catch { }
            ctgCbox.Items.Add("Moda e Acessórios");
            ctgCbox.Items.Add("Automóveis e Veículos");
            ctgCbox.Items.Add("Bebês e Cia");
            ctgCbox.Items.Add("Brinquedos e Games");
            ctgCbox.Items.Add("Casa e Decoração");
            ctgCbox.Items.Add("CDs, DVDs e Blu-Rays");
            ctgCbox.Items.Add("Telefonia");
            ctgCbox.Items.Add("Cosméticos e Telefonia");
            ctgCbox.Items.Add("Eletrodomesticos");
            ctgCbox.Items.Add("Eletrônicos e TVs");
            ctgCbox.Items.Add("Esporte e Lazer");
            ctgCbox.Items.Add("Informática");
            ctgCbox.Items.Add("Livros e E-Books");
            ctgCbox.Items.Add("Bebidas");
            ctgCbox.Items.Add("Cupons de Desconto");

        }
        public async Task<decimal[]> CarregarLocal()
        {
            post.geo_x = 0;
            post.geo_y = 0;
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            var position = await locator.GetGeopositionAsync();
            decimal[] retorno = new decimal[2];
            retorno[0] = decimal.Parse(position.Coordinate.Latitude.ToString());
            retorno[1] = decimal.Parse(position.Coordinate.Longitude.ToString());
            return retorno;
        }
        public async void Postar()
        {

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                App.addLoad(true, "Postando");
            post.description = txtDescricao.Text;
            post.active = true;
            post.category_id = decimal.Parse(ctgCbox.SelectedIndex.ToString());
            post.category_id++;
            post.image = Convert.ToBase64String(ConvertBitmapToByteArray(app.imgTemp));
            var lacal = await CarregarLocal();
            post.geo_x = lacal[0];
            post.geo_y = lacal[1];
            try
            {
                var client = new HttpClient();
                var uri = new Uri(string.Format("http://192.168.43.169/api/posts/", "action", "post", DateTime.Now.Ticks));
                string serialized = JsonConvert.SerializeObject(post);

                StringContent stringContent = new StringContent(
                    serialized,
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(uri, stringContent);
                HttpResponseMessage x = await response;
                HttpContent requestContent = x.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                var retorno = JsonConvert.DeserializeObject<PostsResponse>(jsonContent);
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "Postando");
                Frame.Navigate(typeof(FeedPage), retorno);
            }
            catch (Exception ex)
            {
                MessageDialog errorBox = new MessageDialog("Post não pode ser finalizado, tente novamente mais tarde.");
                await errorBox.ShowAsync();
                Frame.Navigate(typeof(FeedPage));
            }
        }
        byte[] ConvertBitmapToByteArray(WriteableBitmap bitmap)
        {
            using (Stream stream = bitmap.PixelBuffer.AsStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {

            this.Frame.GoBack();
        }

        private void txtDescricao_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }
        
        private void btnPostar_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Postar();
        }
    }
}
