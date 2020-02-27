﻿using GadzhiMicrostation.Extensions.StringAdditional;
using GadzhiMicrostation.Infrastructure.Implementations;
using GadzhiMicrostation.Microstation.Interfaces;
using GadzhiMicrostation.Microstation.Interfaces.ApplicationMicrostationPartial;
using GadzhiMicrostation.Models.Enums;
using GadzhiMicrostation.Models.Implementations;
using GadzhiMicrostation.Models.Implementations.FilesData;
using MicroStationDGN;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GadzhiMicrostation.Microstation.Implementations.ApplicationMicrostationPartial
{
    public partial class ApplicationMicrostation : IApplicationMicrostationCommands
    {

        /// <summary>
        /// Открыть файл
        /// </summary>
        public IDesignFileMicrostation OpenDesignFile(string filePath)
        {
            if (IsDesingFileValidAndSetErrors(filePath))
            {
                _executeAndCatchErrorsMicrostation.ExecuteAndHandleError(
                    () => Application.OpenDesignFile(filePath, false),
                    errorMicrostation: new ErrorMicrostation(ErrorMicrostationType.FileNotOpen,
                                                             $"Ошибка открытия файла {filePath}"));
            };
            return ActiveDesignFile;
        }

        /// <summary>
        /// Сохранить файл
        /// </summary>
        public void SaveDesignFile(string filePath)
        {
            if (ActiveDesignFile.IsDesingFileValid)
            {
                _executeAndCatchErrorsMicrostation.ExecuteAndHandleError(() =>
                {
                    ActiveDesignFile.SaveAs(filePath);
                    _microstationProject.FileDataMicrostation.
                        AddConvertedFilePath(new FileDataSourceMicrostation(filePath, FileExtentionMicrostation.dgn));
                },
                errorMicrostation: new ErrorMicrostation(ErrorMicrostationType.FileNotSaved,
                                                         $"Ошибка сохранения файла DGN {filePath}"),
                ApplicationCatchMethod: () => ActiveDesignFile.Close());
            }
        }

        /// <summary>
        /// Сохранить файл PDF
        /// </summary>
        public void CreatePdfFile(string filePath)
        {
            if (ActiveDesignFile.IsDesingFileValid)
            {
                _executeAndCatchErrorsMicrostation.ExecuteAndHandleError(() =>
                    {
                        ActiveDesignFile.CreatePdfInDesingFile(filePath);
                        _microstationProject.FileDataMicrostation.
                            AddConvertedFilePath(new FileDataSourceMicrostation(filePath, FileExtentionMicrostation.pdf));
                    },
                    errorMicrostation: new ErrorMicrostation(ErrorMicrostationType.PdfPrintingError,
                                                             $"Ошибка сохранения файла PDF {filePath}"));
            }
        }

        /// <summary>
        /// Создать файл типа DWG
        /// </summary>
        public void CreateDwgFile(string filePath)
        {
            if (ActiveDesignFile.IsDesingFileValid)
            {
                _executeAndCatchErrorsMicrostation.ExecuteAndHandleError(() =>
                    {
                        ActiveDesignFile.CreateDwgFile(filePath);
                        _microstationProject.FileDataMicrostation.
                            AddConvertedFilePath(new FileDataSourceMicrostation(filePath, FileExtentionMicrostation.dwg));
                    },
                    errorMicrostation: new ErrorMicrostation(ErrorMicrostationType.DwgCreatingError,
                                                              $"Ошибка сохранения файла DWG {filePath}"));
            }
        }

        /// <summary>
        /// Закрыть файл
        /// </summary>
        public void CloseDesignFile()
        {
            if (ActiveDesignFile.IsDesingFileValid)
            {
                _executeAndCatchErrorsMicrostation.ExecuteAndHandleError(
                    () => ActiveDesignFile.CloseWithSaving(),
                    errorMicrostation: new ErrorMicrostation(ErrorMicrostationType.FileNotSaved,
                                                             $"Ошибка закрытия файла {ActiveDesignFile.FullName}"),
                    ApplicationCatchMethod: () => ActiveDesignFile.Close());
            }
        }

        /// <summary>
        /// Проверить корректность файла. Записать ошибки
        /// </summary>    
        private bool IsDesingFileValidAndSetErrors(string filePath)
        {
            bool isValid = false;

            if (_fileSystemOperationsMicrostation.IsFileExist(filePath))
            {
                string fileExtension = FileSystemOperationsMicrostation.ExtensionWithoutPointFromPath(filePath);
                if (FileExtentionMicrostation.dgn.ToString().ContainsIgnoreCase(fileExtension))
                {
                    isValid = true;
                }
                else
                {
                    ErrorMessagingMicrostation.AddError(new ErrorMicrostation(ErrorMicrostationType.IncorrectExtension,
                                                        $"Расширение файла {filePath} не соответствует типу .dgn"));
                }
            }
            else
            {
                ErrorMessagingMicrostation.AddError(new ErrorMicrostation(ErrorMicrostationType.FileNotFound,
                                                                           $"Файл {filePath} не найден"));
            }

            return isValid;
        }
    }
}