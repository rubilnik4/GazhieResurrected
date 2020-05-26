﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiModules.Helpers.BaseClasses.ViewModels;
using GadzhiModules.Helpers.Converters;
using GadzhiModules.Infrastructure.Interfaces;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.FileConverting.ReactiveSubjects;
using GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.Tabs.FilesConvertingViewModelItems;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;

namespace GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.Tabs
{
    public class FilesConvertingViewModel : ViewModelBase, IDropTarget, IDisposable
    {
        /// <summary>
        /// Слой инфраструктуры
        /// </summary>        
        private readonly IApplicationGadzhi _applicationGadzhi;

        /// <summary>
        /// Текущий статус конвертирования
        /// </summary>        
        private readonly IStatusProcessingInformation _statusProcessingInformation;

        /// <summary>
        /// Подписка на обновление модели
        /// </summary>
        private readonly IDisposable _fileDataChangeSubscribe;

        public FilesConvertingViewModel(IApplicationGadzhi applicationGadzhi, IStatusProcessingInformation statusProcessingInformation)
        {
            _applicationGadzhi = applicationGadzhi ?? throw new ArgumentNullException(nameof(applicationGadzhi));
            _statusProcessingInformation = statusProcessingInformation ?? throw new ArgumentNullException(nameof(statusProcessingInformation));

            FilesDataCollection = new ObservableCollection<FileDataViewModelItem>();

            _fileDataChangeSubscribe = _applicationGadzhi?.FileDataChange.Subscribe(OnFilesInfoUpdated);

            InitializeDelegateCommands();
        }

        /// <summary>
        /// Название
        /// </summary>
        public override string Title => "Конвертушки";

        /// <summary>
        /// Инициализировать команды
        /// </summary>
        private void InitializeDelegateCommands()
        {
            ClearFilesDelegateCommand = new DelegateCommand(
               ClearFiles,
               () => !IsLoading && !IsConverting).
               ObservesProperty(() => IsLoading).
               ObservesProperty(() => IsConverting);

            AddFromFilesDelegateCommand = new DelegateCommand(
               async () => await AddFromFiles(),
               () => !IsLoading && !IsConverting).
               ObservesProperty(() => IsLoading).
               ObservesProperty(() => IsConverting);

            AddFromFoldersDelegateCommand = new DelegateCommand(
               async () => await AddFromFolders(),
               () => !IsLoading && !IsConverting).
               ObservesProperty(() => IsLoading).
               ObservesProperty(() => IsConverting);

            RemoveFilesDelegateCommand = new DelegateCommand(
               RemoveFiles,
               () => !IsLoading && !IsConverting).
               ObservesProperty(() => IsLoading).
               ObservesProperty(() => IsConverting);

            ConvertingFilesDelegateCommand = new DelegateCommand(
              async () => await ConvertingFiles(),
              () => !IsLoading && !IsConverting).
              ObservesProperty(() => IsLoading).
              ObservesProperty(() => IsConverting);

            CloseApplicationDelegateCommand = new DelegateCommand(CloseApplication);
        }
        /// <summary>
        /// Данные о конвертируемых файлах
        /// </summary>
        public ObservableCollection<FileDataViewModelItem> FilesDataCollection { get; }

        /// <summary>
        /// Выделенные строки
        /// </summary>
        private IList<object> _selectedFilesData = new List<object>();

        /// <summary>
        /// Выделенные строки
        /// </summary>
        public IList<object> SelectedFilesData
        {
            get => _selectedFilesData;
            set => SetProperty(ref _selectedFilesData, value);
        }

        /// <summary>
        /// Очистить список файлов
        /// </summary>       
        public DelegateCommand ClearFilesDelegateCommand { get; private set; }

        /// <summary>
        /// Добавить файлы для конвертации
        /// </summary>       
        public DelegateCommand AddFromFilesDelegateCommand { get; private set; }

        /// <summary>
        /// Добавить папки для конвертации
        /// </summary>       
        public DelegateCommand AddFromFoldersDelegateCommand { get; private set; }

        /// <summary>
        /// Удалить файлы из списка
        /// </summary>       
        public DelegateCommand RemoveFilesDelegateCommand { get; private set; }

        /// <summary>
        /// Конвертировать файлы
        /// </summary>       
        public DelegateCommand ConvertingFilesDelegateCommand { get; private set; }

        /// <summary>
        /// Закрыть приложение
        /// </summary>       
        public DelegateCommand CloseApplicationDelegateCommand { get; private set; }

        /// <summary>
        /// Индикатор конвертирования файлов
        /// </summary>
        public bool IsConverting => _statusProcessingInformation.IsConverting;

        /// <summary>
        /// Типы цветов для печати
        /// </summary>
        public IEnumerable<string> ColorPrintToString => ColorPrintConverter.
                                                         ColorPrintString.
                                                         Select(color => color.Value);

        /// <summary>
        /// Статус обработки проекта
        /// </summary>
        public StatusProcessingProject StatusProcessingProject => _statusProcessingInformation.StatusProcessingProject;

        /// <summary>
        /// Статус обработки проекта c процентом выполнения
        /// </summary>
        public string StatusProcessingProjectName => _statusProcessingInformation.GetStatusProcessingProjectName();

        /// <summary>
        /// Отображать ли процент выполнения для ProgressBar
        /// </summary>
        public bool IsIndeterminateProgressBar => !_statusProcessingInformation.HasStatusProcessPercentage;

        /// <summary>
        /// Процент выполнения для ProgressBar
        /// </summary>
        public int PercentageOfComplete => _statusProcessingInformation.PercentageOfComplete;

        /// <summary>
        /// Очистить список файлов
        /// </summary> 
        private void ClearFiles() => ExecuteAndHandleError(_applicationGadzhi.ClearFiles);

        /// <summary>
        /// Добавить файлы для конвертации
        /// </summary> 
        private async Task AddFromFiles() => await ExecuteAndHandleErrorAsync(_applicationGadzhi.AddFromFiles);

        /// <summary>
        /// Добавить папки для конвертации
        /// </summary> 
        private async Task AddFromFolders() => await ExecuteAndHandleErrorAsync(_applicationGadzhi.AddFromFolders);

        /// <summary>
        /// Добавить файлы и папки для конвертации
        /// </summary> 
        private async Task AddFromFilesAndFolders(IEnumerable<string> fileOrDirectoriesPaths) =>
            await ExecuteAndHandleErrorAsync(() => _applicationGadzhi.AddFromFilesOrDirectories(fileOrDirectoriesPaths));

        /// <summary>
        /// Удалить файлы из списка
        /// </summary> 
        private void RemoveFiles()
        {
            var removeFiles = SelectedFilesData?.
                              OfType<FileDataViewModelItem>().
                              Select(fileVm => fileVm.FileData);

            ExecuteAndHandleError(() => _applicationGadzhi.RemoveFiles(removeFiles));
        }

        /// <summary>
        /// Конвертировать файлы
        /// </summary> 
        private async Task ConvertingFiles() =>
            await ExecuteAndHandleErrorAsync(_applicationGadzhi.ConvertingFiles,
                                             async () => await _applicationGadzhi.AbortPropertiesConverting());

        /// <summary>
        /// Удалить файлы из списка
        /// </summary> 
        private void CloseApplication() => ExecuteAndHandleError(_applicationGadzhi.CloseApplication);

        #region FilesInfoUpdate
        /// <summary>
        /// Обновление данных после изменения модели
        /// </summary> 
        private void OnFilesInfoUpdated(FilesChange fileChange)
        {
            switch (fileChange.ActionType)
            {
                case ActionType.Add:
                    ActionOnTypeAdd(fileChange);
                    break;
                case ActionType.Remove:
                case ActionType.Clear:
                    ActionOnTypeRemove(fileChange);
                    break;
            }

            ActionOnTypeStatusChange(fileChange);
        }

        /// <summary>
        /// Изменения коллекции при добавлении файлов
        /// </summary>
        private void ActionOnTypeAdd(FilesChange filesChange)
        {
            var fileDataViewModel = filesChange?.FileData?.Select(fileData => new FileDataViewModelItem(fileData));
            FilesDataCollection.AddRange(fileDataViewModel);
        }

        /// <summary>
        /// Изменения коллекции при удалении файлов
        /// </summary>
        private void ActionOnTypeRemove(FilesChange filesChange)
        {
            if (filesChange?.FileData.Count == 1)
            {
                var fileRemove = FilesDataCollection.First(f => f.FilePath == filesChange.FileData.First().FilePath);
                FilesDataCollection.Remove(fileRemove);
            }
            else
            {
                FilesDataCollection.Clear();
                var fileDataViewModel = filesChange?.FilesDataProject?.Select(fileData => new FileDataViewModelItem(fileData));
                FilesDataCollection.AddRange(fileDataViewModel);
            }
        }

        /// <summary>
        /// Изменения коллекции при корректировке статуса конвертирования
        /// </summary>
        private void ActionOnTypeStatusChange(FilesChange filesChange)
        {
            var fileChangePath = filesChange?.FileData?.Select(file => file.FilePath);
            if (fileChangePath != null)
            {
                var filesDataNeedUpdate = FilesDataCollection.Where(fileData => fileChangePath.Contains(fileData.FilePath));
                foreach (var fileUpdate in filesDataNeedUpdate)
                {
                    fileUpdate.UpdateStatusProcessing();
                }
            }

            RaiseAfterStatusChange(filesChange);
        }

        /// <summary>
        /// Обновить поля после изменения статуса конвертирования
        /// </summary>
        private void RaiseAfterStatusChange(FilesChange filesChange)
        {
            //необходимо сначала вычислить проценты, затем статус
            if (_statusProcessingInformation.HasStatusProcessPercentage)
            {
                RaisePropertyChanged(nameof(PercentageOfComplete));
            }
            if (filesChange.IsStatusProcessingProjectChanged)
            {
                RaisePropertyChanged(nameof(IsIndeterminateProgressBar));
                RaisePropertyChanged(nameof(StatusProcessingProject));
            }

            //Изменяем процент выполнения в зависимости от типа операции
            if (filesChange.IsStatusProcessingProjectChanged || _statusProcessingInformation.HasStatusProcessPercentage)
            {
                RaisePropertyChanged(nameof(StatusProcessingProjectName));
            }
            if (_statusProcessingInformation.IsConvertingChanged)
            {
                RaisePropertyChanged(nameof(IsConverting));
            }
        }
        #endregion


        /// <summary>
        /// Реализация Drag Drop для ссылки на файлы
        /// </summary>       
        public void DragOver(IDropInfo dropInfo)
        {
            if (!(dropInfo?.Data is IDataObject dataObject)) return;

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo?.Data is DataObject dataObject) ||
                !dataObject.ContainsFileDropList()) return;

            var filePaths = dataObject.GetFileDropList().Cast<string>().ToList();
            Task.FromResult(AddFromFilesAndFolders(filePaths));
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _fileDataChangeSubscribe?.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

}