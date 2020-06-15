﻿using ChannelAdam.ServiceModel;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Extensions.Functional;
using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiCommon.Models.Implementations.Errors;
using GadzhiConverting.Infrastructure.Interfaces;
using GadzhiConverting.Infrastructure.Interfaces.Converters;
using GadzhiConverting.Models.Interfaces.FilesConvert;
using GadzhiDTOServer.Contracts.FilesConvert;
using GadzhiDTOServer.TransferModels.FilesConvert;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using GadzhiCommon.Extensions.Functional.Result;
using GadzhiCommon.Extensions.StringAdditional;
using static GadzhiCommon.Infrastructure.Implementations.ExecuteAndCatchErrors;
using static GadzhiCommon.Extensions.Functional.ExecuteTaskHandler;

namespace GadzhiConverting.Infrastructure.Implementations
{
    /// <summary>
    /// Запуск процесса конвертирования
    /// </summary>
    public class ConvertingService : IConvertingService
    {
        /// <summary>
        /// Контейнер зависимостей
        /// </summary>
        private readonly IConvertingFileData _convertingFileData;

        /// <summary>
        /// Параметры приложения
        /// </summary>
        private readonly IProjectSettings _projectSettings;

        /// <summary>
        /// Сервис для добавления и получения данных о конвертируемых пакетах в серверной части, обработки подписей
        /// </summary>     
        private readonly IServiceConsumer<IFileConvertingServerService> _fileConvertingServerService;

        /// <summary>
        /// Конвертер из трансферной модели в серверную
        /// </summary>     
        private readonly IConverterServerPackageDataFromDto _converterServerPackageDataFromDto;

        /// <summary>
        /// Конвертер из серверной модели в трансферную
        /// </summary>
        private readonly IConverterServerPackageDataToDto _converterServerPackageDataToDto;

        /// <summary>
        /// Класс для отображения изменений и логгирования
        /// </summary>
        private readonly IMessagingService _messagingService;

        /// <summary>
        /// Проверка состояния папок и файлов
        /// </summary>   
        private readonly IFileSystemOperations _fileSystemOperations;

        public ConvertingService(IConvertingFileData convertingFileData,
                                 IProjectSettings projectSettings,
                                 IServiceConsumer<IFileConvertingServerService> fileConvertingServerService,
                                 IConverterServerPackageDataFromDto converterServerPackageDataFromDto,
                                 IConverterServerPackageDataToDto converterServerPackageDataToDto,
                                 IMessagingService messagingService,
                                 IFileSystemOperations fileSystemOperations)
        {
            _convertingFileData = convertingFileData ?? throw new ArgumentNullException(nameof(convertingFileData));
            _projectSettings = projectSettings ?? throw new ArgumentNullException(nameof(projectSettings));
            _fileConvertingServerService = fileConvertingServerService ?? throw new ArgumentNullException(nameof(fileConvertingServerService));
            _converterServerPackageDataFromDto = converterServerPackageDataFromDto ?? throw new ArgumentNullException(nameof(converterServerPackageDataFromDto));
            _converterServerPackageDataToDto = converterServerPackageDataToDto ?? throw new ArgumentNullException(nameof(converterServerPackageDataToDto));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _fileSystemOperations = fileSystemOperations ?? throw new ArgumentNullException(nameof(fileSystemOperations));

            _convertingUpdaterSubscriptions = new CompositeDisposable();
            _idPackage = null;
        }

        /// <summary>
        /// Идентификатор конвертируемого пакета
        /// </summary>
        private Guid? _idPackage;

        /// <summary>
        /// Запуск процесса конвертирования
        /// </summary>
        private readonly CompositeDisposable _convertingUpdaterSubscriptions;

        /// <summary>
        /// Запущен ли процесс конвертации
        /// </summary>
        private bool IsConverting { get; set; }

        /// <summary>
        /// Запустить процесс конвертирования
        /// </summary>      
        public async Task StartConverting()
        {
            if (await CheckSignatures() == false) return;

            _messagingService.ShowAndLogMessage("Запуск процесса конвертирования...");
            KillPreviousRunProcesses();

            SubscribeToDataBase();
        }

        /// <summary>
        /// Проверить наличие базы подписей
        /// </summary>
        private async Task<bool> CheckSignatures() =>
            await _projectSettings.ConvertingResources.
            MapAsync(resources => resources.SignatureNames?.Count > 0).
            WhereBadAsync(hasSignatures => hasSignatures,
                badFunc: hasSignatures => hasSignatures.
                         Void(_ => _messagingService.ShowAndLogError(new ErrorCommon(FileConvertErrorType.SignatureNotFound,
                                                                                     "База подписей не загружена. Отмена запуска"))));

        /// <summary>
        /// Подписаться на обновление пакетов из базы данных
        /// </summary>
        private void SubscribeToDataBase()
        {
            var subscribe = Observable.Interval(TimeSpan.FromSeconds(ProjectSettings.IntervalSecondsToServer)).
                                       Where(_ => !IsConverting).
                                       Select(_ => Observable.FromAsync(() => ExecuteAndHandleErrorAsync(ConvertingFirstInQueuePackage,
                                                                                                         beforeMethod: () => IsConverting = true,
                                                                                                         finallyMethod: () => IsConverting = false))).
                                       Concat().
                                       Subscribe();
            _convertingUpdaterSubscriptions.Add(subscribe);
        }

        /// <summary>
        /// Получить пакет на конвертирование и запустить процесс
        /// </summary>        
        private async Task ConvertingFirstInQueuePackage()
        {
            _messagingService.ShowAndLogMessage("Запрос пакета в базе...");

            var packageDataRequest = await _fileConvertingServerService.Operations.GetFirstInQueuePackage(ProjectSettings.NetworkName);
            if (packageDataRequest != null)
            {
                var filesDataServer = await _converterServerPackageDataFromDto.ToFilesDataServerAndSaveFile(packageDataRequest);
                _idPackage = filesDataServer.Id;
                await ConvertingPackage(filesDataServer);
            }
            else
            {
                await ActionsInEmptyQueue();
            }
        }

        /// <summary>
        /// Конвертировать пакет
        /// </summary>
        private async Task ConvertingPackage(IPackageServer packageServer) =>
            await packageServer.
            WhereContinueAsyncBind(package => package.IsValid,
                okFunc: package => package.
                                    Void(_ => _messagingService.ShowAndLogMessage($"Конвертация пакета {package.Id}")).
                                    Map(_ => ConvertingFilesData(package)).
                                    MapAsync(ReplyPackageIsComplete),
                badFunc: package => Task.FromResult(ReplyPackageIsInvalid(package))).
            VoidAsync(SendResponse);

        /// <summary>
        /// Сообщить о пустом/некорректном пакете
        /// </summary>
        private IPackageServer ReplyPackageIsInvalid(IPackageServer packageServer)
        {
            var error = packageServer switch
            {
                _ when !packageServer.IsFilesDataValid => new ErrorCommon(FileConvertErrorType.FileNotFound, "Файлы для конвертации не обнаружены"),
                _ when !packageServer.IsValidByAttemptingCount => new ErrorCommon(FileConvertErrorType.AttemptingCount, "Превышено количество попыток конвертирования пакета"),
                _ => throw new ArgumentOutOfRangeException(nameof(packageServer))
            };

            _messagingService.ShowAndLogError(error);

            return packageServer.SetErrorToAllFiles(error).
                   SetStatusProcessingProject(StatusProcessingProject.Error);
        }

        /// <summary>
        /// Сообщить об обработанном пакете, если процесс не был прерван
        /// </summary>
        private static IPackageServer ReplyPackageIsComplete(IPackageServer packageServer) =>
            packageServer.SetStatusProcessingProject(StatusProcessingProject.ConvertingComplete);

        /// <summary>
        /// Отправить промежуточный отчет
        /// </summary>
        private async Task<IPackageServer> SendIntermediateResponse(IPackageServer packageServer) =>
            await _converterServerPackageDataToDto.FilesDataToIntermediateResponse(packageServer).
            Map(Task.FromResult).
            MapAsyncBind(fileDataRequest => _fileConvertingServerService.Operations.UpdateFromIntermediateResponse(fileDataRequest)).
            MapAsync(packageServer.SetStatusProcessingProject);

        /// <summary>
        /// Отправить окончательный ответ
        /// </summary>
        private async Task SendResponse(IPackageServer packageServer)
        {
            switch (packageServer.StatusProcessingProject)
            {
                case StatusProcessingProject.ConvertingComplete:
                case StatusProcessingProject.Error:
                    {
                        _messagingService.ShowAndLogMessage("Отправка данных в базу...");

                        PackageDataResponseServer packageDataResponse = await _converterServerPackageDataToDto.FilesDataToResponse(packageServer);
                        await _fileConvertingServerService.Operations.UpdateFromResponse(packageDataResponse);

                        _messagingService.ShowAndLogMessage("Конвертация пакета закончена");
                        break;
                    }
                //в случае если пользователь отменил конвертацию
                case StatusProcessingProject.Abort:
                    _messagingService.ShowAndLogMessage("Конвертация пакета прервана");
                    break;
            }

            _idPackage = null;
        }

        /// <summary>
        /// Отмена конвертирования
        /// </summary>      
        private async Task AbortConverting()
        {
            _messagingService.ShowAndLogMessage("Отмена выполнения конвертирования");
            if (_idPackage.HasValue)
            {
                await _fileConvertingServerService.Operations.AbortConvertingById(_idPackage.Value);
            }
        }

        /// <summary>
        /// Конвертировать пакет
        /// </summary>
        private async Task<IPackageServer> ConvertingFilesData(IPackageServer packageServer) =>
            await packageServer.FilesDataServer.
                FirstOrDefault(fileData => !packageServer.IsCompleted && !fileData.IsCompleted).
                Map(fileData => new ResultValue<IFileDataServer>(fileData, new ErrorCommon(FileConvertErrorType.ArgumentNullReference, nameof(IFileDataServer)))).
                ResultOkBad(
                    okFunc: fileData => ConvertingByCountLimit(fileData, packageServer.ConvertingSettings).
                                        MapAsync(packageServer.ChangeFileDataServer).
                                        MapAsyncBind(SendIntermediateResponse).
                                        MapAsyncBind(ConvertingFilesData),
                    badFunc: _ => Task.FromResult(packageServer)).
                Value;

        /// <summary>
        /// Конвертировать файл до превышения лимита
        /// </summary>       
        private async Task<IFileDataServer> ConvertingByCountLimit(IFileDataServer fileDataServer, IConvertingSettings convertingSettings) =>
            await fileDataServer.WhereOkAsyncBind(fileData => !fileData.IsCompleted,
                okFunc: fileData =>
                        ExecuteBindResultValueAsync(() => _convertingFileData.Converting(fileData, convertingSettings)).
                        ResultValueBad(_ => fileData.SetAttemptingCount(fileData.AttemptingConvertCount + 1).
                                            VoidAsync(fileDataUncompleted => ConvertingByCountLimit(fileDataUncompleted, convertingSettings))).
                        Value);

        /// <summary>
        /// Сообщить об отсутствии пакетов на конвертирование
        /// </summary>
        private async Task QueueIsEmpty()
        {
            await Task.Delay(500);
            _messagingService.ShowAndLogMessage("Очередь пакетов пуста...");
        }

        /// <summary>
        /// Системные действия во время пустой очереди
        /// </summary>       
        private async Task ActionsInEmptyQueue()
        {
            await CheckAndDeleteUnusedPackagesOnDataBase();
            await DeleteAllUnusedDataOnDisk();
            await QueueIsEmpty();
        }

        /// <summary>
        /// Удалить все предыдущие запущенные процессы
        /// </summary>
        private static void KillPreviousRunProcesses()
        {
            var processes = Process.GetProcesses().
                            Where(process => process.ProcessName.ContainsIgnoreCase("ustation") ||
                                             process.ProcessName.ContainsIgnoreCase("winword"));

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        /// <summary>
        /// Проверить и удалить ненужные пакеты в базе
        /// </summary>
        private async Task CheckAndDeleteUnusedPackagesOnDataBase()
        {
            var dateTimeNow = DateTime.Now;
            var timeElapsed = new TimeSpan((dateTimeNow - Properties.Settings.Default.UnusedDataCheck).Ticks);
            if (timeElapsed.TotalHours > ProjectSettings.IntervalHoursToDeleteUnusedPackages)
            {
                _messagingService.ShowAndLogMessage("Очистка неиспользуемых пакетов...");
                await _fileConvertingServerService.Operations.DeleteAllUnusedPackagesUntilDate(dateTimeNow);

                Properties.Settings.Default.UnusedDataCheck = new TimeSpan(dateTimeNow.Ticks);
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Очистить папку с обработанными файлами на жестком диске
        /// </summary>
        private async Task DeleteAllUnusedDataOnDisk()
        {
            var dateTimeNow = DateTime.Now;
            var timeElapsed = new TimeSpan((dateTimeNow - Properties.Settings.Default.ConvertingDataFolderCheck).Ticks);
            if (timeElapsed.TotalHours > ProjectSettings.IntervalHoursToDeleteUnusedPackages)
            {
                _messagingService.ShowAndLogMessage("Очистка пространства на жестком диске...");
                await Task.Run(() => _fileSystemOperations.DeleteAllDataInDirectory(ProjectSettings.ConvertingDirectory, DateTime.Now,
                                                                                    ProjectSettings.IntervalHoursToDeleteUnusedPackages));

                Properties.Settings.Default.ConvertingDataFolderCheck = new TimeSpan(dateTimeNow.Ticks);
                Properties.Settings.Default.Save();
            }
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _convertingUpdaterSubscriptions?.Dispose();
            }
            AbortConverting().ConfigureAwait(false);
            _fileConvertingServerService?.Dispose();

            _disposedValue = true;
        }

        ~ConvertingService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
