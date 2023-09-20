using System.ComponentModel;

namespace Cloud1.Models
{
    public class IceCream
    {
        public int Id { get; set; }//identifier 
      
        public string IceName { get; set; }//ice cream flavor
        

        public string? IceDescription { get; set; }//brief description of the ice cream flavor
        
        public double Price { get; set; }
        public string? imageUrl { get; set; }//option of no image
        public int Calories { get; set; }
        //ingridients??list...
    }
}
