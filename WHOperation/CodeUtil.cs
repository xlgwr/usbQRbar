/*****************************************************************************
 * Description: Definitions for the wrapper class around the CodeUtil library.
 * Copyright:   (c) 2010 Code Corporation. All rights reserved.
 *****************************************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Code
{
    class CodeUtil
    {
        public enum DownloadStatus
        {
            Ready = 0,
            DownloadingFile = 1,
            InstallingFile = 2,
            Rebooting = 3,
            DownloadError = -1
        }

        public delegate Int32 DownloadProgressCallback(IntPtr handle, DownloadStatus status, int percentComplete);
        public delegate Int32 OnNewDataCallback(IntPtr handle, IntPtr data, Int32 dataLength);
        public delegate Int32 OnNewImageCallback(IntPtr handle, Int32 width, Int32 height, IntPtr data, Int32 dataLength);
        public delegate Int32 OnProgressCallback(IntPtr handle, Int32 progress);
        public delegate void HardwareChangeCallback(Int32 status);

        public class NativeMethods
        {
            [DllImport("CodeUtil.dll")]
            public static extern IntPtr Code_CreateDevice(string deviceInfo, Int32 deviceInfoLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_DestroyDevice(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_DownloadFile(IntPtr handle, string fileName, Int32 fileNameLength, DownloadProgressCallback downloadProgress);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_GetReaderInfo(IntPtr handle, StringBuilder readerInfo, int readerInfoMaxLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_DeviceDisconnected(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_DeviceReconnected(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_GetLastError(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_GetCommSettings(IntPtr handle, StringBuilder settings, int settingsMaxLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_SetCommSettings(IntPtr handle, string settings, Int32 settingsLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_GetConfiguration(IntPtr handle, StringBuilder settings, int settingsMaxLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_UploadImage(IntPtr handle, IntPtr image, ref Int32 ImageSize, Int32 compressionLevel, Int32 imageField, OnProgressCallback uploadProgress);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_DownloadCrbFile(IntPtr handle, string fileName, Int32 fileNameLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_UploadFile(IntPtr handle, string fileName, Int32 fileNameLength, StringBuilder fileContents, ref Int32 fileContentsLength, OnProgressCallback uploadProgress);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_UploadFile(IntPtr handle, string fileName, Int32 fileNameLength, IntPtr fileContents, ref Int32 fileContentsLength, OnProgressCallback uploadProgress);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_TerminalStart(IntPtr handle, OnNewDataCallback onNewData, Boolean stripHeaders);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_TerminalStartEx(IntPtr handle, OnNewDataCallback onNewData, OnNewImageCallback onNewImage, OnProgressCallback onImageProgress);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_TerminalSendCommand(IntPtr handle, string command, Int32 commandLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_TerminalStop(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_GetFileList(IntPtr handle, string options, Int32 optionsLength, StringBuilder fileList, Int32 fileListMaxSize);

            [DllImport("CodeUtil.dll")]
            public static extern void Code_GetVersionString(StringBuilder version, Int32 versionLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_EraseFile(IntPtr handle, string fileName, Int32 fileNameLength);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_SwitchToKeyboard(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern IntPtr Code_CreateHardwareDetector(HardwareChangeCallback callback);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_DestroyHardwareDetector(IntPtr handle);

            [DllImport("CodeUtil.dll")]
            public static extern UInt32 Code_DetectHardwareXML(IntPtr handle, StringBuilder xmlString, UInt32 xmlStringMaxLength, Boolean temp);

            [DllImport("CodeUtil.dll")]
            public static extern Int32 Code_SwitchKeyboardToHidNative(); 
       
            
        }
    }
}
