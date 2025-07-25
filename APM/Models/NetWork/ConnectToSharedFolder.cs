﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;

namespace APM.Models.NetWork
{
    public class ConnectToSharedFolder : IDisposable
    {
        readonly string _networkName;

        public ConnectToSharedFolder(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                userName,
                0);

            if (result != 0)
            {
                throw new Win32Exception(result, "Error connecting to remote share");
            }
        }

        ~ConnectToSharedFolder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }
    }

    public class ConnectToShareFolderFile
    {
        public string NetworkPath { get; set; }
        NetworkCredential Credentials { get; set; }
        public string myNetworkPath { get; set; }

        public ConnectToShareFolderFile()
        {
            NetworkPath=string.Empty;
            Credentials = new NetworkCredential();
        }
        public ConnectToShareFolderFile(string _NetworkPath,string _UserName,string _Password)
        {
            NetworkPath= _NetworkPath;
            Credentials = new NetworkCredential(_UserName, _Password);
        }

        public bool CheckConnected()
        {
            try
            {
                ConnectToSharedFolder connectToSharedFolder=  new ConnectToSharedFolder(NetworkPath, Credentials);

            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public bool FileUpload(string UploadURL)
        {
            try
            {
                using (new ConnectToSharedFolder(NetworkPath, Credentials))
                {
                    var fileList = Directory.GetDirectories(NetworkPath);

                    foreach (var item in fileList) { if (item.Contains("{ClientDocument}")) { myNetworkPath = item; } }

                    myNetworkPath = myNetworkPath + UploadURL;
                    //using (FileStream fileStream = File.Create(UploadURL, file.Length))
                    //{
                    //    await fileStream.WriteAsync(file, 0, file.Length);
                    //    fileStream.Close();
                    //}
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            } 
        }

        public byte[] DownloadFileByte(string DownloadURL)
        {
            byte[] fileBytes = null;

            using (new ConnectToSharedFolder(NetworkPath, Credentials))
            {
                var fileList = Directory.GetDirectories(NetworkPath);

                foreach (var item in fileList) { if (item.Contains("ClientDocuments")) { myNetworkPath = item; } }

                myNetworkPath = myNetworkPath + DownloadURL;

                try
                {
                    fileBytes = File.ReadAllBytes(myNetworkPath);
                }
                catch (Exception ex)
                {
                    string Message = ex.Message.ToString();
                }
            }

            return fileBytes;
        }



    }
}