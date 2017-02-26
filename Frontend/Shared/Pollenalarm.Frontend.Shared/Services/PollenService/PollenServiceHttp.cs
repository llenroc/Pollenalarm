using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pollenalarm.Core;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.Services
{
	public class PollenServiceHttp : PollenService
	{
		private string _BaseUrl;
		private IHttpService _HttpService;
		private List<Pollen> _PollenList;

		public PollenServiceHttp(IHttpService httpService, SettingsService settingsService) : base(settingsService)
		{
			_HttpService = httpService;
			_BaseUrl = "https://pollenalarm.azurewebsites.net/api";
		}

        //public override async Task InitializeAsync() {}

        public override async Task<bool> GetPollutionsForPlaceAsync(Place place)
		{
			// Download pollutions
			var url = $"{_BaseUrl}/pollution?zip={place.Zip}";
			var result = await _HttpService.GetStringAsync(url).ConfigureAwait(false);
			if (result != null)
			{
				// Parse pollutions
				var update = JsonConvert.DeserializeObject<Place>(result);
				place.PollutionToday = update.PollutionToday;
				place.PollutionTomorrow = update.PollutionTomorrow;
				place.PollutionAfterTomorrow = update.PollutionAfterTomorrow;

				// Init settings
				await _SettingsService.LoadSettingsAsync();

				// Update pollen selection
				UpdatePollenSelection(place);

				return true;
			}

			return false;
		}

		public override async Task<List<Pollen>> GetAllPollenAsync()
		{
			if (_PollenList == null || !_PollenList.Any())
			{
				// Download pollen
				var result = await _HttpService.GetStringAsync($"{_BaseUrl}/pollen").ConfigureAwait(false);
				if (result != null)
				{
					// Parse pollen
					_PollenList = JsonConvert.DeserializeObject<List<Pollen>>(result);
				}
			}

			// Update pollen selection
			if (_PollenList != null && _PollenList.Any())
			{
				foreach (var pollen in _PollenList)
					pollen.IsSelected =
						_SettingsService.CurrentSettings.SelectedPollen.ContainsKey(pollen.Id) ?
						_SettingsService.CurrentSettings.SelectedPollen[pollen.Id] : true;
			}

			return _PollenList;
		}
    }
}