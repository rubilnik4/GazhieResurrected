﻿using GadzhiCommon.Enums.ConvertingSettings;
using GadzhiCommon.Models.Interfaces.Errors;
using GadzhiCommon.Models.Interfaces.LibraryData;
using GadzhiConverting.Models.Interfaces.Printers;

namespace GadzhiConverting.Models.Interfaces.FilesConvert
{
    /// <summary>
    /// Параметры конвертации
    /// </summary>
    public interface IConvertingSettings
    {
        /// <summary>
        /// Идентификатор личной подпись
        /// </summary>
        string PersonId { get; }

        /// <summary>
        /// Принцип именование PDF
        /// </summary>
        PdfNamingType PdfNamingType { get; }

        /// <summary>
        /// Информация о принтере PDF
        /// </summary>
        IResultValue<IPrinterInformation> PdfPrinterInformation { get; }
    }
}