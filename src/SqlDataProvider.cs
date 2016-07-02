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
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace Dictionary {
	public class SqlDataProvider : DataProvider {
		#region Fields

		private const string ProviderType = "data";

		private readonly ProviderConfiguration _providerConfiguration =
			ProviderConfiguration.GetProviderConfiguration(ProviderType);

		private readonly string _connectionString;
		private readonly string _providerPath;
		private readonly string _objectQualifier;
		private readonly string _databaseOwner;
		private readonly string searchTemplate = @"SELECT translate FROM {0} WHERE (word LIKE @SearchWord)";
		private readonly string getAllDictionaryDefinitionTemplate = @"SELECT * FROM {0}Dictionary";

		private readonly string getDictionaryDefinitionTemplate =
			@"SELECT * FROM {0}Dictionary WHERE translateDirection='{1}'";

		#endregion //Fields

		#region Ctor

		public SqlDataProvider() {
			Provider objProvider = ((Provider) (_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]));
			_connectionString = Config.GetConnectionString();
			if ((_connectionString == "")) {
				_connectionString = objProvider.Attributes["connectionString"];
			}
			_providerPath = objProvider.Attributes["providerPath"];
			_objectQualifier = objProvider.Attributes["objectQualifier"];
			if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false) {
				_objectQualifier += "_";
			}
			_databaseOwner = objProvider.Attributes["databaseOwner"];
			if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false) {
				_databaseOwner += ".";
			}
		}

		#endregion //Ctor

		#region Properties

		public string ConnectionString {
			get { return _connectionString; }
		}

		public string ProviderPath {
			get { return _providerPath; }
		}

		public string ObjectQualifier {
			get { return _objectQualifier; }
		}

		public string DatabaseOwner {
			get { return _databaseOwner; }
		}

		#endregion //Properties

		#region Public members

		/// <summary>
		/// Translate word.
		/// </summary>
		/// <param name="dictionaryDefinition">Dictionary definition.</param>
		/// <param name="word">Word to translate.</param>
		/// <returns>Translated word.</returns>
		public override string SearchWord(DictionaryDefinition dictionaryDefinition, string word) {
			string dictionaryName = dictionaryDefinition.DictionaryName;
			return
				(string)
				SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
				                        String.Format(searchTemplate, DatabaseOwner + ObjectQualifier + dictionaryName),
				                        new SqlParameter[] {new SqlParameter("SearchWord", word)});
		}

		/// <summary>
		/// Ged dictionary definition from DB.
		/// </summary>
		/// <param name="translateDirection">Translate direction (En>Ru,...).</param>
		/// <returns>Dictionary drfinition.</returns>
		public override IDataReader GetDictionaryDefinition(string translateDirection) {
			return
				SqlHelper.ExecuteReader(ConnectionString, CommandType.Text,
				                        String.Format(getDictionaryDefinitionTemplate, DatabaseOwner + ObjectQualifier,
				                                      translateDirection));
		}

		/// <summary>
		/// Get all dictionary from DB.
		/// </summary>
		/// <returns>Dictionary list.</returns>
		public override IDataReader GetDictionaryDefinitionList() {
			return
				SqlHelper.ExecuteReader(ConnectionString, CommandType.Text,
				                        String.Format(getAllDictionaryDefinitionTemplate, DatabaseOwner + ObjectQualifier));
		}

		#endregion //Public members
	}
}
