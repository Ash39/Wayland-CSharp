using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using WaylandProtocal.XmlTypes;

namespace WaylandProtocal
{
    public class CodeGenerator
    {
        private static readonly List<Protocol> protocols = new List<Protocol>();

        static void Main(string[] args) 
        {
            string directory = Path.GetDirectoryName(args[0]);

            XmlSerializer serializer = new XmlSerializer(typeof(Protocol));

            foreach (string path in Directory.GetFiles(Path.Combine(directory, "WaylandProtocol")))
            {
                if (!Path.HasExtension(".xml"))
                    continue;
                using (StreamReader streamReader = File.OpenText(path))
                {
                    protocols.Add((Protocol)serializer.Deserialize(streamReader));
                }
            }

            foreach (Protocol protocol in protocols)
            {
                foreach (Interface @interface in protocol.Interfaces)
                {
                    using (StreamWriter streamWriter = File.CreateText(Path.Combine(directory, "Generated", ToTitleCase(@interface.Name) + ".Gen.cs")))
                    {

                        string classComment = string.Format(commentBase, @interface.Description.Summary, @interface.Description.Content);

                        string classCode = "using System;" + Environment.NewLine;
                        classCode += "using System.Collections.Generic;" + Environment.NewLine;
                        classCode += "namespace Wayland" + Environment.NewLine;
                        classCode += "{" + Environment.NewLine;
                        //classCode += string.Format("{0}", classComment) + Environment.NewLine;
                        classCode += string.Format("public partial class {0} : WaylandObject", ToTitleCase(@interface.Name)) + Environment.NewLine;
                        classCode += "{" + Environment.NewLine;
                        classCode += string.Format(@"public const string INTERFACE = ""{0}"";", @interface.Name) + Environment.NewLine;
                        classCode += string.Format("public {0} (uint id, uint version, WaylandConnection connection) : base(id, version, connection)", ToTitleCase(@interface.Name)) + Environment.NewLine;
                        classCode += "{";
                        classCode += "}";
                        classCode += BuildMethods(@interface.Requests) + Environment.NewLine;
                        classCode += BuildWaylandTypesHandler(@interface) + Environment.NewLine;
                        classCode += "}" + Environment.NewLine;
                        classCode += "}" + Environment.NewLine;


                        streamWriter.WriteLine(CSharpSyntaxTree.ParseText(classCode).GetRoot().NormalizeWhitespace().ToFullString());
                    }
                }

            }
        }


        private static string BuildWaylandTypesHandler(Interface @interface)
        {
            StringBuilder stringBuilder = new StringBuilder();

            List<string> opCodes = new List<string>();
            List<string> auguments = new List<string>();
            List<string> augumentsUse = new List<string>();
            List<string> augumentsTypes = new List<string>();
            List<string> augumentsNames = new List<string>();
            List<string> actionEvents = new List<string>();


            foreach (Message message in @interface.Events)
            {
                augumentsTypes.Clear();
                augumentsNames.Clear();

                augumentsNames.Add("this");
                augumentsTypes.Add(ToTitleCase(@interface.Name));

                opCodes.Add(ToTitleCase(message.Name));
                string waylandTypeCaseBase = string.Format("case EventOpcode.{0}:", ToTitleCase(message.Name)) + Environment.NewLine;
                waylandTypeCaseBase += "return new WaylandType[]" + Environment.NewLine;
                waylandTypeCaseBase += "{" + Environment.NewLine;

                string waylandUseBase = string.Format("case EventOpcode.{0}:", ToTitleCase(message.Name)) + Environment.NewLine;
                waylandUseBase += "{" + Environment.NewLine;

                int i = 0;
                foreach (Argument arg in message.Arguments)
                {

                    if (arg.Name == "interface")
                    {
                        arg.Name = "@interface";
                    }


                    waylandTypeCaseBase += string.Format("WaylandType.{0},", ToTitleCase(arg.Type)) + Environment.NewLine;

                    string wtype = GetType(arg.Type);
                    if (wtype == "WaylandObject")
                    {
                        waylandUseBase += string.Format("var {0} = connection[(uint)arguments[{2}]];", ToCamelCase(arg.Name), wtype, i) + Environment.NewLine;
                    }
                    else
                    {
                        waylandUseBase += string.Format("var {0} = ({1})arguments[{2}];", ToCamelCase(arg.Name), wtype, i) + Environment.NewLine;
                    }

                    augumentsTypes.Add(GetType(arg.Type));
                    augumentsNames.Add(ToCamelCase(arg.Name));

                    i++;
                }

                waylandUseBase += string.Format("if(this.{0} != null)", ToCamelCase(message.Name)) + Environment.NewLine;
                waylandUseBase += "{";
                waylandUseBase += string.Format("this.{0}.Invoke({1});", ToCamelCase(message.Name), string.Join(", ", augumentsNames)) + Environment.NewLine;
                waylandUseBase += string.Format(@"DebugLog.WriteLine($""{{ INTERFACE}}@{{this.id}}.{{ EventOpcode.{0} }}({1})"");" + Environment.NewLine, ToTitleCase(message.Name), string.Join(",", augumentsNames.Select(c => "{" + c + "}")));
                waylandUseBase += "}";

                if (augumentsTypes.Count > 0)
                    actionEvents.Add(string.Format("public Action<{0}> {1};",string.Join(", ",augumentsTypes), ToCamelCase(message.Name)) + Environment.NewLine);
                else
                    actionEvents.Add(string.Format("public Action {0};", ToCamelCase(message.Name)) + Environment.NewLine);

                waylandUseBase += "break;" + Environment.NewLine;
                waylandUseBase += "}" + Environment.NewLine;

                waylandTypeCaseBase += "};" + Environment.NewLine;
                auguments.Add(waylandTypeCaseBase);

                augumentsUse.Add(waylandUseBase);
            }

            string waylandTypeBase = "public override WaylandType[] WaylandTypes(ushort opCode)" + Environment.NewLine;
            waylandTypeBase += "{" + Environment.NewLine;
            waylandTypeBase += "switch ((EventOpcode)opCode)" + Environment.NewLine;
            waylandTypeBase += "{" + Environment.NewLine;
            waylandTypeBase += string.Format("{0}", string.Join(Environment.NewLine, auguments));
            waylandTypeBase += "default:" + Environment.NewLine;
            waylandTypeBase += @"throw new ArgumentOutOfRangeException(""unknown event"");" + Environment.NewLine;
            waylandTypeBase += "}" + Environment.NewLine;
            waylandTypeBase += "}" + Environment.NewLine;

            string waylandEventBase = "public override void Event(ushort opCode, object[] arguments)" + Environment.NewLine;
            waylandEventBase += "{" + Environment.NewLine;
            waylandEventBase += "switch ((EventOpcode)opCode)" + Environment.NewLine;
            waylandEventBase += "{" + Environment.NewLine;
            waylandEventBase += string.Format("{0}", string.Join(Environment.NewLine, augumentsUse));
            waylandEventBase += "default:" + Environment.NewLine;
            waylandEventBase += @"throw new ArgumentOutOfRangeException(""unknown event"");" + Environment.NewLine;
            waylandEventBase += "}" + Environment.NewLine;
            waylandEventBase += "}" + Environment.NewLine;

            string eventOpCodeBase = "public enum EventOpcode : ushort" + Environment.NewLine;
            eventOpCodeBase += "{" + Environment.NewLine;
            eventOpCodeBase += string.Join("," + Environment.NewLine,opCodes) + Environment.NewLine;
            eventOpCodeBase += "}" + Environment.NewLine;

            stringBuilder.AppendLine(string.Join(Environment.NewLine, actionEvents));
            stringBuilder.AppendLine(eventOpCodeBase);
            stringBuilder.AppendLine(waylandEventBase);
            stringBuilder.AppendLine(waylandTypeBase);

            return stringBuilder.ToString();
        }

        private static string BuildMethods(List<Message> requests)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<string> opCodes = new List<string>();
            foreach (Message message in requests)
            {
                List<string> methodArgs = new List<string>();
                List<string> methodArgsUsage = new List<string>();
                string returnCall = string.Empty;
                string returnType = "void";
                string generateId = string.Empty;
                string generateNew = string.Empty;
                opCodes.Add(ToTitleCase(message.Name));

                foreach (Argument arg in message.Arguments)
                {
                    if (arg.Name == "interface")
                    {
                        arg.Name = "@interface";
                    }

                    if (arg.Type != "object")
                        methodArgsUsage.Add(arg.Name);

                    if (arg.Type == "new_id")
                    {
                        returnType = string.IsNullOrEmpty(arg.Interface) ? "T" : ToTitleCase(arg.Interface);
                        returnCall = $"return ({returnType})connection[{arg.Name}];";
                        generateId = $"uint {arg.Name} = connection.Create();";
                        generateNew = string.IsNullOrEmpty(arg.Interface) ? $"connection[{arg.Name}] = (WaylandObject)Activator.CreateInstance(typeof(T),{arg.Name}, version, connection);" : $"connection[{arg.Name}] = new {returnType}({arg.Name}, version, connection);";
                        goto extraArgs;
                    }

                    if (arg.Type == "object")
                    {
                        methodArgsUsage.Add(arg.Name + ".id");
                        methodArgs.Add(ToTitleCase(arg.Interface) + " " + arg.Name);
                        goto extraArgs;
                    }
                    methodArgs.Add(GetType(arg.Type) + " " + arg.Name);

                extraArgs:
                    if (arg.Type == "new_id" && string.IsNullOrEmpty(arg.Interface))
                    {
                        //Debug();
                        methodArgs.Add("string @interface");
                        methodArgs.Add("uint version");

                        methodArgsUsage.Insert(1, "@interface");
                        methodArgsUsage.Insert(2, "version");
                    }
                }
                if (methodArgsUsage.Count > 0)
                {
                    methodArgsUsage[0] = ", " + methodArgsUsage[0];
                }

                string comment = string.Format(commentBase, message.Description.Summary, message.Description.Content);

                string methodName = ToTitleCase(message.Name);

                string requestBase = string.Empty;
                //requestBase +=  string.Format("{0}" + Environment.NewLine, comment);
                if (returnType == "T")
                    requestBase += string.Format("public {0} {1}<T>({2}) where T : WaylandObject" + Environment.NewLine, returnType, methodName, string.Join(",", methodArgs));
                else
                    requestBase += string.Format("public {0} {1}({2})" + Environment.NewLine, returnType, methodName, string.Join(",", methodArgs));

                requestBase += "{" + Environment.NewLine;
                requestBase += string.Format("{0}" + Environment.NewLine, generateId);
                requestBase += string.Format("connection.Marshal(this.id ,(ushort)RequestOpcode.{0}{1});" + Environment.NewLine, methodName, string.Join(",", methodArgsUsage));

                if (methodArgsUsage.Count > 0)
                {
                    methodArgsUsage[0] = methodArgsUsage[0].Replace(",", string.Empty);
                }

                requestBase += string.Format(@"DebugLog.WriteLine($""-->{{ INTERFACE}}@{{this.id}}.{{ RequestOpcode.{0} }}({1})"");" + Environment.NewLine, methodName, string.Join(",", methodArgsUsage.Select(c => "{"+c+"}")));
                requestBase += string.Format("{0}" + Environment.NewLine, generateNew);
                requestBase += string.Format("{0}" + Environment.NewLine, returnCall);
                requestBase += "}" + Environment.NewLine;
                
                stringBuilder.AppendLine(requestBase);
            }

            string eventOpCodeBase = "public enum RequestOpcode : ushort" + Environment.NewLine;
            eventOpCodeBase += "{" + Environment.NewLine;
            eventOpCodeBase += string.Join("," + Environment.NewLine, opCodes) + Environment.NewLine;
            eventOpCodeBase += "}" + Environment.NewLine;

            stringBuilder.AppendLine(eventOpCodeBase);


            return stringBuilder.ToString();
        }

        private static string GetType(string type) 
        {
            switch (type)
            {
                case "int":
                case "uint":
                case "string":
                    return type;
                case "fd":
                    return "IntPtr";
                case "new_id":
                    return "int";
                case "fixed":
                    return "double";
                case "array":
                    return "byte[]";
                case "object":
                    return "WaylandObject";
                default:
                    return null;
            }
        }

        private static string ToCamelCase(string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                });
            var tailWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                .ToArray();
            return $"{leadWord}{string.Join(string.Empty, tailWords)}";
        }

        private static string ToTitleCase(string str)
        {
            string[] words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string @string = string.Join(string.Empty, words.Select(c => textInfo.ToTitleCase(c)));
            

            return @string;
            
        }



        private static string commentBase = @"
                                /// <summary>
                                /// {0}
                                /// <para>
                                /// {1}
                                /// </para>
                                /// </summary>
                                    ";

        private static string enumBase = @"
                                public enum {0} : ushort
                                {
                                    {0}
                                }
                                 ";


        [DebuggerHidden]
        public void Debug() 
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

        }

    }
}
