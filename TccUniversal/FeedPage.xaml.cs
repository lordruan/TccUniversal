using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class FeedPage : Page
    {
        App app = Application.Current as App;
        bool log = false;
        public FeedPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            txtPerfil.Text =app.usuarioLogado.login;
            if(!log)
            LoadPosts();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (!e.Parameter.Equals(null))   // I've also used if(data != null) which hasn't worked either
                {
                    LoadPosts();
                }
            }
            catch { }
        }

        private static readonly IEnumerable<string> SupportedImageFileTypes = new List<string> { ".jpeg", ".jpg", ".png" };
        private WriteableBitmap originalBitmap;
        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {
            meuSplitView.IsPaneOpen = !meuSplitView.IsPaneOpen;
        }

        private void btnFiltro_Click(object sender, RoutedEventArgs e)
        {
            filtroSplit.IsPaneOpen = !filtroSplit.IsPaneOpen;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
        private async void sair_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var servicoDados = new ServicoDados();
            await servicoDados.SairUsuario();
            app.Exit();
        }
        private async void postar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var fop = new FileOpenPicker();
            foreach (var fileType in SupportedImageFileTypes)
            {
                fop.FileTypeFilter.Add(fileType);
            }
            var imageFile = await fop.PickSingleFileAsync();
            var imageExt = imageFile.FileType.ToLower();
            if (imageFile != null)
            {
                using (var stream = await imageFile.OpenReadAsync())
                {
                    originalBitmap = await new WriteableBitmap(1, 1).FromStream(stream);
                    originalBitmap = originalBitmap.Resize(400, 360, WriteableBitmapExtensions.Interpolation.Bilinear);
                    Posts post = new Posts();
                    post.users_id = app.usuarioLogado.id;
                    app.imgTemp = originalBitmap;
                    Frame.Navigate(typeof(EditingPage), post);
                }

            }
        }
        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                //Indicate the back button press is handled so the app does not exit  
                e.Handled = true;
            }
        }

        private void update_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LoadPosts();
        }
        private async void LoadPosts()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            App.addLoad(true, "Carregando Posts");
            content.Children.Clear();
            content1.Children.Clear();
            content2.Children.Clear();
            content3.Children.Clear();
            int i = 0;
            var posts = await GetPosts();
            if (App.validador)
            {
                foreach (var post in posts)
                {
                    switch (i)
                    {
                        case 0: definePost(post, "content"); i++; break;
                        case 1: definePost(post, "content1"); i++; break;
                        case 2: definePost(post, "content2"); i++; break;
                        case 3: definePost(post, "content3"); i=0; break;
                    }
                    
                }
                log = true;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
            }
        }
        private void definePost(PostsResponse post, string cont)
        {
            postControl newPost = new postControl();
            newPost.post = post;
            byte[] img = Convert.FromBase64String(post.image);
            originalBitmap = new WriteableBitmap(400, 360).FromByteArray(img, img.Length);
            newPost.imgPost.Source = originalBitmap;
            newPost.description.Text = post.description;
            StackPanel conteudo = (StackPanel)this.FindName(cont);
            conteudo.Children.Add(newPost);
        }
        private async void LoadPostsByCtg(decimal ctg_id)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                App.addLoad(true, "Carregando Posts");
            content.Children.Clear();
            content1.Children.Clear();
            content2.Children.Clear();
            content3.Children.Clear();
            int i = 0;
            var posts = await GetPostsByCtg(ctg_id);
            if (App.validador)
            {
                foreach (var post in posts)
                {
                    switch (i)
                    {
                        case 0: definePost(post, "content"); i++; break;
                        case 1: definePost(post, "content1"); i++; break;
                        case 2: definePost(post, "content2"); i++; break;
                        case 3: definePost(post, "content3"); i = 0; break;
                    }
                }
                log = true;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
            }
        }
        public async Task<List<PostsResponse>> GetPostsByCtg(decimal ctg_id)
        {
            try
            {
                var client = new HttpClient();
                var uri = new Uri(string.Format("http://192.168.43.169/api/posts?category_id=" + ctg_id.ToString(), "action", "get", DateTime.Now.Ticks));
                var response = client.GetAsync(uri);
                HttpResponseMessage x = await response;
                HttpContent requestContent = x.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                App.validador = true;
                return JsonConvert.DeserializeObject<List<PostsResponse>>(jsonContent);
            }
            catch
            {

                MessageDialog msgbox = new MessageDialog("Tente novamente");
                await msgbox.ShowAsync();
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
                App.validador = false;
                return null;
            }
        }
        public async Task<List<PostsResponse>> GetPosts()
        {
            try {
                var client = new HttpClient();
                var uri = new Uri(string.Format("http://192.168.43.169/api/posts", "action", "get", DateTime.Now.Ticks));
                var response = client.GetAsync(uri);
                HttpResponseMessage x = await response;
                HttpContent requestContent = x.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                App.validador = true;
                return JsonConvert.DeserializeObject<List<PostsResponse>>(jsonContent);
            }
            catch
            {

                MessageDialog msgbox = new MessageDialog("Tente novamente");
                await msgbox.ShowAsync();
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
                App.validador = false;
                return null;
            }
        }

        private void ctgCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            decimal category_id = decimal.Parse(ctgCbox.SelectedIndex.ToString());
            category_id++;
            LoadPostsByCtg(category_id);
        }
    }
}
