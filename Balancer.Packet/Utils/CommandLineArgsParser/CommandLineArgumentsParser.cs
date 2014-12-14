#region Copyright
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

﻿namespace Balancer.Common.Utils.CommandLineArgsParser
{
    public class CommandLineArgumentsParser
    {
        private ComandLineArgument[] ComandLineArguments;

        public CommandLineArgumentsParser(ComandLineArgument[] comandLineArguments)
        {
            ComandLineArguments = comandLineArguments;
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
                else
                {
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
}
