﻿using GadzhiApplicationCommon.Models.Interfaces.ApplicationLibrary.Application;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiCommon.Models.Implementations.Errors;
using GadzhiCommon.Models.Interfaces.Errors;
using GadzhiConverting.Infrastructure.Interfaces;
using GadzhiConverting.Infrastructure.Interfaces.ApplicationConvertingPartial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChannelAdam.ServiceModel;
using GadzhiApplicationCommon.Models.Implementation.LibraryData;
using GadzhiApplicationCommon.Models.Interfaces.ApplicationLibrary.Document;
using GadzhiDTOServer.Contracts.FilesConvert;
using GadzhiMicrostation.Microstation.Interfaces.DocumentMicrostationPartial;
using GadzhiWord.Word.Interfaces;
using GadzhiCommon.Extensions.Functional;
using GadzhiConverting.Infrastructure.Implementations.Converters;

namespace GadzhiConverting.Infrastructure.Implementations.ApplicationConvertingPartial
{
    /// <summary>
    /// Класс для работы с приложениями конвертации
    /// </summary>
    public partial class ApplicationConverting : IApplicationConverting
    {
        /// <summary>
        /// Модуль конвертации Microstation
        /// </summary>   
        private readonly IApplicationLibrary<IDocumentMicrostation> _applicationMicrostation;

        /// <summary>
        /// Модуль конвертации Word
        /// </summary>   
        private readonly IApplicationLibrary<IDocumentWord> _applicationWord;

        /// <summary>
        /// Проверка состояния папок и файлов
        /// </summary>   
        private readonly IFileSystemOperations _fileSystemOperations;

        /// <summary>
        /// Управление печатью пдф
        /// </summary>
        private readonly IPdfCreatorService _pdfCreatorService;

        public ApplicationConverting(IApplicationLibrary<IDocumentMicrostation> applicationMicrostation, 
                                     IApplicationLibrary<IDocumentWord> applicationWord,
                                     IFileSystemOperations fileSystemOperations, IPdfCreatorService pdfCreatorService)
        {
            _applicationMicrostation = applicationMicrostation ?? throw new ArgumentNullException(nameof(applicationMicrostation));
            _applicationWord = applicationWord ?? throw new ArgumentNullException(nameof(applicationWord));
            _fileSystemOperations = fileSystemOperations ?? throw new ArgumentNullException(nameof(fileSystemOperations));
            _pdfCreatorService = pdfCreatorService ?? throw new ArgumentNullException(nameof(pdfCreatorService));
        }

        /// <summary>
        /// Выбрать библиотеку конвертации по типу расширения
        /// </summary>        
        public FileExtension GetExportFileExtension(FileExtension fileExtensionMain) =>
            fileExtensionMain switch
            {
                FileExtension.Dgn => FileExtension.Dwg,
                FileExtension.Docx => FileExtension.Xlsx,
                _ => throw new InvalidEnumArgumentException(nameof(fileExtensionMain), (int)fileExtensionMain, typeof(FileExtension))
            };

        /// <summary>
        /// Выбрать библиотеку конвертации по типу расширения
        /// </summary>        
        private IResultValue<IApplicationLibrary<IDocumentLibrary>> GetActiveLibraryByExtension(FileExtension fileExtension) =>
            fileExtension switch
            {
                FileExtension.Dgn => new ResultValue<IApplicationLibrary<IDocumentLibrary>>(_applicationMicrostation),
                FileExtension.Docx => new ResultValue<IApplicationLibrary<IDocumentLibrary>>(_applicationWord),
                _ => new ErrorCommon(FileConvertErrorType.LibraryNotFound, $"Библиотека конвертации для типа {fileExtension} не найдена").
                     ToResultValue<IApplicationLibrary<IDocumentLibrary>>()
            };
    }
}
