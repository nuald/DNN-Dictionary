/********************************************************************************/
/*****        This file is part of Dictionary module for DNN.               *****/
/*****                                                                      *****/
/***** Dictionary module for DNN is free software: you can redistribute it  *****/
/***** and/or modify it under the terms of the GNU General Public License   *****/
/***** as published by the Free Software Foundation, either version 3 of    *****/
/***** the License, or (at your option) any later version.                  *****/
/*****                                                                      *****/
/***** Dictionary module for DNN is distributed in the hope that it will be *****/
/***** useful, but WITHOUT ANY WARRANTY; without even the implied warranty  *****/
/***** of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the      *****/
/***** GNU General Public License for more details.                         *****/
/*****                                                                      *****/
/***** You should have received a copy of the GNU General Public License    *****/
/***** along with Dictionary module for DNN. If not, see                    *****/
/***** <http://www.gnu.org/licenses/>.                                      *****/
/*****                                                                      *****/
/***** Copyright 2008 EELLC                                                 *****/
/********************************************************************************/

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Dictionary.Common;
using DotNetNuke.Modules.Dictionary;

namespace Dictionary {
	
	/// <summary>
	/// Web service for translate word.
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ScriptService]
	public class DictionaryService : WebService {

		#region Fields

		private string _fileName;
		private readonly Sapi5 _sapi5 = new Sapi5();
		private long _timeLastCreatedFile;
		private const long SafeIntervalInTicks = 50000000;
		private const int EnterCode = 13;
		private readonly string EnterString = Char.ConvertFromUtf32(EnterCode);
		private const int WordIndex = 0;
		private const int LanguageIndex = 1;
		private const int ParamCount = 2;
		private const int MaxWordLength = 30;

		#endregion //Fields

		/// <summary>
		/// Translate word.
		/// </summary>
		/// <param name="eventArgument">Format: word_to_translate/ntranslate_direction (example: door/nEn>Ru)</param>
		/// <returns>Translated word.</returns>
		[WebMethod]
		[ScriptMethod(UseHttpGet = false)]
		public String DictionaryCallBack(String eventArgument) {
			string _callbackResult = String.Empty;
			long timeNow = DateTime.Now.Ticks;
			long timeLeft = timeNow - _timeLastCreatedFile;
			if (timeLeft >= SafeIntervalInTicks) {
				_callbackResult = GetTranslatingResult(eventArgument);
			}
			_timeLastCreatedFile = timeNow;
			return _callbackResult;
		}

		#region Private members

		private string GetTranslatingResult(string eventArgument) {
			if (String.IsNullOrEmpty(eventArgument)) {
				return String.Empty;
			}
			string result = String.Empty;
			IFormatProvider culture = CultureInfo.CurrentCulture;
			String[] args = eventArgument.Split(Convert.ToChar(EnterString, culture));
			if (args.Length == ParamCount) {
				string word = ClearWord(args[WordIndex]);
				string language = args[LanguageIndex];
				string translate = Translating(word, language);
				result = word +
				         Convert.ToString(EnterString, culture) + 
				         translate + 
				         Convert.ToString(EnterString, culture) + 
				         _fileName;
			}
			return result;
		}

		private string Translating(string word, string language) {
			string translate;
			//prepeare for translate
			DictionaryDefinition dictionaryDefinition = DictionaryController.GetDictionaryDefinition(language);
			if(dictionaryDefinition == null) {
				throw new ArgumentException("Wrong dictionary!");
			}
			SetupTtsVoice(dictionaryDefinition.VoiceEngine);
			if (word.Length >= MaxWordLength) {
				translate = dictionaryDefinition.ErrorString;
			} else {
				translate = VerifyAndGetTranslate(word, dictionaryDefinition);
				if (String.IsNullOrEmpty(translate)) {
					translate = dictionaryDefinition.WarningString;
				}
			}
			return HtmlFormat(translate);
		}

		private void SetupTtsVoice(string voiceName) {
			string applicationTtsEngineName = ConfigurationManager.AppSettings[voiceName];
			if (!String.IsNullOrEmpty(applicationTtsEngineName)) { // config voice is used
				_sapi5.VoiceByName = applicationTtsEngineName;
			} else { // default voice is used
				_sapi5.VoiceByName = null;
			}
		}

		private string VerifyAndGetTranslate(string word, DictionaryDefinition dictionaryDefinition) {
			string result = String.Empty;
			if (Regex.IsMatch(word, dictionaryDefinition.CheckString)) {
				result = DictionaryController.Translate(dictionaryDefinition, word);
			}
			GenerateMP3Sound(word);
			return result;
		}

		private static string ClearWord(string word) {
			word = Regex.Replace(word, "[^\\w\\s_-]", "");
			word = Regex.Replace(word, "(^\\s*)|(\\s*$)", "");
			word = Regex.Replace(word, "\\s+", " ");
			return word;
		}

		private void GenerateMP3Sound(string word) {
			IFormatProvider culture = CultureInfo.CurrentCulture;
			//setup file format - mp3
			_sapi5.FileFormat = 1;
			// verify and create directory if need
			HttpContext context = HttpContext.Current;
			string dicFilesDictionary = ConfigurationManager.AppSettings["DictionaryFiles"];
			string fileDir = Path.Combine(
				HttpContext.Current.Request.MapPath("~"), 
				dicFilesDictionary).Replace("\\", "/");
			if(!CreateDictionaryFileDirectory(fileDir)) {
				//can't create directory
				return;
			}
			//get web filename
			_fileName = string.Format(culture, "{0}/{1}_{2}.mp3", dicFilesDictionary, 
			                          _sapi5.VoiceName, word);
			//get local file name
			string fileName = Path.Combine(context.Server.MapPath("~"), 
			                               _fileName).Replace("\\", "/");
			// normalise web filename
			_fileName = Server.UrlPathEncode(_fileName);
			//create file if not exist
			if (!File.Exists(fileName)) {
				if(!CheckOnDirectoryWrite(fileName)) {
					//can't write file to directory
					return;
				}
				_sapi5.SpeakToFile(word, fileName, 1);
			}
		}

		private static bool CheckDirectoryFileAccess(string path, FileSystemRights right) {
			string root = Path.GetDirectoryName(path);
			DirectorySecurity security = Directory.GetAccessControl(root);
			AuthorizationRuleCollection ruleCollection = security.GetAccessRules(true, true, typeof (SecurityIdentifier));
			foreach (AuthorizationRule rule in ruleCollection) {
				if(!(rule is FileSystemAccessRule)) {
					continue;
				}
				FileSystemAccessRule fileSystemAccessRule = (FileSystemAccessRule) rule;
				if((fileSystemAccessRule.FileSystemRights & right) == right) {
					return true;
				}
			}
			return false;
		}

		private static bool CheckOnCreateDirectories(string path) {
			return CheckDirectoryFileAccess(path, FileSystemRights.WriteData);
		}

		private static bool CheckOnDirectoryWrite(string path) {
			return CheckDirectoryFileAccess(path, FileSystemRights.WriteData);
		}

		private static bool CreateDictionaryFileDirectory(string fileDir) {
			if (Directory.Exists(fileDir)) {
				return true;
			} else {
				string root = Path.GetDirectoryName(fileDir);
				if(!CreateDictionaryFileDirectory(root)) {
					return false;
				}
			}
			if(!CheckOnCreateDirectories(fileDir)) {
				return false;
			}
			Directory.CreateDirectory(fileDir);
			return true;
		}

		private static string HtmlFormat(string text) {
			try {
				//tranckription: <t> </t>
				text = Regex.Replace(text, "\\[", "<t>");
				text = Regex.Replace(text, "\\]", "</t>");
				text = Regex.Replace(text, "<t>", "[unicode][b][");
				text = Regex.Replace(text, "</t>", "][/b][/unicode]");
				//digits
				text = Regex.Replace(text, "(?<value>(\\d+)\\.)", " [b]$1.[/b]");
				text = Regex.Replace(text, "(?<value>(\\d+)\\))", " [i]$1)[/i]");
				text = Regex.Replace(text, " (?<value>(\\d+)\\>)", " [i]$1)[/i]");
				//cross link: <r> </r>
				text = Regex.Replace(text, "<r>(?<value>(.+))</r>", "<a href=# OnClick=CrossLink(\"$1\")>$1</a>\r\n");

				//bold: <b> </b>
				text = Regex.Replace(text, "<b>", "[b]");
				text = Regex.Replace(text, "</b>", "[/b]");

			} catch (ArgumentException) { }
			text = TagRecognizer.Process(text);
			return text;
		}

		#endregion //Private members
	}
}
