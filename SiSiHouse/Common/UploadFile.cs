using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.AccessControl;

namespace SiSiHouse.Common
{
    public class UploadFile
    {
        public static string SaveFile(HttpPostedFileBase file, string savePath)
        {
            try
            {
                CreateFolder(savePath);

                DirectoryInfo directInfo = new DirectoryInfo(savePath);
                FileSystemAccessRule fullControlRule = new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity directSec = directInfo.GetAccessControl();
                directSec.AddAccessRule(fullControlRule);

                string extension = Path.GetExtension(file.FileName);
                var fileName = Path.GetFileName(file.FileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                var filePath = Path.Combine(savePath, fileName);
                string tempfileName = fileName;

                if (System.IO.File.Exists(filePath))
                {
                    int counter = 2;
                    while (System.IO.File.Exists(filePath))
                    {
                        tempfileName = fileNameWithoutExtension + "(" + counter.ToString() + ")" + extension;
                        filePath = Path.Combine(savePath, tempfileName);
                        counter++;
                    }
                }

                file.SaveAs(filePath);

                return tempfileName;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void MoveFile(string sourceFile, string targetFile)
        {
            if (File.Exists(sourceFile))
            {
                if (File.Exists(targetFile))
                    File.Delete(targetFile);

                File.Move(sourceFile, targetFile);
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            DirectoryInfo directInfo = new DirectoryInfo(folderPath);
            FileSystemAccessRule fullControlRule = new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow);
            DirectorySecurity directSec = directInfo.GetAccessControl();
            directSec.AddAccessRule(fullControlRule);
        }
    }
}