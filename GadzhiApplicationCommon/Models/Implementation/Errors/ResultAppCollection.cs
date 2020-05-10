﻿using GadzhiApplicationCommon.Extensions.Collection;
using GadzhiApplicationCommon.Models.Interfaces.Errors;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GadzhiApplicationCommon.Extensions.Functional;

namespace GadzhiApplicationCommon.Models.Implementation.Errors
{
    /// <summary>
    /// Ответ с коллекцией
    /// </summary>
    public class ResultAppCollection<T> : ResultAppValue<IEnumerable<T>>, IResultAppCollection<T>
    {
        public ResultAppCollection()
          : base(Enumerable.Empty<T>()) { }

        public ResultAppCollection(IErrorApplication error)
            : base(Enumerable.Empty<T>(), error.AsEnumerable()) { }

        public ResultAppCollection(IEnumerable<IErrorApplication> errors)
           : base(Enumerable.Empty<T>(), errors) { }

        public ResultAppCollection(IEnumerable<T> collection, IErrorApplication errorNull = null)
          : this(collection, Enumerable.Empty<IErrorApplication>(), errorNull) { }

        public ResultAppCollection(IEnumerable<T> collection, IEnumerable<IErrorApplication> errors, IErrorApplication errorNull = null)
            : base(errors)
        {
            var collectionList = collection?.ToList();

            Errors = collectionList?.Count switch
            {
                null when errorNull != null => Errors.Concat(errorNull),
                0 when errorNull != null => Errors.Concat(errorNull),
                null => throw new ArgumentNullException(nameof(collection)),
                0 => throw new ArgumentNullException(nameof(collection)),
                _ => Errors
            };

            Value = collectionList;
        }

        /// <summary>
        /// Добавить ответ с коллекцией
        /// </summary>      
        public IResultAppCollection<T> ConcatResult(IResultAppCollection<T> resultCollection) =>
            resultCollection != null ?
            new ResultAppCollection<T>(Value.Union(resultCollection.Value),
                                       Errors.Union(resultCollection.Errors ?? Enumerable.Empty<IErrorApplication>())) :
            this;

        /// <summary>
        /// Добавить ответ со значением
        /// </summary>      
        public IResultAppCollection<T> ConcatResultValue(IResultAppValue<T> resultValue) =>
            resultValue != null ?
            new ResultAppCollection<T>(resultValue.Value != null
                                           ? Value.Concat(new List<T>() { resultValue.Value })
                                           : Value,
                                       Errors.Union(resultValue.Errors)) :
            throw new ArgumentNullException(nameof(resultValue));

        /// <summary>
        /// Добавить значение
        /// </summary>       
        public IResultAppCollection<T> ConcatValue(T value) =>
            value != null
                ? new ResultAppCollection<T>(Value.Concat(new List<T>() { value }), Errors)
                : throw new ArgumentNullException(nameof(value));

        /// <summary>
        /// Добавить значения
        /// </summary>       
        public IResultAppCollection<T> ConcatValues(IEnumerable<T> values) =>
            values.ToList().
            Map(valueCollection => ValidateCollection(valueCollection)
                                    ? new ResultAppCollection<T>(Value.Concat(valueCollection), Errors)
                                    : throw new NullReferenceException(nameof(valueCollection)));

        /// <summary>
        /// Выполнить отложенные функции
        /// </summary>
        public new IResultAppCollection<T> Execute() => new ResultAppCollection<T>(Value.ToList(), Errors.ToList());
    }
}
