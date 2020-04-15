﻿using GadzhiCommon.Models.Interfaces.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiCommon.Models.Implementations.Errors
{
    /// <summary>
    /// Ответ с коллекцией
    /// </summary>
    public class ResultCollection<T> : ResultValue<IEnumerable<T>>, IResultCollection<T>
    {
        public ResultCollection()
          : base(Enumerable.Empty<T>()) { }

        public ResultCollection(IErrorCommon error)
            : base(error.AsEnumerable()) { }

        public ResultCollection(IEnumerable<IErrorCommon> errors)
           : base(errors) { }

        public ResultCollection(IEnumerable<T> collection)
          : this(collection, Enumerable.Empty<IErrorCommon>()) { }

        public ResultCollection(IEnumerable<T> collection, IEnumerable<IErrorCommon> errors)
            : base(collection, errors)
        {
            if (!ValidateCollection(collection)) throw new NullReferenceException(nameof(collection));
        }

        /// <summary>
        /// Добавить ответ с коллекцией
        /// </summary>      
        public IResultCollection<T> ConcatResult(IResultCollection<T> resultCollection) =>
            resultCollection != null ?
            new ResultCollection<T>(Value.Union(resultCollection.Value), 
                                    Errors.Union(resultCollection.Errors ?? Enumerable.Empty<IErrorCommon>())) :
            this;

        /// <summary>
        /// Добавить ответ со значением
        /// </summary>      
        public IResultCollection<T> ConcatResultValue(IResultValue<T> resultValue) =>
            resultValue != null ?
            new ResultCollection<T>(Value.Append(resultValue.Value),
                                    Errors.Union(resultValue.Errors ?? Enumerable.Empty<IErrorCommon>())) :
            this;

        /// <summary>
        /// Выполнить отложенные функции
        /// </summary>
        public new IResultCollection<T> Execute() => new ResultCollection<T>(Value.ToList(), Errors.ToList());
    }
}