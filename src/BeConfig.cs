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

using System.Runtime.InteropServices;
using System;

namespace Dictionary {

	[StructLayout(LayoutKind.Sequential), Serializable]
	public class Be_Config {
		// encoding formats
		public const uint BE_CONFIG_MP3 = 0;
		public const uint BE_CONFIG_LAME = 256;

		public uint dwConfig;
		public Format format;

		public Be_Config(WaveFormat format, uint mpeBitRate) {
			dwConfig = BE_CONFIG_LAME;
			this.format = new Format(format, mpeBitRate);
		}

		public Be_Config(WaveFormat format)
			: this(format, 128) {
		}
	}

}