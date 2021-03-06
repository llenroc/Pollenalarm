﻿using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Forms.Services
{
	public class GeoLocationService : IGeoLoactionService
	{
		private IGeolocator _Locator;

		public GeoLocationService()
		{
			_Locator = CrossGeolocator.Current;
            _Locator.DesiredAccuracy = 2000;
        }

		public async Task<GeoLocation> GetCurrentLocationAsync()
		{
			try
			{
				var position = await _Locator.GetPositionAsync(TimeSpan.FromSeconds(5));

				var geoLocation = new GeoLocation();
				geoLocation.Latitute = position.Latitude;
				geoLocation.Longitute = position.Longitude;

				return geoLocation;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
				return null;
			}
		}
	}
}
