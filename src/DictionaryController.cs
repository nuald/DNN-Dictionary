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
using System.Data;
using Dictionary;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.Dictionary {
	public class DictionaryController : IUpgradeable {

		#region Public members

		/// <summary>
		/// Translate selected word.
		/// </summary>
		/// <param name="dictionaryDefinition">Dictionary definition.</param>
		/// <param name="word">Translating word.</param>
		/// <returns>Translated word.</returns>
		public static string Translate(DictionaryDefinition dictionaryDefinition, string word) {
			return DataProvider.Instance().SearchWord(dictionaryDefinition, word);
		}

		/// <summary>
		/// Get list of all dictionary definition from DB.
		/// </summary>
		/// <returns>Dictionary definition list.</returns>
		public static List<DictionaryDefinition> GetDictionaryDefinitionList() {
			List<DictionaryDefinition> dictionaryDefinitionList = new List<DictionaryDefinition>();
			using (IDataReader dr = DataProvider.Instance().GetDictionaryDefinitionList()) {
				while (dr.Read()) {
					DictionaryDefinition dictionaryDefinition = new DictionaryDefinition();
					dictionaryDefinition.CheckString = Convert.ToString(dr["checkString"]);
					dictionaryDefinition.DictionaryName = Convert.ToString(dr["dictionaryName"]);
					dictionaryDefinition.ErrorString = Convert.ToString(dr["errorString"]);
					dictionaryDefinition.TranslateDirection = Convert.ToString(dr["translateDirection"]);
					dictionaryDefinition.VoiceEngine = Convert.ToString(dr["voiceEngine"]);
					dictionaryDefinition.WarningString = Convert.ToString(dr["warningString"]);

					dictionaryDefinitionList.Add(dictionaryDefinition);
				}
			}
			return dictionaryDefinitionList;
		}

		/// <summary>
		/// Get dictionary definition from translate direction code.
		/// </summary>
		/// <param name="translateDirection">Translate direction (En>Ru, ...).</param>
		/// <returns></returns>
		public static DictionaryDefinition GetDictionaryDefinition(string translateDirection) {
			DictionaryDefinition dictionaryDefinition = null;
			using (IDataReader dr = DataProvider.Instance().GetDictionaryDefinition(translateDirection)) {
				while (dr.Read()) {
					dictionaryDefinition = new DictionaryDefinition();
					dictionaryDefinition.CheckString = Convert.ToString(dr["checkString"]);
					dictionaryDefinition.DictionaryName = Convert.ToString(dr["dictionaryName"]);
					dictionaryDefinition.ErrorString = Convert.ToString(dr["errorString"]);
					dictionaryDefinition.TranslateDirection = Convert.ToString(dr["translateDirection"]);
					dictionaryDefinition.VoiceEngine = Convert.ToString(dr["voiceEngine"]);
					dictionaryDefinition.WarningString = Convert.ToString(dr["warningString"]);
				}
			}
			return dictionaryDefinition;
		}

		#endregion //Public members

		#region IUpgradable

		public string UpgradeModule(string Version) {
			return "Custom upgrade code goes here for Version: " + Version;
		}

		#endregion
	}
}
