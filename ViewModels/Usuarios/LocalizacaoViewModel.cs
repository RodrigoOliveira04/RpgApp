namespace AppRpgEtec.ViewModels.Usuarios;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

public class LocalizacaoViewModel : BaseViewModel
{
	private Map meuMapa;

	public Map MeuMapa
	{
		get => meuMapa;
		set
		{
			if(value != null)
			{
				meuMapa = value;
				OnPropertyChanged();
            }
		}
    }

	public async void InicializarMapa()
	{
		try
		{
			Location location = new Location (-23.5200241d, -46.596498d);
			Pin pinEtec = new Pin()
			{
				Type = PinType.Place,
				Label= "Etec Horácio",
				Address = "RUa Alcântara, 113, Vila Guilherme",
				Location = location
			};
			Map map = new Map();
			MapSpan mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));
			map.Pins.Add(pinEtec);
			map.MoveToRegion(mapSpan);
        }
		catch (Exception ex)
		{
			await App.Current.MainPage.DisplayAlert("Atenção", ex.Message, "OK");
        }
    }
}