//var pdfData = atob($('#pdfBase64').val());
var pdfData = atob($('#MainContent_pdfBase64').val());
/*
*  costanti per i placaholder 
*/
var maxPDFx = 595;
var maxPDFy = 842;
var offsetY = 7;
var NameTag;
var DBSaveCoOrdinatesArray = [];
/*Define the global variables for access to .aspx pages*/
pdfcoordinates = [];
'use strict';
var currPage = 1;
// The workerSrc property shall be specified.
//
pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.0.943/pdf.worker.min.js';
var numPages = 0;
var thePDF = null;
//
// Asynchronous download PDF
//
var loadingTask = pdfjsLib.getDocument({ data: pdfData });
loadingTask.promise.then(function (pdf) {
	thePDF = pdf;
	numPages = pdf.numPages;
	//pdf.getPage(1).then(handlePages);
	pdf.getPage(1).then(function (page) {
		var scale = 1.0;
		var viewport = page.getViewport(scale);
		//
		// Prepare canvas using PDF page dimensions
		//
		var canvas = document.getElementById('the-canvas');
		var context = canvas.getContext('2d');
		canvas.height = viewport.height;
		canvas.width = viewport.width;
		//
		// Render PDF page into canvas context
		//
		var renderContext = {
			canvasContext: context,
			viewport: viewport
		};
		//page.render(renderContext);

		page.render(renderContext).then(function (page) {
			//viewport = page.getViewport({ scale: scale });
			//canvas.height = viewport.height;
			//canvas.width = viewport.width;
			//page.render({ canvasContext: canvas.getContext('2d'), viewport: viewport });
			$(document).trigger("pagerendered");
		}, function () {
			console.log("ERROR");
		});

	});

});


//For placing the controls on to Image (Over Pdf)
AdminControlsPlacedList = "0";
function PositionControls(ControlsToPlace) {
	//debugger;
	Array = [];
	//debugger;
	AdminControlsPlacedList = ControlsToPlace.split('*')[1];
	if (AdminControlsPlacedList != "0") {
		let controlstring = AdminControlsPlacedList;
		let finalString = controlstring.replace('"', ' ').trim();
		var length = finalString.split(':').length;

		//For Date
		var today = new Date();
		var date = today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear();
		var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
		var dateTime = date + ' ' + time;

		//var val;
		for (var i = 0; i < length - 1; i++) {
			for (var j = 3; j < finalString.split(':')[i].split(',').length;) {
				var PdfXPosition = finalString.split(':')[i].split(',')[1].trim();
				var PdfYPosition = finalString.split(':')[i].split(',')[2].trim();
				var x = finalString.split(':')[i].split(',')[j].trim();
				var y = finalString.split(':')[i].split(',')[j + 1].trim();
				//debugger;
				var SignTag = finalString.split(':')[i].split(',')[0].trim();
				//For Signature Tag
				if (SignTag == 'Signature') {
					var ID = "Signature_" + i;
					$("#pageContainer").append('<div id=' + ID + '  data-id="-1" data-page="0" data-toggle="X" data-valore="X" data-x=' + x + ' data-y=' + y + ' style = "transform: translate(' + x + 'px,' + y + 'px);display: block; position: absolute; left: 0px; top: 0px; " clonable="false"><span class=""></span><span class=""><button onclick="SignaturePopup(' + PdfXPosition + ',' + PdfYPosition + ',' + x + ',' + y + ',' + ID + ');return false;"><img title="Required-Sign Here" src="Images/signatureupload.png" style="width:20px;height:20px;" /></button></span></div>');


				}
				//For Name Tag
				if (SignTag == 'Name') {
					var ID = "Name_" + i;
					//$("#pageContainer").append('<div id=' + ID + '  data-id="-1" data-page="0" data-toggle="X" data-valore="X" data-x=' + x + ' data-y=' + y + ' style = "transform: translate(' + x + 'px,' + y + 'px);display: block; position: absolute; left: 0px; top: 0px; " clonable="false"><span class=""></span><span class=""><input type="text" width="80px" title="Required- Name Here" value="Sam Joseph" /></span></div>');
					$("#pageContainer").append('<div id=' + ID + '  data-id="-1" data-page="0" data-toggle="X" data-valore="X" data-x=' + x + ' data-y=' + y + ' style = "transform: translate(' + x + 'px,' + y + 'px);display: block; position: absolute; left: 0px; top: 0px; " clonable="false"><span class=""></span><span class=""><label>' + ControlsToPlace.split('*')[0]+'</label></span></div>');
				}
				//For Date Tag
				/*if (SignTag == 'Date') {
					var ID = "Date_" + i;
					$("#pageContainer").append('<div id=' + ID + '  data-id="-1" data-page="0" data-toggle="X" data-valore="X" data-x=' + x + ' data-y=' + y + ' style = "transform: translate(' + x + 'px,' + y + 'px);display: block; position: absolute; left: 0px; top: 0px; " clonable="false"><span class=""></span><span class=""><label>' + dateTime +'</label></span></div>');

				}*/
				break;
			}

		}

	}

}


//Signature Popup
function SignaturePopup(PdfX, PdfY, x, y, ID) {
	//debugger;	
	let Sign = document.getElementById('Sign').firstChild.currentSrc;
	if (document.getElementById('Sign').innerHTML == "<img src=\"\">") {
		GlobalSavePdfX = PdfX;
		GlobalSavePdfY = PdfY;
		GlobalSavex = x;
		GlobalSavey = y;
		document.getElementById('tabs').style.display = 'block';
	}
	else {
		//debugger;	
		var ele = document.getElementById(ID.id);
		ele.remove();
		document.getElementById('MainContent_hdnSignImage').value = Sign;
		//alert(document.getElementById('MainContent_hdnSignImage').value);
		$("#pageContainer").append('<div id=' + ID.id + '  data-id="-1" data-page="0" data-toggle="X" data-valore="X" data-x=' + x + ' data-y=' + y + ' style = "transform: translate(' + x + 'px,' + y + 'px);display: block; position: absolute; left: 0px; top: 0px; " clonable="false"><span class=""></span><span class=""><img src=' + Sign + ' style="width:120px;height:20px;" /></span></div>');

	}

	return false;
}





//For Save Functionality, Validation checked first
function Validate() {
	//debugger;
	//Getting all signature tags
	var everyChild = document.querySelectorAll("#pageContainer div");
	for (var i = 0; i < everyChild.length; i++) {

	}

	return true;
}


$(document).bind('pagerendered', function (e) {
	//debugger;
	$('#pdfManager').show();
	var parametri = JSON.parse($('#parameters').val());
	$('#parametriContainer').empty();
	renderizzaPlaceholder(0, parametri);
});

function renderizzaPlaceholder(currentPage, parametri) {
	var maxHTMLx = $('#the-canvas').width();
	var maxHTMLy = $('#the-canvas').height();

	var paramContainerWidth = $('#parametriContainer').width();
	var yCounterOfGenerated = 0;
	var numOfMaxItem = 25;
	var notValidHeight = 30;
	var y = 0;
	var x = 6;
	var page = 0;


	var totalPages = Math.ceil(parametri.length / numOfMaxItem);

	for (i = 0; i < parametri.length; i++) {
		var param = parametri[i];
		var page = Math.floor(i / numOfMaxItem);
		var display = currentPage == page ? "block" : "none";

		if (i > 0 && i % numOfMaxItem == 0) {
			yCounterOfGenerated = 0;
		}

		var classStyle = "";
		var valore = param.valore;
		/*il placeholder non è valido: lo incolonna a sinistra*/

		if (i > 0 && i % numOfMaxItem == 0) {
			yCounterOfGenerated = 0;
		}

		var classStyle = "";
		var valore = param.valore;
		/*il placeholder non è valido: lo incolonna a sinistra*/
		y = yCounterOfGenerated;
		yCounterOfGenerated += notValidHeight;
		classStyle = "drag-drop dropped-out";


		NameTag = param.descrizione;
		$("#parametriContainer").append('<div class="' + classStyle + '" data-id="-1" data-page="' + page + '" data-toggle="' + valore + '" data-valore="' + valore + '" data-x="' + x + '" data-y="' + y + '" style="transform: translate(' + x + 'px, ' + y + 'px); display:' + display + '">  <span class="circle"></span><span class="descrizione">' + param.descrizione + ' </span></div>');
	}

	y = notValidHeight * (numOfMaxItem + 1);
	var prevStyle = "";
	var nextStyle = "";
	var prevDisabled = false;
	var nextDisabled = false;
	if (currentPage == 0) {
		prevStyle = "disabled";
		prevDisabled = true;
	}

	if (currentPage >= totalPages - 1 || totalPages == 1) {
		nextDisabled = true;
		nextStyle = "disabled";
	}

	//Aggiunge la paginazione
	$("#parametriContainer").append('<ul id="pager" class="pager" style="transform: translate(' + x + 'px, ' + y + 'px); width:200px;"><li onclick="changePage(' + prevDisabled + ',' + currentPage + ',-1)" class="page-item ' + prevStyle + '"><span>«</span></li><li onclick="changePage(' + nextDisabled + ',' + currentPage + ',1)" class="page-item ' + nextStyle + '" style="margin-left:10px;"><span>&raquo;</span></li></ul>');

}

function renderizzaInPagina(parametri) {
	//debugger;
	var maxHTMLx = $('#the-canvas').width();
	var maxHTMLy = $('#the-canvas').height();

	var paramContainerWidth = $('#parametriContainer').width();
	var yCounterOfGenerated = 0;
	var numOfMaxItem = 26;
	var notValidHeight = 30;
	var y = 0;
	var x = 6;
	for (i = 0; i < parametri.length; i++) {
		var param = parametri[i];

		var classStyle = "drag-drop can-drop";
		var valore = param.valore;
		/*il placeholder non è valido: lo incolonna a sinistra*/

		var pdfY = maxPDFy - param.posizioneY - offsetY;
		y = (pdfY * maxHTMLy) / maxPDFy;
		x = ((param.posizioneX * maxHTMLx) / maxPDFx) + paramContainerWidth;

		$("#parametriContainer").append('<div class="' + classStyle + '" data-id="' + param.idParametroModulo + '" data-toggle="' + valore + '" data-valore="' + valore + '" data-x="' + x + '" data-y="' + y + '" style="transform: translate(' + x + 'px, ' + y + 'px);">  <span class="circle"></span><span class="descrizione">' + param.descrizione + ' </span></div>');
	}
}


function changePage(disabled, currentPage, delta) {
	if (disabled) {
		return;
	}

	/*recupera solo i parametri non posizionati in pagina*/
	var parametri = [];
	$(".drag-drop.dropped-out").each(function () {
		var valore = $(this).data("valore");
		var descrizione = $(this).find(".descrizione").text();
		parametri.push({ valore: valore, descrizione: descrizione, posizioneX: -1000, posizioneY: -1000 });
		$(this).remove();
	});

	//svuota il contentitore
	$('#pager').remove();
	currentPage += delta;
	renderizzaPlaceholder(currentPage, parametri);
	//aggiorna lo stato dei pulsanti
	//aggiorna gli elementi visualizzati
}


function showCoordinates() {
	//debugger;
	var ControlList = [];
	var nonValidi = [];
	var maxHTMLx = $('#the-canvas').width();
	var maxHTMLy = $('#the-canvas').height();
	var paramContainerWidth = $('#parametriContainer').width();

	$('.drag-drop.can-dropp').each(function (index) {
		var x = parseFloat($(this).data("x"));
		var y = parseFloat($(this).data("y"));
		var valore = $(this).data("valore");
		var descrizione = $(this).find(".descrizione").text();

		var pdfY = y * maxPDFy / maxHTMLy;
		var posizioneY = maxPDFy - offsetY - pdfY;
		var posizioneX = (x * maxPDFx / maxHTMLx) - paramContainerWidth;
		var val = { "pdfY": pdfY, "PositionX": posizioneX, "PositionY": posizioneY, "x": x, "y": y };

		//var val = { "ControlName": descrizione.trim().length == 0 ? "Name" : descrizione, "PositionX": posizioneX, "PositionY": posizioneY, "value": "Red" };
		ControlList.push(val);

	});

	//recupera tutti i placholder validi
	$('.drag-drop.can-drop').each(function (index) {

		//debugger;
		var x = parseFloat($(this).data("x"));
		var y = parseFloat($(this).data("y"));
		var valore = $(this).data("valore");
		var descrizione = $(this).find(".descrizione").text();

		var pdfY = y * maxPDFy / maxHTMLy;
		var posizioneY = maxPDFy - offsetY - pdfY;
		var posizioneX = (x * maxPDFx / maxHTMLx) - paramContainerWidth;
		var val = { "pdfY": pdfY, "PositionX": posizioneX, "PositionY": posizioneY, "x": x, "y": y };

		//var val = { "ControlName": descrizione.trim().length == 0 ? "Name" : descrizione, "PositionX": posizioneX, "PositionY": posizioneY, "value": "Blue" };
		ControlList.push(val);

	});
	if (document.getElementById('MainContent_hdnSelectedReceipentValue').value == "") {
		alert('Please select the Recipients.');
	}
	else if (ControlList.length == 0 || ControlList.length == 0) {
		alert('No controls dragged into document.');
	}
	else {

		pdfcoordinates = JSON.stringify(ControlList);

		alert(pdfcoordinates);


	}


}


