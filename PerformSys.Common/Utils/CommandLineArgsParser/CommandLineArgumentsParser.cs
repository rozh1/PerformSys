﻿#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
 #endregion
using System.Collections.Generic;

namespace PerformSys.Common.Utils.CommandLineArgsParser
{
    public class CommandLineArgumentsParser
    {
        private ComandLineArgument[] ComandLineArguments;

        public CommandLineArgumentsParser(ComandLineArgument[] comandLineArguments)
        {
            ComandLineArguments = comandLineArguments;
        }

        public string[] GetDescriptions()
        {
            var descriptions = new List<string>();
            int maxArgumentLength = FindMaxArgumentLenght(ComandLineArguments);

            foreach (var comandLineArgument in ComandLineArguments)
            {
                string spaces = string.Empty;
                string argument = comandLineArgument.GetArgumentString();
                int spaceLenght = maxArgumentLength - argument.Length;

                for (int i = 0; i < spaceLenght; i++)
                {
                    spaces += " ";
                }

                string description = string.Format("{0}{2} - {1}", argument, comandLineArgument.Description, spaces);
                descriptions.Add(description);
            }

            return descriptions.ToArray();
        }

        private int FindMaxArgumentLenght(ComandLineArgument[] comandLineArguments)
        {
            int maxLenght = 0;
            foreach (var comandLineArgument in comandLineArguments)
            {
                int lenght = comandLineArgument.GetArgumentString().Length;

                if (maxLenght < lenght) maxLenght = lenght;
            }
            return maxLenght;
        }

        public ComandLineArgument[] Parse(string[] args)
        {
            ClearParse();
            for (int i = 0; i < args.Length; i++)
            {
                CheckArgument(args[i], i + 1 < args.Length ? args[i + 1] : string.Empty);
            }
            return ComandLineArguments;
        }

        private void ClearParse()
        {
            foreach (ComandLineArgument comandLineArgument in ComandLineArguments)
            {
                comandLineArgument.UnDefine(); 
            }
        }

        private void CheckArgument(string argument, string nextArgument)
        {
            string arg = argument.ToLower();
            foreach (ComandLineArgument comandLineArgument in ComandLineArguments)
            {
                if (arg == comandLineArgument.Argument.ToLower())
                {
                    comandLineArgument.Define(nextArgument);
                    return;
                }
                foreach (var alternativeAgument in comandLineArgument.AlternativeArguments)
                {
                    if (arg == alternativeAgument.ToLower())
                    {
                        comandLineArgument.Define(nextArgument);
                        return;
                    }
                }
            }
        }
    }
}
