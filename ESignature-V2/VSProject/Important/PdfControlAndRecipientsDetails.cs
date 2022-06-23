//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System.Collections.Generic;

namespace Important
{
    public class PdfControlAndRecipientsDetails
    {
        // PositionX,PositionY gives the PDF CoOrdinates
        //X,Y gives the canvas-screen Coordinates
        public string ControlName { get; set; }
        public string PositionX { get; set; }
        public string PositionY { get; set; }
        public string Color { get; set; }
        public string X { get; set; }
         public string Y { get; set; }

        List<PdfControlAndRecipientsDetails> ControlPositions { get; set; }// new List<PdfControlAndRecipientsDetails>();

    }
    public class RecipientsList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Color { get; set; }
       public List<RecipientsList> Recipients { get; set; }
    }

}