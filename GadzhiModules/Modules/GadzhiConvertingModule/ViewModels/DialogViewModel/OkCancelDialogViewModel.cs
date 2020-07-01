﻿using System;
using System.ComponentModel;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Enums.DialogViewModel;
using GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.Base;

namespace GadzhiModules.Modules.GadzhiConvertingModule.ViewModels.DialogViewModel
{
    /// <summary>
    /// Диалоговое окно с кнопкой подтверждения
    /// </summary>
    public class OkCancelDialogViewModel: DialogViewModelBase
    {
        public OkCancelDialogViewModel(DialogButtonsType dialogButtonsType, string message)
        {
            DialogButtonsType = dialogButtonsType;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Типы кнопок для диалогового окна
        /// </summary>
        public DialogButtonsType DialogButtonsType { get; }

        /// <summary>
        /// Информационное сообщение
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public override string Title =>
            DialogButtonsType switch
            {
                DialogButtonsType.OkCancel => "Подтверждение",
                DialogButtonsType.RetryCancel => "Повтор",
                _ => "Подтверждение",
            };

        /// <summary>
        /// Текст первой кнопки
        /// </summary>
        public string FirstButtonText =>
            DialogButtonsType switch
            {
                DialogButtonsType.OkCancel => "ОК",
                DialogButtonsType.RetryCancel => "RETRY",
                _ => "ОК",
            };

        /// <summary>
        /// Текст второй кнопки
        /// </summary>
        public string SecondButtonText => "ОТМЕНА";
    }
}