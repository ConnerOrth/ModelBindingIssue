using ModelBindingIssue.Entities;
using ModelBindingIssue.Factories;
using ModelBindingIssue.Models;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void ChildVM_To_BaseEntity()
        {
            var childvm = new ChildVM(new Child()
            {
                Name = "__child",
                Hobby = "__hobby",
                Age = 8,
            });

            var withMappingVM = new BackendClassContainingMappingVM(new WithMapping(new List<Mapping>() { Mapping.Create("key123", "value123") })
            {
                Name = "__backend",
                Hobby = "__mapping",
                Age = 8,
            });

            Child child = childvm.Map();
            Parent parent = childvm.Map();

            WithMapping backend = withMappingVM.Map();
            Child backendChild = withMappingVM.Map();
            Assert.True(backend.Mappings.Count == 1);
            Assert.True(backend.Mappings[0].Key == "key123");
        }

        [Fact]
        public void ChildVM_To_BaseEntity_WithRedirectViaToDialogItem()
        {
            var childvm = new ChildVM(new Child()
            {
                Name = "__child",
                Hobby = "__hobby",
                Age = 8,
            });

            var withMappingVM = new BackendClassContainingMappingVM(new WithMapping(new List<Mapping>() { Mapping.Create("key123", "value123") })
            {
                Name = "__backend",
                Hobby = "__mapping",
                Age = 8,
            });

            BaseDialogItem child = childvm.ToDialogItem();
            Assert.IsType<Child>(child);

            Parent parent = childvm.Map();

            WithMapping backend = withMappingVM.Map();
            Child backendChild = withMappingVM.Map();
            Assert.True(backend.Mappings.Count == 1);
            Assert.True(backend.Mappings[0].Key == "key123");
        }

        private static readonly IList<HemaStatusMap> statusMaps = new List<HemaStatusMap>()
        {
            new HemaStatusMap("Verzonden",new []{"ARRIVED_AT_DELIVERY_FACILITY","CUSTOMS","DELIVERY_PLANNED_IN_ROUTE","DEPART_FACILITY","DEPOT_SCAN","EXCEPTION","GATEWAY_DEPARTED","IN_DELIVERY","INFORMATION_ON_DELIVERY_TRANSMITTED","NEW_DELIVERY_ATTEMPT","OUT_FOR_DELIVERY","PARCEL_ARRIVED_AT_LOCAL_DEPOT","PARCEL_SORTED_AT_HUB","ROUTE_IN_SCAN","SCAN_OK_GATEWAY","TRANSPORT_DELAY","UNKNOWN","AWAITING_RECEIVER_COLLECTION#Thuisbezorgd","UNDERWAY#Thuisbezorgd"}),
            new HemaStatusMap("Levering Mislukt",new []{"CLOSED_SHIPMENT","DELIVERED_AT_SHIPPER","DESTROYED","PARCEL_READY_FOR_RETURN_TO_HUB","PARCEL_SCANNED_AT_RETURN_HUB","SHIPMENT_STOPPED","RETURNED_TO_SHIPPER#Afhaalpunt","REFUSED_DAMAGED#Thuisbezorgd","RETURNED_TO_SHIPPER#Thuisbezorgd"}),
            new HemaStatusMap("Geleverd aan klant",new []{"COLLECTED_AT_PARCELSTATION","COLLECTED_AT_ACCESSPOINT#Afhaalpunt","COLLECTED_AT_ACCESSPOINT#Thuisbezorgd","COLLECTED_AT_PARCELSHOP#Thuisbezorgd","DELIVERED#Thuisbezorgd","DELIVERED_AT_NEIGHBOURS#Thuisbezorgd","DELIVERED_AT_PREFERED_NEIGHBOURS#Thuisbezorgd","DELIVERED_DAMAGED#Thuisbezorgd","DELIVERED_NOT_IN_TIME#Thuisbezorgd","PARTIAL_DELIVERY#Thuisbezorgd"}),
            new HemaStatusMap("Voorgemeld aan carrier",new []{"INTERVENTION","LEG","LEG_CREATED","PROBLEM","DATA_RECEIVED#Ophalen filiaal","DATA_RECEIVED#Thuisbezorgd"}),
            new HemaStatusMap("Geleverd aan service point",new []{"AWAITING_RECEIVER_COLLECTION#Afhaalpunt","DELIVERED_AT_PARCELSHOP#Afhaalpunt"}),
            new HemaStatusMap("In ontvangst genomen door klant",new []{"COLLECTED_AT_PARCELSHOP#Afhaalpunt"}),
            new HemaStatusMap("Geleverd aan winkel",new []{"DELIVERED#Ophalen filiaal","DELIVERED_DAMAGED#Ophalen filiaal","DELIVERED_NOT_IN_TIME#Ophalen filiaal"}),
            new HemaStatusMap("Ingepakt en klaar voor verzending",new []{"UNDERWAY#Ophalen filiaal"})
        };

        [Fact]
        public void Test1()
        {
            var hemaDialogItem = new HemaDialogItem() { Name = "HemaItem", ResponseNoPackages = "No packages found.", HemaStatuses = statusMaps };

            var model = new BaseDialogItemViewModel();
            var model2 = DialogItemViewModelFactory.GetViewModel(hemaDialogItem);
            //var model2 = new BaseDialogItemViewModel();
            //var model3 = new HemaDialogItemViewModel(new HemaDialogItem() { Name = "HemaItem", ResponseNoPackages = "No packages found.", HemaStatuses = statusMaps });

            var item = model.ToDialogItem();
            var item2 = model2.ToDialogItem();
            //var item3 = model3.ToDialogItem(nameof(HemaDialogItem));
        }
    }
}
