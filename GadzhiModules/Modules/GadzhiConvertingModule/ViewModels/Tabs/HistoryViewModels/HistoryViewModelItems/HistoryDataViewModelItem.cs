﻿using System;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Models.Interfaces.LibraryData.Histories;
using GadzhiModules.Infrastructure.Implementations.Converters;

namespace GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.Tabs.HistoryViewModels.HistoryViewModelItems
{
    /// <summary>
    /// Данные истории конвертаций
    /// </summary>
    public class HistoryDataViewModelItem
    {
        public HistoryDataViewModelItem(IHistoryData historyData)
        {
            HistoryData = historyData;
        }

        /// <summary>
        /// Данные истории конвертаций
        /// </summary>
        public IHistoryData HistoryData { get; }

        /// <summary>
        /// Дата пакета
        /// </summary>
        public string CreationDateTime =>
            HistoryData.CreationDateTime.ToString("G");

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string ClientName =>
            HistoryData.ClientName;

        /// <summary>
        /// Статус обработки проекта
        /// </summary>
        public string StatusProcessingProject =>
            StatusProcessingProjectConverter.StatusProcessingProjectToString(HistoryData.StatusProcessingProject);

        /// <summary>
        /// Количество файлов
        /// </summary>
        public int FilesCount =>
            HistoryData.FilesCount;
    }
}