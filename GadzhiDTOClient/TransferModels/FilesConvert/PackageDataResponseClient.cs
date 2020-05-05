﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GadzhiDTOBase.TransferModels.FilesConvert.Base;

namespace GadzhiDTOClient.TransferModels.FilesConvert
{
    /// <summary>
    /// Класс содержащий данные о отконвертированных файлах для клиента
    /// </summary>
    [DataContract]
    public class PackageDataResponseClient : PackageDataResponseBase<FileDataResponseClient, FileDataSourceResponseClient>
    {
        /// <summary>
        /// Данные о отконвертированных файлах
        /// </summary>
        [DataMember]
        public override IList<FileDataResponseClient> FilesData { get; set; }
    }
}