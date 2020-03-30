﻿using GadzhiCommon.Converters;
using GadzhiCommon.Extentions.Collection;
using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiCommon.Models.Interfaces.Errors;
using System;
using System.Collections.Generic;

namespace GadzhiConverting.Infrastructure.Implementations
{
    /// <summary>
    /// Класс для отображения изменений и логгирования
    /// </summary>
    public class MessagingService : IMessagingService
    {
        /// <summary>
        /// Запись в журнал системных сообщений
        /// </summary>
        private readonly ILoggerService _loggerService;

        public MessagingService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        /// <summary>
        /// Отобразить сообщение
        /// </summary>        
        public virtual void ShowAndLogMessage(string message)
        {
            _loggerService.LogMessage(message);
            ShowMessage(message);
        }

        /// <summary>
        /// Отобразить и добавить в журнал ошибку
        /// </summary>            
        public virtual void ShowAndLogError(IErrorConverting errorConverting)
        {
            ShowError(errorConverting);
            _loggerService.LogError(errorConverting);
        }

        /// <summary>
        /// Отобразить и добавить в журнал ошибки
        /// </summary>       
        public virtual void ShowAndLogErrors(IEnumerable<IErrorConverting> errorsConverting)
        {
            foreach (var error in errorsConverting.EmptyIfNull())
            {
                ShowError(error);
                _loggerService.LogError(error);
            }
        }

        /// <summary>
        /// Отобразить сообщение
        /// </summary>
        protected virtual void ShowMessage(string message) => Console.WriteLine(message);

        /// <summary>
        /// Отобразить сообщение
        /// </summary>
        protected virtual void ShowError(IErrorConverting errorConverting)
        {
            if (errorConverting != null)
            {
                string messageText = "Ошибка | " + ConverterErrorType.FileErrorTypeToString(errorConverting.FileConvertErrorType);
                if (!String.IsNullOrEmpty(errorConverting?.ErrorDescription))
                {
                    messageText += "\n" + errorConverting?.ErrorDescription;
                }
                Console.WriteLine(messageText);
            }
        }
    }
}
