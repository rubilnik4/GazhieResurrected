﻿using GadzhiApplicationCommon.Extensions.Functional;
using GadzhiMicrostation.Extensions.Microstation;
using GadzhiMicrostation.Microstation.Interfaces.Elements;
using GadzhiMicrostation.Models.Enums;
using GadzhiMicrostation.Models.Implementations.Coordinates;
using MicroStationDGN;
using System;

namespace GadzhiMicrostation.Microstation.Implementations.Elements
{
    /// <summary>
    /// Базовый класс для элементов находящихся в рамке
    /// </summary>
    public abstract class RangeBaseElementMicrostation<TElement> : ElementMicrostation, IRangeBaseElementMicrostation<TElement>
                                                                   where TElement : class
    {
        /// <summary>
        /// Экземпляр элемента Microstation
        /// </summary>
        private readonly Element _element;

        /// <summary>
        /// Необходимо ли сжатие в рамке
        /// </summary>
        public bool IsNeedCompress { get; }

        /// <summary>
        /// Вертикальное расположение
        /// </summary>
        public bool IsVertical { get; }

        public RangeBaseElementMicrostation(Element element, IOwnerMicrostation ownerContainerMicrostation,
                                            bool isNeedCompress, bool isVertical)
            : base(element, ownerContainerMicrostation)
        {
            _element = element;
            IsNeedCompress = isNeedCompress;
            IsVertical = isVertical;
        }

        /// <summary>
        /// Координаты точки вставки
        /// </summary>
        public abstract PointMicrostation Origin { get; }

        /// <summary>
        /// Размеры ячейки элемента в текущих координатах
        /// </summary>
        public RangeMicrostation Range => new RangeMicrostation(_element.Range.Low.ToPointMicrostation(),
                                                                 _element.Range.High.ToPointMicrostation());

        /// <summary>
        /// Размеры ячейки элемента в стандартно заданных координатах
        /// </summary>
        private RangeMicrostation RangeAttribute =>
            GetAttributeFromCachOrLoad(ElementMicrostationAttributes.Range).
            Map(rangeInline => RangeMicrostation.StringToRange(rangeInline))
            ?? new RangeMicrostation();

        /// <summary>
        /// Возможено ли сжатие элемента
        /// </summary>
        protected bool IsValidToCompress => RangeAttribute.IsValid;

        /// <summary>
        /// Размеры ячейки элемента в стандартно заданных координатах
        /// </summary>
        public RangeMicrostation RangeAttributeInUnits => RangeAttribute.Scale(UnitScale);

        /// <summary>
        /// Ширина элемента в учетом поворота в текущих координатах
        /// </summary>
        protected double WidthWithRotation => !IsVertical ? Range.Width : Range.Height;

        /// <summary>
        /// Ширина элемента в учетом поворота в текущих координатах
        /// </summary>
        protected double HeightWithRotation => !IsVertical ? Range.Height : Range.Width;

        /// <summary>
        /// Ширина элемента в учетом поворота в стандартно заданных координатах
        /// </summary>
        protected double WidthAttributeWithRotationInUnits => !IsVertical ? RangeAttributeInUnits.Width : RangeAttributeInUnits.Height;

        /// <summary>
        /// Ширина элемента в учетом поворота в стандартно заданных координатах
        /// </summary>
        protected double HeightAttributeWithRotationInUnits => !IsVertical ? RangeAttributeInUnits.Height : RangeAttributeInUnits.Width;

        /// <summary>
        /// Вписать элемент в рамку
        /// </summary>
        public abstract bool CompressRange();

        /// <summary>
        /// Копировать элемент
        /// </summary>     
        public abstract TElement Copy(bool isVertical);
    }
}
