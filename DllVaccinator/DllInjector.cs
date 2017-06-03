/*
 * MIT License
 * 
 * Copyright (c) 2017 - 0x2AFF
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DllVaccinator
{
    public sealed class DllInjector
    {
        private static volatile DllInjector _instance;
        private static object _syncRoot = new object();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private DllInjector() { }

        /// <summary>
        /// Get the current instance of the DllInjector.
        /// </summary>
        public static DllInjector Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new DllInjector();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Inject the dll to the process.
        /// </summary>
        /// <param name="processName">Process name</param>
        /// <param name="dllPath">Path to the DLL file</param>
        /// <returns></returns>
        public DllInjectionResult Inject(string processName, string dllPath)
        {
            if (!File.Exists(dllPath))
                return DllInjectionResult.DllNotFound;

            uint processId = 0;

            Process[] processList = Process.GetProcesses();
            for (int i = 0; i < processList.Length; i++)
            {
                if (processList[i].ProcessName == processName)
                {
                    processId = Convert.ToUInt32(processList[i].Id);
                    break;
                }
            }

            if (processId == 0)
                return DllInjectionResult.ProcessNotFound;

            if (!TryInject(processId, dllPath))
                return DllInjectionResult.InjectionFailed;

            return DllInjectionResult.InjectionSuccess;
        }

        private bool TryInject(uint processId, string dllPath)
        {
            IntPtr processHandle = Kernel32.OpenProcess(0x43A, true, processId);

            if (processHandle == IntPtr.Zero)
                return false;

            IntPtr moduleHandle = Kernel32.GetModuleHandle("kernel32.dll");
            IntPtr lpProcAddress = Kernel32.GetProcAddress(moduleHandle, "LoadLibraryA");

            if (lpProcAddress == IntPtr.Zero)
                return false;

            IntPtr lpAddress = Kernel32.VirtualAllocEx(processHandle, IntPtr.Zero, (IntPtr)dllPath.Length, 0x3000, 0x40);

            if (lpAddress == IntPtr.Zero)
                return false;

            byte[] bytes = Encoding.ASCII.GetBytes(dllPath);

            if (Kernel32.WriteProcessMemory(processHandle, lpAddress, bytes, Convert.ToUInt32(bytes.Length), 0) == 0)
                return false;

            if (Kernel32.CreateRemoteThread(processHandle, IntPtr.Zero, IntPtr.Zero, lpProcAddress, lpAddress, 0, IntPtr.Zero) == IntPtr.Zero)
                return false;

            Kernel32.CloseHandle(processHandle);

            return true;

        }
    }
}
