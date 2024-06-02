using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FraudDetection
{
  public class Order
  {
    public required int OrderId { get; set; }
    public required int DealId { get; set; }

    private string email;
    public string Email
    {
      get => email;
      set
      {
        email = email.ToLower().Split('@')[0].Split('+')[0].Replace(".", "") + "@" + email.Split('@')[1];
      }
    }
    private string address;
    public string Address
    {
      get => address;
      set
      {
        // normalize the address by replacing common abbreviations
        value = Regex.Replace(value, @"\bst\b|\bst.\b", "street", RegexOptions.IgnoreCase);
        value = Regex.Replace(value, @"\brd\b|\brd.\b", "road", RegexOptions.IgnoreCase);
        address = value;
      }
    }

    public required string City { get; set; }

    private string state;
    public string State
    {
      get => state;
      set
      {
        // normalize the lowercased value to a two-letter uppercase code for states IL,CA,NY
        if (value.ToLower() == "illinois") state = "IL";
        else if (value.ToLower() == "california") state = "CA";
        else if (value.ToLower() == "new york") state = "NY";
        else
          state = value;
      }
    }

    public required string ZipCode { get; set; }
    public required string CreditCard { get; set; }
    public string FullAddress { get; private set; }

    public static Order Parse(string line)
    {
      var parts = line.Split(',');
      if (parts.Length != 8) throw new Exception("Invalid order line: " + line);

      var order = new Order
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

      order.buildFullAddress();

      return order;
    }

    private void buildFullAddress()
    {
      FullAddress = $"{Address}, {City}, {State}, {ZipCode}";
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