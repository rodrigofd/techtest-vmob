namespace FraudDetection
{
  public class Order
  {
    public required int OrderId { get; set; }
    public required int DealId { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string CreditCard { get; set; }
    public string FullAddress { get; private set; }

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
        CreditCard = parts[7],
        FullAddress = $"{parts[3]}, {parts[4]}, {parts[5]}, {parts[6]}"
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

      var detector = new OrderFraudDetector();

      // Read records
      var orders = new List<Order>();
      for (int i = 0; i < n; i++)
      {
        var line = Console.ReadLine();
        var order = Order.Parse(line);
        orders.Add(order);
      }

      // Validate records
      detector.ValidateOrders(orders);

      // Print fraudulent orders as a comma-separated list of order IDs
      Console.WriteLine(string.Join(",", detector.FraudulentOrders.Select(o => o.OrderId)));
    }
  }

  public class OrderFraudDetector
  {
    public HashSet<Order> FraudulentOrders { get; private set; }

    public OrderFraudDetector()
    {
      FraudulentOrders = new HashSet<Order>();
    }

    // Keep an index of orders by deal ID and email
    Dictionary<int, Dictionary<string, Order>> dealsByEmail = new Dictionary<int, Dictionary<string, Order>>();
    // Keep an index of orders by deal ID and full address
    Dictionary<int, Dictionary<string, Order>> dealsByAddresses = new Dictionary<int, Dictionary<string, Order>>();

    public void ValidateOrders(List<Order> orders)
    {
      foreach (var order in orders)
      {
        if (dealsByEmail.ContainsKey(order.DealId))
        {
          if (dealsByEmail[order.DealId].ContainsKey(order.Email))
          {
            var existingOrder = dealsByEmail[order.DealId][order.Email];
            if (existingOrder.CreditCard != order.CreditCard)
            {
              FraudulentOrders.Add(existingOrder);
              FraudulentOrders.Add(order);
            }
          }
          else
          {
            dealsByEmail[order.DealId].Add(order.Email, order);
          }
        }
        else
        {
          dealsByEmail.Add(order.DealId, new Dictionary<string, Order> { { order.Email, order } });
        }

        if (dealsByAddresses.ContainsKey(order.DealId))
        {
          if (dealsByAddresses[order.DealId].ContainsKey(order.FullAddress))
          {
            var existingOrder = dealsByAddresses[order.DealId][order.FullAddress];
            if (existingOrder.CreditCard != order.CreditCard)
            {
              FraudulentOrders.Add(existingOrder);
              FraudulentOrders.Add(order);
            }
          }
          else
          {
            dealsByAddresses[order.DealId].Add(order.FullAddress, order);
          }
        }
        else
        {
          dealsByAddresses.Add(order.DealId, new Dictionary<string, Order> { { order.FullAddress, order } });
        }
      }
    }
  }
}