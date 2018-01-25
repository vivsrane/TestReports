using System.Diagnostics.CodeAnalysis;

namespace VB.DomainModel.Oltp
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Is a reference collection used as foreign key values")]
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "The enum values are flags but the limitation to 32 values is unacceptable")]
    public enum Upgrade
    {
        TradeAnalyzer = 1,
        AgingInventoryPlan = 2,
        PurchasingCenter = 3,
        Redistribution = 4,
        AuctionData = 5,
        PerformanceDashboard = 7,
        MarketData = 8,
        AnnualReturnOnInvestment = 9,
        AppraisalLockout = 10,
        EquityAnalyzer = 11,
        PlatinumPackage = 12,
        SearchAndAcquisitionNavigator = 13,
        Ping = 14,
        Max = 15,
        MaxTest = 16,
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BlueBook", Justification = "Brand Name")]
        KelleyBlueBookTradeInValues = 17,
        // ReSharper disable once InconsistentNaming
        JDPowerUsedCarMarketData = 18,
        PingTwo = 19,
        EdmundsTrueMarketValue = 20,
        MakeADeal = 21,
        MarketStocking = 22,
        Marketing = 23,
        Merchandising = 24,
        // ReSharper disable once InconsistentNaming
        WebSitePDF = 25,
        // ReSharper disable once InconsistentNaming
        WebSitePDFMarketListings = 26,
        VB30 = 27,
        NadaValues = 28,
        AppraisalLite = 32,
        MaxOnly = 31
    };
}