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
sessionStorage.setItem("value", "2");
//debugger;
//var Value = "2";
//Session["Receipent"] = "2";
//'<%Session["Receipent"] = "' + Value + '"; %>';
/*Define the global variables for access to .aspx pages*/
pdfcoordinates = [];
'use strict';

// The workerSrc property shall be specified.
//
pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.0.943/pdf.worker.min.js';

  //
  // Asynchronous download PDF
  //
  var loadingTask = pdfjsLib.getDocument({data: pdfData});
  loadingTask.promise.then(function(pdf) {
    //
    // Fetch the first page
    //
    pdf.getPage(1).then(function(page) {
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
      
      page.render(renderContext).then(function() {
    	  $(document).trigger("pagerendered");
    	}, function() {
    	  console.log("ERROR");
    	});
      
    });
  });

  
  /* The dragging code for '.draggable' from the demo above
   * applies to this demo as well so it doesn't have to be repeated. */

  // enable draggables to be dropped into this
  interact('.dropzone').dropzone({
    // only accept elements matching this CSS selector
    accept: '.drag-drop',
    // Require a 100% element overlap for a drop to be possible
    overlap: 1,

    // listen for drop related events:

    ondropactivate: function (event) {
      // add active dropzone feedback
      event.target.classList.add('drop-active');
    },
    ondragenter: function (event) {
      var draggableElement = event.relatedTarget,
          dropzoneElement = event.target;

      // feedback the possibility of a drop
		dropzoneElement.classList.add('drop-target');
		if (document.getElementById('MainContent_hdnSelectedReceipentValue').value=="1")
			draggableElement.classList.add('can-dropp');
		else
			draggableElement.classList.add('can-drop');

      draggableElement.classList.remove('dropped-out');
      //draggableElement.textContent = 'Dragged in';
    },
    ondragleave: function (event) {
      // remove the drop feedback style
		event.target.classList.remove('drop-target');
		if (document.getElementById('MainContent_hdnSelectedReceipentValue').value == "1")
			event.relatedTarget.classList.remove('can-dropp');
		else
			event.relatedTarget.classList.remove('can-drop');
      event.relatedTarget.classList.add('dropped-out');
      //event.relatedTarget.textContent = 'Dragged out';
    },
	  ondrop: function (event) {
		
		   if (event.relatedTarget.textContent.trim() == "Name") {
			  //debugger;
			   event.relatedTarget.textContent = document.getElementById('MainContent_hdnSelectedReceipentName').value;
			  // document.getElementById('MainContent_hdnSelectedReceipentName').value.trim() = "Sam Joseph" ? draggableElement.classList.add('can-drop') : draggableElement.classList.add('can-drop');//'Dropped';
			  //draggableElement.classList.remove('dropped-out');
		  };
		 
     
    },
    ondropdeactivate: function (event) {
      // remove active dropzone feedback
      event.target.classList.remove('drop-active');
      event.target.classList.remove('drop-target');
    }
  });

  interact('.drag-drop')
    .draggable({
		inertia: true,	
     /* restrict: {
        restriction: "#selectorContainer",
        endOnly: true,
        elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
      },*/
      autoScroll: true,
      // dragMoveListener from the dragging demo above
		//onmove: dragMoveListener,
		//listeners: { move: dragMoveListener }
		onmove: function (event) {
			var target = event.target,
				// keep the dragged position in the data-x/data-y attributes
				x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
				y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

			// translate the element
			target.style.webkitTransform =
				target.style.transform = 'translate(' + x + 'px, ' + y + 'px)';

			// update the posiion attributes
			target.setAttribute('data-x', x);
			target.setAttribute('data-y', y);
		}
	}).on('move', function (event) {
		var interaction = event.interaction;
		if (interaction.pointerIsDown && !interaction.interacting() && event.currentTarget.getAttribute('clonable') != 'false') {
			var original = event.currentTarget;
			var clone = event.currentTarget.cloneNode(true);
			var x = clone.offsetLeft;
			var y = clone.offsetTop;
			clone.setAttribute('clonable', 'false');
			clone.style.position = "absolute";
			clone.style.left = original.offsetLeft + "px";
			clone.style.top = original.offsetTop + "px";
			original.parentElement.appendChild(clone);
			interaction.start({ name: 'drag' }, event.interactable, clone);
		}
	});
  
  
  function dragMoveListener (event) {
	    var target = event.target,
	        // keep the dragged position in the data-x/data-y attributes
	        x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
	        y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

	    // translate the element
	    target.style.webkitTransform =
	    target.style.transform ='translate(' + x + 'px, ' + y + 'px)';

	    // update the posiion attributes
	    target.setAttribute('data-x', x);
	    target.setAttribute('data-y', y);
	  }

	  // this is used later in the resizing demo
	 // window.dragMoveListener = dragMoveListener;

$(document).bind('pagerendered', function (e) {
	//debugger;
	   $('#pdfManager').show();
	   var parametri = JSON.parse($('#parameters').val());
		 $('#parametriContainer').empty();
	  renderizzaPlaceholder(0, parametri);
	});
  
  function renderizzaPlaceholder(currentPage, parametri){
		  var maxHTMLx = $('#the-canvas').width();
			var maxHTMLy = $('#the-canvas').height();
		
			var paramContainerWidth = $('#parametriContainer').width();
			var yCounterOfGenerated = 0;
			var numOfMaxItem = 25;
			var notValidHeight = 30;
			var y = 0;
			var x = 6;
			var page=0;
			

			var totalPages=Math.ceil(parametri.length/numOfMaxItem);
			
			for (i = 0; i < parametri.length; i++) {
				var param = parametri[i];
				var page = Math.floor(i/numOfMaxItem);
				var display= currentPage == page ? "block" : "none";
				
				if(i > 0 && i%numOfMaxItem == 0){
					yCounterOfGenerated = 0;
				}

				var classStyle = "";
				var valore = param.valore;
				/*il placeholder non è valido: lo incolonna a sinistra*/
		
				if(i > 0 && i%numOfMaxItem == 0){
					yCounterOfGenerated = 0;
				}

				var classStyle = "";
				var valore = param.valore;
				/*il placeholder non è valido: lo incolonna a sinistra*/
				y = yCounterOfGenerated;
				yCounterOfGenerated += notValidHeight;
				classStyle = "drag-drop dropped-out";


				NameTag = param.descrizione;
				$("#parametriContainer").append('<div class="'+classStyle+'" data-id="-1" data-page="'+page+'" data-toggle="'+valore+'" data-valore="'+valore+'" data-x="'+x+'" data-y="'+y+'" style="transform: translate('+x+'px, '+y+'px); display:'+display+'">  <span class="circle"></span><span class="descrizione">'+param.descrizione+' </span></div>');
			}
			
			y = notValidHeight * (numOfMaxItem+1);
			var prevStyle = "";
			var nextStyle = "";
			var prevDisabled = false;
			var nextDisabled = false;
			if(currentPage == 0){
				prevStyle = "disabled";
				prevDisabled = true;
			}
			
			if(currentPage >= totalPages-1 || totalPages == 1){
				nextDisabled=true;
				nextStyle="disabled";
			} 
			
			//Aggiunge la paginazione
			$("#parametriContainer").append('<ul id="pager" class="pager" style="transform: translate('+x+'px, '+y+'px); width:200px;"><li onclick="changePage('+prevDisabled+','+currentPage+',-1)" class="page-item '+prevStyle+'"><span>«</span></li><li onclick="changePage('+nextDisabled+','+currentPage+',1)" class="page-item '+nextStyle+'" style="margin-left:10px;"><span>&raquo;</span></li></ul>');
			
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
	
			$("#parametriContainer").append('<div class="'+classStyle+'" data-id="'+param.idParametroModulo+'" data-toggle="'+valore+'" data-valore="'+valore+'" data-x="'+x+'" data-y="'+y+'" style="transform: translate('+x+'px, '+y+'px);">  <span class="circle"></span><span class="descrizione">'+param.descrizione+' </span></div>');
		}
  	}
	 
	 
	 function changePage(disabled, currentPage, delta){
		 if(disabled){
			return;	 
		 }

		 /*recupera solo i parametri non posizionati in pagina*/
		 var parametri = [];
		 $(".drag-drop.dropped-out").each(function() {
			var valore = $(this).data("valore");
			var descrizione = $(this).find(".descrizione").text();
			parametri.push({valore:valore, descrizione:descrizione, posizioneX:-1000, posizioneY:-1000});
			$(this).remove();
		 });
		 
		 //svuota il contentitore
		 $('#pager').remove();
		 currentPage += delta;
		 renderizzaPlaceholder(currentPage, parametri);
		 //aggiorna lo stato dei pulsanti
		 //aggiorna gli elementi visualizzati
	 }

  
  function showCoordinates(){
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

		  var val = { "ControlName": descrizione.trim().length == 0 ? "Name" : descrizione, "PositionX": posizioneX, "PositionY": posizioneY, "value": "Red","x":x,"y":y };
		  ControlList.push(val);

	  });

  	  //recupera tutti i placholder validi
  	  $('.drag-drop.can-drop').each(function( index ) {
  		  var x = parseFloat($(this).data("x"));
  		  var y = parseFloat($(this).data("y"));
  		  var valore = $(this).data("valore");
  		  var descrizione = $(this).find(".descrizione").text();
  		    
  		  var pdfY = y * maxPDFy / maxHTMLy;
  		  var posizioneY = maxPDFy - offsetY - pdfY;	  
  		  var posizioneX =  (x * maxPDFx / maxHTMLx)  - paramContainerWidth;
  		  
			  var val = { "ControlName": descrizione.trim().length == 0 ? "Name" : descrizione, "PositionX": posizioneX, "PositionY": posizioneY, "value": "Blue", "x": x, "y": y};
			  ControlList.push(val);
      
  	  });

	  if (ControlList.length == 0 || ControlList.length==0) {
         alert('No controls dragged into document');
      }
     else{
		  
		  pdfcoordinates = JSON.stringify(ControlList);
		 
		 // alert(pdfcoordinates);
		  
		  
	  }


  }
ReceipentsDetails = [];
ReceipentsList=[];
function SelectedReceipent(ID, Name, Email) {
	$('#parametriContainer').show();
	var ControlList = [];
	ClearSession();
	document.getElementById('MainContent_hdnSelectedReceipentValue').value = ID;
	ReceipentsDetails.push(ID);
	document.getElementById('MainContent_hdnSelectedReceipentName').value = Name;
	ReceipentsDetails.push(Name);
	document.getElementById('MainContent_hdnSelectedReceipentEmail').value = Email;
	ReceipentsDetails.push(Email);
	if (ID == 1) ReceipentsDetails.push("Red");
	else
		ReceipentsDetails.push("Blue");
	var Value = { "ID": ID, "Name": Name, "Email": Email,"Color":ID==1?"Red":"Blue" }
	//if(ID==1)
	ControlList.push(Value);
	//else
	//ReceipentsDetails.push(ControlList);

	ReceipentsList = JSON.stringify(ReceipentsDetails);
//	pdfcoordinates.append(ReceipentsDetails);
	
}
function ClearSession() {
	document.getElementById('MainContent_hdnSelectedReceipentValue').value = "";
	document.getElementById('MainContent_hdnSelectedReceipentName').value = "";
	document.getElementById('MainContent_hdnSelectedReceipentEmail').value = "";
}

