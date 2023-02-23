﻿using System.ComponentModel;
using System.Globalization;

namespace SmartEnergy.Localization
{
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        private LocalizationResourceManager()
        {
            Strings.Culture = CultureInfo.CurrentCulture;
        }

        public static LocalizationResourceManager Instance { get; } = new();

        public object this[string resourceKey]
            => Strings.ResourceManager.GetObject(resourceKey, Strings.Culture) ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetCulture(CultureInfo culture)
        {
            Strings.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
