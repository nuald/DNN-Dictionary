<%@ Control Language="C#" AutoEventWireup="false" %>

<script type="text/javascript">
<!--
	window.<%= ClientID %>SpeakFlash = new Object(); 
//-->
</script>

<object type="application/x-shockwave-flash" data='<%= Page.Request.ApplicationPath %>/DesktopModules/Dictionary/Flash/speakmp3js.swf?new'
		width="18" height="18" style="vertical-align:top" id="<%= ClientID %>SpeakFlash">
	<param name="swliveconnect" value="true" />
	<param name="allowScriptAccess" value="sameDomain" />
	<param name="loop" value="false" />
	<param name="quality" value="high" />
	<param name="wmode" value="transparent" />
	<param name="movie" value="<%= Page.Request.ApplicationPath %>/DesktopModules/Dictionary/Flash/speakmp3js.swf?new" />
	<asp:Image runat="server" ID="nf" ImageUrl="~/DesktopModules/Dictionary/Images/noFlash.gif" Width="18" Height="18"
		AlternateText="No flash available" ToolTip="No flash available" />
</object>

<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0" width="1" height="1" id="zero">
	<param name="allowScriptAccess" value="sameDomain" />
	<param name="movie" value="<%= Page.Request.ApplicationPath %>/DesktopModules/Dictionary/Flash/zero.swf" />
	<param name="loop" value="false" />
	<param name="menu" value="false" />
	<param name="quality" value="low" />
	<param name="wmode" value="transparent" />
</object>

<script type="text/javascript">
<!--
	AddBodyLoadListener('InitSpeakFlash', InitSpeakFlash);
	var speakFlash;

	function InitSpeakFlash() {
		speakFlash = GetE('<%= ClientID %>SpeakFlash');
		if (isIE) {
			form = document.forms[0];
			window.<%= ClientID %>SpeakFlash = form.<%= ClientID %>SpeakFlash;
			window.<%= ClientID %>SpeakFlash.PlaySound = function(val) {
				string1 = "<invoke name=\"PlaySound\" returntype=\"javascript\"><arguments><string>"+val+"</string></arguments></invoke>"
				form.<%= ClientID %>SpeakFlash.CallFunction(string1)
			}
			window.<%= ClientID %>SpeakFlash.SetSound = function(val) {
				string1 = "<invoke name=\"SetSound\" returntype=\"javascript\"><arguments><string>"+val+"</string></arguments></invoke>"
				form.<%= ClientID %>SpeakFlash.CallFunction(string1)
			}
			window.<%= ClientID %>SpeakFlash.Clear = function(val) {
				string1 = "<invoke name=\"Clear\" returntype=\"javascript\"><arguments><string>"+val+"</string></arguments></invoke>"
				form.<%= ClientID %>SpeakFlash.CallFunction(string1)
			}
		}
	}

	function CallClear() {
		if (speakFlash != null) {
			speakFlash.Clear();
		}
	}

	function CallSet() {
		if (speakFlash != null) {
			speakFlash.SetSound(file);
		}
	}

	function CallPlay() {
		if (speakFlash != null) {
			speakFlash.PlaySound(file);
		}
	}
-->
</script>
