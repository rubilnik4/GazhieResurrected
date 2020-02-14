﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiDAL.Entities
{
    /// <summary>
    /// Базовый класс для сущностей
    /// </summary>
    public abstract class EntityBase<IdType> where IdType: IEquatable<IdType>
    {
        /// <summary>
        /// Идентефикатор
        /// </summary>
        public abstract IdType Id { get; protected set; }
    }
}