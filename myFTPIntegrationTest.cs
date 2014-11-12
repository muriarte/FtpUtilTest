using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FtpUtil;

namespace FTPUtilTest
{
    [TestClass]
    public class myFTPIntegrationTest
    {
        //Para ejecutar este test es necesario que se ejecute un servidor de ftp en localhost con usuario "test" y password "test"
        //Recomendamos este test servers (acepta cualquier usuario y se puede ejecutar desde DOS con el comando ftpdmin %TEMP% ):
        // http://www.sentex.net/~mwandel/ftpdmin/
        string _testFolder = "ftputilprueba";
        string _testTimeDiffFileName = "integraasync.txt";
        const string FILENAME1 = "Prueba1.prb";
        const string FILENAME2 = "Prueba2.prb";
        const string FILENAME3 = "Prueba3.prb";
        const string FILENAME4 = "Prueba4.prb";
        const string FILENAME5 = "Prueba5.prb";
        const string FOLDERNAME1 = "SubFolder1.prb";
        const string FOLDERNAME2 = "SubFolder2.prb";

        const string FILECONTENT1 = "Contenido del archivo 1";
        const string FILECONTENT2 = "Contenido del archivo 2";
        const string FILECONTENT3 = "Contenido del archivo 3";
        const string FILECONTENT4 = "Contenido del archivo 4";
        const string FILECONTENT5 = "Contenido del archivo 5";

        [TestMethod]
        public void GetFileList() {
            myFTP ftp = PreparaTest();

            List<String> fileNames = ftp.GetFileList("*.*");
            Assert.AreEqual(6, fileNames.Count, "El número de archivos listados no coincide.");
            Assert.IsTrue(fileNames.Contains(FILENAME1), "No se enlistó el archivo 1");
            Assert.IsTrue(fileNames.Contains(FILENAME2), "No se enlistó el archivo 2");
            Assert.IsTrue(fileNames.Contains(FILENAME3), "No se enlistó el archivo 3");
            Assert.IsTrue(fileNames.Contains(FILENAME4), "No se enlistó el archivo 4");
            Assert.IsTrue(fileNames.Contains(FILENAME5), "No se enlistó el archivo 5");
            Assert.IsTrue(fileNames.Contains(FOLDERNAME1), "No se enlistó el subfolder 1");

            fileNames = ftp.GetFileList("Prue*.prb");
            Assert.AreEqual(5, fileNames.Count, "El número de archivos listados no coincide(a).");
            Assert.IsTrue(fileNames.Contains(FILENAME1), "No se enlistó el archivo 1(a)");
            Assert.IsTrue(fileNames.Contains(FILENAME2), "No se enlistó el archivo 2(a)");
            Assert.IsTrue(fileNames.Contains(FILENAME3), "No se enlistó el archivo 3(a)");
            Assert.IsTrue(fileNames.Contains(FILENAME4), "No se enlistó el archivo 4(a)");
            Assert.IsTrue(fileNames.Contains(FILENAME5), "No se enlistó el archivo 5(a)");
        }

        [TestMethod]
        public void DeleteFile() {
            myFTP ftp = PreparaTest();

            var ret = ftp.DeleteFile(FILENAME3);
            Assert.IsTrue(ret, "Falló al intentar eliminar archivo");

            List<String> fileNames = ftp.GetFileList("*.prb");
            Assert.AreEqual(5, fileNames.Count, "El número de archivos listados no coincide.");
            Assert.IsTrue(fileNames.Contains(FILENAME1), "No se enlistó el archivo 1");
            Assert.IsTrue(fileNames.Contains(FILENAME2), "No se enlistó el archivo 2");
            Assert.IsFalse(fileNames.Contains(FILENAME3), "No se enlistó el archivo 3");
            Assert.IsTrue(fileNames.Contains(FILENAME4), "No se enlistó el archivo 4");
            Assert.IsTrue(fileNames.Contains(FILENAME5), "No se enlistó el archivo 5");
            Assert.IsTrue(fileNames.Contains(FOLDERNAME1), "No se enlistó el subfolder 1");
        }

        [TestMethod]
        public void CreateFolder() {
            myFTP ftp = PreparaTest();
            var subFolderName = "SubFolder2.prb";
            var ret = ftp.CreateFolder(subFolderName);
            Assert.IsTrue(ret, "Falló al intentar crear subfolder");

            List<String> fileNames = ftp.GetFileList("*.prb");
            Assert.AreEqual(7, fileNames.Count, "El número de archivos listados no coincide.");
            Assert.IsTrue(fileNames.Contains(FILENAME1), "No se enlistó el archivo 1");
            Assert.IsTrue(fileNames.Contains(FILENAME2), "No se enlistó el archivo 2");
            Assert.IsTrue(fileNames.Contains(FILENAME3), "No se enlistó el archivo 3");
            Assert.IsTrue(fileNames.Contains(FILENAME4), "No se enlistó el archivo 4");
            Assert.IsTrue(fileNames.Contains(FILENAME5), "No se enlistó el archivo 5");
            Assert.IsTrue(fileNames.Contains(FOLDERNAME1), "No se enlistó el subfolder 1");
            Assert.IsTrue(fileNames.Contains(subFolderName), "No se enlistó el subfolder 2");
        }

        [TestMethod]
        public void GetCurrentFolder() {
            myFTP ftp = PreparaTest();
            var subFolderName = FOLDERNAME1;

            var strRet = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + _testFolder, strRet);

            var boolRet = ftp.ChangeFolder(subFolderName);
            Assert.IsTrue(boolRet, "Falló al intentar cambiarse al subfolder");

            strRet = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + _testFolder + "/" + subFolderName, strRet);

            List<String> fileNames = ftp.GetFileList("*.*");
            Assert.AreEqual(0, fileNames.Count, "No debe haber archivos en el subfolder.");
        }

        [TestMethod]
        public void GetFile(bool binaryMode) {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            var boolRet = ftp.GetFile(FILENAME3, tempFileName, binaryMode);
            Assert.IsTrue(boolRet, "Falló al copiar archivo remoto a archivo local");

            var content = System.IO.File.ReadAllText(tempFileName);
            Assert.AreEqual(FILECONTENT3, content);

            System.IO.File.Delete(tempFileName);
        }

        [TestMethod]
        public void GetFileDateTime() {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            DateTime ret = ftp.GetFileDateTime(FILENAME1);
            //Consideramos como validas hasta 24 horas diferencia del cliente con el servidor, la intención es detectar
            // solo las diferencias demasiado grandes
            Assert.IsTrue(((TimeSpan)(ret - DateTime.Now)).TotalHours < 24, "Hubo una diferencia demasiado grande entre la fecha del archivo y la fecha actual");
        }

        [TestMethod]
        public void GetFileListDetailed() {
            myFTP ftp = PreparaTest();

            List<myFTPFileInfo> list = ftp.GetFileListDetailed();
            Assert.AreEqual(6, list.Count, "La lista de archivos no es del tamaño esperado");
            foreach (myFTPFileInfo fileInfo in list) {
                switch (fileInfo.Nombre) {
                    case FILENAME1:
                        Assert.IsFalse(fileInfo.IsFolder, "Se detectó erroneamente como folder 1");
                        break;
                    case FILENAME2:
                        Assert.IsFalse(fileInfo.IsFolder, "Se detectó erroneamente como folder 2");
                        break;
                    case FILENAME3:
                        Assert.IsFalse(fileInfo.IsFolder, "Se detectó erroneamente como folder 3");
                        break;
                    case FILENAME4:
                        Assert.IsFalse(fileInfo.IsFolder, "Se detectó erroneamente como folder 4");
                        break;
                    case FILENAME5:
                        Assert.IsFalse(fileInfo.IsFolder, "Se detectó erroneamente como folder 5");
                        break;
                    case FOLDERNAME1:
                        Assert.IsTrue(fileInfo.IsFolder, "Se detectó erroneamente como archivo");
                        break;
                    default:
                        Assert.IsTrue(false, "Se detectó un archivo que no debe existir.");
                        break;
                }
            }
        }

        [TestMethod]
        public void GetFileSize() {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            long ret = ftp.GetFileSize(FILENAME1);
            Assert.AreEqual(FILECONTENT1.Length, ret, "Hubo una diferencia en el tamaño del archivo 1");
        }

        [TestMethod]
        public void GetFileString(bool binaryMode) {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            string ret = ftp.GetFileString(FILENAME1, binaryMode);
            Assert.AreEqual(FILECONTENT1, ret, "Hubo diferencia en el contenido del archivo 1");
        }

        [TestMethod]
        public void GetTimeDiff() {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            TimeSpan ret = ftp.GetTimeDiff();
            //Consideramos como validas hasta 24 horas diferencia del cliente con el servidor, la intención es detectar
            // solo las diferencias demasiado grandes
            Assert.IsTrue(ret.TotalHours < 24, string.Format("Hubo diferencia demasiado grande de tiempo entre cliente y servidor:{0} horas", ret.TotalHours));
        }

        [TestMethod]
        public void RemoveFolder() {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            List<myFTPFileInfo> list = ftp.GetFileListDetailed(FOLDERNAME1);
            Assert.AreEqual(1, list.Count, "La lista de archivos no tiene los elementos esperados");
            Assert.IsTrue(list[0].IsFolder, "El archivo listado no es un folder");

            bool ret = ftp.DeleteFolder(FOLDERNAME1);
            Assert.IsTrue(ret, "Hubo problemas al intentar eliminar el folder");

            list = ftp.GetFileListDetailed(FOLDERNAME1);
            Assert.AreEqual(0, list.Count, "La lista de archivos no tiene los elementos esperados");
        }

        [TestMethod]
        public void RenameFile() {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();

            List<myFTPFileInfo> list = ftp.GetFileListDetailed(FILENAME1);
            Assert.AreEqual(1, list.Count, "La lista de archivos no tiene los elementos esperados");
            Assert.IsFalse(list[0].IsFolder, "El archivo listado es un folder en vez de un archivo");

            bool ret = ftp.RenameFile(FILENAME1, "X" + FILENAME1);
            Assert.IsTrue(ret, "Hubo problemas al intentar renombrar el archivo");

            list = ftp.GetFileListDetailed(FILENAME1);
            Assert.AreEqual(0, list.Count, "La lista de archivos no tiene los elementos esperados");

            list = ftp.GetFileListDetailed("X" + FILENAME1);
            Assert.AreEqual(1, list.Count, "La lista de archivos no tiene los elementos esperados");
            Assert.IsFalse(list[0].IsFolder, "El archivo listado es un folder en vez de un archivo");

        }

        [TestMethod]
        public void UploadFileString() {
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();
            string contenidoDePrueba = "Prueba de upload";

            bool ret = ftp.UploadFileString("X" + FILENAME1, contenidoDePrueba);
            Assert.IsTrue(ret, "Hubo problemas al intentar renombrar el archivo");

            List<myFTPFileInfo> list = ftp.GetFileListDetailed("X" + FILENAME1);
            Assert.AreEqual(1, list.Count, "La lista de archivos no tiene los elementos esperados");
            Assert.IsFalse(list[0].IsFolder, "El archivo listado es un folder en vez de un archivo");

            string contenido = ftp.GetFileString("X" + FILENAME1, true);
            Assert.AreEqual(contenidoDePrueba, contenido, "El contenido del archivo no concuerda");

        }

        [TestMethod]
        public void UploadFile() {
            string contenidoDePrueba = "Este contenido es de prueba\r\notro contenido de prueba";
            myFTP ftp = PreparaTest();
            var tempFileName = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempFileName, contenidoDePrueba);

            var boolRet = ftp.UploadFile(tempFileName, "X" + FILENAME3);
            Assert.IsTrue(boolRet, "Falló al copiar archivo remoto a archivo local");

            string content = ftp.GetFileString("X" + FILENAME3, true);
            Assert.AreEqual(contenidoDePrueba, content, "No concuerda el contenido del archivo");

            System.IO.File.Delete(tempFileName);
        }


        private myFTP PreparaTest() {
            myFTP ftp;
            ftp = PreparaConexion();
            var ret = GeneraArchivosDePrueba();
            Assert.IsTrue(ret, "Falló la generación de archivos de prueba");
            ret = ftp.ChangeFolder(_testFolder);
            Assert.IsTrue(ret, "Falló al intentar ubicarse en el folder de prueba");
            return ftp;
        }

        private myFTP PreparaConexion() {
            return new myFTP("localhost", "test", "test", "/");
        }

        /// <summary>
        /// Genera archivos de prueba en un subfolder llamado "ftputilprueba"
        /// Los archivos son llamadoas
        /// </summary>
        /// <returns></returns>
        private bool GeneraArchivosDePrueba() {
            bool ret = true;
            myFTP ftp = PreparaConexion();

            //Eliminamos la estructura de folders y archivos de prueba
            List<myFTPFileInfo> files = ftp.GetFileListDetailed(_testFolder.ToLower());
            if (files.Count == 1 && files[0].Nombre.ToLower() == _testFolder.ToLower()) {
                if (files[0].IsFolder) {
                    //Ya existe el folder asi que eliminamos sus archivos y lo eliminamos a el
                    ret = ftp.ChangeFolder(_testFolder.ToLower());
                    if (!ret) return false;
                    //Elimina los archivo .prb
                    List<string> fileNames = ftp.GetFileList("*.prb");
                    foreach (string fileName in fileNames) {
                        if (!fileName.StartsWith("SubFolder")) {
                            ret = ftp.DeleteFile(fileName);
                        }
                        else {
                            ret = ftp.DeleteFolder(fileName);
                        }
                        if (!ret) return false;
                    }

                    //Elimina el archivo que se utiliza para obtener el timediff
                    fileNames = ftp.GetFileList(_testTimeDiffFileName);
                    foreach (string fileName in fileNames) {
                        ret = ftp.DeleteFile(fileName);
                        if (!ret) return false;
                    }

                    //Regresamos al folder raiz para eliminar la carpeta de prueba
                    ret = ftp.ChangeFolder("/");
                    if (!ret) return false;
                    ret = ftp.DeleteFolder(_testFolder);
                    if (!ret) return false;
                }
                else {
                    //Por alguna razon se habia creado un archivo con el nombre del folder de prueba
                    ret = ftp.DeleteFile(_testFolder);
                }
            }

            //Creamos la estructura de folders y archivos de prueba
            ret = ftp.CreateFolder(_testFolder);
            if (!ret) return false;

            ret = ftp.ChangeFolder(_testFolder);
            if (!ret) return false;

            ret = ftp.UploadFileString(FILENAME1, FILECONTENT1);
            if (!ret) return false;
            ret = ftp.UploadFileString(FILENAME2, FILECONTENT2);
            if (!ret) return false;
            ret = ftp.UploadFileString(FILENAME3, FILECONTENT3);
            if (!ret) return false;
            ret = ftp.UploadFileString(FILENAME4, FILECONTENT4);
            if (!ret) return false;
            ret = ftp.UploadFileString(FILENAME5, FILECONTENT5);
            if (!ret) return false;
            ret = ftp.CreateFolder(FOLDERNAME1);
            if (!ret) return false;

            return ret;
        }

    }
}
