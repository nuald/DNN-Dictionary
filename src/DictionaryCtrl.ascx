<%@ Control Language="C#" AutoEventWireup="false" CodeFile="DictionaryCtrl.ascx.cs" Inherits="DicCT" %>
<%@ Register Src="ServerTtsCtrl.ascx" TagName="STTSCT" TagPrefix="uc2" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:ScriptManagerProxy ID="smp" runat="server">
	<Services>
		<asp:ServiceReference Path="DictionaryService.asmx" />
	</Services>
</asp:ScriptManagerProxy>
	
<script type="text/javascript">
//<![CDATA[
	var ua = navigator.userAgent;
	var isOpera = /opera [56789]|opera\/[56789]/i.test(ua);
	var isIE = !isOpera && /msie [56789]/i.test(ua);
	var isMozilla = !isOpera && /mozilla\/[56789]/i.test(ua);
	var bodyOnLoad = new Array();
	var bodyOnClick = new Array();

	function AddBodyLoadListener(name,func) {
		var el = new Array(name,func);
		bodyOnLoad.push(el);
	}

	function BodyLoad() {
		for (var index=0; index < bodyOnLoad.length; index++) {
			bodyOnLoad[index][1]();
		}
	}

	function BodyClick() {
		for (var index=0; index < bodyOnClick.length; index++) {
			bodyOnClick[index][1]();
		}
	}

	function GetE(id) {
		return document.getElementById(id);
	}
//]]>
</script>
	
<table width="100%">
	<tr>
		<td colspan="3">
			<a style="font-size:8pt" href="" onclick="return TranslateSelectedText()">Translate Selected</a>
		</td>
	</tr>
	<tr>
		<td style="width: 100%">
			<asp:TextBox ID="_wordTX" runat="server" Font-Size="8pt" Width="99%" />
		</td>
		<td style="width: 28px; height: 26px;">
			<asp:Button runat="server" ID="_goBT" Font-Size="8pt" Text="Go  !" OnClientClick="return TranslateButtonOnClick()" />
		</td>
	</tr>
	<tr>
		<td style="width:auto;font-size:8pt;font-style:italic;" valign="top" colspan="3">
			<asp:Literal runat="server" ID="_dirLB" Text="Direction:"/>
			<asp:DropDownList Width="80px" ID="_LangDL" runat="server" Font-Size="8pt" OnLoad="_LangDL_Load"></asp:DropDownList>
			<uc2:STTSCT ID="_STTSCT" runat="server" />
			<asp:Label ID="_infoLB" runat="server" />
		</td>
	</tr>
	<tr>
		<td colspan="3" style="width: auto">
			<asp:Label ID="_lastWordLabLB" runat="server" SkinID="SmallLabel" Text="Last word:" />
			<asp:Label ID="_lastWordLB" runat="server" SkinID="SmallLabel" Font-Bold="True" />
		</td>
	</tr>
	<tr>
		<td colspan="3">
			<asp:Label ID="_trandlLB" runat="server" Height="100%" Width="100%" SkinID="SmallLabel" />
		</td>
	</tr>
</table>
<asp:SqlDataSource ID="_sqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:SqlServices %>"
	ProviderName="System.Data.SqlClient" DataSourceMode="DataReader">
	<FilterParameters>
		<asp:Parameter DefaultValue=" " Name="@SearchWord" />
	</FilterParameters>
	<SelectParameters>
		<asp:Parameter DefaultValue=" " Name="SearchWord" />
	</SelectParameters>
</asp:SqlDataSource>

<script type="text/javascript">
<!--
//Start
	var wordTextBox = GetE('<%= _wordTX.ClientID %>');
	var languageListDropDown = GetE('<%= _LangDL.ClientID %>');
	var lastWordLabelLabel = GetE('<%= _lastWordLabLB.ClientID %>');
	var lastWordLabel = GetE('<%= _lastWordLB.ClientID %>');
	var translateLabel = GetE('<%= _trandlLB.ClientID %>');
	var infoLabel = GetE('<%= _infoLB.ClientID %>');
	
// KeyPress
	wordTextBox.onkeypress = WordTextBoxKeyPress;

	function WordTextBoxKeyPress(e) {
		var keyCode;
		if (isIE) {
			keyCode = self.event.keyCode;
		} else {
			keyCode = e.which;
		}
		if (keyCode == 13) {
			TranslateButtonOnClick();
			return false;
		}
		infoLabel.style.visibility = 'hidden';
	}
	
// Translate
	var soundFile;

	function GetWord() {
		var language = languageListDropDown.options[languageListDropDown.selectedIndex].value;
		var param = wordTextBox.value + String.fromCharCode(13) + language;
		infoLabel.innerHTML = 'translating...';
		infoLabel.style.visibility = 'visible';
		//<%= ClientID %>Request(param, '<%= ClientID %>');
		Dictionary.DictionaryService.DictionaryCallBack(param, <%=ClientID %>Response);
	}

	function <%= ClientID %>Response(translated, context) {
		infoLabel.style.visibility = 'hidden';
		var param = translated.split(String.fromCharCode(13));
		if (param[1] != "") {
			lastWordLabelLabel.style.visibility = 'visible';
			lastWordLabel.innerHTML = param[0];
			translateLabel.innerHTML = param[1];
			soundFile = param[2];
			if (speakFlash != null) {
				speakFlash.Clear();
				speakFlash.SetVariable('sound_file', soundFile);
			} else {
				speakFlash('var speakFlash == null');
			}
		}
	}

	function CrossLink(link) {
		if (link != "") {
			wordTextBox.value = link;
			GetWord();
		}
	}

//Validate
	function TranslateButtonOnClick() {
		if (wordTextBox.value.length > 0 && wordTextBox.value.length < 30) {
			GetWord();
		} else {
			//infoLabel.style.visibility = 'visible';
			//infoLabel.innerHTML = 'Input error';
		};
		return false;
	}

	function GetSelectedText() {
		var text;
		if (window.getSelection) {
			text = window.getSelection();
		} else if (document.getSelection) {
			text = document.getSelection();
		} else if (document.selection) {
			text = document.selection.createRange().text;
		} 
		if (text == null) { 
			alert("Your browser does not support text selection or IFRAME"); 
			return '';
		}
		if (text != '') {
			return text;
		}
		return '';
	}

//Copy selection
	function TranslateSelectedText() {
		text = GetSelectedText();
		if (text !='') {
			wordTextBox.value = text;
			GetWord();
		}
		return false;
	}

//Init
	function <%= ClientID %>Init() {
		infoLabel.style.visibility = 'hidden';
		lastWordLabelLabel.style.visibility = 'hidden';
		<%= ClientID %>text = GetE('<%= _wordTX.ClientID%>');
		if (isMozilla) {
			<%= ClientID %>text.style.width = (<%= ClientID %>text.clientWidth - 10) + 'px';
		} else {
			<%= ClientID %>text.style.pixelWidth = <%= ClientID %>text.clientWidth - 10;
		}
	}
	
	AddBodyLoadListener('<%= ClientID %>Init', <%= ClientID %>Init);
//-->
</script>