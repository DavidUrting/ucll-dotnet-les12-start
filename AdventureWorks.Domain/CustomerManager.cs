using AdventureWorks.Domain.Models;
using Aspose.Words;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventureWorks.Domain
{
    public class CustomerManager: ICustomerManager
    {
        private readonly string _connString;

        public CustomerManager(string connString)
        {
            _connString = connString;
        }

        public IList<Customer> SearchCustomers(string keyword)
        {
            keyword = keyword.ToUpper();

            List<Customer> customers = new List<Customer>();
            using (var dbContext = new Entities.AdventureWorks2017Context(_connString))
            {
                IEnumerable<Entities.Customer> entities = dbContext.Customer
                    .Include(c => c.Person)
                    .Include(c => c.Person.EmailAddress)
                    .Where(c => (c.Person.FirstName.ToUpper().Contains(keyword)
                                ||
                                c.Person.LastName.ToUpper().Contains(keyword)) && c.Person != null);
                foreach (var entity in entities)
                {
                    customers.Add(new Customer()
                    {
                        Id = entity.CustomerId,
                        FirstName = entity.Person.FirstName,
                        LastName = entity.Person.LastName,
                        Email = entity.Person.EmailAddress
                            .Select(ea => ea.EmailAddress1)
                            .FirstOrDefault()
                    });
                }
            }
            return customers;
        }

        public Customer GetCustomer(int id)
        {
            using (var dbContext = new Entities.AdventureWorks2017Context(_connString))
            {
                Entities.Customer customer = dbContext.Customer
                    .Include(c => c.Person)
                    .Include(c => c.Person.EmailAddress)
                    .Where(c => c.CustomerId == id && c.Person != null)
                    .FirstOrDefault();
                if (customer != null)
                {
                    return new Customer()
                    {
                        Id = customer.CustomerId,
                        FirstName = customer.Person.FirstName,
                        LastName = customer.Person.LastName,
                        Email = customer.Person.EmailAddress
                            .Select(ea => ea.EmailAddress1)
                            .FirstOrDefault()
                    };
                }

            }
            return null;
        }

        public Customer InsertCustomer(Customer customer)
        {
            using (var dbContext = new Entities.AdventureWorks2017Context(_connString))
            {
                // Omvormen 'domein' object naar 'entity' object(en).
                // Een customer aanmaken komt neer op het opvullen van 3 tabellen!
                Entities.Customer customerEntity = new Entities.Customer();
                customerEntity.Person = new Entities.Person()
                {
                    PersonType = "IN", // https://www.sqldatadictionary.com/AdventureWorks2014/Person.Person.html
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };
                customerEntity.Person.BusinessEntity = new Entities.BusinessEntity()
                {
                };
                customerEntity.Person.EmailAddress.Add(new Entities.EmailAddress()
                {
                    EmailAddress1 = customer.Email
                });

                // Toevoegen aan DbContext
                dbContext.Add(customerEntity);

                // Wegschrijven naar database
                dbContext.SaveChanges();

                // Toegekende ID (door database) in domein object kopiëren.
                customer.Id = customerEntity.CustomerId;
                return customer;
            }
        }

        public Customer UpdateCustomer(Customer customer)
        {
            using (var dbContext = new Entities.AdventureWorks2017Context(_connString))
            {
                var customerEntity = dbContext.Customer
                        .Include(c => c.Person)
                        .Include(c => c.Person.EmailAddress)
                        .Where(c => c.CustomerId == customer.Id)
                        .FirstOrDefault();

                customerEntity.Person.FirstName = customer.FirstName;
                customerEntity.Person.LastName = customer.LastName;
                customerEntity.Person.EmailAddress.First().EmailAddress1 = customer.Email;

                dbContext.SaveChanges();

                return customer;
            }
        }

        public void DeleteCustomer(int id)
        {
            using (var dbContext = new Entities.AdventureWorks2017Context(_connString))
            {
                var customerEntity = dbContext.Customer
                        .Include(c => c.Person)
                        .Include(c => c.Person.EmailAddress)
                        .Where(c => c.CustomerId == id)
                        .FirstOrDefault();

                foreach (Entities.EmailAddress ea in customerEntity.Person.EmailAddress)
                {
                    dbContext.EmailAddress.Remove(ea);
                }
                dbContext.Person.Remove(customerEntity.Person);
                dbContext.Customer.Remove(customerEntity);

                dbContext.SaveChanges();
            }
        }

        public string GenerateAllCustomersReport()
        {
            string reportPath = Path.GetTempFileName();

            var customers = this.SearchCustomers(string.Empty);

            Document doc = new Document();
            DocumentBuilder docBuilder = new DocumentBuilder(doc);
            Paragraph p = docBuilder.InsertParagraph();
            p.Runs.Add(new Run(doc, "AdventureWorks Customers"));
            docBuilder.StartTable();
            docBuilder.InsertCell();
            docBuilder.Write("ID");
            docBuilder.InsertCell();
            docBuilder.Write("Last name");
            docBuilder.InsertCell();
            docBuilder.Write("First name");
            docBuilder.EndRow();
            foreach (var customer in customers)
            {
                docBuilder.InsertCell();
                docBuilder.Write(customer.Id.ToString());
                docBuilder.InsertCell();
                docBuilder.Write(customer.LastName);
                docBuilder.InsertCell();
                docBuilder.Write(customer.FirstName);
                docBuilder.EndRow();
            }
            docBuilder.EndTable();
            doc.Save(reportPath, SaveFormat.Pdf);

            return reportPath;
        }
    }
}
