var cntrlType = "";
var cursorImg = "";
var usrColor = "";
var signatures = [[217.27496337890625, 111.60000610351562, 1, "sign", "1", "Deepu Jose", "#b29f5a", "2022/04/05"], [318.27496337890625, 240.5999984741211, 1, "sign", "2", "kichu", "#b43ccf", "2022/04/05"], [158.27496337890625, 287.5999984741211, 1, "sign", "3", "pramod", "#9d3ef4", "2022/04/05"], [269.27496337890625, 364.5999984741211, 1, "sign", "4", "manu", "#9ad196", "2022/04/05"]];
var c = document.getElementById("the-canvas");
var ctx = c.getContext("2d");
var $canvas = $("#the-canvas");
var canvasOffset = $canvas.offset();
var offsetX = canvasOffset.left;
var offsetY = canvasOffset.top;
var scrollX = $canvas.scrollLeft();
var scrollY = $canvas.scrollTop();
var placeHolder = [];
var shp = [];
var shapes = [];
$(document).ready(function () {

    $("#loadpdf").click(function () {
        var url = location.origin + '/Files/UploadedFile.pdf';
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            pdfDoc = pdfDoc_;
            document.getElementById('page_count').textContent = pdfDoc.numPages;
            // Initial/first page rendering
            renderPage(pageNum);

        });

    });



});


//code for render pdf in canvas
var pdfjsLib = window['pdfjs-dist/build/pdf'];
// The workerSrc property shall be specified.
pdfjsLib.GlobalWorkerOptions.workerSrc = '//mozilla.github.io/pdf.js/build/pdf.worker.js';

var pdfDoc = null,
    pageNum = 1,
    pageRendering = false,
    pageNumPending = null,
    scale = 1,
    canvas = document.getElementById('the-canvas'),
    ctx = canvas.getContext('2d');

function renderPage(num) {
    pageRendering = true;
    // Using promise to fetch the page
    pdfDoc.getPage(num).then(function (page) {
        var viewport = page.getViewport({ scale: scale });
        canvas.height = viewport.height;
        canvas.width = viewport.width;

        // Render PDF page into canvas context
        var renderContext = {
            canvasContext: ctx,
            viewport: viewport
        };
        var renderTask = page.render(renderContext);

        // Wait for rendering to finish
        renderTask.promise.then(function () {
            pageRendering = false;
            if (pageNumPending !== null) {
                // New page rendering is pending
                renderPage(pageNumPending);
                pageNumPending = null;
            }

            if (signatures.length > 0) {
                for (i = 0; i < signatures.length; i++) {

                    if (pageNum == signatures[i][2]) {
                        drawRect(signatures[i][0], signatures[i][1], signatures[i][2], signatures[i][3], signatures[i][4], signatures[i][5], signatures[i][6], signatures[i][7])
                    }
                }
            }
        });

    });

    // Update page counters
    document.getElementById('page_num').textContent = num;


}

/**
 * If another page rendering in progress, waits until the rendering is
 * finised. Otherwise, executes rendering immediately.
 */
function queueRenderPage(num) {
    if (pageRendering) {
        pageNumPending = num;
    } else {
        renderPage(num);
    }

}

/**
 * Displays previous page.
 */
function onPrevPage() {
    if (pageNum <= 1) {
        return;
    }
    pageNum--;
    queueRenderPage(pageNum);
    console.log("prevpage");
}
document.getElementById('prev').addEventListener('click', onPrevPage);

/**
 * Displays next page.
 */
function onNextPage() {
    if (pageNum >= pdfDoc.numPages) {
        return;
    }
    pageNum++;
    queueRenderPage(pageNum);
    console.log("nxtpg");
}
document.getElementById('next').addEventListener('click', onNextPage);

function drawRect(x, y, pagenum, cnttype, usrid, usrnme, clr, dte) {
    shp = {
        points: [{
            x: x,
            y: y
        }, {
            x: 20 + x,
            y: 100 + y
        }, {
            x: 20 + x,
            y: 100 + y
        }, {
            x: 70 + x,
            y: 20 + y
        }, {
            x: 20 + x,
            y: 20 + y
        }],
        message: usrnme
    }
    shapes.push(shp);
    ctx.beginPath();
    if (cnttype == "name") {
        ctx.fillStyle = clr;
        ctx.font = "32px Arial";
        ctx.fillText(usrnme, x, y);
    }
    if (cnttype == "sign") {
        ctx.fillStyle = clr;
        ctx.font = "32px  Babylonica";
        ctx.fillText(usrnme, x, y);
    }
    if (cnttype == "date") {

        ctx.fillStyle = clr;
        ctx.font = "22px Arial";
        ctx.fillText(dte, x, y);
    }
    ctx.lineWidth = "1";
    //ctx.strokeStyle = clr;
    //ctx.fillRect(x, y, 40, 40);
    //ctx.stroke();
    define(shp)
    ctx.fill();
    ctx.stroke();
}


function handleMouseDown(e) {
    e.preventDefault();

    // get the mouse position
    var mouseX = parseInt(e.clientX - offsetX);
    var mouseY = parseInt(e.clientY - offsetY);

    // iterate each shape in the shapes array
    for (var i = 0; i < shapes.length; i++) {
        var shape = shapes[i];
        // define the current shape
        define(shape);
        // test if the mouse is in the current shape
        if (ctx.isPointInPath(mouseX, mouseY)) {
            // if inside, display the shape's message
            alert(shape.message);
        }
        console.log("clicked" + mouseX + "-" + mouseY);
    }

}

// listen for mousedown events
$("#the-canvas").mousedown(function (e) {
    handleMouseDown(e);
});

function define(shape) {
    var points = shape.points;
    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y)
    //ctx.fillRect(points[0].x, points[0].y, points[1].x, points[1].y);
    var x = points[0].x;
    var y = points[0].y;
 /*   ctx.moveTo(20 + x, 20 + y);*/
    //ctx.lineTo(20 + x, 100 + y);
    //ctx.lineTo(70 + x, 100 + y);
    //ctx.lineTo(70 + x, 20 + y);
    //ctx.lineTo(20 + x, 20 + y);
    ctx.moveTo(points[0].x, points[0].y);
    for (var i = 1; i < points.length; i++) {
        ctx.lineTo(points[i].x, points[i].y);
        console.log("x :"+x+", y:"+y)
    }

}