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
    public class ComandLineArgument
    {
        public ComandLineArgument(string argument)
        {
            Argument = argument;
            Value = string.Empty;
        }

        public ComandLineArgument(string argument, string[] alternativeArguments) :
            this(argument)
        {
            AlternativeArguments = alternativeArguments;
        }

        public string Argument { get; protected set; }
        public string[] AlternativeArguments { get; protected set; }
        public string Value { get; protected set; }
        public bool IsDefined { get; protected set; }

        public void Define(string value)
        {
            IsDefined = true;
            Value = value;
        }

        public void UnDefine()
        {
            IsDefined = false;
            Value = string.Empty;
        }
    }
}