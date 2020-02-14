﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiDTO.TransferModels.FilesConvert
{
    /// <summary>
    /// Класс содержащий данные о конвертируемых файлах
    /// </summary>
    [DataContract]
    public class FilesDataRequest
    {
        /// <summary>
        /// ID идентефикатор
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентефикация пользователя
        /// </summary>
        [DataMember]
        public string IdentityName { get; set; }

        /// <summary>
        /// Количество попыток конвертирования
        /// </summary>   
        [DataMember]
        public int AttemptingConvertCount { get; set; }

        /// <summary>
        /// Данные о конвертируемых файлах
        /// </summary>
        [DataMember]
        public IEnumerable<FileDataRequest> FilesData { get; set; }

        /// <summary>
        /// Удовлетворяет ли модель условиям для отправки
        /// </summary>
        [IgnoreDataMember]
        public bool IsValid => FilesData?.Any() == true;
    }
}
