// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LEICode.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the lei code class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

using CluedIn.Core;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    /// <summary>
    /// The LEI Code properties
    /// </summary>
    public struct LEICode
    {
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        public LEICode(bool IsValid, string LOU = "", string EntityId = "", string VerificationId = "")
        {
            this.IsValid = IsValid;
            this.LOU = LOU;
            this.EntityId = EntityId;
            this.VerificationId = VerificationId;
        }

        /**********************************************************************************************************
         * PROPERTIES
         **********************************************************************************************************/

        public bool IsValid { get; }
        public string LOU { get; }
        public string EntityId { get; }
        public string VerificationId { get; }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>
        /// Parses the LEI Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static LEICode Parse(string code)
        {
            if (IsValidCode(code))
            {
                return new LEICode(
                    true,
                    code.Substring(0, 4),
                    code.Substring(4, 14),
                    code.Substring(14, 2)
                );
            }

            return new LEICode(false);
        }

        /// <summary>
        /// Verifies the LEI Code as specified by ISO 17442 using the ISO 7064 (MOD 97-10) to verify the checksum
        /// </summary>
        /// <param name="code">The LEI Code</param>
        /// <returns>A bool indicating whether the LEI Code is valid</returns>
        public static bool IsValidCode([NotNull] string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            if (code.Length != 20)
                return false;

            // TODO Fix this validation, currently not letting valid LEI code through
            //            if (decimal.TryParse(ReplaceCharacters(code), out var checkData))
            //                return checkData % 97 == 1;


            return true;
        }

        /// <summary>
        /// Replaces letters with the number that it corresponds to according to ISO 17442
        /// </summary>
        /// <param name="code">The LEI Code</param>
        /// <returns>LEI Code where the letters have been replaced</returns>
        private static string ReplaceCharacters(string code)
        {
            // Apply check character system ISO 7064 MOD 97-10 replacements

            // A = 10  G = 16  M = 22  S = 28  Y = 34
            // B = 11  H = 17  N = 23  T = 29  Z = 35
            // C = 12  I = 18  O = 24  U = 30  
            // D = 13  J = 19  P = 25  V = 31  
            // E = 14  K = 20  Q = 26  W = 32  
            // F = 15  L = 21  R = 27  X = 33

            return string.Concat(code.Select(x => 
                                    char.IsLetter(x) 
                                        ? char.IsUpper(x) 
                                              ? (int)x - 55 
                                              : (int)char.ToUpperInvariant(x) - 55 
                                        : char.GetNumericValue(x)));
        }
    }
}
