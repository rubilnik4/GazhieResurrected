﻿using System.Collections.Generic;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Models.Interfaces.Errors;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.FileConverting.Information;

namespace GadzhiModules.Modules.GadzhiConvertingModule.Models.Interfaces.FileConverting
{
    /// <summary>
    /// Класс для хранения информации о конвертируемом файле
    /// </summary>
    public interface IFileData
    {
        /// <summary>
        /// Расширение файла
        /// </summary>
        FileExtension FileExtension { get; }

        /// <summary>
        /// Имя файла
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Путь файла
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Цвет печати
        /// </summary>
        ColorPrint ColorPrint { get; }

        /// <summary>
        /// Статус обработки файла
        /// </summary>
        StatusProcessing StatusProcessing { get; }

        /// <summary>
        /// Тип ошибки при конвертации файла
        /// </summary>
        IReadOnlyCollection<IErrorCommon> FileErrors { get; }

        /// <summary>
        /// Статус ошибок
        /// </summary>
        StatusError StatusError { get; }

        /// <summary>
        /// Изменить цвет печати
        /// </summary>
        void SetColorPrint(ColorPrint colorPrint);

        /// <summary>
        /// Изменить статус и вид ошибки при необходимости
        /// </summary>
        IFileData ChangeByFileStatus(FileStatus fileStatus);
    }
}