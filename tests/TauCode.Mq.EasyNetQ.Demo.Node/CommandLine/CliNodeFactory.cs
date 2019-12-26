using System;
using TauCode.Parsing;
using TauCode.Parsing.Building;
using TauCode.Parsing.Nodes;
using TauCode.Parsing.TinyLisp;
using TauCode.Parsing.TinyLisp.Data;

namespace TauCode.Mq.EasyNetQ.Demo.Node.CommandLine
{
    public class CliNodeFactory : NodeFactory
    {
        public CliNodeFactory()
            : base("Node CLI")
        {
        }

        public override INode CreateNode(PseudoList item)
        {
            var car = item.GetCarSymbolName();
            var links = item.GetItemLinks();

            INode node;

            switch (car)
            {
                case "TERM":
                    node = new ExactWordNode(
                        item.GetSingleKeywordArgument<StringAtom>(":value").Value,
                        null,
                        this.NodeFamily,
                        item.GetItemName());
                    break;

                case "ANY-TERM":
                    node = new WordNode(
                        null,
                        this.NodeFamily,
                        item.GetItemName());
                    break;

                case "ANY-STRING":
                    node = new StringNode(
                        null,
                        this.NodeFamily,
                        item.GetItemName());
                    break;

                default:
                    throw new NotImplementedException();
            }

            return node;
        }
    }
}