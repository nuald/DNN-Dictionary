using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Dictionary.Common;
using System.Web;
using SpeechLib;

namespace Dictionary {

	#region Delegate

	delegate void OnTTSStartDelegate();
	delegate void OnTTSEndDelegate();
	delegate void OnTTSWordDelegate(long nStart, long nEnd);
	delegate void OnTTSProgressDelegate(long nPercent);

	#endregion

	public class Sapi5 {

		SpVoice _voice;

		#region Property

		/// <summary>
		/// Property, get the current voice name
		/// </summary>
		public string VoiceName {
			get { return _voice.Voice.GetAttribute("Name"); }
		}

		/// <summary>
		/// Property, get the current voice gender
		/// </summary>
		public string VoiceGender {
			get { return _voice.Voice.GetAttribute("Gender"); }
		}

		/// <summary>
		/// Property, get the current voice vendor
		/// </summary>
		public string VoiceVendor {
			get {
				string text = String.Empty;
				try {
					text = _voice.Voice.GetAttribute("Vendor");
				} catch (IOException) {
				}
				return text;
			}
		}

		/// <summary>
		/// Property, get the current voice vendor
		/// </summary>
		public string VoiceLanguage {
			get {
				string text = String.Empty;
				try {
					text = _voice.Voice.GetAttribute("Language");
				} catch (IOException) {
				}
				return text;
			}
		}

		/// <summary>
		/// Property, get the current voice description
		/// </summary>
		public string VoiceDescription {
			get { return _voice.Voice.GetDescription(0); }
		}

		/// <summary>
		/// Property, get the count of installed voices
		/// </summary>
		public int VoiceCount {
			get { return _voice.GetVoices(String.Empty, String.Empty).Count; }
		}


		int _voiceById;
		/// <summary>
		/// Property, get/set the current voice index (Select current voice), which is between 0 and VoiceCount-1
		/// </summary>
		public int VoiceById {
			get { return _voiceById; }
			set {
				int count = _voice.GetVoices(String.Empty, String.Empty).Count;
				if (value <= count - 1 && value >= 0) {
					_voiceById = value;
					_voice.Voice = _voice.GetVoices(String.Empty, String.Empty).Item(value);
				}
			}
		}

		/// <summary>
		/// Property, get/set the current voice bu name. If name not found set default voice;
		/// </summary>
		public string VoiceByName {
			get { return VoiceName; }
			set {
				if (String.IsNullOrEmpty(value)) {
					_voice.Voice = _voice.GetVoices(String.Empty, String.Empty).Item(0);
					return;
				}
				int count = _voice.GetVoices(String.Empty, String.Empty).Count;
				for (int i = 0; i < count; i++) {
					string voiceName = GetVoiceNameById(i);
					if (voiceName == value) {
						_voice.Voice = _voice.GetVoices(String.Empty, String.Empty).Item(i);
						return;
					}
				}
				_voice.Voice = _voice.GetVoices(String.Empty, String.Empty).Item(0);
			}
		}

		/// <summary>
		/// Property, get/set the rate of current voice
		/// </summary>
		public int Rate {
			get { return _voice.Rate; }
			set {
				if (value <= 10 && value >= -10) {
					_voice.Rate = value;
				}
			}
		}

		/// <summary>
		/// Property, get/set the volume of current voice
		/// </summary>
		public int Volume {
			get { return _voice.Volume; }
			set {
				if (value <= 100 && value >= 00) {
					_voice.Volume = value;
				}
			}
		}

		int _fileFormat;

		/// <summary>
		/// Property, get/set the output audio file format. Currently support WAV (0), Sapi (1) The default format is WAV. 
		/// </summary>
		public int FileFormat {
			get { return _fileFormat; }
			set { _fileFormat = value; }
		}

		#endregion

		#region Constructor

		public Sapi5() {
			try {
				//Create a TTS voice and speak.
				_voice = new SpVoice();
			} catch (IOException) {
				return;
			}
		}

		#endregion

		#region Public

		/// <summary>
		/// Get the name of voice
		/// </summary>
		/// <param name="nVoiceIdx"></param>
		public string GetVoiceNameById(int nVoiceIdx) {
			ISpeechObjectTokens voices = _voice.GetVoices(String.Empty, String.Empty);
			string name = voices.Item(nVoiceIdx).GetAttribute("Name");
			return name;
		}

		/// <summary>
		/// Speak text with sound card
		/// </summary>
		/// <param name="strText">The text to speak aloud</param>
		/// <param name="nFlag">nFlag: 0 is for synchronously speaking , 1 is for asynchronously speaking</param>
		public void Speak(String strText, long nFlag) {
			SpeechVoiceSpeakFlags flag = GetFlag(nFlag);
			try {
				_voice.Speak(strText, flag);
			} catch (IOException) {
				return;
			}
		}

		/// <summary>
		/// Convert text to wav
		/// </summary>
		/// <param name="strText">the text to convert</param>
		/// <param name="strFile">the file to save the speech</param>
		/// <param name="nFlag">nFlag: 0 is for synchronously speaking , 1 is for asynchronously speaking</param>
		public void SpeakToFile(String strText, String strFile, long nFlag) {
			try {
				string dllPath = Path.Combine(
					HttpContext.Current.Server.MapPath("~/"), @"bin\lame_enc.dll");
				NativeMethods.LoadLibrary(dllPath);

				SpeechVoiceSpeakFlags flag = GetFlag(nFlag);
				SpeechStreamFileMode SpFileMode = SpeechStreamFileMode.SSFMCreateForWrite;

				if (_fileFormat == 0) {
					SpFileStream spFileStream = new SpFileStream();
					spFileStream.Open(strFile, SpFileMode, false);
					_voice.AudioOutputStream = spFileStream;
					_voice.Speak(strText, flag);
					_voice.WaitUntilDone(Timeout.Infinite);
					spFileStream.Close();
				}
				//Convert to mp3
				if (_fileFormat == 1) {
					SpeakToMP3File(strText, strFile, nFlag);
				}
			} catch (IOException) {
				return;
			} catch (ArgumentException) {
				return;
			}
		}

		/// <summary>
		/// Stop playing or conversion
		/// </summary>
		public void StopTts() {
			try {
				_voice.Speak(null, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
			} catch (IOException) {
				return;
			}
		}

		/// <summary>
		/// Pause playing or conversion
		/// </summary>
		public void Pause() {
			try {
				_voice.Pause();
			} catch (IOException) {
				return;
			}
		}

		/// <summary>
		/// Resume playing or conversion
		/// </summary>
		public void Resume() {
			try {
				_voice.Resume();
			} catch (IOException) {
				return;
			}
		}

		#endregion

		#region Pivate

		static private Mp3WriterConfig _config;

		/// <summary>
		/// Convert text to wav
		/// </summary>
		/// <param name="strText">the text to convert</param>
		/// <param name="strFile">the file to save the speech</param>
		/// <param name="nFlag">nFlag: 0 is for synchronously speaking , 1 is for asynchronously speaking</param>
		private void SpeakToMP3File(String strText, String strFile, long nFlag) {
			try {
				SpeechVoiceSpeakFlags flag = GetFlag(nFlag);

				//Memory stream to Sapi
				SpAudioFormat audioFormat = new SpAudioFormat();
				//audioFormat.Type = SpeechAudioFormatType.SAFT48kHz16BitMono; //Hard setup!!!
				audioFormat.Type = SpeechAudioFormatType.SAFT22kHz16BitMono; //Hard setup!!!

				//Set memory stream
				SpMemoryStream spMemStream = new SpMemoryStream();
				_voice.AudioOutputStream = spMemStream;
				spMemStream.Format = audioFormat;

				_voice.WaitUntilDone(3000);
				//Speak
				_voice.Speak(strText, flag);


				//Copy memory stream
				spMemStream.Seek(0, SpeechStreamSeekPositionType.SSSPTRelativeToStart);
				object buf = spMemStream.GetData();
				byte[] b = (byte[])buf;
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.Write(b, 0, b.Length);

				//Convert memory stream to Sapi file
				ConvertMemoryStreamToMP3File(memoryStream, audioFormat, strFile);
			} catch (IOException) {
				return;
			} 
		}

		static void ConvertMemoryStreamToMP3File(MemoryStream memoryStream, SpAudioFormat audioFormat, String mp3_fileName) {
			try {
				SpWaveFormatEx waveFormatEx = audioFormat.GetWaveFormatEx();
				int rate = waveFormatEx.SamplesPerSec;
				int bits = waveFormatEx.BitsPerSample;
				int channels = waveFormatEx.Channels;
				WaveFormat waveFormat = new WaveFormat(rate, bits, channels);
				Be_Config be_config = new Be_Config(waveFormat, 48);
				_config = new Mp3WriterConfig(waveFormat, be_config);
				using (Stream output = new FileStream(mp3_fileName, FileMode.CreateNew))
				using (Mp3Writer writer = new Mp3Writer(output, _config)) {
					byte[] buff = new byte[writer.OptimalBufferSize];
					int actual = 0;
					memoryStream.Seek(0, SeekOrigin.Begin);
					WriteStream(memoryStream, writer, buff, ref actual);
				}
			} catch (IOException) {
				return;
			}
		}

		private static int WriteStream(MemoryStream memoryStream, Mp3Writer writer, byte[] buff, ref int actual) {
			int read;
			while ((read = memoryStream.Read(buff, 0, buff.Length)) > 0) {
				writer.Write(buff, 0, read);
				actual += read;
			}
			return read;
		}

		/// <summary>
		/// Get SpeechVoiceSpeakFlags from int value
		/// </summary>
		/// <param name="nFlag"></param>
		/// <returns></returns>
		static private SpeechVoiceSpeakFlags GetFlag(long nFlag) {
			SpeechVoiceSpeakFlags flag;
			switch (nFlag) {
				case 1:
					flag = SpeechVoiceSpeakFlags.SVSFDefault;
					break;
				case 2:
					flag = SpeechVoiceSpeakFlags.SVSFIsFilename;
					break;
				case 3:
					flag = SpeechVoiceSpeakFlags.SVSFIsNotXML;
					break;
				case 4:
					flag = SpeechVoiceSpeakFlags.SVSFIsXML;
					break;
				case 5:
					flag = SpeechVoiceSpeakFlags.SVSFlagsAsync;
					break;
				case 6:
					flag = SpeechVoiceSpeakFlags.SVSFNLPMask;
					break;
				case 7:
					flag = SpeechVoiceSpeakFlags.SVSFNLPSpeakPunc;
					break;
				case 8:
					flag = SpeechVoiceSpeakFlags.SVSFPersistXML;
					break;
				case 9:
					flag = SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak;
					break;
				case 10:
					flag = SpeechVoiceSpeakFlags.SVSFUnusedFlags;
					break;
				case 11:
					flag = SpeechVoiceSpeakFlags.SVSFVoiceMask;
					break;
				default:
					flag = SpeechVoiceSpeakFlags.SVSFDefault;
					break;
			}
			return flag;
		}

		#endregion
	}
}
