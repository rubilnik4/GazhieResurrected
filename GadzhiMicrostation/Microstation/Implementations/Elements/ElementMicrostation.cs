﻿using GadzhiMicrostation.Extensions.Microstation;
using GadzhiMicrostation.Microstation.Interfaces;
using GadzhiMicrostation.Microstation.Interfaces.ApplicationMicrostationPartial;
using GadzhiMicrostation.Microstation.Interfaces.Elements;
using GadzhiMicrostation.Models.Coordinates;
using GadzhiMicrostation.Models.Enums;
using MicroStationDGN;
using System;

namespace GadzhiMicrostation.Microstation.Implementations.Elements
{
    public abstract class ElementMicrostation : IElementMicrostation
    {
        /// <summary>
        /// Экземпляр элемента Microstation
        /// </summary>
        private readonly Element _element;

        /// <summary>
        /// Родительский элемент
        /// </summary>
        protected IOwnerContainerMicrostation OwnerContainerMicrostation { get; private set; }

        /// <summary>
        /// Класс для работы с приложением Microstation
        /// </summary>
        public IApplicationMicrostation ApplicationMicrostation { get; }

        /// <summary>
        /// Модель или лист в файле
        /// </summary>
        public IModelMicrostation ModelMicrostation { get; }

        public ElementMicrostation(Element element,
                                   IOwnerContainerMicrostation ownerContainerMicrostation)
        {
            _element = element;
            OwnerContainerMicrostation = ownerContainerMicrostation;
            ApplicationMicrostation = OwnerContainerMicrostation?.ApplicationMicrostation;
            ModelMicrostation = OwnerContainerMicrostation.ModelMicrostation;
        }

        /// <summary>
        /// Идентефикатор элемента
        /// </summary>
        public long Id => _element.ID64;

        /// <summary>
        /// Коэффициент преобразования координат в текущие относительно родительского элемента
        /// </summary>
        public virtual double UnitScale => OwnerContainerMicrostation.UnitScale;

        /// <summary>
        /// Является ли базовый элемент Microstation текстовым
        /// </summary>       
        public bool IsTextElementMicrostation => this is ITextElementMicrostation;

        /// <summary>
        /// Является ли базовый элемент Microstation текстовым полем
        /// </summary>       
        public bool IsTextNodeElementMicrostation => this is ITextNodeElementMicrostation;

        /// <summary>
        /// Является ли базовый элемент Microstation ячейкой
        /// </summary>       
        public bool IsCellElementMicrostation => this is ICellElementMicrostation;

        /// <summary>
        /// Преобразование базового элемента Microstation в текстовый элемент
        /// </summary>       
        public ITextElementMicrostation AsTextElementMicrostation => (ITextElementMicrostation)this;

        /// <summary>
        /// Преобразование базового элемента Microstation в текстовым полем
        /// </summary>       
        public ITextNodeElementMicrostation AsTextNodeElementMicrostation => (ITextNodeElementMicrostation)this;

        /// <summary>
        /// Преобразование базового элемента Microstation в ячейку
        /// </summary>       
        public ICellElementMicrostation AsCellElementMicrostation => (ICellElementMicrostation)this;

        /// <summary>
        /// Тип элемента Microstation
        /// </summary>
        public ElementMicrostationType ElementType
        {
            get
            {
                ElementMicrostationType elementType = ElementMicrostationType.Element;

                if (IsTextElementMicrostation)
                {
                    elementType = ElementMicrostationType.TextElement;
                }
                else if (IsTextNodeElementMicrostation)
                {
                    elementType = ElementMicrostationType.TextNodeElement;
                }
                else if (IsCellElementMicrostation)
                {
                    elementType = ElementMicrostationType.CellElement;
                }

                return elementType;
            }
        }

        /// <summary>
        /// Переместить элемент
        /// </summary>
        public virtual void Move(PointMicrostation offset)
        {
            _element.Move(offset.ToPoint3d());
        }

        /// <summary>
        /// Повернуть элемент
        /// </summary>
        public virtual void Rotate(PointMicrostation origin, double degree)
        {
            _element.RotateAboutZ(origin.ToPoint3d(), degree * (Math.PI / 180));
        }

        /// <summary>
        /// Масштабировать элемент
        /// </summary>
        public virtual void ScaleAll(PointMicrostation origin, PointMicrostation scaleFactor)
        {
            _element.ScaleAll(origin.ToPoint3d(), scaleFactor.X, scaleFactor.Y, scaleFactor.Z);
        }

        /// <summary>
        /// Удалить текущий элемент
        /// </summary>
        public void Remove()
        {
           DLong parentId = _element.ParentID;
            if(parentId.High == 0 && parentId.Low == 0)
            {
                ModelMicrostation.RemoveElement(Id);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Получить значение аттрибута по его Id номеру
        /// </summary>       
        public string GetAttributeById(ElementMicrostationAttributes elementAttributes) =>
            _element.GetAttributeById(elementAttributes);

        /// <summary>
        /// Записать значение аттрибута по его Id номеру
        /// </summary>       
        public void SetAttributeById(ElementMicrostationAttributes elementAttributes, string attributeValue) =>
            _element.SetAttributeById(elementAttributes, attributeValue);

        /// <summary>
        /// Имя элемента из аттрибутов
        /// </summary>
        public string AttributeControlName
        {
            get => _element.GetAttributeControlName();
            set => _element.SetAttributeControlName(value);
        }

        /// <summary>
        /// Получить идентефикатор личности
        /// </summary>    
        public string AttributePersonId => _element.GetAttributePersonId();
    }
}