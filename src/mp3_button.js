//debugger;
var Swf = "../Flash/speakmp3.swf";
function WriteMP3Button(theFile, width, height, configs){
	var myid = Math.round((Math.random()*1000)+1);
	var flashCode = "";
	var newlineChar = "\n";
	flashCode += '<object type="application/x-shockwave-flash" data="'+Swf+'" width="'+width+'" height="'+height+'" id="MP3BT'+myid+'">'+newlineChar;
	flashCode += '<param name="movie" value="'+Swf+'" />'+newlineChar;
	flashCode += '<param name="loop" value="false" />'+newlineChar;
	flashCode += '<param name="menu" value="false" />'+newlineChar;
	flashCode += '<param name="quality" value="high" />'+newlineChar;
	flashCode += '<param name="wmode" value="transparent" />'+newlineChar;
	flashCode += '<param name="flashvars" value="theFile='+theFile+configs+'" />'+newlineChar;
	flashCode += '</object>'+newlineChar;
	//document.write('<br><textarea name="textarea" cols="100" rows="20">'+flashCode+'</textarea><br>')+newlineChar;
	document.write(flashCode);
}
