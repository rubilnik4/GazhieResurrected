﻿using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiDTOClient.TransferModels.FilesConvert;
using GadzhiModules.Infrastructure.Interfaces.Converters;
using GadzhiModules.Modules.FilesConvertModule.Models.Implementations;
using System.Linq;
using System.Threading.Tasks;

namespace GadzhiModules.Infrastructure.Implementations.Converters
{
    /// <summary>
    /// Конвертеры из локальной модели в трансферную
    /// </summary>  
    public class ConverterClientFilesDataToDTO : IConverterClientFilesDataToDTO
    {
        /// <summary>
        /// Проверка состояния папок и файлов
        /// </summary>   
        private readonly IFileSystemOperations _fileSystemOperations;

        public ConverterClientFilesDataToDTO(IFileSystemOperations fileSystemOperations)
        {
            _fileSystemOperations = fileSystemOperations;
        }

        /// <summary>
        /// Конвертер пакета информации о файле из локальной модели в трансферную
        /// </summary>      
        public async Task<FilesDataRequestClient> ConvertToFilesDataRequest(IFilesData filesData)
        {
            var filesRequestExist = await Task.WhenAll(filesData?.FilesInfo?.Where(file => _fileSystemOperations.IsFileExist(file.FilePath))?.
                                                                             Select(file => ConvertToFileDataRequest(file)));
            var filesRequestEnsuredWithBytes = filesRequestExist?.Where(file => file.FileDataSource != null);

            return new FilesDataRequestClient()
            {
                Id = filesData.Id,
                FilesData = filesRequestEnsuredWithBytes.ToList(),
            };
        }

        /// <summary>
        /// Конвертер информации о файле из локальной модели в трансферную
        /// </summary>      
        private async Task<FileDataRequestClient> ConvertToFileDataRequest(FileData fileData)
        {
            byte[] fileDataSource = await _fileSystemOperations.ConvertFileToByteAndZip(fileData.FilePath);

            return new FileDataRequestClient()
            {
                ColorPrint = fileData.ColorPrint,
                FilePath = fileData.FilePath,
                StatusProcessing = fileData.StatusProcessing,
                FileDataSource = fileDataSource,
            };
        }
    }
}
