using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models.Personagens;
using AppRpgEtec.Services.Personagens;


namespace AppRpgEtec.ViewModels.Personagens
{
    public class ListagemPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;

        public ObservableCollection<Personagem> Personagems { get; set; }

        public ListagemPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            Personagems = new ObservableCollection<Personagem>();

            _ = ObterPersonagens();

            NovoPersonagemCommand = new Command(async () => { await ExibirCadastroPersonagem();});
            RemoverPersonagemCommand = new Command<Personagem>(async (Personagem p) => { await RemoverPersonagem(p); }); 
        }

        public ICommand NovoPersonagemCommand { get; }
        public ICommand RemoverPersonagemCommand { get; set; }

        public async Task ObterPersonagens()
        {
            try
            {
                Personagems = await pService.GetPersonagemsAsync();
                OnPropertyChanged(nameof(Personagems));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }

        }

        public async Task ExibirCadastroPersonagem()
        {
            try
            {
                await Shell.Current.GoToAsync("cadPersonagemView");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");

            }
        }

        public async Task RemoverPersonagem(Personagem p)
        {
            try
            {
                if (await Application.Current.MainPage.DisplayAlert("Confirmação", $"Confirma a remoção de {p.Nome}?", "Sim", "Não"))
                {
                    await pService.DeletePersonagemAsync(p.Id);

                    await Application.Current.MainPage.DisplayAlert("Mensagem", "Personagem removido com sucesso!", "Ok");

                    _ = ObterPersonagens();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private Personagem personagemSelecionado;

        public Personagem PersonagemSelecionado
        {
            get { return personagemSelecionado; }
            set
            {
                if(value != null)
                {
                    personagemSelecionado = value;
                    Shell.Current.GoToAsync($"cadPersonagemView?pID={personagemSelecionado.Id}");
                }
            }
        }
    }   
}

