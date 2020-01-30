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
        public FilesDataRequest()
        {
            ID = new Guid();
        }
        /// <summary>
        /// ID идентефикатор
        /// </summary>
        [DataMember]
        public Guid ID { get; private set; }

        /// <summary>
        /// Данные о конвертируемых файлах
        /// </summary>
        [DataMember]
        public IEnumerable<FileDataRequest> FilesData { get; set; }

        /// <summary>
        /// Удовлетворяет ли модель условиям для отправки
        /// </summary>
        public bool IsValidToSend => FilesData?.Any() == true;
    }
}
