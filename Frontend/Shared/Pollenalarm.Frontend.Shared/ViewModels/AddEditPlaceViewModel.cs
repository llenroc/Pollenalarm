﻿using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class AddEditPlaceViewModel : AsyncViewModelBase
    {
        private INavigationService _NavigationService;
        private IFileSystemService _FileSystemService;
        private IDialogService _DialogService;
        private ILocalizationService _LocalizationService;
        private PlaceService _PlaceService;
        private IPollenService _PollenService;

        private string _PlaceName;
        public string PlaceName
        {
            get { return _PlaceName; }
            set { _PlaceName = value; RaisePropertyChanged(); }
        }

        private string _PlaceZip;
        public string PlaceZip
        {
            get { return _PlaceZip; }
            set { _PlaceZip = value; RaisePropertyChanged(); }
        }

        private RelayCommand _AddEditPlaceCommand;
        public RelayCommand AddEditPlaceCommand
        {
            get
            {
                return _AddEditPlaceCommand ?? (_AddEditPlaceCommand = new RelayCommand(async () =>
                {
                    // Check if entered field are valid
                    if (string.IsNullOrWhiteSpace(_PlaceName) || !Regex.IsMatch(_PlaceZip, "^[0-9]*$") || _PlaceZip.Trim().Length != 5)
                    {
                        // Invalid entries
                        await _DialogService.DisplayAlertAsync(_LocalizationService.GetString("InvalidEntriesTitle"), _LocalizationService.GetString("InvalidEntriesMessage"), _LocalizationService.GetString("OK"));
                        return;
                    }

                    IsBusy = true;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();

                    if (_PlaceService.CurrentPlace != null)
                    {
                        // Update existing place
                        _PlaceService.CurrentPlace.Name = _PlaceName;
                        _PlaceService.CurrentPlace.Zip = _PlaceZip;
                        await _PlaceService.UpdatePlaceAsync(_PlaceService.CurrentPlace);
                    }
                    else
                    {
                        // Add new place
                        var place = new Place();
                        place.Name = _PlaceName;
                        place.Zip = _PlaceZip;
                        await _PlaceService.AddPlaceAsync(place);
                    }

                    // Trigger MainViewModel refresh on specific changes
                    var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                    mainViewModel.IsLoaded = false;

                    _PlaceName = string.Empty;
                    _PlaceZip = string.Empty;
                    _NavigationService.GoBack();

                    IsBusy = false;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();
                }, () => !IsBusy));
            }
        }

        private RelayCommand _DeletePlaceCommand;
        public RelayCommand DeletePlaceCommand
        {
            get
            {
                return _DeletePlaceCommand ?? (_DeletePlaceCommand = new RelayCommand(async () =>
                {
                    // Let user confirm deletion
                    if (!await _DialogService.DisplayConfirmationAsync(_LocalizationService.GetString("DeletePlaceTitle"), _LocalizationService.GetString("DeletePlaceMessage"), _LocalizationService.GetString("Delete"), _LocalizationService.GetString("Cancel")))
                        return;

                    IsBusy = true;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();

                    await _PlaceService.DeletePlaceAsync(_PlaceService.CurrentPlace);
                    _PlaceService.CurrentPlace = null;

                    _PlaceName = string.Empty;
                    _PlaceZip = string.Empty;
                    _NavigationService.GoBack();

                    IsBusy = false;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();
                }));
            }
        }

        private RelayCommand _GetCurrentPositionCommand;
        public RelayCommand GetCurrentPositionCommand
        {
            get
            {
                return _GetCurrentPositionCommand ?? (_GetCurrentPositionCommand = new RelayCommand(async () =>
                {
                    IsBusy = true;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();

                    var geolocation = await _PlaceService.GetCurrentGeoLocationAsync();
                    if (geolocation == null)
                    {
                        IsBusy = false;
                        AddEditPlaceCommand.RaiseCanExecuteChanged();
                        DeletePlaceCommand.RaiseCanExecuteChanged();
                        GetCurrentPositionCommand.RaiseCanExecuteChanged();

                        await _DialogService.DisplayAlertAsync(_LocalizationService.GetString("GeoLocationFailedTitle"), _LocalizationService.GetString("GeoLocationFailedMessage"), _LocalizationService.GetString("OK"));
                        return;
                    }

                    // Update place fields
                    PlaceName = geolocation.Name;
                    PlaceZip = geolocation.Zip;

                    IsBusy = false;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();
                }, () => !IsBusy));
            }
        }

        public AddEditPlaceViewModel(INavigationService navigationService, IFileSystemService fileSystemService, IDialogService dialogService, ILocalizationService localizationService, PlaceService placeService, IPollenService pollenService)
        {
            _NavigationService = navigationService;
            _FileSystemService = fileSystemService;
            _DialogService = dialogService;
            _LocalizationService = localizationService;
            _PlaceService = placeService;
            _PollenService = pollenService;
        }

        public void Refresh()
        {
            if (_PlaceService.CurrentPlace != null)
            {
                PlaceName = _PlaceService.CurrentPlace.Name;
                PlaceZip = _PlaceService.CurrentPlace.Zip;
            }
            else
            {
                PlaceName = string.Empty;
                PlaceZip = string.Empty;
            }
        }
    }
}
