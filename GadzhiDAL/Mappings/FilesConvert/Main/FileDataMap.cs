﻿using FluentNHibernate.Mapping;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiDAL.Entities.FilesConvert;
using GadzhiDAL.Entities.FilesConvert.Main;
using NHibernate.Type;

namespace GadzhiDAL.Mappings.FilesConvert.Main
{
    /// <summary>
    /// Структура в БД для конвертируемого файла
    /// </summary>
    public class FileDataMap : ClassMap<FileDataEntity>
    {
        public FileDataMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.FilePath).Not.Nullable().Default("");         
            Map(x => x.ColorPrint).CustomType<ColorPrint>().Not.Nullable();
            Map(x => x.StatusProcessing).CustomType<StatusProcessing>().Not.Nullable();          
            Map(x => x.FileDataSourceClient).CustomType<BinaryBlobType>().LazyLoad();
            HasMany(x => x.FileConvertErrorType).Element("FileConvertErrorType");
            HasMany(x => x.FileDataSourceServerEntities).Inverse().Cascade.All();
            References(x => x.FilesDataEntity);
        }
    }
}
