using Doera.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Utilities {
    internal class SlugGenerator : ISlugGenerator {
        public string Create(string s) {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            // Normalize accents
            s = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in s) {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            s = sb.ToString().Normalize(NormalizationForm.FormC);

            // Lowercase, remove invalid chars, replace spaces
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"[^\w\s-]", ""); // Remove punctuation/symbols
            s = Regex.Replace(s, @"\s+", "-");     // Replace spaces with hyphens
            s = Regex.Replace(s, @"-+", "-");      // Collapse multiple hyphens
            return s.Trim('-');                     // Trim leading/trailing hyphens
        }

        public string GenerateTagSlug(string tag) {
            if (string.IsNullOrWhiteSpace(tag))
                return string.Empty;

            // Special replacements
            tag = tag.Replace("#", "Sharp"); // C# -> CSharp, F# -> FSharp, etc.

            return Create(tag);
        }
    }
}
