﻿using ConvertingModels.Models.Interfaces.FilesConvert;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Infrastructure.Implementations;
using GadzhiConverting.Models.Interfaces.FilesConvert;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GadzhiConverting.Models.Implementations.FilesConvert
{
    /// <summary>
    /// Класс для хранения информации о конвертируемом файле в базовом виде
    /// </summary>
    public class FileDataServer : IFileDataServer, IEquatable<FileDataServer>
    {
        ///// <summary>
        ///// Пути отконвертированных файлов
        ///// </summary>
        //private IReadOnlyList<IFileDataSourceServer> _fileDatasSourceServerBase;

        ///// <summary>
        ///// Тип ошибки при конвертации файла
        ///// </summary>
        //private readonly List<FileConvertErrorType> _fileConvertErrorTypesBase;

        public FileDataServer(string filePathServer, string filePathClient, ColorPrint colorPrint)
            : this(filePathServer, filePathClient, colorPrint, Enumerable.Empty<FileConvertErrorType>())
        { }

        public FileDataServer(string filePathServer, string filePathClient, ColorPrint colorPrint,
                              IEnumerable<FileConvertErrorType> fileConvertErrorType)
            : this(filePathServer, filePathClient, colorPrint, Enumerable.Empty<IFileDataSourceServer>(), fileConvertErrorType)
        { }

        public FileDataServer(string filePathServer, string filePathClient, ColorPrint colorPrint,
                              IEnumerable<IFileDataSourceServer> fileDatasSourceServer, IEnumerable<FileConvertErrorType> filesConvertErrorType)
        {
            string fileType = FileSystemOperations.ExtensionWithoutPointFromPath(filePathServer);

            if (!ValidFileExtentions.DocAndDgnFileTypes.Keys.Contains(fileType))
            {
                throw new KeyNotFoundException(nameof(filePathServer));
            }
            FileExtentionType = ValidFileExtentions.DocAndDgnFileTypes[fileType.ToLower(CultureInfo.CurrentCulture)];
            FilePathServer = filePathServer;
            FilePathClient = filePathClient;
            ColorPrint = colorPrint;

            FileConvertErrorTypes = filesConvertErrorType;
            FileDatasSourceServer = fileDatasSourceServer;
        }

        /// <summary>
        /// Тип расширения файла
        /// </summary>
        public FileExtention FileExtentionType { get; }

        /// <summary>
        /// Путь файла на сервере
        /// </summary>
        public string FilePathServer { get; }

        /// <summary>
        /// Путь файла на клиенте
        /// </summary>
        public string FilePathClient { get; }

        /// <summary>
        /// Имя файла на клиенте
        /// </summary>
        public string FileNameClient => Path.GetFileName(FilePathClient);

        /// <summary>
        /// Имя файла без расширения на клиенте
        /// </summary>
        public string FileNameWithoutExtensionClient => Path.GetFileNameWithoutExtension(FilePathClient);

        /// <summary>
        /// Цвет печати
        /// </summary>
        public ColorPrint ColorPrint { get; }

        /// <summary>
        /// Путь и тип отконвертированных файлов
        /// </summary>
        public IEnumerable<IFileDataSourceServer> FileDatasSourceServer { get; }

        /// <summary>
        /// Тип ошибки при конвертации файла
        /// </summary>
        public IEnumerable<FileConvertErrorType> FileConvertErrorTypes { get; }

        /// <summary>
        /// Статус обработки файла
        /// </summary>
        public StatusProcessing StatusProcessing { get; set; }

        /// <summary>
        /// Завершена ли обработка файла
        /// </summary>
        public bool IsCompleted => CheckStatusProcessing.CompletedStatusProcessingServer.Contains(StatusProcessing);

        /// <summary>
        /// Количество попыток конвертирования
        /// </summary>      
        public int AttemptingConvertCount { get; set; }

        /// <summary>
        /// Корректна ли модель
        /// </summary>
        public bool IsValid => IsValidByErrorType && IsValidByAttemptingCount;

        /// <summary>
        /// Присутствуют ли ошибки конвертирования
        /// </summary>
        public bool IsValidByErrorType => FileConvertErrorTypes.Any();

        /// <summary>
        /// Не превышает ли количество попыток конвертирования
        /// </summary>
        public bool IsValidByAttemptingCount => AttemptingConvertCount <= 2;

        /// <summary>
        /// Установить пути для отконвертированных файлов
        /// </summary>
        //public void SetFileDatasSourceServerConverting(IEnumerable<IFileDataSourceServer> fileDatasSourceServer)
        //{
        //    _fileDatasSourceServerBase = new List<IFileDataSourceServer>(fileDatasSourceServer ?? Enumerable.Empty<IFileDataSourceServer>());
        //}

        ///// <summary>
        ///// Добавить ошибку
        ///// </summary>
        //public void AddFileConvertErrorType(FileConvertErrorType fileConvertErrorType)
        //{
        //    _fileConvertErrorTypesBase.Add(fileConvertErrorType);
        //}

        ///// <summary>
        ///// Добавить ошибки
        ///// </summary>
        //public void AddRangeFileConvertErrorType(IEnumerable<FileConvertErrorType> fileConvertErrorTypes)
        //{
        //    _fileConvertErrorTypesBase.AddRange(fileConvertErrorTypes ?? Enumerable.Empty<FileConvertErrorType>());
        //}

        public override bool Equals(object obj)
        {
            return obj is FileDataServer fileDataServer &&
                   Equals(fileDataServer);
        }

        public bool Equals(FileDataServer other)
        {
            return FilePathServer == other?.FilePathServer;
        }

        public override int GetHashCode()
        {
            return -1576186305 + FilePathServer.GetHashCode();
        }
    }
}
