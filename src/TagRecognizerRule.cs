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

using System.Text.RegularExpressions;
using System.Globalization;
using System;

namespace Dictionary.Common {

	/// <summary>
	/// Operations for TagRule
	/// </summary>
	public enum TagRuleOperation {
		/// <summary>
		/// Remove all tags linked with current rule.
		/// </summary>
		ClearTags,

		/// <summary>
		/// Replace all tags linked with current rule.
		/// </summary>
		ReplaceTags
	}

	/// <summary>
	/// Process string and change it with current rules.
	/// </summary>
	public class TagRecognizerRule {

		public delegate string RecognizeRule(string input, TagRuleOperation operation);

		enum RuleType {
			Delegate,
			Regex,
			Pair
		}

		#region Consts

		public const string CleanedString = " ";
		private const int MaxNVarcharSize = 4000;

		#endregion //Consts

		#region Fields

		readonly RuleType _type = RuleType.Regex;
		readonly string _regexString;
		readonly string _outputString;
		readonly Regex _regex;
		readonly RecognizeRule _rule;
		readonly TagRecognizerRule _beginRule;
		readonly TagRecognizerRule _endRule;
		readonly bool _trim;

		#endregion //Fields

		#region Ctors

		/// <summary>
		/// Creates a new instance of the TagRecognizerRule
		/// </summary>
		/// <param name="pattern">Regex pattern.</param>
		/// <param name="output">OUtput string.</param>
		public TagRecognizerRule(string pattern, string output) {
			_regexString = pattern.Replace("[", @"\[").Replace("]", @"\]").
				Replace("*", @"\*").Replace("(", @"\(").Replace(")", @"\)").Replace("|", @"\|");
			_outputString = output;
			_regex = new Regex(_regexString, RegexOptions.IgnoreCase);
			_type = RuleType.Regex;
		}

		/// <summary>
		/// Creates a new instance of the TagRecognizerRule
		/// </summary>
		/// <param name="pattern">Regex pattern.</param>
		/// <param name="output">Output string.</param>
		/// <param name="formated"></param>
		public TagRecognizerRule(string pattern, string output, bool formated) {
			_regexString = formated ? pattern : pattern.Replace("[", @"\[").Replace("]", @"\]").
			                                    	Replace("*", @"\*").Replace("(", @"\(").Replace(")", @"\)").Replace("|", @"\|");
			_outputString = output;
			_regex = new Regex(_regexString, RegexOptions.IgnoreCase);
			_type = RuleType.Regex;
		}

		/// <summary>
		/// Creates a new instance of the TagRecognizerRule
		/// </summary>
		public TagRecognizerRule(RecognizeRule rule) {
			_rule = rule;
			_type = RuleType.Delegate;
		}

		public TagRecognizerRule(TagRecognizerRule beginRule, TagRecognizerRule endRule, bool trim) {
			_beginRule = beginRule;
			_endRule = endRule;
			_trim = trim;
			_type = RuleType.Pair;
		}

		#endregion //Ctors

		#region Public members

		/// <summary>
		/// Apply rule to string.
		/// </summary>
		/// <param name="input">Source string.</param>
		/// <returns>Processed string.</returns>
		public string Process(string input) {
			switch (_type) {
				case RuleType.Delegate:
					return _rule(input, TagRuleOperation.ReplaceTags);
				case RuleType.Regex:
					return _regex.Replace(input, _outputString);
				case RuleType.Pair:
					return ProcessPair(input);
				default:
					return input;
			}
		}

		/// <summary>
		/// Remove tags linked with current rule from source string.
		/// </summary>
		/// <param name="input">Source string.</param>
		/// <returns>String without tags.</returns>
		public string ClearTags(string input) {
			switch (_type) {
				case RuleType.Delegate:
					return _rule(input, TagRuleOperation.ClearTags);
				case RuleType.Regex:
					return _regex.Replace(input, CleanedString);
				case RuleType.Pair:
					return ClearTagsPair(input);
				default:
					return input;
			}
		}

		#endregion //Public members

		#region Private members

		private string ClearTagsPair(string input) {
			if (_beginRule._type != RuleType.Regex || _endRule._type != RuleType.Regex) {
				return input;
			}
			string output = 
				_beginRule._regex.Replace(input, CleanedString);
			return _endRule._regex.Replace(output, CleanedString);
		}

		private string ProcessPair(string input) {
			if (_beginRule._type != RuleType.Regex || _endRule._type != RuleType.Regex) {
				return input;
			}
			Regex beginRegex = _beginRule._regex;
			Regex endRegex = _endRule._regex;
			IFormatProvider culture = CultureInfo.CurrentCulture;
			for(int i = 0; i < MaxNVarcharSize; i++) {
				Match beginMatch = beginRegex.Match(input);
				if (!beginMatch.Success) {
					return input;
				}
				string firtsPart = input.Substring(0, beginMatch.Index);
				string newString = input.Substring(beginMatch.Index + beginMatch.Length);
				Match endMatch = endRegex.Match(newString);
				if (!endMatch.Success) {
					return input;
				}
				string result = newString.Substring(0, endMatch.Index);
				if (_trim) {
					result = result.Replace(Environment.NewLine, String.Empty);
				}
				string lastPart = newString.Substring(endMatch.Index + endMatch.Length);
				input = String.Format(culture, "{0}{1}{2}{3}{4}", firtsPart, 
				                      _beginRule._outputString, result, _endRule._outputString, lastPart);
			}
			return input;
		}

		#endregion //Private members

	}

}