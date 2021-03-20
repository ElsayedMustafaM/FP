using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using System;
using System.Runtime.CompilerServices;

namespace Declarative
{
    internal static class Program
    {


        public static void Main(string[] args)
        {
            InvoicingPath InvoicePath = new InvoicingPath();
            AvailabilityPath AvailabilityPath = new AvailabilityPath();
            (Order order, ProcessConfiguration processConfiguration) = setConfiguration();

            Func<Order, Double> CostOfOrder = CalcAdjustedCostofOrder(processConfiguration, InvoicePath, AvailabilityPath);

            Console.WriteLine(CostOfOrder(order));
            Console.ReadLine();
        }

        //Setup of the Process Configuration and Data
        public static (Order order, ProcessConfiguration processConfiguration) setConfiguration()
        {
            ProcessConfiguration processConfiguration = new ProcessConfiguration();
            Customer customer = new Customer();
            Order order = new Order();
            processConfiguration.invoiceChoice = InvoiceChoice.Inv3;
            processConfiguration.shippingChoice = ShippingChoice.Sh2;
            processConfiguration.freightChoice = FreightChoice.fr3;
            processConfiguration.availabilityChoice = AvailabilityChoice.AV2;
            processConfiguration.shippingDateChoice = ShippingDateChoice.SD2;
            order.customer = customer;
            order.date = new DateTime(2021, 3, 16);
            order.cost = 2000;
            return (order, processConfiguration);
        }

        //Adjusted Cost for Order
        public static Func<Order, double> CalcAdjustedCostofOrder(ProcessConfiguration c, InvoicingPath InvoicePath, AvailabilityPath AvailabilityPath)
        {
            return (x) => AdjustCost(x, InvoicePathFunc(c, InvoicePath), AvailabilityPathFunc(c, AvailabilityPath));
        }

        //Adjusted Cost
        public static double AdjustCost(Order r, Func<Order, Freight> calcFreigt, Func<Order, ShippingDate> calcShippingDate)
        {

            Freight f = calcFreigt(r);
            ShippingDate s = calcShippingDate(r);
            Console.WriteLine("\n\nDay of Shipping : " + s.date.DayOfWeek.ToString() + "\n");

            double cost = (s.date.DayOfWeek.ToString() == "Monday") ? f.cost + 1000 : f.cost + 500;

            ///Finall Cost 
            return cost;
        }

        /// Return InvoicePath Composed Function
        public static Func<Order, Freight> InvoicePathFunc(ProcessConfiguration c, InvoicingPath fpl)
        {

            Func<Order, Freight> p = fpl.InvoiceFunctions.Where((x) => x.InvoiceChoose == c.invoiceChoice).Select((x) => x.CalcInvoice).FirstOrDefault()
                           .Compose(fpl.ShippingFunctions.Where((x) => x.ShippingChoose == c.shippingChoice).Select((x) => x.calcShipping).FirstOrDefault())
                           .Compose(fpl.frieghtFunctions.Where((x) => x.freightChoose == c.freightChoice).Select((x) => x.calcFrieght).FirstOrDefault());
            return p;
        }

        /// ///  Return AvailabilityPath Composed Funcrtion 
        public static Func<Order, ShippingDate> AvailabilityPathFunc(ProcessConfiguration c, AvailabilityPath spl)
        {

            Func<Order, ShippingDate> p = spl.AvailabilityFunctions.Where((x) => x.availabilityChoose == c.availabilityChoice).Select((x) => x.calcAvailability).FirstOrDefault()
                                  .Compose(spl.ShippingDateFunctions.Where((x) => x.shippingDateChoose == c.shippingDateChoice).Select((x) => x.calcShippingDate).FirstOrDefault());

            return p;
        }


        // Compose Function
        public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T1, T2> f, Func<T2, T3> g)
        {
            return (x) => g(f(x));
        }

        #region Basic Data
        public class InvoicingPath
         {
            public List<(InvoiceChoice InvoiceChoose, Func<Order, Invoice> CalcInvoice)> InvoiceFunctions;
            public List<(ShippingChoice ShippingChoose, Func<Invoice, Shipping> calcShipping)> ShippingFunctions;
            public List<(FreightChoice freightChoose, Func<Shipping, Freight> calcFrieght)> frieghtFunctions;

            public InvoicingPath()
            {
                InvoiceFunctions = new List<(InvoiceChoice InvoiceChoose, Func<Order, Invoice> CalcInvoice)>()
                    {
                         (InvoiceChoice.Inv1,calcInvoice1),
                         (InvoiceChoice.Inv2,calcInvoice2),
                         (InvoiceChoice.Inv3,calcInvoice3),
                         (InvoiceChoice.Inv4,calcInvoice4),
                         (InvoiceChoice.Inv5,calcInvoice5)
                    };
                ShippingFunctions = new List<(ShippingChoice ShippingChoose, Func<Invoice, Shipping> calcShipping)>()
                    {
                         (ShippingChoice.Sh1,calcShipping1),
                         (ShippingChoice.Sh2,calcShipping2),
                         (ShippingChoice.Sh3,calcShipping3)
                    };
                frieghtFunctions = new List<(FreightChoice freightChoose, Func<Shipping, Freight> calcFrieght)>()
                        {
                             (FreightChoice.fr1,calcFreightCost1),
                             (FreightChoice.fr2,calcFreightCost2),
                             (FreightChoice.fr3,calcFreightCost3),
                             (FreightChoice.fr4,calcFreightCost4),
                             (FreightChoice.fr5,calcFreightCost5),
                             (FreightChoice.fr6,calcFreightCost6)
                        };
            }

         }
        public class AvailabilityPath
        {

            public List<(AvailabilityChoice availabilityChoose, Func<Order, Availability> calcAvailability)> AvailabilityFunctions;

            public List<(ShippingDateChoice shippingDateChoose, Func<Availability, ShippingDate> calcShippingDate)> ShippingDateFunctions;

            public AvailabilityPath ()
            {
                AvailabilityFunctions = new List<(AvailabilityChoice availabilityChoose, Func<Order, Availability> calcAvailability)>()
                        {
                                 (AvailabilityChoice.AV1,calcAvailability1),
                                 (AvailabilityChoice.AV2,calcAvailability2),
                                 (AvailabilityChoice.AV3,calcAvailability3),
                                 (AvailabilityChoice.AV4,calcAvailability4)
                        };
                ShippingDateFunctions = new List<(ShippingDateChoice shippingDateChoose, Func<Availability, ShippingDate> calcShippingDate)>()
                        {
                                 (ShippingDateChoice.SD1,calcShippingDate1),
                                 (ShippingDateChoice.SD2,calcShippingDate2),
                                 (ShippingDateChoice.SD3,calcShippingDate3),
                                 (ShippingDateChoice.SD4,calcShippingDate4),
                                 (ShippingDateChoice.SD5,calcShippingDate5),
                        };
            }

        }

        public static Invoice calcInvoice1(Order o)
        {
            Console.WriteLine("Invoice 1");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.1;
            return invoice;
        }
        public static Invoice calcInvoice2(Order o)
        {
            Console.WriteLine("Invoice 2");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.2;
            return invoice;
        }
        public static Invoice calcInvoice3(Order o)
        {
            Console.WriteLine("Invoice 3");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.3;
            return invoice;
        }
        public static Invoice calcInvoice4(Order o)
        {
            Console.WriteLine("Invoice 4");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.4;
            return invoice;

        }
        public static Invoice calcInvoice5(Order o)
        {
            Console.WriteLine("Invoice 5");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.5;
            return invoice;
        }

        public static Shipping calcShipping1(Invoice o)
        {
            Console.WriteLine("Shipping 1");
            Shipping s = new Shipping();
            s.ShipperID = (o.cost > 1000) ? 1 : 2;
            s.cost = o.cost;

            return s;
        }
        public static Shipping calcShipping2(Invoice i)
        {
            Console.WriteLine("Shipping 2");
            Shipping s = new Shipping();

            s.ShipperID = (i.cost > 1100) ? 1 : 2;
            s.cost = i.cost;

            return s;
        }
        public static Shipping calcShipping3(Invoice i)
        {
            Console.WriteLine("Shipping 3");
            Shipping s = new Shipping();
            s.ShipperID = (i.cost > 1200) ? 1 : 2;
            s.cost = i.cost;

            return s;
        }

        public static Freight calcFreightCost1(Shipping s)
        {
            Console.WriteLine("Freight 1");
            Freight f = new Freight();
            f.cost = (s.ShipperID == 1) ? s.cost * 0.25 : s.cost * 0.5;
            return f;
        }
        public static Freight calcFreightCost2(Shipping s)
        {
            Console.WriteLine("Freight 2");
            Freight f = new Freight();
            f.cost = (s.ShipperID == 1) ? s.cost * 0.28 : s.cost * 0.52;
            return f;
        }
        public static Freight calcFreightCost3(Shipping s)
        {
            Console.WriteLine("Freight 3");
            Freight f = new Freight();
            f.cost = (s.ShipperID == 1) ? s.cost * 0.3 : s.cost * 0.6;
            return f;
        }
        public static Freight calcFreightCost4(Shipping s)
        {
            Console.WriteLine("Freight 4");
            Freight f = new Freight();
            f.cost = (s.ShipperID == 1) ? s.cost * 0.35 : s.cost * 0.65;
            return f;
        }
        public static Freight calcFreightCost5(Shipping s)
        {
            Console.WriteLine("Freight 5");
            Freight f = new Freight();
            f.cost = (s.ShipperID == 1) ? s.cost * 0.15 : s.cost * 0.2;
            return f;
        }
        public static Freight calcFreightCost6(Shipping s)
        {
            Console.WriteLine("Freight 6");
            Freight f = new Freight();
            f.cost = (s.ShipperID == 1) ? s.cost * 0.1 : s.cost * 0.15;
            return f;
        }

        public static Availability calcAvailability1(Order o)
        {
            Console.WriteLine("Availability 1");
            Availability a = new Availability();
            a.date = o.date.AddDays(3);
            return a;
        }
        public static Availability calcAvailability2(Order o)
        {
            Console.WriteLine("Availability 2");
            Availability a = new Availability();
            a.date = o.date.AddDays(2);
            return a;
        }
        public static Availability calcAvailability3(Order o)
        {
            Console.WriteLine("Availability 3");
            Availability a = new Availability();
            a.date = o.date.AddDays(1);
            return a;
        }
        public static Availability calcAvailability4(Order o)
        {
            Console.WriteLine("Availability 4");
            Availability a = new Availability();
            a.date = o.date.AddDays(4);
            return a;
        }

        public static ShippingDate calcShippingDate1(Availability o)
        {
            Console.WriteLine("ShippingDate 1");
            ShippingDate a = new ShippingDate();
            a.date = o.date.AddDays(1);
            return a;
        }
        public static ShippingDate calcShippingDate2(Availability o)
        {
            Console.WriteLine("ShippingDate 2");
            ShippingDate a = new ShippingDate();
            a.date = o.date.AddDays(2);
            return a;
        }
        public static ShippingDate calcShippingDate3(Availability o)
        {
            Console.WriteLine("ShippingDate 3");
            ShippingDate a = new ShippingDate();
            a.date = o.date.AddHours(14);
            return a;
        }
        public static ShippingDate calcShippingDate4(Availability o)
        {
            Console.WriteLine("ShippingDate 4");
            ShippingDate a = new ShippingDate();
            a.date = o.date.AddHours(20);
            return a;
        }
        public static ShippingDate calcShippingDate5(Availability o)
        {
            Console.WriteLine("ShippingDate 5");
            ShippingDate a = new ShippingDate();
            a.date = o.date.AddHours(10);
            return a;
        }
        #endregion
 
    }

    //  Classes
    public class ProcessConfiguration
    {
        public InvoiceChoice invoiceChoice { get; set; }
        public ShippingChoice shippingChoice { get; set; }
        public FreightChoice freightChoice { get; set; }
        public AvailabilityChoice availabilityChoice { get; set; }
        public ShippingDateChoice shippingDateChoice { get; set; }
     
    }

    public class Customer
    {
      
    }
    public class Order
    {
        public Customer customer { get; set; }
        public DateTime date { get; set; }
        public double cost { get; set; }
      
    }
    public class Invoice
    {

        public double cost { get; set; }
        public Invoice()
        {
            cost = 0;
        }
    }
    public class Shipping
    {

        public double cost { get; set; }
        public int ShipperID { get; set; }
        public Shipping()
        {
            cost = 0;
        }
    }
    public class Freight
    {
        public double cost { get; set; }

        public Freight()
        {
            cost = 0;
        }
    }
    public class Availability
    {
        public DateTime date { get; set; }
        public Availability()
        {

        }
    }
    public class ShippingDate
    {
        public DateTime date { get; set; }

        public ShippingDate()
        {
        }
    }

    public enum InvoiceChoice
    {
        Inv1 = 0,
        Inv2 = 1,
        Inv3 = 2,
        Inv4 = 3,
        Inv5 = 4
    }
    public enum ShippingChoice
    {
        Sh1,
        Sh2,
        Sh3,
    }
    public enum FreightChoice
    {
        fr1,
        fr2,
        fr3,
        fr4,
        fr5,
        fr6
    }
    public enum AvailabilityChoice
    {
        AV1,
        AV2,
        AV3,
        AV4
    }
    public enum ShippingDateChoice
    {
        SD1,
        SD2,
        SD3,
        SD4,
        SD5
    }

}