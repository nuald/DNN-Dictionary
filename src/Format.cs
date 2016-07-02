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
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Dictionary {

	[Serializable]
	public struct Format {
		public LHV1 lhv1;

		public Format(WaveFormat format, uint mpeBitRate) {
			lhv1 = new LHV1(format, mpeBitRate);
		}
	}
}