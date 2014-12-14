namespace Balancer.Common.Utils.CommandLineArgsParser
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
