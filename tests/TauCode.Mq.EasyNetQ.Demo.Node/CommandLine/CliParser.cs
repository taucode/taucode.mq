using System.Collections.Generic;
using System.Linq;
using TauCode.Cli;
using TauCode.Cli.Tokens;
using TauCode.Extensions;
using TauCode.Mq.EasyNetQ.Demo.All.Commands;
using TauCode.Parsing;
using TauCode.Parsing.Building;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Nodes;
using TauCode.Parsing.TinyLisp;
using TauCode.Parsing.Tokens;

namespace TauCode.Mq.EasyNetQ.Demo.Node.CommandLine
{
    public class CliParser : Parser
    {
        private readonly CliLexer _lexer;
        private readonly IParser _parser;
        private readonly INode _root;

        public CliParser()
        {
            var nodeFactory = new CliNodeFactory();
            var input = this.GetType().Assembly.GetResourceText("Grammar.lisp", true);
            ILexer lispLexer = new TinyLispLexer();
            var tokens = lispLexer.Lexize(input);

            var reader = new TinyLispPseudoReader();
            var list = reader.Read(tokens);
            IBuilder builder = new Builder();
            _root = builder.Build(nodeFactory, list);
            _parser = new Parser();
            _lexer = new CliLexer();

            var allNodes = _root.FetchTree();
            this.AssignActions(allNodes);

            var exactWordNodes = allNodes
                .Where(x => x is ExactWordNode)
                .Cast<ExactWordNode>()
                .ToList();

            foreach (var exactWordNode in exactWordNodes)
            {
                exactWordNode.IsCaseSensitive = false;
            }
        }

        private void AssignActions(IReadOnlyCollection<INode> nodes)
        {
            var pub = (ActionNode)nodes.Single(x => x.Name == "PUB");
            pub.Action = (token, accumulator) =>
            {
                var command = new PublishCommand();
                accumulator.AddResult(command);
            };

            var sendTo = (ActionNode)nodes.Single(x => x.Name == "SEND-TO");
            sendTo.Action = (token, accumulator) =>
            {
                var command = accumulator.GetLastResult<PublishCommand>();
                command.To = ((WordToken)token).Word;
            };

            var messageText = (ActionNode)nodes.Single(x => x.Name == "MESSAGE-TEXT");
            messageText.Action = (token, accumulator) =>
            {
                var command = accumulator.GetLastResult<PublishCommand>();
                command.MessageText = ((StringToken)token).Value;
            };
        }

        public PublishCommand Parse(string input)
        {
            var tokens = _lexer.Lexize(input).Select(Convert).ToList();
            var objs = _parser.Parse(_root, tokens);
            return (PublishCommand)objs.Single();
        }

        private static IToken Convert(IToken token)
        {
            if (token is TermToken termToken)
            {
                return new WordToken(termToken.Value);
            }
            else
            {
                return token;
            }
        }
    }
}
