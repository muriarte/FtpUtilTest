using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FtpUtil;


namespace FTPUtilTest
{
    [TestClass]
    public class myFTPTest
    {
        [TestMethod]
        public void ChangeFolder_FolderRaizVacioConFolderRelativoVacio_RegresaVacio() {
            var folderRaiz = "";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("", currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizVacioConFolderRelativo_RegresaFolderRelativo() {
            var folderRaiz = "";
            var folderRelativo = "boo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            ftp.ChangeFolder(folderRelativo);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRelativo, currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizSimpleSinFolderRelativo_RegresaVacio() {
            var folderRaiz = "/";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("", currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizSimpleConFolderRelativo_RegresaFolderRelativo() {
            var folderRaiz = "/";
            var folderRelativo = "boo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            ftp.ChangeFolder(folderRelativo);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRelativo, currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizCompletoSinDiagonalInicialSinFolderRelativo_RegresaFolderRaizConDiagonalInicial() {
            var folderRaiz = "foo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRaiz, currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizCompletoSinDiagonalInicialConFolderRelativo_RegresaFolderRaizConDiagonalInicialYConFolderRelativo() {
            var folderRaiz = "foo";
            var folderRelativo = "boo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            ftp.ChangeFolder(folderRelativo);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRaiz + "/" + folderRelativo, currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizCompletoConDiagonalInicialSinFolderRelativo_RegresaFolderRaizConDiagonalInicial() {
            var folderRaiz = "/foo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRaiz.Replace("/", ""), currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizCompletoConDiagonalInicialConFolderRelativo_RegresaFolderRaizConDiagonalInicialYConFolderRelativo() {
            var folderRaiz = "/foo";
            var folderRelativo = "boo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            ftp.ChangeFolder(folderRelativo);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRaiz.Replace("/", "") + "/" + folderRelativo, currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizCompletoConDiagonalInicialYFinalSinFolderRelativo_RegresaFolderRaizConDiagonalInicialSinDiagonalFinal() {
            var folderRaiz = "/foo/";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRaiz.Replace("/", ""), currentFolder);
        }

        [TestMethod]
        public void ChangeFolder_FolderRaizCompletoConDiagonalInicialYFinalConFolderRelativo_RegresaFolderRaizConDiagonalInicialYConFolderRelativo() {
            var folderRaiz = "/foo/";
            var folderRelativo = "boo";
            var ftp = new myFTP("ftp://example.com", "", "", folderRaiz);
            ftp.ChangeFolder(folderRelativo);
            var currentFolder = ftp.GetCurrentFolder();
            Assert.AreEqual("/" + folderRaiz.Replace("/", "") + "/" + folderRelativo, currentFolder);
        }
    }
}
