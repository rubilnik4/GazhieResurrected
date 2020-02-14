﻿using GadzhiCommon.Enums.FilesConvert;
using GadzhiCommon.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiCommon.Infrastructure.Implementations
{
    /// <summary>
    /// Класс обертка для отлова ошибок
    /// </summary> 
    public class ExecuteAndCatchErrors : IExecuteAndCatchErrors
    {
        /// <summary>
        /// Стандартные диалоговые окна
        /// </summary> 
        private readonly IMessageAndLoggingService _messageAndLoggingService;

        public ExecuteAndCatchErrors(IMessageAndLoggingService messageAndLoggingService)
        {
            _messageAndLoggingService = messageAndLoggingService;
        }

        /// <summary>
        ///Отлов ошибок и вызов постметода       
        /// </summary> 
        public void ExecuteAndHandleError(Action method,
                                          Action ApplicationBeforeMethod = null,
                                          Action ApplicationCatchMethod = null,
                                          Action ApplicationFinallyMethod = null)
        {
            try
            {
                ApplicationBeforeMethod?.Invoke();
                method();
            }
            catch (Exception ex)
            {
                ApplicationCatchMethod?.Invoke();

                FileConvertErrorType fileConvertErrorType = GetTypeException(ex);

                _messageAndLoggingService.ShowError(fileConvertErrorType,
                                                    ex.Message);
            }
            finally
            {
                ApplicationFinallyMethod?.Invoke();
            }
        }

        /// <summary>
        ///Отлов ошибок и вызов постметода асинхронно     
        /// </summary> 
        public async Task ExecuteAndHandleErrorAsync(Func<Task> asyncMethod,
                                                     Action ApplicationBeforeMethod = null,
                                                     Action ApplicationCatchMethod = null,
                                                     Action ApplicationFinallyMethod = null)
        {
            try
            {
                ApplicationBeforeMethod?.Invoke();
                await asyncMethod();
            }
            catch (Exception ex)
            {
                ApplicationCatchMethod?.Invoke();

                FileConvertErrorType fileConvertErrorType = GetTypeException(ex);

                _messageAndLoggingService.ShowError(fileConvertErrorType,
                                                    ex.Message);
            }
            finally
            {
                ApplicationFinallyMethod?.Invoke();
            }
        }

        /// <summary>
        /// Получить тип ошибки
        /// </summary>       
        private FileConvertErrorType GetTypeException(Exception ex)
        {
            var fileConvertErrorType = FileConvertErrorType.UnknownError;

            if (ex is NullReferenceException nullReferenceException)
            {
                fileConvertErrorType = FileConvertErrorType.NullReference;
            }
            else if (ex is ArgumentNullException argumentNullException)
            {
                fileConvertErrorType = FileConvertErrorType.ArgumentNullReference;
            }
            else if (ex is TimeoutException timeoutException)
            {
                fileConvertErrorType = FileConvertErrorType.TimeOut;
            }
            else if (ex is CommunicationException communicationException)
            {
                fileConvertErrorType = FileConvertErrorType.Communication;
            }

            return fileConvertErrorType;
        }
    }
}