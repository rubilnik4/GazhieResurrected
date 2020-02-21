﻿using GadzhiModules.Modules.FilesConvertModule.Models.Implementations;
using GadzhiModules.Modules.FilesConvertModule.Models.Implementations.ReactiveSubjects;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace GadzhiModules.Infrastructure.Interfaces
{
    /// <summary>
    /// Слой приложения, инфраструктура
    /// </summary>
    public interface IApplicationGadzhi : IDisposable
    {
        /// <summary>
        /// Подписка на изменение коллекции
        /// </summary>
        ISubject<FilesChange> FileDataChange { get; }

        /// <summary>
        /// Добавить файлы для конвертации
        /// </summary>
        Task AddFromFiles();

        /// <summary>
        /// Указать папку для конвертации
        /// </summary>       
        Task AddFromFolders();

        /// <summary>
        /// Добавить файлы или папки для конвертации
        /// </summary>
        Task AddFromFilesOrDirectories(IEnumerable<string> filesToRemove);

        /// <summary>
        /// Очистить список файлов
        /// </summary>       
        void ClearFiles();

        /// <summary>
        /// Удалить файлы
        /// </summary>
        void RemoveFiles(IEnumerable<FileData> fileOrDirectoriesPaths);


        /// <summary>
        /// Закрыть приложение
        /// </summary>
        void CloseApplication();

        /// <summary>
        /// Конвертировать файлы на сервре
        /// </summary>
        Task ConvertingFiles();

        /// <summary>
        /// Сбросить индикаторы конвертации
        /// </summary>
        Task AbortPropertiesConverting(bool isDispose = false);
    }
}
