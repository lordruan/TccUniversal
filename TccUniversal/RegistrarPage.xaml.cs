using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class RegistrarPage : Page
    {
        public RegistrarPage()
        {
            this.InitializeComponent();
        }
        public async Task<bool> RegistrarUsario()
        {
            Uri url = new Uri("http://localhost/api/users/");
            User user = new User();
            user.email = txtEmail.Text;
            user.login = txtLogin.Text;
            user.password = txtSenha.Password;
            user.active = true;
            try
            {
                var client = new HttpClient();
                var uri = new Uri(string.Format("http://192.168.43.169/api/users/", "action", "post", DateTime.Now.Ticks));
                string serialized = JsonConvert.SerializeObject(user);

                StringContent stringContent = new StringContent(
                    serialized,
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(uri, stringContent);
                HttpResponseMessage x = await response;
                if ((x.StatusCode != System.Net.HttpStatusCode.OK) || (x.StatusCode != System.Net.HttpStatusCode.Created))
                {
                   //MessageDialog errorbox = new MessageDialog("While puting: http://192.168.43.169/api/users/ we got the following status code: " + x.StatusCode);
                    //await errorbox.ShowAsync(); 

                }
                HttpContent requestContent = x.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                var retorno = JsonConvert.DeserializeObject<UserResponse>(jsonContent);
                App a = Application.Current as App;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    App.addLoad(false, "");
                a.usuarioLogado = retorno;
                return true;
            }
            catch (Exception ex)
            {
                //
            }
            return false;
        }
        private async void btnRegistrar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Validacoes.validarEmail(txtEmail.Text))
            {
                if (txtLogin.Text != "")
                {
                    if (txtSenha.Password != "")
                    {
                        if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                            App.addLoad(true, "Registrando");
                        var retorno = await RegistrarUsario();
                        if (retorno)
                            Frame.Navigate(typeof(FeedPage));
                        else
                        {
                            MessageDialog errorBox = new MessageDialog("Usuario não pode ser cadastrado!");
                            await errorBox.ShowAsync();
                        }
                        if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                            App.addLoad(false, "");
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
            else
            {
                MessageDialog msgbox = new MessageDialog("Preencha seu email corretamente.");
                await msgbox.ShowAsync();
            }

        }

        private void btnLogin_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
