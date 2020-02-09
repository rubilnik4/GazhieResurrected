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
    public abstract class EntityBase
    {
        public virtual uint Id { get; set; }
    }
}
