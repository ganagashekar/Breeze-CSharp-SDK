using Breeze;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

//.Net Core 3.1
namespace ConsoleAppTestProject
{
    public class OptionDataLoad
    {
        public string CE_Symbol { get; set; }
        public string PE_Symbol { get; set; }
        public string stock_name { get; set; }
        public string stock_name_exp { get; set; }
        public string expiry { get; set; }
        public int Strike { get; set; }
    }
    internal class Program
    {

        static async Task Main(string[] args)
        {



            //var builder = new ConfigurationBuilder() // .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
            //   .SetBasePath(Directory.GetCurrentDirectory())
            //   .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true);

            // IConfiguration config = builder.Build();

            var url = "http://localhost:46/breezeOperation"; ;/// config.GetSection("appSettings:url").Value;

            Console.WriteLine(url);

            var text = System.IO.File.ReadAllText("C:\\Hosts\\ICICI_Key\\jobskeys.txt");

            string[] lines = text.Split(
        new string[] { Environment.NewLine },
        StringSplitOptions.None
        );

            string HUbUrl = url;

            await using var connection = new HubConnectionBuilder().WithUrl(url).WithAutomaticReconnect().Build();
            //connection.KeepAliveInterval = TimeSpan.FromSeconds(10);
            //connection.ServerTimeout.Add(TimeSpan.FromMinutes(120));

            Random r = new Random();

            Console.WriteLine(connection.ConnectionId);
            // string HUbUrl = "http://localhost/StockSignalRServer/livefeedhub";
            try
            {
                string APIKEY = string.Empty;
                string APISecret = string.Empty;
                string token = string.Empty;
                //Initialize SDK
                string[] line;
                string arg = "0";
                if (args.Any())
                    arg = args[0];



                switch (Convert.ToInt16(arg))
                {
                    case 0:
                        line = lines[0].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 1:
                        line = lines[1].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 2:
                        line = lines[2].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 3:
                        line = lines[3].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 4:
                        line = lines[4].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                }
                // Generate Session
                Console.WriteLine(arg);
                BreezeConnect breeze = new BreezeConnect(APIKEY);
                //Console.WriteLine(args[1].ToString());
                breeze.generateSessionAsPerVersion(APISecret, token);

                // Connect to WebSocket
                var responseObject = await breeze.wsConnectAsync();
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(responseObject));

                await connection.StartAsync();
                //Console.WriteLine("GetAllStocksForLoad" + Convert.ToInt16(arg));
                //// await connection.SendAsync("GetAllStocksForLoad", Convert.ToInt16(arg));

                //await connection.SendAsync("GetAllStocksForLoadAll");
                //Console.WriteLine(breeze.subscribeFeedsAsync("4.1!53216"));
                //Console.WriteLine(breeze.subscribeFeedsAsync("8.1!1140714"));


                var jobs = System.IO.File.ReadAllText(@"C:\Hosts\JobStocksJson\optiondata0.json");
                var paramss = JsonConvert.DeserializeObject<List<OptionDataLoad>>(jobs);

                foreach (var item in paramss.Where(x => x.stock_name == "BSESEN").ToList())
                {
                    try
                    {
                        await breeze.subscribeFeedsAsync(item.PE_Symbol.Replace("4.1!","8.1!"));
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        await breeze.subscribeFeedsAsync(item.CE_Symbol.Replace("4.1!", "8.1!"));
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(item.ToString());

                    }

                }




                breeze.ticker((data) =>
                {

                    try
                    {
                        //System.IO.File.AppendAllText("C:\\Hosts\\ICICI_Key\\ticks1234.txt", System.Text.Json.JsonSerializer.Serialize(data));


                        if (connection.State == HubConnectionState.Connected)
                        {
                            connection.InvokeAsync("GetTickDataOptionSensex", System.Text.Json.JsonSerializer.Serialize(data));
                            // Console.WriteLine(JsonSerializer.Serialize(data));
                            // Console.WriteLine("Ticker Data:" + JsonSerializer.Serialize(data));
                            // await connection.InvokeAsync("CaptureLiveDataForBuyForAutomationNIFTYBANK", JsonSerializer.Serialize(data));

                        }
                        else
                        {

                        }




                    }
                    catch (Exception)
                    {


                    }





                });

               // breeze.subscribeFeedsAsync("4.1!NIFTY");

                Console.WriteLine("");


                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
        //static async Task Main(string[] args)
        //{
        //    try
        //    {
        //        ////////////////////////Initiate////////////////////////

        //        BreezeConnect breeze = new BreezeConnect("App key");
        //        breeze.generateSessionAsPerVersion("Secret key", "Session key");
        //        ////////////////////////WebSocket////////////////////////
        //        var responseobject = await breeze.wsConnectAsync();
        //        // Get Customer details by api-session value.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getCustomerDetail(apiSession: "48479684")));

        //        //Get Demat Holding details of your account.
        //       Console.WriteLine(JsonSerializer.Serialize(breeze.getDematHoldings()));

        //        // Get Funds details of your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getFunds()));

        //        // Set Funds of your account by transaction-type as "Credit" or "Debit" with amount in numeric string as rupees and segment-type as "Equity" or "FNO".
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.setFunds(transactionType: "debit", amount: "200", segment: "Equity")));

        //        // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getHistoricalData(interval: "1minute", fromDate: "2022-08-15T07:00:00.000Z", toDate: "2022-08-17T07:00:00.000Z", stockCode: "ICIBAN", exchangeCode: "NFO", productType: "futures", expiryDate: "2022-08-25T07:00:00.000Z", right: "others", strikePrice: "0")));


        //        // Add Margin to your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.addMargin(productType: "margin", stockCode: "ICIBAN", exchangeCode: "BSE", settlementId: "2021220", addAmount: "100", marginAmount: "3817.10", openQuantity: "10", coverQuantity: "0", categoryIndexPerStock: "", expiryDate: "", right: "", contractTag: "", strikePrice: "", segmentCode: "")));

        //        // Get Margin of your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getMargin(exchangeCode: "NSE")));

        //        // Place an order from your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.placeOrder(stockCode: "ICIBAN", exchangeCode: "NFO", productType: "futures", action: "buy", orderType: "limit", stoploss: "0", quantity: "3200", price: "200", validity: "day", validityDate: "2022-08-22T06:00:00.000Z", disclosedQuantity: "0", expiryDate: "2022-08-25T06:00:00.000Z", right: "others", strikePrice: "0", userRemark: "Test", orderTypeFresh: "", orderRateFresh: "")));

        //        //place an option plus order

        //        Console.WriteLine(JsonSerializer.Serialize(breeze.placeOrder(stockCode: "NIFTY", exchangeCode: "NFO", productType: "optionplus", action: "buy", orderType: "limit", stoploss: "15", quantity: "50", price: "11.25", validity: "day", validityDate: "2022-12-02T06:00:00.000Z", disclosedQuantity: "0", expiryDate: "2022-12-08T06:00:00.000Z", right: "call", strikePrice: "19000", orderTypeFresh : "Limit", orderRateFresh : "20", userRemark: "Test")));


        //        // Get an order details by exchange-code and order-id from your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getOrderDetail(exchangeCode: "NSE", orderId: "20220819N100000001")));

        //        // Get order list of your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getOrderList(exchangeCode: "NSE", fromDate: "2022-08-01T10:00:00.000Z", toDate: "2022-08-19T10:00:00.000Z")));

        //        // Cancel an order from your account whose status are not Executed. 
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.cancelOrder(exchangeCode: "NSE", orderId: "20220819N100000001")));

        //        // Modify an order from your account whose status are not Executed. 
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.modifyOrder(orderId: "202208191100000001", exchangeCode: "NFO", orderType: "limit", stoploss: "0", quantity: "250", price: "290100", validity: "day", disclosedQuantity: "0", validityDate: "2022-08-22T06:00:00.000Z")));

        //        // Get Portfolio Holdings of your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getPortfolioHoldings(exchangeCode: "NFO", fromDate: "2022-08-01T06:00:00.000Z", toDate: "2022-08-19T06:00:00.000Z", stockCode: "", portfolioType: "")));

        //        // Get Portfolio Positions from your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getPortfolioPositions()));

        //        // Get quotes of mentioned stock-code
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getQuotes(stockCode: "ICIBAN", exchangeCode: "NFO", expiryDate: "2022-08-25T06:00:00.000Z", productType: "futures", right: "others", strikePrice: "0")));

        //        // Get option-chain of mentioned stock-code for product-type Futures where input of expiry-date is not  compulsory
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getOptionChainQuotes(stockCode: "ICIBAN",
        //            exchangeCode: "NFO",
        //            productType: "futures",
        //            expiryDate: "2022-08-25T06:00:00.000Z",
        //            right : "others",
        //            strikePrice: "0")));

        //        //Get option-chain of mentioned stock-code for product-type Options where atleast 2 input is required out of expiry-date, right and strike-price
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getOptionChainQuotes(stockCode: "ICIBAN",
        //            exchangeCode: "NFO",
        //            productType: "options",
        //            expiryDate: "2022-08-25T06:00:00.000Z",
        //            right: "call",
        //            strikePrice: "16850")));

        //        // Square off an Equity Margin Order
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.squareOff(exchangeCode: "NSE", productType: "margin", stockCode: "NIFTY", quantity: "10", price: "0", action: "sell", orderType: "market", validity: "day", stoploss: "0", disclosedQuantity: "0", protectionPercentage: "", settlementId: "", coverQuantity: "", openQuantity: "", marginAmount: "", sourceFlag: "", expiryDate: "", right: "", strikePrice: "", validityDate: "", tradePassword: "", aliasName: "")));
        //        // Note: Please refer getPortfolioPositions() for settlementId and marginAmount

        //        // Square off an FNO Futures Order
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.squareOff(exchangeCode: "NFO", productType: "futures", stockCode: "ICIBAN", expiryDate: "2022-08-25T06:00:00.000Z", action: "sell", orderType: "market", validity: "day", stoploss: "0", quantity: "50", price: "0", validityDate: "2022-08-12T06:00:00.000Z", tradePassword: "", disclosedQuantity: "0", sourceFlag: "", protectionPercentage: "", settlementId: "", marginAmount: "", openQuantity: "", coverQuantity: "", right: "", strikePrice: "", aliasName: "")));

        //        // Square off an FNO Options Order
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.squareOff(exchangeCode: "NFO", productType: "options", stockCode: "ICIBAN", expiryDate: "2022-08-25T06:00:00.000Z", right: "Call", strikePrice: "16850", action: "sell", orderType: "market", validity: "day", stoploss: "0", quantity: "50", price: "0", validityDate: "2022-08-12T06:00:00.000Z", tradePassword: "", disclosedQuantity: "0", sourceFlag: "", protectionPercentage: "", settlementId: "", marginAmount: "", openQuantity: "", coverQuantity: "", aliasName: "")));

        //        // Get trade list of your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getTradeList(fromDate: "2022-08-01T06:00:00.000Z", toDate: "2022-08-19T06:00:00.000Z", exchangeCode: "NSE", productType: "", action: "", stockCode: "")));

        //        // Get trade detail of your account.
        //        Console.WriteLine(JsonSerializer.Serialize(breeze.getTradeDetail(exchangeCode: "NSE", orderId: "20220819N100000005")));

        //        //Get Names for NSE codes

        //       Console.WriteLine(JsonSerializer.Serialize(breeze.getNames(exchange: "NSE", stockCode: "RELIANCE")));
        //        //Note: Use this method to find ICICI specific stock codes / token

        //        //Place an order from your account.
        //       Console.WriteLine(JsonSerializer.Serialize(breeze.placeOrder(stockCode: "NIFTY", exchangeCode: "NFO", productType: "options", action: "buy", orderType: "limit", stoploss: "0", quantity: "25", price: "0.30", validity: "day", validityDate: "2024-10-24T06:00:00.000Z", disclosedQuantity: "0", expiryDate: "2024-10-24T06:00:00.000Z", right: "call", strikePrice: "0", userRemark: "Test", orderTypeFresh : "", orderRateFresh : "")));
        //        //Console.WriteLine(JsonSerializer.Serialize(await breeze.subscribeFeedsAsync(
        //        //        /* exchangeCode: */"NFO",
        //        //        /* stockCode:*/ "NIFTY",
        //        //        /* productType:*/ "options",
        //        //        /* expiryDate: */ "10-Oct-2024",
        //        //        /* strikePrice: */ "24900",
        //        //        /* right: */ "Put",
        //        //        /* getExchangeQuotes:*/ true,
        //        //        /* getMarketDepth: */ false)
        //        //    ));

        //        breeze.ticker((data) =>
        //        {
        //            Console.WriteLine("Ticker Data:" + JsonSerializer.Serialize(data));
        //        });
        //        Console.WriteLine(JsonSerializer.Serialize(responseobject));

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
    }
}
