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
        public static List<(InvoiceChoose InvoiceChoose, Func<Order, Invoice> CalcInvoice)> InvoiceFunctions = new List<(InvoiceChoose InvoiceChoose, Func<Order, Invoice> CalcInvoice)>()
            {
             (InvoiceChoose.Inv1,calcInvoice1),
             (InvoiceChoose.Inv2,calcInvoice2),
             (InvoiceChoose.Inv3,calcInvoice3),
             (InvoiceChoose.Inv4,calcInvoice4),
             (InvoiceChoose.Inv5,calcInvoice5)
            };

        public static List<(ShppingChoose ShippingChoose, Func<Invoice, Shipping> calcShipping)> ShippingFunctions = new List<(ShppingChoose ShippingChoose, Func<Invoice, Shipping> calcShipping)>()
            {
             (ShppingChoose.Sh1,calcShipping1),
             (ShppingChoose.Sh2,calcShipping2),
             (ShppingChoose.Sh3,calcShipping3)
            };

        public static List<(FreightChoose freightChoose, Func<Shipping, Freight> calcFrieght)> frieghtFunctions = new List<(FreightChoose freightChoose, Func<Shipping, Freight> calcFrieght)>()
            {
             (FreightChoose.fr1,calcFreightCost1),
             (FreightChoose.fr2,calcFreightCost2),
             (FreightChoose.fr3,calcFreightCost3),
             (FreightChoose.fr4,calcFreightCost4),
             (FreightChoose.fr5,calcFreightCost5),
             (FreightChoose.fr6,calcFreightCost6)
            };

        public static List<(AvailabilirtyChoose availabilityChoose, Func<Order, Availability> calcAvailability)> AvailabilityFunctions = new List<(AvailabilirtyChoose availabilityChoose, Func<Order, Availability> calcAvailability)>()
            {
             (AvailabilirtyChoose.AV1,calcAvailability1),
             (AvailabilirtyChoose.AV2,calcAvailability2),
             (AvailabilirtyChoose.AV3,calcAvailability3),
             (AvailabilirtyChoose.AV4,calcAvailability4)
            };

        public static List<(ShippingDateChoose shippingDateChoose, Func<Availability, ShippingDate> calcShippingDate)> ShippingDateFunctions = new List<(ShippingDateChoose shippingDateChoose, Func<Availability, ShippingDate> calcShippingDate)>()
            {
             (ShippingDateChoose.SD1,calcShippingDate1),
             (ShippingDateChoose.SD2,calcShippingDate2),
             (ShippingDateChoose.SD3,calcShippingDate3),
             (ShippingDateChoose.SD4,calcShippingDate4),
             (ShippingDateChoose.SD5,calcShippingDate5),

            };


        public static void Main(string[] args)
        {
            
            CustomerConfiguration customerConfiguration = new CustomerConfiguration();
            Customer customer = new Customer();
            Order ord = new Order();

            customerConfiguration.invoiceChoose = InvoiceChoose.Inv1;
            customerConfiguration.shppingChoose = ShppingChoose.Sh1;
            customerConfiguration.freightChoose = FreightChoose.fr1;
            customerConfiguration.availabilirtyChoose = AvailabilirtyChoose.AV1;
            customerConfiguration.shippingDateChoose = ShippingDateChoose.SD1;
            customer.customerConfiguration = customerConfiguration;
            ord.customer = customer;
            ord.cost = 2000;
            Console.WriteLine(ord.date.DayOfWeek);
             Console.WriteLine(CalcAdjustedCost(customerConfiguration)(ord));
 


        }


        public static Func<Order, double> CalcAdjustedCost(CustomerConfiguration c)
        {

            return (x) => AdjustCost(x, FirstComposeFunc(c), SecondComposeFunc(c));

        }


                                                  //Adjusted Cost//
        public static double AdjustCost(Order r, Func<Order, Freight> calcFreigt, Func<Order, ShippingDate> calcShippingDate)
        {
            Freight f= calcFreigt(r);
            ShippingDate  s= calcShippingDate(r);

            Console.WriteLine(s.date.DayOfWeek);

            double cost= (s.date.DayOfWeek.ToString()== "Monday") ? f.cost+1000 : f.cost+500;
       
           
            ///Finall Cost 
            return cost;
        }


                                           //// Second PipLine ////


        /// ///  Return Second Compose Funcrtion function
        public static Func<Order, ShippingDate> SecondComposeFunc(CustomerConfiguration c)
        {

            /// Availability Functions 

            Func<Order, Availability> dAvailability1 = calcAvailability1;
            Func<Order, Availability> dAvailability2 = calcAvailability2;
            Func<Order, Availability> dAvailability3 = calcAvailability3;
            Func<Order, Availability> dAvailability4 = calcAvailability4;

            // ShippingDate Functions 

            Func<Availability, ShippingDate> dShippingDate1 = calcShippingDate1;
            Func<Availability, ShippingDate> dShippingDate2 = calcShippingDate2;
            Func<Availability, ShippingDate> dShippingDate3 = calcShippingDate3;
            Func<Availability, ShippingDate> dShippingDate4 = calcShippingDate4;
            Func<Availability, ShippingDate> dShippingDate5 = calcShippingDate5;

            Func<Order, ShippingDate> p = AvailabilityFunctions.Where((x) => x.availabilityChoose ==c.availabilirtyChoose).Select((x) => x.calcAvailability).FirstOrDefault()
                                  .Compose(ShippingDateFunctions.Where((x) => x.shippingDateChoose == c.shippingDateChoose).Select((x) => x.calcShippingDate).FirstOrDefault());

            return p;
        }

        /// ShippingDate functions
        public static ShippingDate calcShippingDate1(Availability o)
        {
            Console.WriteLine("ShippingDate1");
            ShippingDate a = new ShippingDate();
            a.date = o.date.AddDays(1);
            return a;
        }
        public static ShippingDate calcShippingDate2(Availability o)
        {
            Console.WriteLine("ShippingDate2");
            ShippingDate a = new ShippingDate();
            
            return a;
        }
        public static ShippingDate calcShippingDate3(Availability o)
        {
            Console.WriteLine("ShippingDate3");
            ShippingDate a = new ShippingDate();
            return a;
        }
        public static ShippingDate calcShippingDate4(Availability o)
        {
            Console.WriteLine("ShippingDate4");
            ShippingDate a = new ShippingDate();
            return a;
        }
        public static ShippingDate calcShippingDate5(Availability o)
        {
            Console.WriteLine("ShippingDate5");
            ShippingDate a = new ShippingDate();
            return a;
        }


        /// Availability Functions
        public static Availability calcAvailability1(Order o)
        {
            Console.WriteLine("Availability1");
            Availability a = new Availability();
            a.date = o.date.AddDays(3);
            return a;
        }
        public static Availability calcAvailability2(Order o)
        {
            Console.WriteLine("Availability2");
            Availability a = new Availability();
            a.date = o.date.AddDays(3);
            return a;
        }
        public static Availability calcAvailability3(Order o)
        {
            Console.WriteLine("Availability3");
            Availability a = new Availability();
            return a;
        }
        public static Availability calcAvailability4(Order o)
        {
            Console.WriteLine("Availability4");
            Availability a = new Availability();
            return a;
        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //// First PipLine ////

        /// Return First Compose Function
        public static Func<Order, Freight> FirstComposeFunc(CustomerConfiguration c)
        {
            //// Invoice Functions 
            Func<Order, Invoice> dInv1 = calcInvoice1;
            Func<Order, Invoice> dInv2 = calcInvoice2;
            Func<Order, Invoice> dInv3 = calcInvoice3;
            Func<Order, Invoice> dInv4 = calcInvoice4;
            Func<Order, Invoice> dInv5 = calcInvoice5;

            //// Shipping Function
            Func<Invoice, Shipping> dShipping1 = calcShipping1;
            Func<Invoice, Shipping> dShipping2 = calcShipping2;
            Func<Invoice, Shipping> dShipping3 = calcShipping3;

            /// Freight Functions 
            Func<Shipping, Freight> dFreight1 = calcFreightCost1;
            Func<Shipping, Freight> dFreight2 = calcFreightCost2;
            Func<Shipping, Freight> dFreight3 = calcFreightCost3;
            Func<Shipping, Freight> dFreight4 = calcFreightCost4;
            Func<Shipping, Freight> dFreight5 = calcFreightCost5;
            Func<Shipping, Freight> dFreight6 = calcFreightCost6;


            Func<Order, Freight> p = InvoiceFunctions.Where((x) => x.InvoiceChoose == c.invoiceChoose).Select((x) => x.CalcInvoice).FirstOrDefault()
                           .Compose(ShippingFunctions.Where((x) => x.ShippingChoose == c.shppingChoose).Select((x) => x.calcShipping).FirstOrDefault())
                           .Compose(frieghtFunctions.Where((x) => x.freightChoose == c.freightChoose).Select((x) => x.calcFrieght).FirstOrDefault());


            return p;
        }




        // Invoicing Functions 
        public static Invoice calcInvoice1(Order o)
        {
            Console.WriteLine("Invoice1");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.1 ;
            return invoice;
        }
        public static Invoice calcInvoice2(Order o)
        {
            Console.WriteLine("Invoice2");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.2;
            return invoice;
        }
        public static Invoice calcInvoice3(Order o)
        {
            Console.WriteLine("Invoice3");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.3;
            return invoice;

            return invoice;
        }
        public static Invoice calcInvoice4(Order o)
        {
            Console.WriteLine("Invoice4");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.4;
            return invoice;

            return invoice;
        }
        public static Invoice calcInvoice5(Order o)
        {
            Console.WriteLine("Invoice5");
            Invoice invoice = new Invoice();
            invoice.cost = o.cost * 1.5;
            return invoice;
  
        }


        // Shippping Functions 
        public static Shipping calcShipping1(Invoice o)
        {
            Console.WriteLine("Shipping1");
            Shipping s = new Shipping();
            s.ShipperID= (o.cost > 1000) ? 1 : 2;
            s.cost = o.cost;
         
            return s;
        }

        public static Shipping calcShipping2(Invoice i)
        {
            Console.WriteLine("Shipping2");
            Shipping s = new Shipping();
            
            s.ShipperID = (i.cost > 1000) ? 1 : 2;
            s.cost = i.cost;
            return s;
        }

        public static Shipping calcShipping3(Invoice i)
        {
            Console.WriteLine("Shipping 3");
            Shipping s = new Shipping();
            s.ShipperID = (i.cost > 1000) ? 1 : 2;
            s.cost = i.cost;
      
            return s;
        }


        // Freight functions 
        public static Freight calcFreightCost1(Shipping s)
        {
            Console.WriteLine("Freight1");
            Freight f = new Freight();
            f.cost= (s.ShipperID==1 ) ? s.cost*0.25 : s.cost * 0.5;
  
            return f;
        }
        public static Freight calcFreightCost2(Shipping s)
        {
            Console.WriteLine("Freight2");
            Freight f = new Freight();
            f.cost = 2000;
            return f;
        }
        public static Freight calcFreightCost3(Shipping s)
        {
            Console.WriteLine("Freight3");
            Freight f = new Freight();
            f.cost = 1000;
            return f;
        }
        public static Freight calcFreightCost4(Shipping s)
        {
            Console.WriteLine("Freight4");
            Freight f = new Freight();
            f.cost = 4000;
            return f;
        }
        public static Freight calcFreightCost5(Shipping s)
        {
            Console.WriteLine("Freight5");
            Freight f = new Freight();
            f.cost = 2000;
            return f;
        }
        public static Freight calcFreightCost6(Shipping s)
        {
            Console.WriteLine("Freight6");
            Freight f = new Freight();
            f.cost = 1000;
            return f;
        }


        public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T1, T2> f, Func<T2, T3> g)
        {
            return (x) => g(f(x));
        }


    }


    //  Classes

    /// Enumeration
    public enum InvoiceChoose
    {
        Inv1 = 0,
        Inv2 = 1,
        Inv3 = 2,
        Inv4 = 3,
        Inv5 = 4
    }
    public enum ShppingChoose
    {
        Sh1,
        Sh2,
        Sh3,
    }
    public enum FreightChoose
    {
        fr1,
        fr2,
        fr3,
        fr4,
        fr5,
        fr6
    }
    public enum AvailabilirtyChoose
    {
        AV1,
        AV2,
        AV3,
        AV4
    }
    public enum ShippingDateChoose
    {
        SD1,
        SD2,
        SD3,
        SD4,
        SD5
    }


    public class CustomerConfiguration
    {
        public InvoiceChoose invoiceChoose { get; set; }
        public ShppingChoose shppingChoose { get; set; }
        public FreightChoose freightChoose { get; set; }
        public AvailabilirtyChoose availabilirtyChoose { get; set; }
        public ShippingDateChoose shippingDateChoose { get; set; }
  
    }
    public class Customer
    {

        public CustomerConfiguration customerConfiguration { get; set; }
    }
    public class Order
    {
        public Customer customer { get; set; }
        public DateTime date { get; set; }
        public double cost { get; set; }
        public Order()
        {
            date = new DateTime(2021, 3,11);
      
        }
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
  
    }
    public class ShippingDate
    {
        public DateTime date { get; set; }

        public ShippingDate()
        {
        }
    }

}