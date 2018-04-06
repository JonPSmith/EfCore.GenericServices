// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Dtos;
using GenericServices;

namespace DataLayer.EfClasses
{
    public class Order
    {
        private HashSet<LineItem> _lineItems;


        private Order() { }

        public int OrderId { get; private set; }
        public DateTime DateOrderedUtc { get; private set; }
        public DateTime ExpectedDeliveryDate { get; private set; }
        public bool HasBeenDelivered { get; private set; }
        public string CustomerName { get; private set; }

        // relationships

        public IEnumerable<LineItem> LineItems => _lineItems?.ToList();

        public string OrderNumber => $"SO{OrderId:D6}";

        public static IStatusGeneric<Order> CreateOrder(
            string customerName, DateTime expectedDeliveryDate,
            IEnumerable<OrderBooksDto> bookOrders)
        {
            var status = new StatusGenericHandler<Order>();
            var order = new Order
            {
                CustomerName = customerName,
                ExpectedDeliveryDate = expectedDeliveryDate,
                DateOrderedUtc = DateTime.UtcNow,
                HasBeenDelivered = expectedDeliveryDate < DateTime.Today
            };

            byte lineNum = 1;
            order._lineItems = new HashSet<LineItem>(bookOrders
                .Select(x => new LineItem(x.numBooks, x.ChosenBook, lineNum++)));
            if (!order._lineItems.Any())
                status.AddError("No items in your basket.");

            return status.SetResult(order); //don't worry, the Result will return default(T) if there are errors
        }

        public IStatusGeneric ChangeDeliveryDate(string userId, DateTime newDeliveryDate)
        {
            var status = new StatusGenericHandler();
            if (CustomerName != userId)
            {
                status.AddError("I'm sorry, but that order does not belong to you");
                //Log a security issue
                return status;
            }

            if (HasBeenDelivered)
            {
                status.AddError("I'm sorry, but that order has been delivered.");
                return status;
            }

            if (newDeliveryDate < DateTime.Today.AddDays(1))
            {
                status.AddError("I'm sorry, we cannot get the order to you that quickly. Please choose a new date.", "NewDeliveryDate");
                return status;
            }

            if (newDeliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                status.AddError("I'm sorry, we don't deliver on Sunday. Please choose a new date.", "NewDeliveryDate");
                return status;
            }

            //All Ok
            ExpectedDeliveryDate = newDeliveryDate;
            return status;
        }
    }
}