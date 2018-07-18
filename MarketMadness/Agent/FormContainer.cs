using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;

namespace MarketMadness.Agent
{
	[Serializable]
	public class FormContainer
	{
		[Serializable]
		public class InputElement
		{
			public enum TagTypeEnum
			{
				TextInput,
				Hidden,
				TextArea,
				CheckBox,
				RadioButton,
				DropDown,
				ListBox,
				Button,
				SubmitButton
			}

			public TagTypeEnum TagType;
			public string Name;
			public string Value;
			public string[] PossibleValues;
			public bool SendFieldInResponse;

			public InputElement(TagTypeEnum tagtype)
			{
				TagType = tagtype;
			}
		}

		public List<InputElement> Elements;
		public string ActionURL;

		private char[] HTML;
		private int CurrentIndex;
		private bool inCharBlock;

		public FormContainer(string html)
		{
			HTML = html.ToCharArray();
		}

		public List<InputElement> Parse()
		{
			// reset parsing values
			CurrentIndex = 0;
			inCharBlock = false;
			Elements = new List<InputElement>();

			// read ahead to start of first form
			string token = GetNextToken();

			while (token != null)
			{
				if (token == "<form")
					break;

				token = GetNextToken();
			}

			ParseForm();

			// grab everything until the end of the form is reached
			while (token != "</form>")
			{
				if (token == "<input")
					ParseInput(false);
				else
					if (token == "<button")
						ParseInput(true);
					else
						if (token == "<select")
							ParseSelect();
						else
							if (token == "<textarea")
								ParseTextArea();

				token = GetNextToken();
			}

			return Elements;
		}

		public void Apply(AgentAction action)
		{
			foreach (InputElement element in Elements)
			{
				if (element.SendFieldInResponse && element.Value != null)
					action.AddParam(element.Name, element.Value);
			}
		}

		private void ParseInput(bool isButton)
		{
			string type = null;
			string value = null;
			string name = null;
			bool isChecked = false;

			string token = GetNextToken();

			string attribute = null;
			bool wasAssignment = false;

			while (!token.EndsWith(">"))
			{
				if (token == "checked")
					isChecked = true;

				else if (token == "=")
					wasAssignment = true;

				else if (wasAssignment)
				{
					if (attribute == "type")
						type = token;

					else if (attribute == "name")
						name = token;

					else if (attribute == "value")
						value = token;

					wasAssignment = false;
				}
				else
					attribute = token;

				token = GetNextToken();
			}

			if (isButton)
			{
				InputElement textField = new InputElement(InputElement.TagTypeEnum.Button);
				textField.Name = name;
				textField.Value = value;
				Elements.Add(textField);
			}
			else if (type == null || type == "text" || type == "password")
			{
				InputElement textField = new InputElement(InputElement.TagTypeEnum.TextInput);
				textField.Name = name;
				textField.Value = value;
				Elements.Add(textField);
			}
			else if (type == "hidden")
			{
				InputElement textField = new InputElement(InputElement.TagTypeEnum.Hidden);
				textField.Name = name;
				textField.Value = value;
				Elements.Add(textField);
			}
			else if (type == "submit")
			{
				InputElement textField = new InputElement(InputElement.TagTypeEnum.SubmitButton);
				textField.Name = name;
				textField.Value = value;
				Elements.Add(textField);
			}
			else if (type == "checkbox" && isChecked)
			{
				InputElement textField = new InputElement(InputElement.TagTypeEnum.CheckBox);
				textField.Name = name;
				textField.Value = "on";
				Elements.Add(textField);
			}
			else if (type == "radio")
			{
				InputElement radioButton = null;

				foreach (InputElement element in Elements)
					if (element.Name == name)
					{
						radioButton = element;
						break;
					}

				if (radioButton == null)
				{
					radioButton = new InputElement(InputElement.TagTypeEnum.RadioButton);
					radioButton.Name = name;
					Elements.Add(radioButton);
				}

				ArrayList newValues = new ArrayList();
				
				if (radioButton.PossibleValues != null)
					newValues.AddRange(radioButton.PossibleValues);

				newValues.Add(value);
				radioButton.PossibleValues = (string[]) newValues.ToArray(typeof(string));

				if (isChecked)
					radioButton.Value = value;
			}
		}

		private void ParseTextArea()
		{
			string text;
			string name = null;

			string token = GetNextToken();

			string attribute = null;
			bool wasAssignment = false;

			while (!token.EndsWith(">"))
			{
				if (token == "=")
					wasAssignment = true;
				
				else if (wasAssignment)
				{
					if (attribute == "name")
						name = token;

					wasAssignment = false;
				}
				else
					attribute = token;

				token = GetNextToken();
			}
			text = GetNextToken();

			InputElement textArea = new InputElement(InputElement.TagTypeEnum.TextArea);
			textArea.Name = name;
			textArea.Value = text;
			Elements.Add(textArea);
		}

		private void ParseSelect()
		{
			string name = null;

			string token = GetNextToken();

			string attribute = null;
			bool wasAssignment = false;

			while (!token.EndsWith(">"))
			{
				if (token == "=")
					wasAssignment = true;

				else
					if (wasAssignment)
					{
						if (attribute == "name")
							name = token;

						wasAssignment = false;
					}
					else
						attribute = token;

				token = GetNextToken();
			}

			while (token != "</select>")
				token = GetNextToken();

			InputElement select = new InputElement(InputElement.TagTypeEnum.DropDown);
			select.Name = name;
			Elements.Add(select);
		}

		private void ParseForm()
		{
			string nextToken;
			bool wasHref = false;

			while ((nextToken = GetNextToken()) != ">")
			{
				if (wasHref && nextToken != "=")
				{
					ActionURL = nextToken;
					wasHref = false;
				} else
					if (nextToken == "action")
						wasHref = true;
			}
		}

		private string GetNextToken()
		{
			StringBuilder builder = new StringBuilder();

			if (inCharBlock)
			{
				while (CurrentIndex < HTML.Length)
				{
					if (HTML[CurrentIndex] == '<')
						break;

					builder.Append(HTML[CurrentIndex++]);
				}

				inCharBlock = false;
				return builder.ToString();
			}

			bool inQuotes = false;
			bool endSlash = false;
			bool alphachars = false;

			while (CurrentIndex < HTML.Length)
			{
				char currentChar = HTML[CurrentIndex++];
				bool wasQuote = currentChar == '"';
				bool wasTagStart = currentChar == '<';
				bool wasTagEnd = currentChar == '>';
				bool wasEndSlash = currentChar == '/';
				bool wasEquals = currentChar == '=';
				bool wasNot = currentChar == '!';
				bool wasAlphaChar = (currentChar >= 'a' && currentChar <= 'z') ||
				                    (currentChar >= 'A' && currentChar <= 'Z');

				if (inQuotes && wasQuote)
					return builder.ToString();

				if (wasQuote)
					inQuotes = true;

				else
					if (inQuotes)
						builder.Append(currentChar);

					else
						if (wasEquals)
						{
							if (alphachars)
							{
								CurrentIndex--;
								return builder.ToString().ToLower();
							}

							return "=";
						} else
							if (wasEndSlash)
							{
								builder.Append(currentChar);
								endSlash = true;
							} else
								if (wasAlphaChar)
								{
									builder.Append(currentChar);
									alphachars = true;
								} else
									if (wasTagStart || wasNot)
									{
										builder.Append(currentChar);
									} else
										if (wasTagEnd)
										{
											inCharBlock = true;
											builder.Append(currentChar);
											return builder.ToString();
										} else
											if (!endSlash && alphachars)
												return builder.ToString().ToLower();
			}

			return null;
		}
	}
}


