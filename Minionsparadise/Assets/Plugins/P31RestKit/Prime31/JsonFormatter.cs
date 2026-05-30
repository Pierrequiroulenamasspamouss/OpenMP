namespace Prime31
{
	public class JsonFormatter
	{
		private enum JsonContextType
		{
			Object = 0,
			Array = 1
		}

		private const int defaultIndent = 0;

		private const string indent = "\t";

		private const string space = " ";

		private bool inDoubleString = false;

		private bool inSingleString = false;

		private bool inVariableAssignment = false;

		private char prevChar = '\0';

		private global::System.Collections.Generic.Stack<global::Prime31.JsonFormatter.JsonContextType> context = new global::System.Collections.Generic.Stack<global::Prime31.JsonFormatter.JsonContextType>();

		public static string prettyPrint(string input)
		{
			try
			{
				return new global::Prime31.JsonFormatter().print(input);
			}
			catch (global::System.Exception)
			{
				return null;
			}
		}

		private static void buildIndents(int indents, global::System.Text.StringBuilder output)
		{
			for (; indents > 0; indents--)
			{
				output.Append("\t");
			}
		}

		private bool inString()
		{
			return inDoubleString || inSingleString;
		}

		public string print(string input)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(input.Length * 2);
			foreach (char c in input)
			{
				switch (c)
				{
				case '{':
					if (!inString())
					{
						if (inVariableAssignment || (context.Count > 0 && context.Peek() != global::Prime31.JsonFormatter.JsonContextType.Array))
						{
							stringBuilder.Append(global::System.Environment.NewLine);
							buildIndents(context.Count, stringBuilder);
						}
						stringBuilder.Append(c);
						context.Push(global::Prime31.JsonFormatter.JsonContextType.Object);
						stringBuilder.Append(global::System.Environment.NewLine);
						buildIndents(context.Count, stringBuilder);
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '}':
					if (!inString())
					{
						stringBuilder.Append(global::System.Environment.NewLine);
						context.Pop();
						buildIndents(context.Count, stringBuilder);
						stringBuilder.Append(c);
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '[':
					stringBuilder.Append(c);
					if (!inString())
					{
						context.Push(global::Prime31.JsonFormatter.JsonContextType.Array);
					}
					break;
				case ']':
					if (!inString())
					{
						stringBuilder.Append(c);
						context.Pop();
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '=':
					stringBuilder.Append(c);
					break;
				case ',':
					stringBuilder.Append(c);
					if (!inString())
					{
						stringBuilder.Append(" ");
					}
					if (!inString() && context.Peek() != global::Prime31.JsonFormatter.JsonContextType.Array)
					{
						buildIndents(context.Count, stringBuilder);
						stringBuilder.Append(global::System.Environment.NewLine);
						buildIndents(context.Count, stringBuilder);
						inVariableAssignment = false;
					}
					break;
				case '\'':
					if (!inDoubleString && prevChar != '\\')
					{
						inSingleString = !inSingleString;
					}
					stringBuilder.Append(c);
					break;
				case ':':
					if (!inString())
					{
						inVariableAssignment = true;
						stringBuilder.Append(c);
						stringBuilder.Append(" ");
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '"':
					if (!inSingleString && prevChar != '\\')
					{
						inDoubleString = !inDoubleString;
					}
					stringBuilder.Append(c);
					break;
				case ' ':
					if (inString())
					{
						stringBuilder.Append(c);
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				prevChar = c;
			}
			return stringBuilder.ToString();
		}
	}
}
