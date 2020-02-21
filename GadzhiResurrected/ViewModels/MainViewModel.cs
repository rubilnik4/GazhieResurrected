﻿using GadzhiModules.Modules;
using Prism.Mvvm;

namespace GadzhiResurrected.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {

        }

        /// <summary>
        /// Название и версия
        /// </summary>
        private string _title = "Gadzhi";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        /// Название модуля для конвертации файлов
        /// </summary>
        public string FilesConvertRegionName => RegionNames.FilesConvertModule;
    }
}
