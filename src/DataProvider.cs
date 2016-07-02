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

using System.Collections.Generic;
using System.Data;

namespace Dictionary
{

	public class DictionaryDefinition {
		private string _translateDirection;
		private string _dictionaryName;
		private string _checkString;
		private string _warningString;
		private string _errorString;
		private string _voiceEngine;

		public string TranslateDirection {
			get { return _translateDirection; }
			set { _translateDirection = value; }
		}

		public string DictionaryName {
			get { return _dictionaryName; }
			set { _dictionaryName = value; }
		}

		public string CheckString {
			get { return _checkString; }
			set { _checkString = value; }
		}

		public string WarningString {
			get { return _warningString; }
			set { _warningString = value; }
		}

		public string ErrorString {
			get { return _errorString; }
			set { _errorString = value; }
		}

		public string VoiceEngine {
			get { return _voiceEngine; }
			set { _voiceEngine = value; }
		}
	}


    public abstract class DataProvider {

        // singleton reference to the instantiated object 
        private static DataProvider objProvider = null;

        //  constructor
        static DataProvider() {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider() {
            objProvider = ((DataProvider)(DotNetNuke.Framework.Reflection.CreateObject("data", "Dictionary", "")));
        }

        // return the provider
        public static DataProvider Instance() {
            return objProvider;
        }

        // all core methods defined below

    	public abstract string SearchWord(DictionaryDefinition dictionaryDefinition, string word);
    	public abstract IDataReader GetDictionaryDefinition(string translateDirection);
    	public abstract IDataReader GetDictionaryDefinitionList();
    }
}

