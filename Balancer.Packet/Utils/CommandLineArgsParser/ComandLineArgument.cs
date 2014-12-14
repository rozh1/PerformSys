namespace Balancer.Common.Utils.CommandLineArgsParser
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