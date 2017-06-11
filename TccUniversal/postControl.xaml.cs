using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TccUniversal
{
    public sealed partial class postControl : UserControl
    {
        public PostsResponse post { get; set; }
        public postControl()
        {
            this.InitializeComponent();
        }

        private async void dafoultNewPost_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var retorno = this.post;
            await Task.Delay(TimeSpan.FromSeconds(3));
            App.rootFrame.Navigate(typeof(PostPage), retorno);
        }
    }
}
