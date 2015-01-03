using System.Collections.Generic;

namespace Balancer.Common.Utils.CommandLineArgsParser
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
