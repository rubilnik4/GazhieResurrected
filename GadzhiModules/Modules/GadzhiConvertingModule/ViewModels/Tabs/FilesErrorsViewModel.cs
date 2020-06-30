﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiModules.Helpers.BaseClasses.ViewModels;
using GadzhiModules.Infrastructure.Interfaces;
using GadzhiModules.Infrastructure.Interfaces.ApplicationGadzhi;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.FileConverting.ReactiveSubjects;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Interfaces.FileConverting;
using GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.Tabs.FilesErrorsViewModelItems;

namespace GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.Tabs
{
    /// <summary>
    /// Представление ошибок конвертации
    /// </summary>
    public class FilesErrorsViewModel : ViewModelBase
    {
        /// <summary>
        /// Текущий статус конвертирования
        /// </summary>        
        private readonly IStatusProcessingInformation _statusProcessingInformation;

        public FilesErrorsViewModel(IApplicationGadzhi applicationGadzhi, IStatusProcessingInformation statusProcessingInformation)
        {
            if (applicationGadzhi == null) throw new ArgumentNullException(nameof(applicationGadzhi));

            applicationGadzhi.FileDataChange.Subscribe(ActionOnTypeStatusChange);
            _statusProcessingInformation = statusProcessingInformation ?? throw new ArgumentNullException(nameof(statusProcessingInformation));

            FilesErrorsCollection = new ObservableCollection<FileErrorViewModelItem>();
            BindingOperations.EnableCollectionSynchronization(FilesErrorsCollection, _filesErrorsCollectionLock);
        }

        /// <summary>
        /// Название
        /// </summary>
        public override string Title => "Ошибки";

        /// <summary>
        /// Видимость
        /// </summary>
        public override bool Visibility => FilesErrorsCollection.Count > 0;

        /// <summary>
        /// Блокировка списка ошибок для других потоков
        /// </summary>
        private readonly object _filesErrorsCollectionLock = new object();

        /// <summary>
        /// Список ошибок
        /// </summary>
        public ObservableCollection<FileErrorViewModelItem> FilesErrorsCollection { get; }

        /// <summary>
        /// Изменения коллекции при корректировке статуса конвертирования
        /// </summary>
        private void ActionOnTypeStatusChange(FilesChange filesChange)
        {
            if (filesChange.IsStatusProcessingProjectChanged &&
                _statusProcessingInformation.StatusProcessingProject == StatusProcessingProject.End)
            {
                ActionOnTypeAddErrors(filesChange);
            }
        }

        /// <summary>
        /// Добавить ошибки после окончания конвертирования
        /// </summary>
        private void ActionOnTypeAddErrors(FilesChange filesChange)
        {
            FilesErrorsCollection.Clear();
            var errorViewModelItems = filesChange.FilesData.SelectMany(GetErrorViewModelItemsFromFileData).ToList();
            FilesErrorsCollection.AddRange(errorViewModelItems);

            VisibilityChange.OnNext(Visibility);
        }

        /// <summary>
        /// Получить представления об ошибках после конвертирования файлов
        /// </summary>
        private static IEnumerable<FileErrorViewModelItem> GetErrorViewModelItemsFromFileData(IFileData fileData) =>
            fileData.FileErrors.
            Select(errorType => new FileErrorViewModelItem(fileData.FileName, errorType.ErrorType, errorType.Description));
    }
}