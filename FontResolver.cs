
/* 
using System;
using System.IO;
using System.Linq;
using System.Reflection;
//using PdfSharpCore.Fonts;

class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Tinos";

        public byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {
                var assembly = typeof(FontResolver).GetTypeInfo().Assembly;
                var resourceName = assembly.GetManifestResourceNames().First(x => x.EndsWith(faceName));
                using (var rs = assembly.GetManifestResourceStream(resourceName))
                {
                    rs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Tinos", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Tinos-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Tinos-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Tinos-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("Tinos-Regular.ttf");
                }
            }
            return null;
        }
    }*/