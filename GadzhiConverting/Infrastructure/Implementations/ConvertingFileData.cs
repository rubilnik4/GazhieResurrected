﻿using GadzhiApplicationCommon.Models.Interfaces.ApplicationLibrary.Document;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Extensions.Functional;
using GadzhiCommon.Infrastructure.Implementations;
using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiCommon.Models.Implementations.Errors;
using GadzhiCommon.Models.Interfaces.Errors;
using GadzhiConverting.Infrastructure.Interfaces;
using GadzhiConverting.Infrastructure.Interfaces.ApplicationConvertingPartial;
using GadzhiConverting.Models.Implementations.FilesConvert;
using GadzhiConverting.Models.Interfaces.Printers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GadzhiCommon.Extensions.Functional.Result;
using GadzhiCommon.Extensions.StringAdditional;
using GadzhiConverting.Models.Interfaces.FilesConvert;

namespace GadzhiConverting.Infrastructure.Implementations
{
    public class ConvertingFileData : IConvertingFileData
    {
        /// <summary>
        /// Класс для отображения изменений и логгирования
        /// </summary>
        private readonly IMessagingService _messagingService;

        /// <summary>
        /// Обработка и конвертирование файла
        /// </summary>
        private readonly IApplicationConverting _applicationConverting;

        /// <summary>
        /// Проверка состояния папок и файлов
        /// </summary>   
        private readonly IFileSystemOperations _fileSystemOperations;

        public ConvertingFileData(IMessagingService messagingService, IApplicationConverting applicationConverting,
                                  IFileSystemOperations fileSystemOperations)
        {
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _applicationConverting = applicationConverting ?? throw new ArgumentNullException(nameof(applicationConverting));
            _fileSystemOperations = fileSystemOperations ?? throw new ArgumentNullException(nameof(fileSystemOperations));
        }

        /// <summary>
        /// Конвертировать файл
        /// </summary>
        public async Task<IFileDataServer> Converting(IFileDataServer fileDataServer, IConvertingSettings convertingSettings) =>
            await fileDataServer.
            Void(fileData => _messagingService.ShowAndLogMessage($"Конвертация файла {fileDataServer.FileNameClient}")).
            WhereContinueAsyncBind(fileData => fileData.IsValidByAttemptingCount,
                okFunc: fileData => Task.Run(() => ConvertingFile(fileData, convertingSettings, ProjectSettings.PrintersInformation)),
                badFunc: fileData => Task.FromResult(GetErrorByAttemptingCount(fileDataServer)));

        /// <summary>
        /// Запустить конвертацию. Инициировать начальные значения
        /// </summary>      
        private IFileDataServer ConvertingFile(IFileDataServer fileDataServer, IConvertingSettings convertingSettings, IPrintersInformation printersInformation) =>
            LoadAndSaveDocument(fileDataServer).
            ResultValueOkBind(document =>
                GetSavedFileDataSource(document, fileDataServer).
                ToResultCollection().
                Map(saveResult => MakeConvertingFileActions(saveResult, document, fileDataServer, convertingSettings, printersInformation)).
                Map(filesData => CloseFile(document, fileDataServer.FilePathServer, fileDataServer.FileNameClient).
                                 Map(closeResult => filesData.ConcatErrors(closeResult.Errors)).
                                 ToResultCollection())).
            Map(result => new FileDataServer(fileDataServer, StatusProcessing.ConvertingComplete, result.Value,
                                             result.Errors.Select(error => error.FileConvertErrorType)));

        /// <summary>
        /// Печать и экспорт файла
        /// </summary>
        private IResultCollection<IFileDataSourceServer> MakeConvertingFileActions(IResultCollection<IFileDataSourceServer> fileDataSourceServer,
                                                                                   IDocumentLibrary documentLibrary, IFileDataServer fileDataServer,
                                                                                   IConvertingSettings convertingSettings, IPrintersInformation printersInformation) =>
            fileDataSourceServer.
            ResultValueEqualOkRawCollection(saveResult => CreatePdf(documentLibrary, fileDataServer, convertingSettings, printersInformation).
                                                          Map(saveResult.ConcatResult).
                                                          Map(filesData => ExportFile(documentLibrary, fileDataServer).
                                                                           Map(filesData.ConcatResultValue))).
                                                          Map(CheckDataSourceExistence);
        /// <summary>
        /// Присвоить ошибку по количеству попыток конвертирования
        /// </summary>      
        private IFileDataServer GetErrorByAttemptingCount(IFileDataServer fileDataServer) =>
            new ErrorCommon(FileConvertErrorType.AttemptingCount, "Превышено количество попыток конвертирования файла").
            Void(errorDataServer => _messagingService.ShowAndLogError(errorDataServer)).
            Map(errorDataServer => new FileDataServer(fileDataServer, StatusProcessing.ConvertingComplete,
                                                      errorDataServer.Select(error => error.FileConvertErrorType)));

        /// <summary>
        /// Загрузить файл и сохранить в папку для обработки
        /// </summary>   
        private IResultValue<IDocumentLibrary> LoadAndSaveDocument(IFilePath filePath) =>
            new ResultError().
            ResultVoidOk(_ => _messagingService.ShowAndLogMessage("Загрузка файла")).
            ResultValueOkBind(_ => CreateSavingPathByExtension(filePath.FilePathServer, filePath.FileExtension)).
            ResultValueOkBind(savingPath => _fileSystemOperations.CopyFile(filePath.FilePathServer, savingPath).
                                            WhereContinue(copyResult => copyResult,
                                                okFunc: _ => new ResultValue<string>(savingPath),
                                                badFunc: _ => new ErrorCommon(FileConvertErrorType.FileNotSaved, $"Ошибка сохранения файла {savingPath}").
                                            ToResultValue<string>())).
            ResultValueOkBind(_ => _applicationConverting.OpenDocument(filePath.FilePathServer)).
            Void(result => _messagingService.ShowAndLogErrors(result.Errors));

        /// <summary>
        /// Получить путь к сохраненному файлу для обработки
        /// </summary>        
        private static IResultValue<IFileDataSourceServer> GetSavedFileDataSource(IDocumentLibrary documentLibrary, IFilePath filePath) =>
            new FileDataSourceServer(documentLibrary.FullName, filePath.FilePathClient).
            Map(fileDataSource => new ResultValue<IFileDataSourceServer>(fileDataSource));

        /// <summary>
        /// Создать PDF
        /// </summary>
        private IResultCollection<IFileDataSourceServer> CreatePdf(IDocumentLibrary documentLibrary, IFileDataServer fileDataServer,
                                                                   IConvertingSettings convertingSettings, IPrintersInformation printersInformation) =>
            new ResultError().
            ResultVoidOk(_ => _messagingService.ShowAndLogMessage("Создание файлов PDF")).
            ResultValueOkBind(_ => CreateSavingPathByExtension(fileDataServer.FilePathServer, FileExtension.Pdf)).
            ResultValueOkBind(filePdfPath => _applicationConverting.CreatePdfFile(documentLibrary, fileDataServer.ChangeServerPath(filePdfPath),
                                                                                  convertingSettings, fileDataServer.ColorPrint,
                                                                                  printersInformation?.PrintersPdf.FirstOrDefault())).
            Void(result => _messagingService.ShowAndLogErrors(result.Errors)).
            ToResultCollection();

        /// <summary>
        /// Экспортировать в другие форматы
        /// </summary>
        private IResultValue<IFileDataSourceServer> ExportFile(IDocumentLibrary documentLibrary, IFilePath filePath) =>
            new ResultError().
            ResultVoidOk(_ => _messagingService.ShowAndLogMessage("Экспорт файла")).
            ResultValueOkBind(_ => CreateSavingPathByExtension(filePath.FilePathServer,
                                                               _applicationConverting.GetExportFileExtension(filePath.FileExtension))).
            ResultValueOkBind(fileExportPath => _applicationConverting.CreateExportFile(documentLibrary, filePath.ChangeServerPath(fileExportPath))).
            Void(result => _messagingService.ShowAndLogErrors(result.Errors));

        /// <summary>
        /// Закрыть файл
        /// </summary>
        private IResultError CloseFile(IDocumentLibrary documentLibrary, string filePathServer, string fileNameClient) =>
            _applicationConverting.CloseDocument(documentLibrary, filePathServer).
            Void(_ => _messagingService.ShowAndLogMessage($"Конвертация файла {fileNameClient} завершена"));

        /// <summary>
        /// Создать папку для сохранения отконвертированных файлов по типу расширения
        /// </summary>       
        private IResultValue<string> CreateSavingPathByExtension(string filePathServer, FileExtension fileExtension) =>
            new ResultValue<string>(filePathServer).
            ResultValueOk(Path.GetDirectoryName).
            ResultValueOk(directory => _fileSystemOperations.CreateFolderByName(directory, fileExtension.ToString())).
            ResultValueOk(serverDirectory => FileSystemOperations.CombineFilePath(serverDirectory,
                                                                                  Path.GetFileNameWithoutExtension(filePathServer),
                                                                                  fileExtension.ToString().ToLowerCaseCurrentCulture()));

        /// <summary>
        /// Проверить наличие сохраненных файлов
        /// </summary>
        private IResultCollection<IFileDataSourceServer> CheckDataSourceExistence(IResultCollection<IFileDataSourceServer> fileDataSourceResult) =>
            fileDataSourceResult.Value.
            Where(fileDataSource => !_fileSystemOperations.IsFileExist(fileDataSource.FilePathServer)).
            ToList() .
            Map(fileDataSources => new
            {
                fileDataSources,
                errors = fileDataSources.Select(fileDataSource => new ErrorCommon(FileConvertErrorType.FileNotFound, 
                                                                                  $"Файл {fileDataSource.FilePathServer} не найден"))
            }).
            Void(filesOrErrors => _messagingService.ShowAndLogErrors(filesOrErrors.errors)).
            Map(filesOrErrors => new ResultCollection<IFileDataSourceServer>(fileDataSourceResult.Value.Except(filesOrErrors.fileDataSources),
                                                                             fileDataSourceResult.Errors.Concat(filesOrErrors.errors)));

    }
}
