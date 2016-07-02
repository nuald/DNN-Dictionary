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
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Dictionary.Common {
	/// <summary>
	/// Summary description for TagRecognizer
	/// </summary>
	public static class TagRecognizer {
		#region Consts

		public const string EmailRegex =
			@"^[\w\d][\w\d\-\.]*@[\w\d]{2}[\w\d\-]*\.[\w\d]{2}(\.?[\w\d\-]+)*$";

		#endregion //Consts

		#region Fields

		private static readonly string[] _sizes = new string[] {
		                                                       	"xx-small",
		                                                       	"x-small",
		                                                       	"small",
		                                                       	"smaller",
		                                                       	"medium",
		                                                       	"large",
		                                                       	"larger",
		                                                       	"x-large",
		                                                       	"xx-large"
		                                                       };

		private static readonly string[] _families = new string[] {
		                                                          	"Arial",
		                                                          	"Courier",
		                                                          	"Courier New",
		                                                          	"Georgia",
		                                                          	"Lucida Console",
		                                                          	"MS Sans Serif",
		                                                          	"Symbol",
		                                                          	"System",
		                                                          	"Tahoma",
		                                                          	"Terminal",
		                                                          	"Times New Roman",
		                                                          	"Verdana",
		                                                          	"Webdings"
		                                                          };

		private static readonly string[] _colors = new string[] {
		                                                        	"Aqua",
		                                                        	"Black",
		                                                        	"Blue",
		                                                        	"Fuchsia",
		                                                        	"Gray",
		                                                        	"Green",
		                                                        	"Lime",
		                                                        	"Maroon",
		                                                        	"Navy",
		                                                        	"Olive",
		                                                        	"Purple",
		                                                        	"Red",
		                                                        	"Silver",
		                                                        	"Teal",
		                                                        	"White",
		                                                        	"Yellow"
		                                                        };

		#endregion //Fields

		#region Private members

		private static List<TagRecognizerRule> Init() {
			List<TagRecognizerRule> rules = new List<TagRecognizerRule>();

			rules.Add(new TagRecognizerRule("[hr]", "<hr />"));

			rules.Add(new TagRecognizerRule(new TagRecognizerRule("[i]", "<i>"),
			                                new TagRecognizerRule("[/i]", "</i>"), false));

			rules.Add(new TagRecognizerRule(new TagRecognizerRule("[b]", "<b>"),
			                                new TagRecognizerRule("[/b]", "</b>"), false));

			rules.Add(new TagRecognizerRule(
			          	new TagRecognizerRule("[unicode]",
			          	                      "<span style='font-family: Arial Unicode MS'>"),
			          	new TagRecognizerRule("[/unicode]", "</span>"), false));

			rules.Add(new TagRecognizerRule(UrlRule));

			rules.Add(new TagRecognizerRule(ImgRule));

			rules.Add(new TagRecognizerRule(EmailRule));

			rules.Add(new TagRecognizerRule(
			          	new TagRecognizerRule("[q]", "<blockquote class='q'><p>"),
			          	new TagRecognizerRule("[/q]", "</p></blockquote>"), false));

			rules.Add(new TagRecognizerRule(new TagRecognizerRule("[list]",
			                                                      "<ul style='margin-top:0;margin-bottom:0;'>"),
			                                new TagRecognizerRule("[/list]", "</ul>"), true));

			rules.Add(new TagRecognizerRule(new TagRecognizerRule("[list=1]",
			                                                      "<ol type='1' start='1' style='margin-top:0;margin-bottom:0;'>"),
			                                new TagRecognizerRule("[/list=]", "</ol>"), true));

			rules.Add(new TagRecognizerRule(new TagRecognizerRule("[list=a]",
			                                                      "<ol type='a' style='margin-top:0;margin-bottom:0;'>"),
			                                new TagRecognizerRule("[/list=]", "</ol>"), true));

			rules.Add(new TagRecognizerRule(@"[*]", "<li />"));
			rules.Add(new TagRecognizerRule(TextInputRule));

			AddFontSizes(rules);
			AddFontFamilies(rules);
			AddFontColors(rules);

			rules.Add(new TagRecognizerRule(
			          	new TagRecognizerRule(@"(\r\n){0,1}\[table\](\r\n)*",
			          	                      "<table border='1' cellspacing='0' cellpadding='0' class='ArtTable'>", true),
			          	new TagRecognizerRule(@"(\r\n)*\[/table\](\r\n){0,1}", "</table>", true),
			          	false));
			rules.Add(new TagRecognizerRule(
			          	new TagRecognizerRule(@"(\r\n)*\[tr\](\r\n)*", "<tr>", true),
			          	new TagRecognizerRule(@"(\r\n)*\[/tr\](\r\n)*", "</tr>", true),
			          	false));
			rules.Add(new TagRecognizerRule(
			          	new TagRecognizerRule(@"(\r\n)*\[td\](\r\n)*", "<td  class='ArtTd'>",
			          	                      true),
			          	new TagRecognizerRule(@"(\r\n)*\[/td\](\r\n)*", "</td>", true),
			          	false));

			return rules;
		}

		private static bool IsUrlValid(string url) {
			Regex regex =
				new Regex(
					@"^(?:(?:http?|https):\/\/(?:[a-z0-9_-]{1,32}(?::[a-z0-9_-]{1,32})?@)?)?(?:(?:[a-z0-9-]{1,128}\.)+(?:com|net|org|mil|edu|arpa|ru|gov|biz|info|aero|inc|name|[a-z]{2})|(?!0)(?:(?!0[^.]|255)[0-9]{1,3}\.){3}(?!0|255)[0-9]{1,3})(?:\/[a-z0-9.,_@%&?+=\~\/-]*)?(?:#[^ \'\""&<>]*)?$");
			return regex.Match(url).Success;
		}

		private static bool IsEmailValid(string mail) {
			Regex regex = new Regex(EmailRegex);
			return regex.Match(mail).Success;
		}

		private static string TextInputRule(string input, TagRuleOperation operation) {
			Regex regex = new Regex(@"\[input\]", RegexOptions.IgnoreCase);
			return (operation == TagRuleOperation.ReplaceTags)
			       	? regex.Replace(input, new MatchEvaluator(MatchTextInputRule))
			       	: regex.Replace(input, TagRecognizerRule.CleanedString);
		}

		private static string MatchTextInputRule(Match match) {
			string id = Guid.NewGuid().ToString();
			return string.Format(CultureInfo.CurrentCulture,
			                     "<input type='text' id='{0}' class='textBoxStyle' />", id);
		}

		private static void AddFontSizes(ICollection<TagRecognizerRule> rules) {
			IFormatProvider culture = CultureInfo.CurrentCulture;
			for (int i = 0; i < GetSizes().Length; i++) {
				string size = string.Format(culture, "[font-size={0}]", GetSizes()[i]);
				string html = string.Format(culture, "<span style='font-size: {0}'>",
				                            GetSizes()[i]);
				rules.Add(new TagRecognizerRule(size, html));
			}
			rules.Add(new TagRecognizerRule("[/font-size]", "</span>"));
		}

		private static void AddFontFamilies(ICollection<TagRecognizerRule> rules) {
			IFormatProvider culture = CultureInfo.CurrentCulture;
			for (int i = 0; i < GetFamilies().Length; i++) {
				string family = String.Format(culture, "[font-family={0}]", GetFamilies()[i]);
				string html = String.Format(culture, "<span style='font-family: {0}'>",
				                            GetFamilies()[i]);
				rules.Add(new TagRecognizerRule(family, html));
			}
			rules.Add(new TagRecognizerRule("[/font-family]", "</span>"));
		}

		private static void AddFontColors(ICollection<TagRecognizerRule> rules) {
			IFormatProvider culture = CultureInfo.CurrentCulture;
			for (int i = 0; i < GetColors().Length; i++) {
				string color = String.Format(culture, "[font-color={0}]", GetColors()[i]);
				string html = String.Format(culture, "<span style='color: {0}'>",
				                            GetColors()[i]);
				rules.Add(new TagRecognizerRule(color, html));
			}
			rules.Add(new TagRecognizerRule("[/font-color]", "</span>"));
		}

		private static string UrlRule(string input, TagRuleOperation operation) {
			Regex regex = new Regex(@"\[url=?(?<URL>[^\]]*)\]?(?<TEXT>.*?(?=\[/url\]))\[/url\]", RegexOptions.IgnoreCase);
			return (operation == TagRuleOperation.ReplaceTags)
			       	? regex.Replace(input, new MatchEvaluator(MatchUrlRule))
			       	: regex.Replace(input, new MatchEvaluator(MatchUrlRuleClear));
		}

		private static string MatchUrlRuleClear(Match match) {
			return match.Groups["TEXT"].Value;
		}

		private static string MatchUrlRule(Match match) {
			string url = match.Groups["URL"].Value;
			if (!IsUrlValid(url)) {
				return "Some not valid URL: " + match.Value;
			}
			return
				"<a onclick=\"window.open(this.href,'_blank');return false;\" class='m' href='" + url + "'>" +
				match.Groups["TEXT"].Value + "</a>";
		}

		private static string ImgRule(string input, TagRuleOperation operation) {
			Regex regex = new Regex(
				@"\[img=?(?<URL>[^\]]*)\]?(?<TEXT>.*?(?=\[/img\]))\[/img\]",
				RegexOptions.IgnoreCase);
			return (operation == TagRuleOperation.ReplaceTags)
			       	? regex.Replace(input, new MatchEvaluator(MatchImgRule))
			       	: regex.Replace(input, TagRecognizerRule.CleanedString);
		}

		private static string MatchImgRule(Match match) {
			string url = match.Groups["URL"].Value;
			if (!IsUrlValid(url)) {
				return "Some not valid image URL: " + match.Value;
			}
			string altText = match.Groups["TEXT"].Value;
			string imgTag = String.Format(CultureInfo.CurrentCulture,
			                              "<img src='{0}' alt='{1}' title='{1}' />", url, altText);
			return imgTag;
		}

		private static string EmailRule(string input, TagRuleOperation operation) {
			Regex regex = new Regex(
				@"\[email=?(?<EMAIL>[^\]]*)\]?(?<TEXT>.*?(?=\[/email\]))\[/email\]",
				RegexOptions.IgnoreCase);
			//			Regex regex = new Regex(@"\[email\]?(?<EMAIL>[^\[]*)\[/email\]", RegexOptions.IgnoreCase);
			return (operation == TagRuleOperation.ReplaceTags)
			       	? regex.Replace(input, new MatchEvaluator(MatchEmailRule))
			       	: regex.Replace(input, new MatchEvaluator(MatchEmailRuleClear));
		}

		private static string MatchEmailRuleClear(Match match) {
			return match.Groups["TEXT"].Value;
		}

		private static string MatchEmailRule(Match match) {
			string url = match.Groups["EMAIL"].Value;
			string text = match.Groups["TEXT"].Value;
			if (!IsEmailValid(url)) {
				return "Some not valid Email address: " + match.Value;
			}
			return "<a class='m' href='mailto:" + url + "'>" + text + "</a>";
		}

		private static string[] GetColors() {
			return _colors;
		}

		private static string[] GetSizes() {
			return _sizes;
		}

		private static string[] GetFamilies() {
			return _families;
		}

		#endregion //Private members

		#region Public members

		/// <summary>
		/// Process all tags and replace on rules settings.
		/// </summary>
		/// <param name="input">Source string.</param>
		/// <returns>Processed string.</returns>
		public static string Process(string input) {
			List<TagRecognizerRule> rules = Init();
			string output = input;
			foreach (TagRecognizerRule rule in rules.AsReadOnly()) {
				if (rule != null) {
					output = rule.Process(output);
				}
			}
			return output;
		}

		/// <summary>
		/// Remove all tags from source string.
		/// </summary>
		/// <param name="input">Source string.</param>
		/// <returns>Processed string without tags.</returns>
		public static string ClearTags(string input) {
			List<TagRecognizerRule> rules = Init();
			string output = input;
			foreach (TagRecognizerRule rule in rules.AsReadOnly()) {
				if (rule != null) {
					output = rule.ClearTags(output);
				}
			}
			return output;
		}

		#endregion //Public members
	}
}