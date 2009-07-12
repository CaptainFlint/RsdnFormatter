using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Rsdn.Framework.Common;

namespace Rsdn.Framework.Formatting
{
	/// <summary>
	/// �����, ��� ��������� ����������
	/// ��������� ������� ��������� �� xml-�����
	/// </summary>
	public class CodeFormatter
	{
		/// <summary>
		/// Source XML validating schemas (XSD)
		/// </summary>
		#pragma warning disable 618
		protected static XmlSchemaCollection xmlSchemas;
		#pragma warning restore 618

		static CodeFormatter()
		{
			//Load the schema collection.
			#pragma warning disable 618
			xmlSchemas = new XmlSchemaCollection
     	{
     		XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream(
     		               	"Rsdn.Framework.Formatting.Patterns.PatternSchema.xsd"), null)
     	};
			#pragma warning restore 618
		}

		/// <summary>
		/// ���������� ���������, ������������ ��� ���������.
		/// ���������� ����� �������������� �������� ������.
		/// </summary>
		protected Regex colorerRegex;
		
		/// <summary>
		/// ������ ���� ���������� ����� � ���������� ���������
		/// ������������ ��� ������ ����� ������ �� �� ������
		/// </summary>
		protected string[] groupNames;
		
		/// <summary>
		/// ����� ����� � ���������� ���������
		/// </summary>
		protected int countGroups;
	
		/// <summary>
		/// �������� ���������� ����������������.
		/// </summary>
		/// <param name="xmlSource">�������� xml-�����</param>
		public CodeFormatter(Stream xmlSource) : this(xmlSource, RegexOptions.None) {}

		/// <summary>
		/// �������� ���������� ���������������� � ��������������� ������� ��� ����������� ���������.
		/// </summary>
		/// <param name="xmlSource">�������� xml-�����</param>
		/// <param name="options">Regex �����</param>
		public CodeFormatter(Stream xmlSource, RegexOptions options)
		{
			try
			{
				var regexString = new StringBuilder();

				#pragma warning disable 618
				var validatingReader = new XmlValidatingReader(new XmlTextReader(xmlSource));
				#pragma warning restore 618

				validatingReader.ValidationType = ValidationType.Schema;
				validatingReader.Schemas.Add(xmlSchemas);

				var doc = new XmlDocument();
				doc.Load(validatingReader);

				var namespaceManager = new XmlNamespaceManager(doc.NameTable);
				namespaceManager.AddNamespace("cc", "http://rsdn.ru/coloring");

				// ����� �������� ��������
				var root = doc.SelectSingleNode("cc:language", namespaceManager);

				// ��������� regex �����, ���� ����
				if (root.Attributes["options"] != null)
					regexString.Append(root.Attributes["options"].Value);

				// ������� ��������
				var syntaxis = root.SelectNodes("cc:pattern", namespaceManager);
				for (var i = 0; i < syntaxis.Count; i++)
				{
					if (i > 0)
						regexString.Append('|');
					regexString.AppendFormat("(?<{0}>", syntaxis[i].Attributes["name"].Value);

					var prefix = syntaxis[i].Attributes["prefix"] != null ? syntaxis[i].Attributes["prefix"].Value : null;
					var postfix = syntaxis[i].Attributes["postfix"] != null ? syntaxis[i].Attributes["postfix"].Value : null;

					// ������� ��������� �������
					var items = syntaxis[i].SelectNodes("cc:entry", namespaceManager);
					for (var j = 0; j < items.Count; j++)
					{
						if (j > 0)
							regexString.Append('|');
						regexString.Append(prefix).Append(items[j].InnerText).Append(postfix);
					}

					regexString.Append(')');
				}			

				// �������� ����������� ���������
				colorerRegex = new Regex(regexString.ToString(), options);
				// ������ ���������� ����������� ���������
				countGroups = colorerRegex.GetGroupNumbers().Length;
				var numbers = colorerRegex.GetGroupNumbers();
				var names = colorerRegex.GetGroupNames();
				groupNames = new string[numbers.Length];
				for (var i = 0; i < numbers.Length; i++)
					groupNames[numbers[i]] = names[i];
			}
			catch (XmlException xmlException)
			{
				throw new RsdnException("Language color pattern source xml stream is not valid", xmlException);
			}
			catch (XmlSchemaException schemaException)
			{
				throw new RsdnException("Language color pattern source xml stream is not valid", schemaException);
			}
		}

		/// <summary>
		/// �������������� ������ �����������������
		/// </summary>
		/// <param name="sourceText">�������� �����</param>
		/// <returns>��������������� �����</returns>
		public string Transform(string sourceText)
		{
			return colorerRegex.Replace(sourceText, new MatchEvaluator(ReplaceEvaluator));
		}

		/// <summary>
		/// ������� ��������� ���������� ��������� �� ����� �������������
		/// </summary>
		/// <param name="match">�����������</param>
		/// <returns>������������ �����������</returns>
		protected string ReplaceEvaluator(Match match)
		{
			var capturedGroup = "";
			// get captured group's name
			// 0 is all matched expression, start from 1
			for (var i = 1; i < countGroups; i++)
				if (match.Groups[i].Success)
				{
					capturedGroup = groupNames[i];
					break;
				}

			return string.Format("<{0}>{1}</{0}>", capturedGroup, match.Value);
		}
	}
}