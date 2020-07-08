﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChannelAdam.ServiceModel;
using GadzhiCommon.Enums.LibraryData;
using GadzhiCommon.Extensions.Functional;
using GadzhiCommon.Extensions.Functional.Result;
using GadzhiCommon.Infrastructure.Implementations;
using GadzhiCommon.Infrastructure.Implementations.Logger;
using GadzhiCommon.Infrastructure.Implementations.Reflection;
using GadzhiCommon.Infrastructure.Interfaces.Logger;
using GadzhiCommon.Models.Enums;
using GadzhiCommon.Models.Implementations.Errors;
using GadzhiCommon.Models.Interfaces.Errors;
using GadzhiCommon.Models.Interfaces.LibraryData;
using GadzhiDTOBase.Infrastructure.Implementations.Converters;
using GadzhiDTOClient.Contracts.FilesConvert;
using GadzhiDTOClient.TransferModels.FilesConvert;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.ProjectSettings;
using Nito.AsyncEx.Synchronous;
using static GadzhiCommon.Infrastructure.Implementations.ExecuteAndCatchErrors;

namespace GadzhiModules.Infrastructure.Implementations.ApplicationGadzhi
{
    /// <summary>
    /// Слой приложения, инфраструктура. Сервисы
    /// </summary>
    public partial class ApplicationGadzhi
    {
        /// <summary>
        /// Журнал системных сообщений
        /// </summary>
        private readonly ILoggerService _loggerService = LoggerFactory.GetFileLogger();

        /// <summary>
        /// Выполняется ли промежуточный запрос
        /// </summary>
        private bool IsIntermediateResponseInProgress { get; set; }

        /// <summary>
        /// Запустить процесс конвертирования
        /// </summary>
        [Logger]
        public async Task ConvertingFiles()
        {
            if (_statusProcessingInformation.IsConverting) return;

            var packageDataRequest = await PrepareFilesToSending();
            if (!packageDataRequest.IsValid)
            {
                await AbortPropertiesConverting(false);
                await _dialogService.ShowMessage("Загрузите файлы для конвертирования");
                return;
            }

            FileConvertingClientService = _wcfServiceFactory.GetFileConvertingServiceService();
            var sendResult = await SendFilesToConverting(packageDataRequest);
            if (sendResult.OkStatus) SubscribeToIntermediateResponse();
        }

        /// <summary>
        /// Подготовить данные к отправке
        /// </summary>
        private async Task<PackageDataRequestClient> PrepareFilesToSending()
        {
            var filesStatusInSending = await _fileDataProcessingStatusMark.GetFilesInSending();
            _packageInfoProject.ChangeFilesStatus(filesStatusInSending);

            var filesDataRequest = await _fileDataProcessingStatusMark.GetFilesDataToRequest();
            return filesDataRequest;
        }

        /// <summary>
        /// Отправить файлы для конвертации
        /// </summary>
        private async Task<IResultError> SendFilesToConverting(PackageDataRequestClient packageDataRequest) =>
            await ExecuteAndHandleErrorAsync(() => FileConvertingClientService.Operations.SendFiles(packageDataRequest)).
            WhereContinueAsync(packageResult => packageResult.OkStatus,
                okFunc: packageResult => SendFilesToConvertingConnect(packageDataRequest, packageResult.Value),
                badFunc: packageResult => packageResult.
                                          ResultVoidAsyncBind(_ => AbortPropertiesConverting(false)).
                                          ResultVoidAsyncBind(_ => _dialogService.ShowErrors(packageResult.Errors)).
                                          MapAsync(package => package.ToResult()));

        /// <summary>
        /// Отправить файлы для конвертации после подтверждения сервера
        /// </summary>
        private async Task<IResultError> SendFilesToConvertingConnect (PackageDataRequestClient packageDataRequest, PackageDataIntermediateResponseClient packageDataResponse) =>
            await packageDataResponse.
            Void(_ => _loggerService.LogByObjects(LoggerLevel.Info, LoggerAction.Upload, ReflectionInfo.GetMethodBase(this),
                                                  packageDataRequest.FilesData, packageDataRequest.Id.ToString())).
            Map(_ => _fileDataProcessingStatusMark.GetPackageStatusAfterSend(packageDataRequest, packageDataResponse)).
            VoidAsync(filesStatusAfterSending => _packageInfoProject.ChangeFilesStatus(filesStatusAfterSending)).
            MapAsync(_ => new ResultError());

        /// <summary>
        /// Подписаться на изменение статуса файлов при конвертировании
        /// </summary>
        private void SubscribeToIntermediateResponse() =>
            _statusProcessingSubscriptions.
            Add(Observable.
                Interval(TimeSpan.FromSeconds(ProjectSettings.IntervalSecondsToIntermediateResponse)).
                Where(_ => _statusProcessingInformation.IsConverting && !IsIntermediateResponseInProgress).
                Select(_ => Observable.FromAsync(() => ExecuteAndHandleErrorAsync(UpdateStatusProcessing,
                                                                                  () => IsIntermediateResponseInProgress = true,
                                                                                  () => AbortPropertiesConverting(true).WaitAndUnwrapException(),
                                                                                  () => IsIntermediateResponseInProgress = false))).
                Concat().
                Subscribe());

        /// <summary>
        /// Получить информацию о состоянии конвертируемых файлов
        /// </summary>
        private async Task UpdateStatusProcessing()
        {
            var packageDataResponse = await FileConvertingClientService.Operations.CheckFilesStatusProcessing(_packageInfoProject.Id);
            var packageStatus = await _fileDataProcessingStatusMark.GetPackageStatusIntermediateResponse(packageDataResponse);
            _packageInfoProject.ChangeFilesStatus(packageStatus);

            if (CheckStatusProcessing.CompletedStatusProcessingProject.Contains(packageDataResponse.StatusProcessingProject))
            {
                await GetCompleteFiles();
                ClearSubscriptions();
            }
        }

        /// <summary>
        /// Получить отконвертированные файлы
        /// </summary>
        private async Task GetCompleteFiles()
        {
            var packageDataResponse = await FileConvertingClientService.Operations.GetCompleteFiles(_packageInfoProject.Id);
            _loggerService.LogByObjects(LoggerLevel.Info, LoggerAction.Download, ReflectionInfo.GetMethodBase(this),
                                        packageDataResponse.FilesData, packageDataResponse.Id.ToString());

            var filesStatusBeforeWrite = await _fileDataProcessingStatusMark.GetFilesStatusCompleteResponseBeforeWriting(packageDataResponse);
            _packageInfoProject.ChangeFilesStatus(filesStatusBeforeWrite);

            var filesStatusWrite = await _fileDataProcessingStatusMark.GetFilesStatusCompleteResponseAndWritten(packageDataResponse);
            _packageInfoProject.ChangeFilesStatus(filesStatusWrite);

            await FileConvertingClientService.Operations.SetFilesDataLoadedByClient(_packageInfoProject.Id);
        }

        /// <summary>
        /// Отмена операции
        /// </summary>
        private async Task AbortConverting()
        {
            if (_statusProcessingInformation?.IsConverting == true && _packageInfoProject != null)
            {
                await FileConvertingClientService.Operations.AbortConvertingById(_packageInfoProject.Id);
                _loggerService.LogByObject(LoggerLevel.Info, LoggerAction.Abort, ReflectionInfo.GetMethodBase(this), _packageInfoProject.Id.ToString());
            }
        }

        /// <summary>
        /// Загрузить подписи из базы данных
        /// </summary>
        [Logger]
        public async Task<IResultCollection<ISignatureLibrary>> GetSignaturesNames() =>
            await _wcfServiceFactory.UsingSignatureServiceAsync(
                (signatureService) => signatureService.Operations.GetSignaturesNames().
                                      MapAsync(ConverterDataFileFromDto.SignaturesLibraryFromDto)).
            MapAsync(result => result.ToResultCollection());

        /// <summary>
        /// Загрузить отделы из базы данных
        /// </summary>
        [Logger]
        public async Task<IResultCollection<DepartmentType>> GetSignaturesDepartments() =>
            await _wcfServiceFactory.UsingSignatureServiceAsync(
                (signatureService) => signatureService.Operations.GetSignaturesDepartments()).
            MapAsync(result => result.ToResultCollection());

        /// <summary>
        /// Сбросить индикаторы конвертации
        /// </summary>
        public async Task AbortPropertiesConverting(bool abortConnection)
        {
            IsIntermediateResponseInProgress = false;

            if (abortConnection) await AbortConverting();

            if (_statusProcessingInformation?.IsConverting == true)
            {
                _packageInfoProject?.ChangeAllFilesStatusAndMarkError();
            }
            ClearSubscriptions();
        }

        /// <summary>
        /// Очистить подписки на обновление пакета конвертирования
        /// </summary>
        private void ClearSubscriptions()
        {
            _statusProcessingSubscriptions?.Clear();
            FileConvertingClientService?.Dispose();
            FileConvertingClientService = null;
        }
    }
}