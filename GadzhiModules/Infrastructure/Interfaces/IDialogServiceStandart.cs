﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiModules.Infrastructure.Interfaces
{
    /// <summary>
    /// Стандартные диалоговые окна
    /// </summary>
    public interface IDialogServiceStandard
    {
        /// <summary>
        /// Выбор файлов
        /// </summary>     
        Task<IEnumerable<string>> OpenFileDialog(bool isMultiselect, string filter);

        /// <summary>
        /// Выбор папки
        /// </summary>     
        Task<IEnumerable<string>> OpenFolderDialog(bool isMultiselect);

        /// <summary>
        /// Отобразить сообщение
        /// </summary>     
       void ShowMessage(string messageText);
    }
}