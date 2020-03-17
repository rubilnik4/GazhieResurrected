﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GadzhiApplicationCommon.Models.Interfaces.StampCollections
{
    /// <summary>
    /// Строка с ответственным лицом и подписью
    /// </summary>
    public interface IStampPersonSignature<out TField> where TField : IStampField
    {
        /// <summary>
        /// Тип действия
        /// </summary>
        TField ActionType { get; }

        /// <summary>
        /// Ответственное лицо
        /// </summary>
        TField ResponsiblePerson { get; }

        /// <summary>
        /// Подпись
        /// </summary>
        TField Signature { get; }

        /// <summary>
        /// Дата
        /// </summary>
        TField DateSignature { get; }

        /// <summary>
        /// Идентефикатор личности
        /// </summary>    
        string AttributePersonId { get; }
    }
}