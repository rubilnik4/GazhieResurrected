﻿using System.Collections.Generic;
using GadzhiCommon.Models.Interfaces.LibraryData;
using Nito.Mvvm;

namespace GadzhiModules.Modules.GadzhiConvertingModule.Models.Interfaces.ProjectSettings
{
    /// <summary>
    /// Параметры приложения
    /// </summary>
    public interface IProjectSettings
    {
        /// <summary>
        /// Параметры конвертации
        /// </summary>
        IConvertingSettings ConvertingSettings { get; }
    }
}
