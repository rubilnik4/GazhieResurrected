﻿using GadzhiApplicationCommon.Extensions.Functional.Result;
using GadzhiApplicationCommon.Models.Enums;
using GadzhiApplicationCommon.Models.Interfaces.Errors;
using GadzhiMicrostation.Microstation.Interfaces.Elements;
using GadzhiMicrostation.Models.Enums;
using GadzhiMicrostation.Models.Implementations.StampFieldNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GadzhiApplicationCommon.Extensions.Functional;
using GadzhiApplicationCommon.Models.Enums.StampCollections;
using GadzhiApplicationCommon.Models.Implementation.Errors;
using GadzhiApplicationCommon.Models.Interfaces.LibraryData;
using GadzhiApplicationCommon.Models.Interfaces.StampCollections.Fields;
using GadzhiApplicationCommon.Models.Interfaces.StampCollections.Signatures;
using GadzhiMicrostation.Models.Implementations.StampCollections.Signatures;
using GadzhiMicrostation.Models.Interfaces.StampCollections;

namespace GadzhiMicrostation.Models.Implementations.StampCollections.StampMainPartial
{
    /// <summary>
    /// Строки с согласованиями
    /// </summary>
    public partial class StampMainMicrostation
    {
        /// <summary>
        /// Получить строки согласования с ответственным лицом без подписи
        /// </summary>
        private IResultAppCollection<IStampApproval> GetStampApprovalRows() =>
            GetStampSignatureRows(StampFieldType.ApprovalSignature, GetStampApprovalRow).
            Map(signatureRows => new ResultAppCollection<IStampApproval>(signatureRows, new ErrorApplication(ErrorApplicationType.SignatureNotFound,
                                                                                                             "Штамп подписей согласования не найден")));

        /// <summary>
        /// Преобразовать элементы Microstation в строку подписей согласования
        /// </summary>
        private IResultAppValue<IStampApproval> GetStampApprovalRow(IEnumerable<string> approvalNames) =>
            FindElementsInStamp<ITextElementMicrostation>(approvalNames, new ErrorApplication(ErrorApplicationType.SignatureNotFound,
                                                                                              "Поля подписей согласования не найдены")).
            ResultValueOkBind(GetStampApprovalFromFields);

        /// <summary>
        /// Получить строку с подписью согласования из полей штампа
        /// </summary>
        private IResultAppValue<IStampApproval> GetStampApprovalFromFields(IList<ITextElementMicrostation> foundFields)
        {
            var actionType = GetFieldFromElements(foundFields, StampFieldApprovals.GetFieldsDepartmentApproval(), StampFieldType.ApprovalSignature);
            var responsiblePerson = GetFieldFromElements(foundFields, StampFieldApprovals.GetFieldsResponsiblePerson(), StampFieldType.ApprovalSignature);
            var dateSignature = GetFieldFromElements(foundFields, StampFieldApprovals.GetFieldsDateSignature(), StampFieldType.ApprovalSignature);
            var insertSignatureFunc = InsertSignatureFunc(responsiblePerson.ElementStamp, dateSignature.ElementStamp, StampFieldType.ApprovalSignature);

            return GetStampApprovalById(responsiblePerson, insertSignatureFunc, actionType, dateSignature);
        }

        /// <summary>
        /// Сформировать строку с подписью согласования согласно идентификатору
        /// </summary>
        private IResultAppValue<IStampApproval> GetStampApprovalById(IStampTextFieldMicrostation responsiblePerson,
                                                                     Func<ISignatureLibraryApp, IResultAppValue<IStampField>> insertSignatureFunc,
                                                                     IStampTextField actionType, IStampTextField dateSignature) =>
            SignaturesSearching.FindByIdOrFullNameOrRandom(responsiblePerson.ElementStamp.AttributePersonId,
                                                           responsiblePerson.Text, StampSettings.PersonId).
            ResultValueOk(personSignature => new StampApprovalMicrostation(personSignature, insertSignatureFunc, actionType,
                                                                           responsiblePerson, dateSignature));
    }
}