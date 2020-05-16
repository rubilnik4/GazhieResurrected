﻿using System;

namespace MicrostationSignatures.Models.Implementations
{
    public readonly struct SignatureLibrary : IEquatable<SignatureLibrary>
    {
        public SignatureLibrary(string id, string fullName, byte[] signatureJpeg)
            : this(id, fullName)
        {
            SignatureJpeg = ValidateSignatureJpeg(signatureJpeg)
                            ? signatureJpeg
                            : throw new ArgumentNullException(nameof(signatureJpeg));
        }

        public SignatureLibrary(string id, string fullName)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Fullname = fullName ?? throw new ArgumentNullException(nameof(fullName));
            SignatureJpeg = null;
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Имя 
        /// </summary>
        public string Fullname { get; }

        /// <summary>
        /// Изображение подписи
        /// </summary>
        public byte[] SignatureJpeg { get; }

        /// <summary>
        /// Проверить корректность данных
        /// </summary>
        public static bool ValidateSignatureJpeg(byte[] signatureJpeg) =>
            signatureJpeg?.Length > 0;

        #region IEquatable
        public override bool Equals(object obj) => obj != null && Equals((SignatureLibrary)obj);

        public bool Equals(SignatureLibrary other) => other.Id == Id;

        public static bool operator ==(SignatureLibrary left, SignatureLibrary right) => left.Equals(right);

        public static bool operator !=(SignatureLibrary left, SignatureLibrary right) => !(left == right);

        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode = hashCode * 31 + Id.GetHashCode();

            return hashCode;
        }
        #endregion
    }
}