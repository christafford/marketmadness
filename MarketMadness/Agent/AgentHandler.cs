using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;
using MarketMadness.Util;

namespace MarketMadness.Agent
{
	[Serializable]
	public class AgentSession
	{
		public CookieContainer Cookies;

		public AgentSession()
		{
			Cookies = new CookieContainer();
		}
	}

	public class AgentAction
	{
		public bool IsPost;
		public String WebURL;
		public String Referer;
		public IList ElementKeyOrder;
		public IDictionary Elements;
		public bool AllowRedirect = true;
		public string Version = "1.1";

		public AgentAction(string url) : this(url, true)
		{
		}

		public AgentAction(string url, bool isPost)
		{
			WebURL = url;
			IsPost = isPost;
			ElementKeyOrder = new ArrayList();
			Elements = new Hashtable();
		}

		public void AddParam(string key, string requestValue)
		{
			if (Elements.Contains(key))
			{
				Elements.Remove(key);
				ElementKeyOrder.Remove(key);
			}

			ElementKeyOrder.Add(key);
			Elements.Add(key, requestValue);
		}

		public override string ToString()
		{
			string output = "URL: " + WebURL;

			foreach (string key in ElementKeyOrder)
				output += "\n\t\tParam: " + key + " = " + Elements[key];

			return output;
		}

	}

	public class AgentDocument
	{
		public String ResponseString;
		public String Uri;
		public String RedirectUri;

		public override string ToString()
		{
			return ResponseString + "\n\tRedirectUri = " + RedirectUri;
		}

		public string getCData(AgentPath path)
		{
			string cData = "";

			// get us jump-started to the root element, putting us past any script noise typically in the beginning of the document;
			// note that if the root element is not the first of its type, then we can't just jump down like this

			int startOfBody = path.ElementIndex == 1 ? ResponseString.LastIndexOf("<" + path.ElementType) : 0;

			int index = path.positionOf(ResponseString, startOfBody);
			char[] docChars = ResponseString.ToCharArray();

			bool flag = true;

			while (flag)
			{
				while (docChars[index] != '<')
					cData += docChars[index++];

				// skip past comments if there are any
				if (docChars[index + 1] == '!' && docChars[index + 2] == '-' && docChars[index + 3] == '-')
				{
					while (!(docChars[index] == '>' && docChars[index - 1] == '-' && docChars[index - 2] == '-'))
						index ++;
					index ++;
				}
				else
					flag = false;
			}
			return cData;
		}

		public string getValue(AgentPath path, string attribute)
		{
			// get us jump-started to the root element, putting us past any script noise typically in the beginning of the document;
			// note that if the root element is not the first of its type, then we can't just jump down like this
			int startOfBody = path.ElementIndex == 1 ? ResponseString.LastIndexOf("<" + path.ElementType) : 0;
			int index = path.positionOf(ResponseString, startOfBody) - 2; // go behind the ending tag

			char[] docChars = ResponseString.ToCharArray();

			// go backwards, and read off key/value pairs - if we reach the start tag, then the key wasn't found
			while (true)
			{
				// get data
				bool inQuotes = false;
				bool foundEquals = false;
				string elmValue = "";

				while (!foundEquals)
				{
					if (docChars[index] == '\"')
						inQuotes = !inQuotes;

					else if (docChars[index] == '\'')
						inQuotes = !inQuotes;

					else if (docChars[index] == '=' && !inQuotes)
						foundEquals = true;

					else if (docChars[index] == '<' && !inQuotes)
						throw new Exception("attribute " + attribute + " not found.");

					else
						elmValue = docChars[index] + elmValue;

					index--;
				}

				while (docChars[index] == ' ')
					index--;

				string key = "";

				while (docChars[index] != ' ')
					key = docChars[index--] + key;

				key = key.ToLower();

				if (key.Equals(attribute))
					return elmValue;
			}
		}
	}

	public class AgentPath
	{
		public string ElementType;
		public int ElementIndex;

		public AgentPath Next;

		// *** Constructors ***

		public AgentPath()
		{
		}

		public AgentPath(string type, int index) : this()
		{
			ElementType = type;
			ElementIndex = index;
		}

		// *** Public Methods ***
		public AgentPath CreateChild(string type)
		{
			return CreateChild(type, 1);
		}

		public AgentPath CreateChild(string type, int index)
		{
			Next = new AgentPath(type, index);

			return Next;
		}

		public int positionOf(string document, int index)
		{
			Stack stack = new Stack();

			char[] docChars = document.ToCharArray();

			while (true)
			{
				// go until either an element starts, or an element ends
				while (docChars[index] != '<')
					index ++;

				index++;

				// if an elements starts, check if it's a sibling
				if (docChars[index] != '/')
				{
					string elementType = "";
					while (docChars[index] != ' ' && docChars[index] != '>')
						elementType += docChars[index++];

					elementType = elementType.ToLower();

					if (!elementType.StartsWith("!--"))
					{
						// zip past the element's end, stopping if for some reason a new element is discovered
						while (docChars[index] != '>' && docChars[index] != '<')
							index++;

						if (docChars[index] == '>')
							index++;

						// check if this is our guy - make sure the stack count is zero, since we only want to consider siblings
						if (stack.Count == 0 && elementType.Equals(ElementType))
						{
							ElementIndex--;

							if (ElementIndex == 0)
							{
								// recursively call to our child, if we have one
								if (Next == null)
									return index;
								else
									return Next.positionOf(document, index);
							}
						}

						// add to the stack
						stack.Push(elementType);
					}
				}
					// if an element ends, pop off of the stack
				else
				{
					index ++;

					string elementType = "";
					int backingUpIndex = index;

					// go to end of tag
					while (docChars[backingUpIndex] != '>')
						backingUpIndex ++;

					// back up and get the closing element name
					while (docChars[--backingUpIndex] != '/')
						elementType = docChars[backingUpIndex] + elementType;

					elementType = elementType.ToLower();

					// pop off until either the stack is empty, or our start is found
					string popped = "";
					while (stack.Count > 0 && !popped.Equals(elementType))
						popped = (string) stack.Pop();
				}
			}
		}
	}

	public class AgentHandler
	{
		private static AgentHandler instance = null;

		public bool DoLogging = false;

		public static AgentHandler Instance
		{
			get
			{
				if (instance == null)
					instance = new AgentHandler();
				return instance;
			}
		}

		private AgentHandler()
		{
		}

		public AgentDocument PerformAction(AgentSession session, AgentAction action, int timeout)
		{
			AgentDocument document = null;

			// these resources must be released if any exceptions occur
			HttpWebRequest actionRequest = null;
			StreamWriter paramWriter = null;
			HttpWebResponse actionResponse = null;
			Stream responseStream = null;

			try
			{
				// set up the request and its headers
				actionRequest = (HttpWebRequest) WebRequest.Create(new Uri(action.WebURL));

				if (action.Referer != null && action.Referer.Length > 0)
					actionRequest.Referer = action.Referer;

				if (action.IsPost)
					actionRequest.Method = "POST";
				else
					actionRequest.Method = "GET";

				actionRequest.ContentType = "application/x-www-form-urlencoded";
				actionRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1";
				actionRequest.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,video/x-mng,image/png,image/jpeg,image/gif;q=0.2,*/*;q=0.1";
				actionRequest.AllowAutoRedirect = action.AllowRedirect;
				actionRequest.KeepAlive = false; // true;

				if(timeout >0 )
					actionRequest.Timeout = timeout;
	
				actionRequest.Headers.Add("Accept-Language", "en-us,en;q=0.5");
				actionRequest.ProtocolVersion = new Version(action.Version);

				actionRequest.CookieContainer = session.Cookies;

				if (action.IsPost)
				{
					string content = "";

					foreach (string key in action.ElementKeyOrder)
					{
						string requestValue = HttpUtility.UrlEncode((string) action.Elements[key]);
						content += key + "=" + requestValue + "&";
					}

					if (content.Length > 0)
						content = content.Substring(0, content.Length - 1);

					paramWriter = new StreamWriter(actionRequest.GetRequestStream());
					paramWriter.Write(content);
					paramWriter.Flush();
					paramWriter.Close();
				}

				document = new AgentDocument();

				actionResponse = (HttpWebResponse) actionRequest.GetResponse();

				document.RedirectUri = actionResponse.GetResponseHeader("Location");

				responseStream = actionResponse.GetResponseStream();
				StringWriter responseBuilder = new StringWriter();

				// now this looks less efficient than using a StreamReader (and it probably is), but at least this
				// doesn't result in huge memory leaks.  the stream reader class bears investiation!
				byte[] buffer = new byte[1024];
				int n;

				try
				{
					do
					{
						n = responseStream.Read(buffer, 0, buffer.Length);

						char[] charBuffer = new char[buffer.Length];
						for (int i = 0; i < buffer.Length; i++)
							charBuffer[i] = (char) buffer[i];

						responseBuilder.Write(charBuffer, 0, n);
					} while (n > 0);
				}
				catch (Exception ex)
				{
					Logger.Instance.Error(action.WebURL, ex);
				}

				document.ResponseString = responseBuilder.GetStringBuilder().ToString();
				responseBuilder.Close();

				// new logging thing
				
				
				if (DoLogging)
					Logger.Instance.Debug("In: " + action + "\n-------\nOut: " + document);

				document.Uri = actionResponse.ResponseUri.ToString();
			}
			catch (Exception ex)
			{
				if (DoLogging)
					Logger.Instance.Error("In: " + action + "\n-------\nOut: " + document, ex);

				throw ex;
			}
			finally
			{
				try
				{
					if (paramWriter != null) paramWriter.Close();
				}
				catch
				{
				}
				try
				{
					if (responseStream != null) responseStream.Close();
				}
				catch
				{
				}
				try
				{
					if (actionResponse != null) actionResponse.Close();
				}
				catch
				{
				}
			}

			return document;
			
		}

		// *** Public API ***
		public AgentDocument PerformAction(AgentSession session, AgentAction action)
		{
			return PerformAction(session, action, 0);
		}
	}
}