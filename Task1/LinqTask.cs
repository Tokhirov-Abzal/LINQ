using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers == null) throw new ArgumentNullException();
            return customers.Where(customer => customer.Orders.Sum(order => order.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers == null || suppliers == null)
            {
                throw new ArgumentNullException();
            }

            return customers
            .Select(customer => (
                customer,
                suppliers.Where(supplier => supplier.City == customer.City)
            ));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers == null || suppliers == null)
            {
                throw new ArgumentNullException();
            }


            return customers.GroupJoin(suppliers,
            customer => customer.City,
            supplier => supplier.City,
            (customer, supplierGroup) => (customer, supplierGroup));
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers == null)
            {
                throw new ArgumentNullException();
            }


            return customers.Where(customer => customer?.Orders != null &&
                                                                   customer.Orders.Any(order => order.Total > limit));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null) throw new ArgumentNullException();
            return customers
           .Select(customer => (
               customer,
               dateOfEntry: FindCustomerOrdersMinDate(customer)
           ))
           .Where(customer => customer.dateOfEntry != DateTime.MaxValue);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null) { throw new ArgumentNullException(); }


            return customers
                .Where(c => c.Orders.Any())
                .Select(c => (customer: c, dateOfEntry: FindCustomerOrdersMinDate(c)))
                .OrderBy(c => c.dateOfEntry.Year)
                .ThenBy(c => c.dateOfEntry.Month)
                .ThenByDescending(c => c.customer.Orders.Sum(o => o.Total))
                .ThenBy(c => c.customer.CompanyName);

        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            if (customers == null) { throw new ArgumentNullException(); }

            return customers
            .Where(customer =>
                !IsDigitPostalCode(customer.PostalCode) ||
                string.IsNullOrEmpty(customer.Region) ||
                !HasOperatorCodeInPhone(customer.Phone)
            );
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */

            if (products == null) throw new ArgumentNullException();

            return products
           .GroupBy(product => product.Category)
           .Select(categoryGroup => new Linq7CategoryGroup
           {
               Category = categoryGroup.Key,
               UnitsInStockGroup = categoryGroup
                   .GroupBy(product => product.UnitsInStock)
                   .OrderByDescending(availGroup => availGroup.Key)
                   .Select(availGroup => new Linq7UnitsInStockGroup
                   {
                       UnitsInStock = availGroup.Key,
                       Prices = availGroup.OrderBy(product => product.UnitPrice).Select(product => product.UnitPrice)
                   })
           });

        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            if (products == null)
                throw new ArgumentNullException();

            return new[]
             {
                (cheap, products.Where(p => p.UnitPrice <= cheap)),
                (middle, products.Where(p => p.UnitPrice > cheap && p.UnitPrice <= middle)),
                (expensive, products.Where(p => p.UnitPrice > middle && p.UnitPrice <= expensive))
             };

        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {

            if (customers == null)
                throw new ArgumentNullException();
            return customers
               .GroupBy(c => c.City)
               .Select(group => (
                   city: group.Key,
                   averageIncome: (int)Math.Round(group
                        .Where(c => c.Orders != null)
                        .Select(c => c.Orders.Sum(o => o.Total))
                        .Average()),
                   averageIntensity: (int)Math.Round(group
                        .Where(c => c.Orders != null)
                        .Average(c => c.Orders.Length))))
               .ToList();
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            if (suppliers == null)
                throw new ArgumentNullException();

            return string.Concat(suppliers
               .Select(s => s.Country)
               .Distinct()
               .OrderBy(s => s.Length)
               .ThenBy(s => s));
        }

        private static DateTime FindCustomerOrdersMinDate(Customer customer)
        {
            var min = DateTime.MaxValue;
            foreach (var order in customer.Orders)
            {
                if (order.OrderDate < min)
                {
                    min = order.OrderDate;
                }
            }

            return min;
        }

        private static bool IsDigitPostalCode(string postalCode)
        {
            return postalCode != null && postalCode.All(char.IsDigit);
        }

        private static bool HasOperatorCodeInPhone(string phone)
        {
            return phone != null && phone.Contains("(") && phone.Contains(")");
        }


    }
}