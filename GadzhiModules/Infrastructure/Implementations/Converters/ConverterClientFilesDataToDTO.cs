﻿using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiDTO.TransferModels.FilesConvert;
using GadzhiModules.Infrastructure.Interfaces.Converters;
using GadzhiModules.Modules.FilesConvertModule.Models.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiModules.Infrastructure.Implementations.Converters
{
    /// <summary>
    /// Конвертеры из локальной модели в трансферную
    /// </summary>  
    public class ConverterClientFilesDataToDTO: IConverterClientFilesDataToDTO
    {
        /// <summary>
        /// Проверка состояния папок и файлов
        /// </summary>   
        private IFileSystemOperations FileSystemOperations { get; }

        public ConverterClientFilesDataToDTO(IFileSystemOperations fileSystemOperations)
        {
            FileSystemOperations = fileSystemOperations;
        }
       
        /// <summary>
        /// Конвертер пакета информации о файле из локальной модели в трансферную
        /// </summary>      
        public async Task<FilesDataRequest> ConvertToFilesDataRequest(IFilesData filesData)
        {
            var filesRequestExist = await Task.WhenAll(filesData.FilesInfo?.Where(file => FileSystemOperations.IsFileExist(file.FilePath))?.
                                                                                    Select(file => ConvertToFileDataRequest(file)));
            var filesRequestEnsuredWithBytes = filesRequestExist?.Where(file => file.FileDataSource != null);

            return new FilesDataRequest()
            {
                Id = filesData.ID,
                FilesData = filesRequestEnsuredWithBytes,
            };
        }

        /// <summary>
        /// Конвертер информации о файле из локальной модели в трансферную
        /// </summary>      
        private async Task<FileDataRequest> ConvertToFileDataRequest(FileData fileData)
        {
            byte[] fileDataSource = await FileSystemOperations.ConvertFileToByteAndZip(fileData.FilePath);

            return new FileDataRequest()
            {
                ColorPrint = fileData.ColorPrint,               
                FilePath = fileData.FilePath,              
                StatusProcessing = fileData.StatusProcessing,
                FileDataSource = fileDataSource,
            };
        }
    }
}
