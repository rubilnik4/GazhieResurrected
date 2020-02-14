﻿using FluentNHibernate.Mapping;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiDAL.Entities.FilesConvert;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiDAL.Mappings.FilesConvert
{
    public class FileDataMap : ClassMap<FileDataEntity>
    {
        public FileDataMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.FilePath).Not.Nullable().Default("");
            Map(x => x.IsCompleted).Not.Nullable();
            Map(x => x.ColorPrint).CustomType<ColorPrint>().Not.Nullable();
            Map(x => x.StatusProcessing).CustomType<StatusProcessing>().Not.Nullable();
            HasMany(x => x.FileConvertErrorType).Element("FileConvertErrorType");          
            Map(x => x.FileDataSource).CustomType<BinaryBlobType>().LazyLoad();           
            References(x => x.FilesDataEntity);
        }
    }
}