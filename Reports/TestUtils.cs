using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
namespace electroweb.Reports
{
    public static class TestUtils
    {
         public static string GetBaseDir()
        {
            var currentAssembly = typeof(TestUtils).GetTypeInfo().Assembly;
            var root = Path.GetDirectoryName(currentAssembly.Location);
            var idx = root.IndexOf($"{Path.DirectorySeparatorChar}bin", StringComparison.OrdinalIgnoreCase);
            return root.Substring(0, idx);
        }

        public static string GetImagePath(string fileName)
        {

            return Path.Combine(GetBaseDir(), "wwwroot/Images", fileName);
        }

          public static string GetImagePathRemote(string fileName)
        {

            return Path.Combine("http://54.86.105.4/" ,"Fotos/", fileName);
        }

        public static string GetDataFilePath(string fileName)
        {

            return Path.Combine(GetBaseDir(), "Data", fileName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetOutputFileName([CallerMemberName] string methodName = null)
        {
            return Path.Combine(GetOutputFolder(), $"{methodName}.pdf");
        }
        public static string GetHtmlPage(string strURL)
        {

                String strResult;
                WebResponse objResponse;
                WebRequest objRequest = HttpWebRequest.Create(strURL);
                objResponse = objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    sr.Close();
                }
                return strResult;
        }
        public static string GetOutputFolder()
        {
            var dir = Path.Combine(GetBaseDir(), "bin", "out");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public static string GetRouteFonts()
        {
            return Path.Combine(GetBaseDir(), "wwwroot");
        }
        
        public static string GetWingdingFontPath()
        {
            return Path.Combine(GetBaseDir(), "wwwroot/fonts", "wingding.ttf");
        }

        public static string GetTahomaFontPath()
        {
            return Path.Combine(GetBaseDir(), "wwwroot/fonts", "tahoma.ttf");
        }

        public static string GetVerdanaFontPath()
        {
            return Path.Combine(GetBaseDir(), "wwwroot/fonts", "verdana.ttf");
        }

        public static Font GetUnicodeFont(
                    string fontName, string fontFilePath, float size, int style, BaseColor color)
        {
            if (!FontFactory.IsRegistered(fontName))
            {
                FontFactory.Register(fontFilePath);
            }
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
        }

        public static void VerifyPdfFileIsReadable(byte[] file)
        {
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(file);
                var author = reader.Info["Author"] as string;
                if (string.IsNullOrWhiteSpace(author) || !author.Equals("Vahid"))
                {
                    throw new InvalidPdfException("This is not a valid PDF file.");
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        public static void VerifyPdfFileIsReadable(string filePath)
        {
            VerifyPdfFileIsReadable(File.ReadAllBytes(filePath));
        }
    }
}