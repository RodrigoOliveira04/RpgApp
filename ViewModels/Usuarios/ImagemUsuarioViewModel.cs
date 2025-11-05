using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class ImagemUsuarioViewModel : BaseViewModel
    {

        private UsuarioService uService;
        private static string conexaoAzureStorage = "COLAR A CHAVE DO AZURE";
        private static string container = "arquivos";

        public ImagemUsuarioViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService();

        }

        private ImageSource fonteImagem;
        private byte[] foto;

        public ImageSource FonteImagem
        {
            get => fonteImagem;
            set
            {
                fonteImagem = value;
                OnPropertyChanged();
            }
        }
        public byte[] Foto
        {
            get => foto;
            set
            {
                foto = value;
                OnPropertyChanged();

            }
        }

        public async void Fotografar()
        {
            try
            {
                if(MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await sourceStream.CopyToAsync(ms);
                                Foto = ms.ToArray();

                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(Foto));

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");

            }
        }

        public async void SalvarImagemAzure()
        {
            try
            {
                Usuario u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);

                string fileName = $"{u.Id}.jpg";

                var blobCliente = new BlobClient(conexaoAzureStorage, container, fileName);

                if(blobCliente.Exists())
                    blobCliente.Delete();

                using (var stream = new MemoryStream(u.Foto))
                {
                    blobCliente.Upload(stream);
                }

                await Application.Current.MainPage
                                    .DisplayAlert("Sucesso",
                                            "Imagem salva no Azure Storage", "Ok");

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                                    .DisplayAlert("ops",
                                            ex.Message +
                                            "Detalhes:" + ex.InnerException, "Ok");
            }
        }
    }
}
