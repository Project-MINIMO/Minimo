// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
// ReSharper disable StringLiteralTypo

namespace Doozy.Runtime.Common.Utils
{
    /// <summary>
    /// Class that holds a collection of useful methods to show code snippets in TextMeshProUGUI components.
    /// It's useful to show in an example scene the code that is used in a specific example.
    /// </summary>
    public static class CodeExampleUtils
    {
        /// <summary> Wrap the given text in a color tag with the given color type </summary>
        /// <param name="text"> Text to colorize </param>
        /// <param name="colorType"> ColorType </param>
        /// <returns> Colorized text </returns>
        public static string Colorize(string text, Colors.ColorType colorType) =>
            text?.Colorize(Colors.GetColor(colorType));

        /// <summary> Wrap the given text in a color tag used for punctuation </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsPunctuation(string text) => Colorize(text, Colors.ColorType.Punctuation);

        /// <summary> Wrap the given text in a color tag used for numbers </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsNumber(string text) => Colorize(text, Colors.ColorType.Number);

        /// <summary> Wrap the given text in a color tag used for strings </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsString(string text) => Colorize(text, Colors.ColorType.String);

        /// <summary> Wrap the given text in a color tag used for class names </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsClassName(string text) => Colorize(text, Colors.ColorType.ClassName);

        /// <summary> Wrap the given text in a color tag used for function names </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsFunctionName(string text) => Colorize(text, Colors.ColorType.FunctionName);

        /// <summary> Wrap the given text in a color tag used for property names </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsProperty(string text) => Colorize(text, Colors.ColorType.PropertyName);

        /// <summary> Wrap the given text in a color tag used for property values </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsPropertyValue(string text) => Colorize(text, Colors.ColorType.PropertyValue);

        /// <summary> Wrap the given text in a color tag used for comments </summary>
        /// <param name="text"> Text to colorize </param>
        /// <returns> Colorized text </returns>
        public static string IsComment(string text) => Colorize(text, Colors.ColorType.Comment);

        /// <summary>
        /// Colors used for the code example text.
        /// These colors are used to colorize the code example text by wrapping it in a color tag based on the color type.
        /// Types of colors are the most common ones used in code examples.
        /// </summary>
        public static class Colors
        {
            /// <summary> Describes the type of color used for the code example text </summary>
            public enum ColorType
            {
                Punctuation,
                Number,
                String,
                ClassName,
                FunctionName,
                PropertyName,
                PropertyValue,
                Comment
            }

            /// <summary> Get the corresponding color for the given ColorType </summary>
            /// <param name="colorType"> ColorType </param>
            /// <returns> Color </returns>
            public static Color GetColor(ColorType colorType)
            {
                switch (colorType)
                {
                    case ColorType.Punctuation: return punctuationColor;
                    case ColorType.Number: return numberColor;
                    case ColorType.String: return stringColor;
                    case ColorType.ClassName: return classNameColor;
                    case ColorType.FunctionName: return functionNameColor;
                    case ColorType.PropertyName: return propertyNameColor;
                    case ColorType.PropertyValue: return propertyValueColor;
                    case ColorType.Comment: return commentColor;
                    default: return Color.white;
                }
            }


            private static Color s_punctuationColor;
            /// <summary> Color used for punctuation </summary>
            public static Color punctuationColor => s_punctuationColor != default ? s_punctuationColor : s_punctuationColor = new Color().FromHEX("BDBDBD");

            private static Color s_numberColor;
            /// <summary> Color used for numbers </summary>
            public static Color numberColor => s_numberColor != default ? s_numberColor : s_numberColor = new Color().FromHEX("ED94C0");

            private static Color s_stringColor;
            /// <summary> Color used for strings </summary>
            public static Color stringColor => s_stringColor != default ? s_stringColor : s_stringColor = new Color().FromHEX("C9A26D");

            private static Color s_classNameColor;
            /// <summary> Color used for class names </summary>
            public static Color classNameColor => s_classNameColor != default ? s_classNameColor : s_classNameColor = new Color().FromHEX("C191FF");

            private static Color s_functionNameColor;
            /// <summary> Color used for function and method names </summary>
            public static Color functionNameColor => s_functionNameColor != default ? s_functionNameColor : s_functionNameColor = new Color().FromHEX("39CC8F");

            private static Color s_propertyNameColor;
            /// <summary> Color used for property names </summary>
            public static Color propertyNameColor => s_propertyNameColor != default ? s_propertyNameColor : s_propertyNameColor = new Color().FromHEX("6C95EB");

            private static Color s_propertyValueColor;
            /// <summary> Color used for property values </summary>
            public static Color propertyValueColor => s_propertyValueColor != default ? s_propertyValueColor : s_propertyValueColor = new Color().FromHEX("FFC66D");

            private static Color s_commentColor;
            /// <summary> Color used for comments </summary>
            public static Color commentColor => s_commentColor != default ? s_commentColor : s_commentColor = new Color().FromHEX("85C46C");
        }
    }
}
