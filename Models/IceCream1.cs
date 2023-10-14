using System.ComponentModel.DataAnnotations;

namespace Cloud1.Models
{
    public class IceCream1
    {
        public int Id { get; set; }//identifier 

        public string IceName { get; set; }//ice cream flavor


        public string? IceDescription { get; set; }//brief description of the ice cream flavor

        public double Price { get; set; }
        //[RegularExpression(@"^(?!.*\.{2,})[a-zA-Z0-9]+\.(jpg|jpeg|png|gif)$", ErrorMessage = "Invalid image file name.")]
        public string? imageUrl { get; set; }//option of no image

        //public string? imageUrl { get; set; }//option of no image
        public bool isAvailable { get; set; }
    }
}
