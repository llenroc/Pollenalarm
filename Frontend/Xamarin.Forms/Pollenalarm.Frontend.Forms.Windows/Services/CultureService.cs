﻿using Pollenalarm.Frontend.Forms.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Pollenalarm.Frontend.Forms.Windows.Services.CultureService))]

namespace Pollenalarm.Frontend.Forms.Windows.Services
{
    public class CultureService : ICultureService
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}
