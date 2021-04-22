﻿using GadzhiDTOBase.TransferModels.ServerStates;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.ServerStates;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Interfaces.ServerStates;

namespace GadzhiModules.Infrastructure.Implementations.Converters.ServerStates
{
    /// <summary>
    /// Конвертер информации об обработанных файлах на клиенте
    /// </summary>
    public static class ServerCompleteFilesConverter
    {
        /// <summary>
        /// Преобразовать информацию об обработанных файлах в клиентскую модель
        /// </summary>
        public static IServerCompleteFilesClient ToClient(ServerCompleteFilesResponse serverCompleteFilesResponse) =>
            new ServerCompleteFilesClient(serverCompleteFilesResponse.DgnCount, serverCompleteFilesResponse.DocCount,
                                          serverCompleteFilesResponse.PdfCount, serverCompleteFilesResponse.DwgCount,
                                          serverCompleteFilesResponse.XlsCount);
    }
}