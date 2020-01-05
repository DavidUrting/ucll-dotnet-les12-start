using AdventureWorks.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AdventureWorks.Domain.Test
{
    [TestClass]
    public class CustomerManagerTests
    {
        const string CONN_STRING = "Data Source=localhost;Initial Catalog=AdventureWorks2017;Integrated Security=True";

        [TestMethod]
        public void TestGetCustomer()
        {
            // ARRANGE
            CustomerManager cm = new CustomerManager(CONN_STRING);

            // ACT
            Customer c = cm.GetCustomer(29546);

            // ASSERT
            Assert.IsNotNull(c);
            Assert.AreEqual("Christopher", c.FirstName);
            Assert.AreEqual("Beck", c.LastName);
        }

        [TestMethod]
        public void TestSearchCustomers()
        {
            // ARRANGE
            CustomerManager cm = new CustomerManager(CONN_STRING);

            // ACT
            IList<Customer> customers = cm.SearchCustomers("Lucy");

            // ASSERT
            Assert.IsNotNull(customers);
            Assert.IsTrue(customers.Count > 0);
        }

        [TestMethod]
        public void TestInsertCustomer()
        {
            // ARRANGE
            CustomerManager cm = new CustomerManager(CONN_STRING);
            Customer customerToInsert = new Customer()
            { 
                FirstName = "Test",
                LastName = Guid.NewGuid().ToString(),
                Email = "test@test.com"
            };

            // ACT
            Customer insertedCustomer = cm.InsertCustomer(customerToInsert);

            // ASSERT
            Assert.IsNotNull(insertedCustomer);
            Assert.IsTrue(insertedCustomer.Id >= 0);
            Customer retrievedCustomer = cm.GetCustomer(insertedCustomer.Id);
            Assert.AreEqual(customerToInsert.LastName, retrievedCustomer.LastName);
        }

        [TestMethod]
        public void TestUpdateCustomer()
        {
            // ARRANGE
            CustomerManager cm = new CustomerManager(CONN_STRING);
            Customer customerToInsert = new Customer()
            {
                FirstName = "Test",
                LastName = Guid.NewGuid().ToString(),
                Email = "test@test.com"
            };
            Customer insertedCustomer = cm.InsertCustomer(customerToInsert);

            // ACT
            Customer customerToUpdate = cm.GetCustomer(insertedCustomer.Id);
            customerToUpdate.Email = "test-UPDATED@test.com";
            Customer updatedCustomer = cm.UpdateCustomer(customerToUpdate);

            // ASSERT
            Assert.IsNotNull(updatedCustomer);
            Assert.AreEqual(customerToUpdate.Id, updatedCustomer.Id);
            Assert.AreEqual("test-UPDATED@test.com", updatedCustomer.Email);
            Customer retrievedCustomer = cm.GetCustomer(updatedCustomer.Id);
            Assert.AreEqual("test-UPDATED@test.com", retrievedCustomer.Email);
        }

        [TestMethod]
        public void TestDeleteCustomer()
        {
            // ARRANGE
            CustomerManager cm = new CustomerManager(CONN_STRING);
            Customer customerToInsert = new Customer()
            {
                FirstName = "Test",
                LastName = Guid.NewGuid().ToString(),
                Email = "test@test.com"
            };
            Customer insertedCustomer = cm.InsertCustomer(customerToInsert);

            // ACT
            cm.DeleteCustomer(insertedCustomer.Id);

            // ASSERT
            Customer retrievedCustomer = cm.GetCustomer(insertedCustomer.Id);
            Assert.IsNull(retrievedCustomer);
        }

        [ClassCleanup]
        public static void CleanTestData()
        {
            CustomerManager cm = new CustomerManager(CONN_STRING);
            foreach (Customer testCustomer in cm.SearchCustomers("Test"))
            {
                cm.DeleteCustomer(testCustomer.Id);
            }
        }
    }
}
