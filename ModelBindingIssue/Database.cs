using ModelBindingIssue.Entities;
using System;
using System.Collections.Generic;

namespace ModelBindingIssue
{
    public static class Database
    {
        /// <summary>
        /// Creates a Guid based on a index, guid will have the following format:
        /// 12345678-1234-1234-yyyy-xxxxxxxxxxxx
        /// where y will be replaced with the sequenceId
        /// where x will be replaced with the index
        /// both y and x will have zero's as padding
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Guid CreateGuid(int index, int sequenceId = 1234)
        {
            return Guid.Parse($"12345678-1234-1234-{sequenceId.ToString().PadLeft(4, '0')}-{index.ToString().PadLeft(12, '0')}");
        }

        public static IList<HemaStatusMap> StatusMaps = new List<HemaStatusMap>()
        {
            //new HemaStatusMap("Verzonden",new []{"ARRIVED_AT_DELIVERY_FACILITY","CUSTOMS","DELIVERY_PLANNED_IN_ROUTE","DEPART_FACILITY","DEPOT_SCAN","EXCEPTION","GATEWAY_DEPARTED","IN_DELIVERY","INFORMATION_ON_DELIVERY_TRANSMITTED","NEW_DELIVERY_ATTEMPT","OUT_FOR_DELIVERY","PARCEL_ARRIVED_AT_LOCAL_DEPOT","PARCEL_SORTED_AT_HUB","ROUTE_IN_SCAN","SCAN_OK_GATEWAY","TRANSPORT_DELAY","UNKNOWN","AWAITING_RECEIVER_COLLECTION#Thuisbezorgd","UNDERWAY#Thuisbezorgd"}),
            //new HemaStatusMap("Levering Mislukt",new []{"CLOSED_SHIPMENT","DELIVERED_AT_SHIPPER","DESTROYED","PARCEL_READY_FOR_RETURN_TO_HUB","PARCEL_SCANNED_AT_RETURN_HUB","SHIPMENT_STOPPED","RETURNED_TO_SHIPPER#Afhaalpunt","REFUSED_DAMAGED#Thuisbezorgd","RETURNED_TO_SHIPPER#Thuisbezorgd"}),
            //new HemaStatusMap("Geleverd aan klant",new []{"COLLECTED_AT_PARCELSTATION","COLLECTED_AT_ACCESSPOINT#Afhaalpunt","COLLECTED_AT_ACCESSPOINT#Thuisbezorgd","COLLECTED_AT_PARCELSHOP#Thuisbezorgd","DELIVERED#Thuisbezorgd","DELIVERED_AT_NEIGHBOURS#Thuisbezorgd","DELIVERED_AT_PREFERED_NEIGHBOURS#Thuisbezorgd","DELIVERED_DAMAGED#Thuisbezorgd","DELIVERED_NOT_IN_TIME#Thuisbezorgd","PARTIAL_DELIVERY#Thuisbezorgd"}),
            //new HemaStatusMap("Voorgemeld aan carrier",new []{"INTERVENTION","LEG","LEG_CREATED","PROBLEM","DATA_RECEIVED#Ophalen filiaal","DATA_RECEIVED#Thuisbezorgd"}),
            //new HemaStatusMap("Geleverd aan service point",new []{"AWAITING_RECEIVER_COLLECTION#Afhaalpunt","DELIVERED_AT_PARCELSHOP#Afhaalpunt"}),
            new HemaStatusMap("In ontvangst genomen door klant",new []{"COLLECTED_AT_PARCELSHOP#Afhaalpunt"}),
            //new HemaStatusMap("Geleverd aan winkel",new []{"DELIVERED#Ophalen filiaal","DELIVERED_DAMAGED#Ophalen filiaal","DELIVERED_NOT_IN_TIME#Ophalen filiaal"}),
            new HemaStatusMap("Ingepakt en klaar voor verzending",new []{"UNDERWAY#Ophalen filiaal"})
        };
        public static HemaDialogItem HemaDialogItem = new HemaDialogItem() { Id = CreateGuid(1, 1), InteractionModelSectionId = CreateGuid(0, 1), Name = "HemaItem", ResponseNoPackages = "No packages found.", HemaStatuses = StatusMaps };

        public static Section Section = new Section() { Id = CreateGuid(0, 1), Items = { HemaDialogItem } };
        public static IDictionary<Type, BaseDialogItem> DialogItemTypes = new Dictionary<Type, BaseDialogItem>()
        {
            { typeof(BaseFlowDialogItem), new BaseFlowDialogItem() },
            { typeof(HemaDialogItem), new HemaDialogItem() }
        };
    }
}
