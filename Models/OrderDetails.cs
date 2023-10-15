namespace Cloud1.Models
{
	public class OrderDetails
	{
		//while creating an order, we want to save its details. that include weather,dates,flavours,
		//and the order that we are talking about.
		public int Id { get; set; }
		public WeatherResponse weatherResponse { get; set; }
		public HebcalResponse hebcalResponse { get; set; }
		public List<CartItem> cartItemsList { get; set; }
		public Order order { get; set; }//for communication


	}

}
