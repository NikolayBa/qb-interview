namespace Backend.Models
{
	public class CountryPopulationAggregate
	{
		public string CountryName { get; set; }

		//We store the population as Long because Int limit 2 billion, and there's a real chance a country will have more than 2b
		public long Population { get; set; }
	}
}