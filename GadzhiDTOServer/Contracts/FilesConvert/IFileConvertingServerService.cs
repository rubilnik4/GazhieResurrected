﻿using GadzhiCommon.Enums.FilesConvert;
using GadzhiDTOServer.TransferModels.FilesConvert;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using GadzhiCommon.Models.Implementations.Functional;

namespace GadzhiDTOServer.Contracts.FilesConvert
{
    /// <summary>
    /// Сервис для конвертирования файлов.Контракт используется серверной частью
    /// </summary>
    [ServiceContract]
    public interface IFileConvertingServerService
    {
        /// <summary>
        /// Получить первый в очереди пакет на конвертирование
        /// </summary>    
        [OperationContract]
        Task<PackageDataRequestServer> GetFirstInQueuePackage(string identityServerName);

        /// <summary>
        /// Обновить информацию после промежуточного ответа
        /// </summary>      
        [OperationContract]
        Task<StatusProcessingProject> UpdateFromIntermediateResponse(PackageDataIntermediateResponseServer packageDataIntermediateResponse);

        /// <summary>
        /// Обновить информацию после окончательного ответа
        /// </summary>  
        [OperationContract]
        Task<Unit> UpdateFromResponse(PackageDataResponseServer packageDataResponse);

        /// <summary>
        /// Удалить все устаревшие пакеты
        /// </summary> 
        [OperationContract]
        Task DeleteAllUnusedPackagesUntilDate(DateTime dateDeletion);

        /// <summary>
        /// Удалить все устаревшие пакеты с ошибками
        /// </summary> 
        [OperationContract]
        Task DeleteAllUnusedErrorPackagesUntilDate(DateTime dateDeletion);

        /// <summary>
        /// Отмена операции по номеру ID
        /// </summary>   
        [OperationContract]
        Task AbortConvertingById(Guid id);
    }
}

