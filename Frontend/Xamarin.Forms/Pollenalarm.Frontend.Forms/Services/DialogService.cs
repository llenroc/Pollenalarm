﻿using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Forms.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayConfirmationAsync(string title, string message, string confirm, string cancel)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, confirm, cancel);
        }
    }
}