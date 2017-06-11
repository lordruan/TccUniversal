using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Async;
using SQLite.Net;
using System.IO;
using Windows.Storage;
using SQLite.Net.Platform.WinRT;
using SQLite.Net.Interop;
using Windows.UI.Xaml;

namespace TccUniversal
{
    public class ServicoDados : IServicoDados
    {
        private SQLiteAsyncConnection _conexao;
        public ServicoDados()
        {
            string databasePath = Constantes.DatabasePath;
            var connection = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), new SQLiteConnectionString(databasePath, storeDateTimeAsTicks: false)));
            _conexao = new SQLiteAsyncConnection(connection);

        }
        public SQLiteAsyncConnection Contexto
        {
            get
            {
                return _conexao;
            }
        }

        public async Task InicializarBaseDeDados(int versao)
        {
            bool existe = false;
            try
            {
                var dbFile = await StorageFile.GetFileFromPathAsync(Constantes.DatabasePath);
                existe = true;
            }
            catch (FileNotFoundException)
            {
                existe = false;
            }

            if (!existe)
            {
                //await ApplicationData.Current.LocalFolder.CreateFolderAsync("database", CreationCollisionOption.ReplaceExisting);
                await _conexao.CreateTableAsync<UserResponse>();
                await _conexao.CreateTableAsync<PostsResponse>();
            }
            else
            {
                //Aqui é feito a atualização do banco de dados (alteração de estrutura)
                //SQLiteAsyncConnection db = new SQLiteAsyncConnection(Constantes.DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, false);

                //var config = await _conexao.Table<UserResponse>().Where(c => c.Nome == "versao").FirstOrDefaultAsync();
                //if (config == null)
                /*{
                    await _conexao.InsertAsync(new Configuracao() { Nome = "versao", Valor = "1", Sincronizado = false });
                }*/
            }
        }
        public async Task<bool> EstaLogado()
        {
            var user = await _conexao.Table<UserResponse>().FirstOrDefaultAsync();
            if (user != null)
            {
                var a = Application.Current as App;
                a.usuarioLogado = user;
                return true;
            }
            return false;
        }

        public async Task InserirUsuarioLogado(UserResponse user)
        {
            var userLogado = await _conexao.Table<UserResponse>().FirstOrDefaultAsync();
            if (userLogado != null)
            {
                await _conexao.DeleteAsync(userLogado);
            }
            await _conexao.InsertAsync(user);
        }
        public async Task SairUsuario()
        {
            var userLogado = await _conexao.Table<UserResponse>().FirstOrDefaultAsync();
            if (userLogado != null)
            {
                await _conexao.DeleteAsync(userLogado);
            }
        }
        public async Task InserirPost(PostsResponse post)
        {
            await _conexao.InsertAsync(post);
        }
    }
}
