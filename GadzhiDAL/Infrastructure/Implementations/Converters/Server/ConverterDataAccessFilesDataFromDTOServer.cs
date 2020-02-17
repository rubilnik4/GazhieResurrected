﻿using GadzhiDAL.Entities.FilesConvert;
using GadzhiDAL.Infrastructure.Interfaces.Converters.Server;
using GadzhiDTOServer.TransferModels.FilesConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiDAL.Infrastructure.Implementations.Converters.Server
{
    /// <summary>
    /// Конвертер из трансферной модели в модель базы данных
    /// </summary>      
    public class ConverterDataAccessFilesDataFromDTOServer : IConverterDataAccessFilesDataFromDTOServer
    {
        public ConverterDataAccessFilesDataFromDTOServer()
        {

        }
        
        /// <summary>
        /// Обновить модель базы данных на основе промежуточного ответа
        /// </summary>      
        public FilesDataEntity UpdateFilesDataAccessFromIntermediateResponse(FilesDataEntity filesDataEntity,
                                                                             FilesDataIntermediateResponseServer filesDataIntermediateResponse)
        {
            if (filesDataEntity != null && filesDataIntermediateResponse != null)
            {
                filesDataEntity.IsCompleted = filesDataIntermediateResponse.IsCompleted;
                filesDataEntity.StatusProcessingProject = filesDataIntermediateResponse.StatusProcessingProject;

                foreach (var fileDataAccess in filesDataEntity.FilesData)
                {
                    FileDataIntermediateResponseServer fileDataIntermediate = filesDataIntermediateResponse.FilesData?.
                        FirstOrDefault(fileDataInter => fileDataInter.FilePath == fileDataAccess.FilePath);

                    UpdateFileDataAccessFromIntermediateResponse(fileDataAccess,
                                                                 fileDataIntermediate);
                }
            }
            return filesDataEntity;
        }

        /// <summary>
        /// Обновить модель базы данных на основе окончательного ответа
        /// </summary>      
        public FilesDataEntity UpdateFilesDataAccessFromResponse(FilesDataEntity filesDataEntity,
                                                                 FilesDataResponseServer filesDataResponse)
        {
            if (filesDataEntity != null && filesDataResponse != null &&
                !filesDataEntity.IsCompleted)
            {
                filesDataEntity.IsCompleted = filesDataResponse.IsCompleted;
                filesDataEntity.StatusProcessingProject = filesDataResponse.StatusProcessingProject;
             
                foreach (var fileDataAccess in filesDataEntity.FilesData)
                {
                    FileDataResponseServer fileData = filesDataResponse.FilesData?.
                        FirstOrDefault(fileDataInter => fileDataInter.FilePath == fileDataAccess.FilePath);

                    UpdateFileDataAccessFromResponse(fileDataAccess,
                                                     fileData);
                }
            }
            return filesDataEntity;
        }      

        /// <summary>
        /// Обновить модель файла данных на основе промежуточного ответа
        /// </summary>      
        public FileDataEntity UpdateFileDataAccessFromIntermediateResponse(FileDataEntity fileDataEntity,
                                                                           FileDataIntermediateResponseServer fileDataIntermediateResponse)
        {
            if (fileDataEntity != null && fileDataIntermediateResponse != null)
            {
                fileDataEntity.IsCompleted = fileDataIntermediateResponse.IsCompleted;
                fileDataEntity.StatusProcessing = fileDataIntermediateResponse.StatusProcessing;
                fileDataEntity.SetFileConvertErrorType(fileDataIntermediateResponse.FileConvertErrorType);
            }

            return fileDataEntity;
        }

        /// <summary>
        /// Обновить модель файла данных на основе окончательного ответа
        /// </summary>      
        public FileDataEntity UpdateFileDataAccessFromResponse(FileDataEntity fileDataEntity,
                                                               FileDataResponseServer fileDataResponse)
        {
            if (fileDataEntity != null && fileDataResponse != null)
            {
                fileDataEntity.IsCompleted = fileDataResponse.IsCompleted;
                fileDataEntity.StatusProcessing = fileDataResponse.StatusProcessing;
                fileDataEntity.SetFileConvertErrorType(fileDataResponse.FileConvertErrorType);
                fileDataEntity.FileDataSource = fileDataResponse.FileDataSource;                          
            }

            return fileDataEntity;
        }
    }
}