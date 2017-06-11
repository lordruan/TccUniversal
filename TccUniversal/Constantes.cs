using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TccUniversal
{
    public class Constantes
    {
        public static string DatabasePath = Path.Combine(new[] { ApplicationData.Current.LocalFolder.Path, "tccdatabe.sqlite" });
    }
}
