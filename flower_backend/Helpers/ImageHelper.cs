using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public static class ImageHelper
    {
        public static string SaveBase64(string base64ImageString)
        {
            try
            {
                var fileType = base64ImageString.TrimStart(',').Split(',')[0].TrimStart(';').Split(';')[0].TrimStart('/').Split('/')[1];
                string fileName = Guid.NewGuid().ToString();
                var currentDirectory = Directory.GetCurrentDirectory();
                string folder = currentDirectory + "/" + "uploadFiles/images";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = folder + "/" + fileName + "." + fileType;
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64ImageString.Split(";base64,")[1]));
                return fileName + "." + fileType;
            }
            catch
            {
                return "";
            }
        }

        public static void DeleteImage(string fileName)
        {
            try
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                string filePath = currentDirectory + "/" + "uploadFiles/images" + "/" + fileName;
                File.Delete(filePath);
            } catch (Exception e)
            {

            }
        }
    }
}
