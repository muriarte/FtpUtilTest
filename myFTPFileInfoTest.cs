using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FtpUtil;


namespace IntegraSyncTest
{
    [TestClass]
    public class myFTPFileInfoTest
    {
        [TestMethod]
        public void New_ConAnio_Ok() {
            string ftpListingLine = "-rwx------ 1 user group          16228 May 01 2013 TRAP121231.txt";
            var ftpFileInfo = new myFTPFileInfo(ftpListingLine);
            Assert.AreEqual("TRAP121231.txt", ftpFileInfo.Nombre);
            Assert.AreEqual(2013, ftpFileInfo.Fecha.Year);
            Assert.AreEqual(5, ftpFileInfo.Fecha.Month);
            Assert.AreEqual(1, ftpFileInfo.Fecha.Day);
            Assert.AreEqual(0, ftpFileInfo.Fecha.Hour);
            Assert.AreEqual(0, ftpFileInfo.Fecha.Minute);
            Assert.AreEqual(0, ftpFileInfo.Fecha.Second);
            Assert.AreEqual(16228, ftpFileInfo.Tamaño);
            Assert.AreEqual("user", ftpFileInfo.Usuario);
            Assert.AreEqual("group", ftpFileInfo.Grupo);
            Assert.AreEqual("rwx------", ftpFileInfo.Permisos);
            Assert.IsFalse(ftpFileInfo.IsFolder);
        }

        [TestMethod]
        public void New_SinAnioMesYDiaMenoresOIgualesAlActual_AñoActual() {
            IDateTimeProvider dateTimeProvider = new FakeDateTimeProvider(new DateTime(2014, 01, 26));
            DateTime fechaHoy = dateTimeProvider.GetCurrentDateTime();
            DateTime fechaAyer = fechaHoy - new TimeSpan(1, 0, 0, 0);
            string ftpListingLine = string.Format("-rwx------ 1 user group          16228 {0} {1:00} 13:15 TRAP121231.txt",
                                            "JanFebMarAprMayJunJulAugSepOctNovDec".Substring((fechaAyer.Month - 1) * 3, 3),
                                            fechaAyer.Day);
            var ftpFileInfo = new myFTPFileInfo(ftpListingLine, dateTimeProvider);
            Assert.AreEqual("TRAP121231.txt", ftpFileInfo.Nombre, "Nombre de archivo");
            Assert.AreEqual(fechaAyer.Year, ftpFileInfo.Fecha.Year, "Año");
            Assert.AreEqual(fechaAyer.Month, ftpFileInfo.Fecha.Month, "Mes");
            Assert.AreEqual(fechaAyer.Day, ftpFileInfo.Fecha.Day, "Dia");
            Assert.AreEqual(13, ftpFileInfo.Fecha.Hour, "Hora");
            Assert.AreEqual(15, ftpFileInfo.Fecha.Minute, "Minuto");
            Assert.AreEqual(0, ftpFileInfo.Fecha.Second, "Segundo");
            Assert.AreEqual(16228, ftpFileInfo.Tamaño, "Tamaño");
            Assert.AreEqual("user", ftpFileInfo.Usuario, "Usuario");
            Assert.AreEqual("group", ftpFileInfo.Grupo, "Grupo");
            Assert.AreEqual("rwx------", ftpFileInfo.Permisos, "Permisos");
            Assert.IsFalse(ftpFileInfo.IsFolder, "Indicador de Folder");
        }

        [TestMethod]
        public void New_SinAnioMesYDiaPosterioresAHoy_EsDelAñoPasado() {
            IDateTimeProvider dateTimeProvider = new FakeDateTimeProvider(new DateTime(2014, 01, 26));
            DateTime fechaHoy = dateTimeProvider.GetCurrentDateTime();
            DateTime fechaMañana = fechaHoy + new TimeSpan(1, 0, 0, 0);
            
            string ftpListingLine = string.Format("drwx------ 1 user group          16228 {0} {1:00} 13:15 backup",
                                            "JanFebMarAprMayJunJulAugSepOctNovDec".Substring((fechaMañana.Month - 1) * 3, 3),
                                            fechaMañana.Day);
            var ftpFileInfo = new myFTPFileInfo(ftpListingLine, dateTimeProvider);
            Assert.AreEqual(ftpFileInfo.Fecha.Year, fechaHoy.Year - 1);
        }

        [TestMethod]
        public void New_Folder_IsFolder() {
            string ftpListingLine = "drwx------ 1 user group          16228 May 01 2013 backup";
            var ftpFileInfo = new myFTPFileInfo(ftpListingLine);
            Assert.IsTrue(ftpFileInfo.IsFolder);
        }

    }
}
