using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TccUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }
        public async Task<List<UserResponse>> GetUsers()
        {
            try {
                var client = new HttpClient();
                CultureInfo ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
                client.DefaultRequestHeaders.Add("Accept-Language", ci.TwoLetterISOLanguageName);
                var uri = new Uri(string.Format("http://192.168.43.169/api/users", "action", "get", DateTime.Now.Ticks));
                var response = client.GetAsync(uri);
                HttpResponseMessage x = await response;
                if (x.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageDialog errorbox = new MessageDialog("While posting: " + uri + " we got the following status code: " + x.StatusCode);
                    await errorbox.ShowAsync();

                }
                HttpContent requestContent = x.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                client.Dispose();
                App.validador = true;
                return JsonConvert.DeserializeObject<List<UserResponse>>(jsonContent);
            } catch (HttpRequestException e)
            {
                MessageDialog msgbox = new MessageDialog("Tente novamente");
                await msgbox.ShowAsync();
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
                App.validador = false;
                return null;
            }
        }
        private async void Logar()
        {
            bool erroLogin = true;
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                App.addLoad(true, "Logando");
            var users = await GetUsers();
            var flag = false;
            if (App.validador)
            {
                foreach (var user in users)
                {
                    if (string.Compare(txtLogin.Text.ToString(), user.login) == 0)
                    {
                        erroLogin = false;
                        if (user.password.Equals(txtSenha.Password.ToString()))
                        {
                            App a = Application.Current as App;
                            a.usuarioLogado = user;
                            var servicoDados = new ServicoDados();
                            await servicoDados.InserirUsuarioLogado(user);
                            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                                App.addLoad(false, "");
                            Frame.Navigate(typeof(FeedPage));
                            flag = true;
                        }

                    }
                }
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
                if (erroLogin)
                {
                    MessageDialog msgbox = new MessageDialog("Seu login não foi encontrado.");
                    await msgbox.ShowAsync();
                }
                else
                {
                   if (!flag) {
                        MessageDialog msgbox = new MessageDialog("Sua senha não confere com seu login.");
                        await msgbox.ShowAsync();
                    }
                }
            }
        }
        private void btnRegistrar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistrarPage));
        }

        private async void btnLogin_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (txtLogin.Text != "")
            {
                if (txtSenha.Password != "")
                {
                    Logar();
                }
                else
                {
                    MessageDialog msgbox = new MessageDialog("Preencha sua senha corretamente.");
                    await msgbox.ShowAsync();
                }
            }
            else
            {
                MessageDialog msgbox = new MessageDialog("Preencha seu login corretamente.");
                await msgbox.ShowAsync();
            }
        }
    }
}
