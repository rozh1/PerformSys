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
using System.Collections.Generic;

namespace PerformSys.Common.Utils.CommandLineArgsParser
{
    public class ComandLineArgument
    {
        public ComandLineArgument(string argument, bool isRequired = false)
        {
            Argument = argument;
            Value = string.Empty;
            IsRequired = isRequired;
            AlternativeArguments = new string[0];
        }

        public ComandLineArgument(string argument, string[] alternativeArguments, bool isRequired = false) :
            this(argument, isRequired)
        {
            AlternativeArguments = alternativeArguments;
        }

        public ComandLineArgument(string argument, string description, bool isRequired = false) :
            this(argument, isRequired)
        {
            Description = description;
        }

        public ComandLineArgument(string argument, string[] alternativeArguments, string description, bool isRequired = false) :
            this(argument, alternativeArguments, isRequired)
        {
            Description = description;
        }

        public string Argument { get; protected set; }
        public string[] AlternativeArguments { get; protected set; }
        public string Value { get; protected set; }
        public string Description { get; protected set; }
        public bool IsDefined { get; protected set; }
        public bool IsRequired { get; protected set; }

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

        public override string ToString()
        {
            return string.Format("{0}={2} - {1}", Argument, Description, IsDefined ? Value : "UnDef");
        }

        public string GetArgumentString()
        {
            var list = new List<string> { Argument };
            list.AddRange(AlternativeArguments);
            return string.Join(", ", list.ToArray());
        }
    }
}