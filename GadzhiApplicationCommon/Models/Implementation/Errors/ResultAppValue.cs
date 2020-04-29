﻿using GadzhiApplicationCommon.Extensions.Functional;
using GadzhiApplicationCommon.Models.Interfaces.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GadzhiApplicationCommon.Models.Implementation.Errors
{
    /// <summary>
    /// Базовый вариант ответа
    /// </summary>
    public class ResultAppValue<TValue> : IResultAppValue<TValue>
    {
        public ResultAppValue(IErrorApplication error)
           : this(error.AsEnumerable()) { }

        public ResultAppValue(IEnumerable<IErrorApplication> errors)
        {
            var errorCollection = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
            if (!ValidateCollection(errorCollection)) throw new NullReferenceException(nameof(errors));

            Errors = errorCollection;
        }

        public ResultAppValue(TValue value, IErrorApplication errorNull = null)
          : this(value, Enumerable.Empty<IErrorApplication>(), errorNull) { }

        public ResultAppValue(TValue value, IEnumerable<IErrorApplication> errors, IErrorApplication errorNull = null)
            : this(errors)
        {
            InitializeValue(value, errorNull);
        }

        protected void InitializeValue(TValue value, IErrorApplication errorNull = null)
        {
            if (value == null && errorNull != null)
            {
                Errors = Errors.Concat(errorNull);
            }
            else if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Список значений
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Список ошибок
        /// </summary>
        public IEnumerable<IErrorApplication> Errors { get; private set; }

        /// <summary>
        /// Присутствуют ли ошибки
        /// </summary>
        public bool HasErrors => Errors.Any();

        /// <summary>
        /// Отсутствие ошибок
        /// </summary>
        public bool OkStatus => !HasErrors;

        /// <summary>
        /// Добавить ошибку
        /// </summary>      
        public IResultAppValue<TValue> ConcatErrors(IEnumerable<IErrorApplication> errors) =>
            errors != null ?
            Value.WhereContinue(value => value != null,
                okFunc: value => new ResultAppValue<TValue>(value, Errors.Union(errors)),
                badFunc: value => new ResultAppValue<TValue>(Errors.Union(errors))) :
            this;

        /// <summary>
        /// Преобразовать в результирующий тип
        /// </summary>
        public IResultApplication ToResultApplication() => new ResultApplication(Errors);

        /// <summary>
        /// Выполнить отложенные функции
        /// </summary>
        public IResultAppValue<TValue> Execute() => new ResultAppValue<TValue>(Value, Errors.ToList());

        /// <summary>
        /// Проверить ошибки на корректность
        /// </summary>      
        protected static bool ValidateCollection<T>(IEnumerable<T> collection) =>
            collection?.All(t => t != null) == true;
    }
}
