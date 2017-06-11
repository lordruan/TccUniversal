using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class PostPage : Page
    {
        public PostPage()
        {
            this.InitializeComponent();
        }
        public PostsResponse post { get; set; }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (!e.Parameter.Equals(null))   // I've also used if(data != null) which hasn't worked either
                {
                    this.post = (PostsResponse)e.Parameter;
                    if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                        App.addLoad(true, "Carregando");
                    var ok = await LoadPost();
                    if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                        App.addLoad(false, "Carregando");
                }
            }
            catch { }
        }
        private async Task<bool> LoadPost()
        {
            byte[] img = Convert.FromBase64String(this.post.image);
            WriteableBitmap originalBitmap = new WriteableBitmap(400, 360).FromByteArray(img, img.Length);
            imgPost.Source = originalBitmap;
            description.Text = this.post.description;
            var map = await LoadMap();
            return true;
        }
        private async Task<bool> LoadMap()
        {
            MapIcon mapIcon = new MapIcon();
            // Locate your MapIcon  
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            // Show above the MapIcon  
            mapIcon.Title = "Local da Oferta";
            // Setting up MapIcon location  
            mapIcon.Location = new Geopoint(new BasicGeoposition()
            {
                //Latitude = geoposition.Coordinate.Latitude, [Don't use]  
                //Longitude = geoposition.Coordinate.Longitude [Don't use]  
                Latitude = double.Parse(this.post.geo_x.ToString()),
                Longitude = double.Parse(this.post.geo_y.ToString())
            });
            // Positon of the MapIcon  
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            myMap.MapElements.Add(mapIcon);
            // Showing in the Map  
            await myMap.TrySetViewAsync(mapIcon.Location, 18D, 0, 0, MapAnimationKind.Bow);
            return true;
        }

        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {

            this.Frame.GoBack();
        }
    }
}
