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
using System.Web.UI;

namespace Dictionary {

	/// <summary>
	/// AudioWriterConfig class.
	/// </summary>
	[Serializable]
	class AudioWriterConfig {

		[NonSerialized]
		protected WaveFormat _format;

		public AudioWriterConfig(WaveFormat f) {
			_format = new WaveFormat(f.nSamplesPerSec, f.wBitsPerSample, f.nChannels);
		}

		public AudioWriterConfig(): this(new WaveFormat(44100, 16, 2)) {}

		public WaveFormat Format {
			get { return _format; }
		}
	}

}
