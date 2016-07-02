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
using System.Web.UI;
using Dictionary;
using DotNetNuke.Modules.Dictionary;
using DotNetNuke.Services.Exceptions;

public partial class DicCT : UserControl {
	protected void _LangDL_Load(object sender, EventArgs e) {
		try {
			if (!Page.IsPostBack) {
				List<DictionaryDefinition> dictionaryDefinitionList = DictionaryController.GetDictionaryDefinitionList();
				_LangDL.Items.Clear();
				foreach (DictionaryDefinition dictionaryDefinition in dictionaryDefinitionList) {
					_LangDL.Items.Add(dictionaryDefinition.TranslateDirection);
				}
				_LangDL.SelectedIndex = 0;
			}
		} catch (Exception exc) {
			// Module failed to load
			Exceptions.ProcessModuleLoadException(this, exc);
		}
	}
}
