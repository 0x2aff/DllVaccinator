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

namespace DllVaccinator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DllVaccinator.exe processname dllpath");
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
                return;
            }

            string processName = args[0];
            string dllPath = args[1];

            DllInjector dllInjector = DllInjector.Instance;
            DllInjectionResult result = dllInjector.Inject(processName, dllPath);

            switch (result)
            {
                case DllInjectionResult.ProcessNotFound:
                    Console.WriteLine("Error: Can't find process: " + processName);
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                    break;
                case DllInjectionResult.DllNotFound:
                    Console.WriteLine("Error: Can't find dll: " + dllPath);
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                    break;
                case DllInjectionResult.InjectionFailed:
                    Console.WriteLine("Error: Injection failed!");
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                    break;
                case DllInjectionResult.InjectionSuccess:
                    Console.WriteLine("Info: Injection successful.");
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                    break;
            }
        }
    }
}
