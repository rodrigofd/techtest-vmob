namespace FraudDetection
{
  class Order
  {
    public required int OrderId { get; set; }
    public required int DealId { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string CreditCard { get; set; }

    public static Order Parse(string line)
    {
      var parts = line.Split(',');
      if (parts.Length != 8) throw new Exception("Invalid order line: " + line);

      return new Order
      {
        OrderId = int.Parse(parts[0]),
        DealId = int.Parse(parts[1]),
        Email = parts[2],
        Address = parts[3],
        City = parts[4],
        State = parts[5],
        ZipCode = parts[6],
        CreditCard = parts[7]
      };
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      // Read number of records
      int n = 0;
      try
      {
        n = int.Parse(Console.ReadLine());
      }
      catch (Exception e)
      {
        Console.WriteLine("Invalid number of records: " + e.Message);
        return;
      }

      // Read records
      var records = new List<Order>();
      for (int i = 0; i < n; i++)
      {
        var line = Console.ReadLine();
        var order = Order.Parse(line);
        records.Add(order);
      }
    }
  }
}