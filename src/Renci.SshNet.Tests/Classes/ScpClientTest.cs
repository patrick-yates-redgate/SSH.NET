﻿using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Renci.SshNet.Common;
using Renci.SshNet.Tests.Common;

namespace Renci.SshNet.Tests.Classes
{
    /// <summary>
    /// Provides SCP client functionality.
    /// </summary>
    [TestClass]
    public partial class ScpClientTest : TestBase
    {
        private Random _random;

        [TestInitialize]
        public void SetUp()
        {
            _random = new Random();
        }

        [TestMethod]
        public void Ctor_ConnectionInfo_Null()
        {
            const ConnectionInfo connectionInfo = null;

            try
            {
                _ = new ScpClient(connectionInfo);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.InnerException);
                Assert.AreEqual("connectionInfo", ex.ParamName);
            }
        }

        [TestMethod]
        public void Ctor_ConnectionInfo_NotNull()
        {
            var connectionInfo = new ConnectionInfo("HOST", "USER", new PasswordAuthenticationMethod("USER", "PWD"));

            var client = new ScpClient(connectionInfo);
            Assert.AreEqual(16 * 1024U, client.BufferSize);
            Assert.AreSame(connectionInfo, client.ConnectionInfo);
            Assert.IsFalse(client.IsConnected);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.KeepAliveInterval);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.OperationTimeout);
            Assert.AreSame(RemotePathTransformation.DoubleQuote, client.RemotePathTransformation);
            Assert.IsNull(client.Session);
        }

        [TestMethod]
        public void Ctor_HostAndPortAndUsernameAndPassword()
        {
            var host = _random.Next().ToString();
            var port = _random.Next(1, 100);
            var userName = _random.Next().ToString();
            var password = _random.Next().ToString();

            var client = new ScpClient(host, port, userName, password);
            Assert.AreEqual(16 * 1024U, client.BufferSize);
            Assert.IsNotNull(client.ConnectionInfo);
            Assert.IsFalse(client.IsConnected);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.KeepAliveInterval);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.OperationTimeout);
            Assert.AreSame(RemotePathTransformation.DoubleQuote, client.RemotePathTransformation);
            Assert.IsNull(client.Session);

            var passwordConnectionInfo = client.ConnectionInfo as PasswordConnectionInfo;
            Assert.IsNotNull(passwordConnectionInfo);
            Assert.AreEqual(host, passwordConnectionInfo.Host);
            Assert.AreEqual(port, passwordConnectionInfo.Port);
            Assert.AreSame(userName, passwordConnectionInfo.Username);
            Assert.IsNotNull(passwordConnectionInfo.AuthenticationMethods);
            Assert.AreEqual(1, passwordConnectionInfo.AuthenticationMethods.Count);

            var passwordAuthentication = passwordConnectionInfo.AuthenticationMethods[0] as PasswordAuthenticationMethod;
            Assert.IsNotNull(passwordAuthentication);
            Assert.AreEqual(userName, passwordAuthentication.Username);
            Assert.IsTrue(Encoding.UTF8.GetBytes(password).IsEqualTo(passwordAuthentication.Password));
        }

        [TestMethod]
        public void Ctor_HostAndUsernameAndPassword()
        {
            var host = _random.Next().ToString();
            var userName = _random.Next().ToString();
            var password = _random.Next().ToString();

            var client = new ScpClient(host, userName, password);
            Assert.AreEqual(16 * 1024U, client.BufferSize);
            Assert.IsNotNull(client.ConnectionInfo);
            Assert.IsFalse(client.IsConnected);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.KeepAliveInterval);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.OperationTimeout);
            Assert.AreSame(RemotePathTransformation.DoubleQuote, client.RemotePathTransformation);
            Assert.IsNull(client.Session);

            var passwordConnectionInfo = client.ConnectionInfo as PasswordConnectionInfo;
            Assert.IsNotNull(passwordConnectionInfo);
            Assert.AreEqual(host, passwordConnectionInfo.Host);
            Assert.AreEqual(22, passwordConnectionInfo.Port);
            Assert.AreSame(userName, passwordConnectionInfo.Username);
            Assert.IsNotNull(passwordConnectionInfo.AuthenticationMethods);
            Assert.AreEqual(1, passwordConnectionInfo.AuthenticationMethods.Count);

            var passwordAuthentication = passwordConnectionInfo.AuthenticationMethods[0] as PasswordAuthenticationMethod;
            Assert.IsNotNull(passwordAuthentication);
            Assert.AreEqual(userName, passwordAuthentication.Username);
            Assert.IsTrue(Encoding.UTF8.GetBytes(password).IsEqualTo(passwordAuthentication.Password));
        }

        [TestMethod]
        public void Ctor_HostAndPortAndUsernameAndPrivateKeys()
        {
            var host = _random.Next().ToString();
            var port = _random.Next(1, 100);
            var userName = _random.Next().ToString();
            var privateKeys = new[] {GetRsaKey(), GetDsaKey()};

            var client = new ScpClient(host, port, userName, privateKeys);
            Assert.AreEqual(16 * 1024U, client.BufferSize);
            Assert.IsNotNull(client.ConnectionInfo);
            Assert.IsFalse(client.IsConnected);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.KeepAliveInterval);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.OperationTimeout);
            Assert.AreSame(RemotePathTransformation.DoubleQuote, client.RemotePathTransformation);
            Assert.IsNull(client.Session);

            var privateKeyConnectionInfo = client.ConnectionInfo as PrivateKeyConnectionInfo;
            Assert.IsNotNull(privateKeyConnectionInfo);
            Assert.AreEqual(host, privateKeyConnectionInfo.Host);
            Assert.AreEqual(port, privateKeyConnectionInfo.Port);
            Assert.AreSame(userName, privateKeyConnectionInfo.Username);
            Assert.IsNotNull(privateKeyConnectionInfo.AuthenticationMethods);
            Assert.AreEqual(1, privateKeyConnectionInfo.AuthenticationMethods.Count);

            var privateKeyAuthentication = privateKeyConnectionInfo.AuthenticationMethods[0] as PrivateKeyAuthenticationMethod;
            Assert.IsNotNull(privateKeyAuthentication);
            Assert.AreEqual(userName, privateKeyAuthentication.Username);
            Assert.IsNotNull(privateKeyAuthentication.KeyFiles);
            Assert.AreEqual(privateKeys.Length, privateKeyAuthentication.KeyFiles.Count);
            Assert.IsTrue(privateKeyAuthentication.KeyFiles.Contains(privateKeys[0]));
            Assert.IsTrue(privateKeyAuthentication.KeyFiles.Contains(privateKeys[1]));
        }

        [TestMethod]
        public void Ctor_HostAndUsernameAndPrivateKeys()
        {
            var host = _random.Next().ToString();
            var userName = _random.Next().ToString();
            var privateKeys = new[] { GetRsaKey(), GetDsaKey() };

            var client = new ScpClient(host, userName, privateKeys);
            Assert.AreEqual(16 * 1024U, client.BufferSize);
            Assert.IsNotNull(client.ConnectionInfo);
            Assert.IsFalse(client.IsConnected);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.KeepAliveInterval);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, -1), client.OperationTimeout);
            Assert.AreSame(RemotePathTransformation.DoubleQuote, client.RemotePathTransformation);
            Assert.IsNull(client.Session);

            var privateKeyConnectionInfo = client.ConnectionInfo as PrivateKeyConnectionInfo;
            Assert.IsNotNull(privateKeyConnectionInfo);
            Assert.AreEqual(host, privateKeyConnectionInfo.Host);
            Assert.AreEqual(22, privateKeyConnectionInfo.Port);
            Assert.AreSame(userName, privateKeyConnectionInfo.Username);
            Assert.IsNotNull(privateKeyConnectionInfo.AuthenticationMethods);
            Assert.AreEqual(1, privateKeyConnectionInfo.AuthenticationMethods.Count);

            var privateKeyAuthentication = privateKeyConnectionInfo.AuthenticationMethods[0] as PrivateKeyAuthenticationMethod;
            Assert.IsNotNull(privateKeyAuthentication);
            Assert.AreEqual(userName, privateKeyAuthentication.Username);
            Assert.IsNotNull(privateKeyAuthentication.KeyFiles);
            Assert.AreEqual(privateKeys.Length, privateKeyAuthentication.KeyFiles.Count);
            Assert.IsTrue(privateKeyAuthentication.KeyFiles.Contains(privateKeys[0]));
            Assert.IsTrue(privateKeyAuthentication.KeyFiles.Contains(privateKeys[1]));
        }

        [TestMethod]
        public void RemotePathTransformation_Value_NotNull()
        {
            var client = new ScpClient("HOST", 22, "USER", "PWD");

            Assert.AreSame(RemotePathTransformation.DoubleQuote, client.RemotePathTransformation);
            client.RemotePathTransformation = RemotePathTransformation.ShellQuote;
            Assert.AreSame(RemotePathTransformation.ShellQuote, client.RemotePathTransformation);
        }

        [TestMethod]
        public void RemotePathTransformation_Value_Null()
        {
            var client = new ScpClient("HOST", 22, "USER", "PWD")
            {
                RemotePathTransformation = RemotePathTransformation.ShellQuote
            };

            try
            {
                client.RemotePathTransformation = null;
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.InnerException);
                Assert.AreEqual("value", ex.ParamName);
            }

            Assert.AreSame(RemotePathTransformation.ShellQuote, client.RemotePathTransformation);
        }

        /// <summary>
        ///A test for OperationTimeout
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void OperationTimeoutTest()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            var expected = new TimeSpan(); // TODO: Initialize to an appropriate value
            target.OperationTimeout = expected;
            var actual = target.OperationTimeout;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BufferSize
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void BufferSizeTest()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            target.BufferSize = expected;
            actual = target.BufferSize;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Upload
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void UploadTest()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            DirectoryInfo directoryInfo = null; // TODO: Initialize to an appropriate value
            var filename = string.Empty; // TODO: Initialize to an appropriate value
            target.Upload(directoryInfo, filename);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Upload
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void UploadTest1()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            FileInfo fileInfo = null; // TODO: Initialize to an appropriate value
            var filename = string.Empty; // TODO: Initialize to an appropriate value
            target.Upload(fileInfo, filename);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Upload
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void UploadTest2()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            Stream source = null; // TODO: Initialize to an appropriate value
            var filename = string.Empty; // TODO: Initialize to an appropriate value
            target.Upload(source, filename);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Download
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void DownloadTest()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            var directoryName = string.Empty; // TODO: Initialize to an appropriate value
            DirectoryInfo directoryInfo = null; // TODO: Initialize to an appropriate value
            target.Download(directoryName, directoryInfo);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Download
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void DownloadTest1()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            var filename = string.Empty; // TODO: Initialize to an appropriate value
            FileInfo fileInfo = null; // TODO: Initialize to an appropriate value
            target.Download(filename, fileInfo);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Download
        ///</summary>
        [TestMethod]
        [Ignore] // placeholder for actual test
        public void DownloadTest2()
        {
            ConnectionInfo connectionInfo = null; // TODO: Initialize to an appropriate value
            var target = new ScpClient(connectionInfo); // TODO: Initialize to an appropriate value
            var filename = string.Empty; // TODO: Initialize to an appropriate value
            Stream destination = null; // TODO: Initialize to an appropriate value
            target.Download(filename, destination);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        private PrivateKeyFile GetRsaKey()
        {
            using (var stream = GetData("Key.RSA.txt"))
            {
                return new PrivateKeyFile(stream);
            }
        }

        private PrivateKeyFile GetDsaKey()
        {
            using (var stream = GetData("Key.SSH2.DSA.txt"))
            {
                return new PrivateKeyFile(stream);
            }
        }
    }
}
