using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#region BitPay

app.MapGet("/bitpay/rates/{basecurrency}", ([FromRoute] string basecurrency) =>
{
    var id = Guid.NewGuid().ToString();
    var result = new
    {
        test = id,
        data = new List<object>(){
            new {
                code = "BTC",
                name = "Bitcoin",
                rate = 1
            },
            new {
                code = "BCH",
                name = "Bitcoin Cash",
                rate = 50.77
            },
            new {
                code = "USD",
                name = "US Dollar",
                rate = 41248.11
            },
            new {
                code = "EUR",
                name = "Eurozone Euro",
                rate = 33823.04
            },
            new {
                code = "GBP",
                name = "Pound Sterling",
                rate = 29011.49
            },
            new {
                code = "JPY",
                name = "Japanese Yen",
                rate = 4482741
            },
            new {
                code = "CAD",
                name = "Canadian Dollar",
                rate = 49670.85
            },
            new {
                code = "AUD",
                name = "Australian Dollar",
                rate = 53031.99
            },
            new {
                code = "CNY",
                name = "Chinese Yuan",
                rate = 265266.57
            }
        }
    };
    return result;
})
.WithName("BitPay-GetRates");

app.MapPost("/bitpay/invoices", (BitPay.Invoice request) =>
{
    var id = Guid.NewGuid().ToString();
    var result = new
    {
        facade = "merchant/invoice",
        data = new
        {
            url = $"https://bitpay.com/invoice?id={id}",
            status = "new",
            price = request.Price,
            currency = request.Currency,
            orderId = request.OrderId,
            invoiceTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            expirationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + TimeSpan.FromSeconds(15).Milliseconds,
            currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            id = id,
            lowFeeDetected = false,
            amountPaid = 0,
            displayAmountPaid = "0",
            exceptionStatus = false,
            targetConfirmations = 6,
            transactions = new List<object>(),
            transactionSpeed = "medium",
            buyer = request.Buyer,
            redirectURL = "https://merchantwebsite.com/shop/return",
            refundAddresses = new List<object>(),
            refundAddressRequestPending = false,
            buyerProvidedEmail = request.Buyer.Email,
            buyerProvidedInfo = new
            {
                emailAddress = request.Buyer.Email
            },
            paymentSubtotals = new
            {
                BTC = 18000,
                BCH = 739100,
                ETH = 2505000000000000,
                GUSD = 1000,
                PAX = 10000000000000000000,
                BUSD = 10000000000000000000,
                USDC = 10000000,
                XRP = 7015685,
                DOGE = 1998865000,
                DAI = 9990000000000000000,
                WBTC = 18000
            },
            paymentTotals = new
            {
                BTC = 29600,
                BCH = 739100,
                ETH = 2505000000000000,
                GUSD = 1000,
                PAX = 10000000000000000000,
                BUSD = 10000000000000000000,
                USDC = 10000000,
                XRP = 7015685,
                DOGE = 1998865000,
                DAI = 9990000000000000000,
                WBTC = 18000
            },
            paymentDisplayTotals = new
            {
                BTC = "0.000296",
                BCH = "0.007391",
                ETH = "0.002505",
                GUSD = "10.00",
                PAX = "10.00",
                BUSD = "10.00",
                USDC = "10.00",
                XRP = "7.015685",
                DOGE = "19.988650",
                DAI = "9.99",
                WBTC = "0.000180"
            },
            paymentDisplaySubTotals = new
            {
                BTC = "0.000180",
                BCH = "0.007391",
                ETH = "0.002505",
                GUSD = "10.00",
                PAX = "10.00",
                BUSD = "10.00",
                USDC = "10.00",
                XRP = "7.015685",
                DOGE = "19.988650",
                DAI = "9.99",
                WBTC = "0.000180"
            },
            exchangeRates = new
            {
                BTC = new
                {
                    USD = 55413.609335,
                    EUR = 45540.39841,
                    BCH = 40.84109737914668,
                    ETH = 13.870219975470258,
                    GUSD = 55413.609335,
                    PAX = 55413.609335,
                    BUSD = 55413.609335,
                    USDC = 55413.609335,
                    XRP = 38758.09372049268,
                    DOGE = 110606.00665668662,
                    DAI = 55359.96552840298,
                    WBTC = 0.9981333606461704
                },
                BCH = new
                {
                    USD = 1352.90925,
                    EUR = 1111.2150000000001,
                    BTC = 0.02440102556111244,
                    ETH = 0.33863791096704754,
                    GUSD = 1352.90925,
                    PAX = 1352.90925,
                    BUSD = 1352.90925,
                    USDC = 1352.90925,
                    XRP = 946.2690507998013,
                    DOGE = 2700.4176646706587,
                    DAI = 1351.599550036015,
                    WBTC = 0.024369173431532262
                },
                ETH = new
                {
                    USD = 3992.672665000001,
                    EUR = 3278.9696950000002,
                    BTC = 0.0720117094001833,
                    BCH = 2.9426910658087726,
                    GUSD = 3992.672665000001,
                    PAX = 3992.672665000001,
                    BUSD = 3992.672665000001,
                    USDC = 3992.672665000001,
                    XRP = 2792.6060619837313,
                    DOGE = 7969.406516966069,
                    DAI = 3988.807510522304,
                    WBTC = 0.07191770817497412
                },
                GUSD = new
                {
                    USD = 1,
                    EUR = 0.821674,
                    BTC = 0.000018035966241721267,
                    BCH = 0.0007370228698196506,
                    ETH = 0.0002503034929852446,
                    PAX = 1,
                    BUSD = 1,
                    USDC = 1,
                    XRP = 0.6994327600316144,
                    DOGE = 1.9960079840319362,
                    DAI = 0.9990319380520276,
                    WBTC = 0.000018012422807762058
                },
                PAX = new
                {
                    USD = 1,
                    EUR = 0.821674,
                    BTC = 0.000018035966241721267,
                    BCH = 0.0007370228698196506,
                    ETH = 0.0002503034929852446,
                    GUSD = 1,
                    BUSD = 1,
                    USDC = 1,
                    XRP = 0.6994327600316144,
                    DOGE = 1.9960079840319362,
                    DAI = 0.9990319380520276,
                    WBTC = 0.000018012422807762058
                },
                BUSD = new
                {
                    USD = 1,
                    EUR = 0.821674,
                    BTC = 0.000018035966241721267,
                    BCH = 0.0007370228698196506,
                    ETH = 0.0002503034929852446,
                    GUSD = 1,
                    PAX = 1,
                    USDC = 1,
                    XRP = 0.6994327600316144,
                    DOGE = 1.9960079840319362,
                    DAI = 0.9990319380520276,
                    WBTC = 0.000018012422807762058
                },
                USDC = new
                {
                    USD = 1,
                    EUR = 0.821674,
                    BTC = 0.000018035966241721267,
                    BCH = 0.0007370228698196506,
                    ETH = 0.0002503034929852446,
                    GUSD = 1,
                    PAX = 1,
                    BUSD = 1,
                    XRP = 0.6994327600316144,
                    DOGE = 1.9960079840319362,
                    DAI = 0.9990319380520276,
                    WBTC = 0.000018012422807762058
                },
                XRP = new
                {
                    USD = 1.4253776249999999,
                    EUR = 1.17088545,
                    BTC = 0.00002570806272620483,
                    BCH = 0.0010505359077542177,
                    ETH = 0.0003567769983605121,
                    GUSD = 1.4253776249999999,
                    PAX = 1.4253776249999999,
                    BUSD = 1.4253776249999999,
                    USDC = 1.4253776249999999,
                    DOGE = 2.845065119760479,
                    DAI = 1.423997771159746,
                    WBTC = 0.00002567450444222371
                },
                DOGE = new
                {
                    USD = 0.5002839,
                    EUR = 0.4110702732486,
                    BTC = 0.000009023103531676658,
                    BCH = 0.0003687206757025671,
                    ETH = 0.00012522280765428083,
                    GUSD = 0.5002839,
                    PAX = 0.5002839,
                    BUSD = 0.5002839,
                    USDC = 0.5002839,
                    XRP = 0.3499149489763802,
                    DAI = 0.49979959419322684,
                    WBTC = 0.000009011325130716152
                },
                DAI = new
                {
                    USD = 1.000968,
                    EUR = 0.822469380432,
                    BTC = 0.000018053425057043255,
                    BCH = 0.0007377363079576361,
                    ETH = 0.00025054578676645436,
                    GUSD = 1.000968,
                    PAX = 1.000968,
                    BUSD = 1.000968,
                    USDC = 1.000968,
                    XRP = 0.7001098109433249,
                    DOGE = 1.9979401197604791,
                    WBTC = 0.000018029858833039973
                },
                WBTC = new
                {
                    USD = 55466.58,
                    EUR = 45575.44665492,
                    BTC = 1.000393364423732,
                    BCH = 40.88013797068123,
                    ETH = 13.883478717945511,
                    GUSD = 55466.58,
                    PAX = 55466.58,
                    BUSD = 55466.58,
                    USDC = 55466.58,
                    XRP = 38795.14313891434,
                    DOGE = 110711.73652694612,
                    DAI = 55412.88491451783
                }
            },
            minerFees = new
            {
                BTC = new
                {
                    satoshisPerByte = 79.151,
                    totalFee = 11600
                },
                BCH = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                ETH = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                GUSD = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                PAX = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                BUSD = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                USDC = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                XRP = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                DOGE = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                DAI = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                },
                WBTC = new
                {
                    satoshisPerByte = 0,
                    totalFee = 0
                }
            },
            shopper = new { },
            jsonPayProRequired = false,
            supportedTransactionCurrencies = new
            {
                BTC = new
                {
                    enabled = true
                },
                BCH = new
                {
                    enabled = true
                },
                ETH = new
                {
                    enabled = true
                },
                GUSD = new
                {
                    enabled = true
                },
                PAX = new
                {
                    enabled = true
                },
                BUSD = new
                {
                    enabled = true
                },
                USDC = new
                {
                    enabled = true
                },
                XRP = new
                {
                    enabled = true
                },
                DOGE = new
                {
                    enabled = true
                },
                DAI = new
                {
                    enabled = true
                },
                WBTC = new
                {
                    enabled = true
                }
            },
            paymentCodes = new
            {
                BTC = new
                {
                    BIP72b = "bitcoin:?r=https://bitpay.com/i/" + id,
                    BIP73 = "https://bitpay.com/i/" + id
                },
                BCH = new
                {
                    BIP72b = "bitcoincash:?r=https://bitpay.com/i/" + id,
                    BIP73 = "https://bitpay.com/i/" + id
                },
                ETH = new
                {
                    EIP681 = "ethereum:?r=https://bitpay.com/i/" + id
                },
                GUSD = new
                {
                    EIP681b = "ethereum:?r=https://bitpay.com/i/" + id
                },
                PAX = new
                {
                    EIP681b = "ethereum:?r=https://bitpay.com/i/" + id
                },
                BUSD = new
                {
                    EIP681b = "ethereum:?r=https://bitpay.com/i/" + id
                },
                USDC = new
                {
                    EIP681b = "ethereum:?r=https://bitpay.com/i/" + id
                },
                XRP = new
                {
                    BIP72b = "ripple:?r=https://bitpay.com/i/" + id,
                    BIP73 = "https://bitpay.com/i/" + id,
                    RIP681 = "https://bitpay.com/i/" + id
                },
                DOGE = new
                {
                    BIP72b = "dogecoin:?r=https://bitpay.com/i/" + id,
                    BIP73 = "https://bitpay.com/i/" + id
                },
                DAI = new
                {
                    EIP681b = "ethereum:?r=https://bitpay.com/i/" + id
                },
                WBTC = new
                {
                    EIP681b = "ethereum:?r=https://bitpay.com/i/" + id
                }
            },
            token = request.Token
        }
    };
    return result;
})
.WithName("BitPay-PostInvoice");

app.MapPost("bitpay/webhook", async ([FromQuery] string id, [FromQuery] string cryptocurrency, [FromBody] BitPay.Invoice request) =>
{
    var sendRequest = new
    {
        id = id,
        url = $"https://bitpay.com/invoice?id={id}",
        status = "confirmed",
        price = request.Price,
        currency = request.Currency,
        invoiceTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), //Should be the same as response
        expirationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), //Should be the same as response
        currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
        exceptionStatus = false,
        buyerFields = new
        {
            buyerName = request.Buyer.Name,
            buyerAddress1 = request.Buyer.Address1,
            buyerAddress2 = request.Buyer.Address2,
            buyerCity = request.Buyer.Locality,
            buyerZip = request.Buyer.PostalCode,
            buyerCountry = request.Buyer.Country,
            buyerPhone = request.Buyer.Phone,
            buyerNotify = false,
            buyerEmail = request.Buyer.Email
        },
        paymentSubtotals = new
        {
            BTC = 17500,
            BCH = 700700,
            ETH = 2406000000000000,
            GUSD = 1000,
            PAX = 10000000000000000000,
            BUSD = 10000000000000000000,
            USDC = 10000000,
            XRP = 6668704,
            DOGE = 2077327700,
            DAI = 9990000000000000000,
            WBTC = 17500
        },
        paymentTotals = new
        {
            BTC = 29800,
            BCH = 700700,
            ETH = 2406000000000000,
            GUSD = 1000,
            PAX = 10000000000000000000,
            BUSD = 10000000000000000000,
            USDC = 10000000,
            XRP = 6668704,
            DOGE = 2077327700,
            DAI = 9990000000000000000,
            WBTC = 17500
        },
        exchangeRates = new
        {
            BTC = new
            {
                USD = 57204.993195,
                EUR = 47077.319565,
                BCH = 39.966599499063804,
                ETH = 13.755531957178825,
                GUSD = 57204.993195,
                PAX = 57204.993195,
                BUSD = 57204.993195,
                USDC = 57204.993195,
                XRP = 38003.14441595195,
                DOGE = 118682.55849585062,
                DAI = 57120.83386098427,
                WBTC = 1.0001084497591646
            },
            BCH = new
            {
                USD = 1427.1930750000001,
                EUR = 1173.9677250000002,
                BTC = 0.024936271286002874,
                ETH = 0.3431833281634361,
                GUSD = 1427.1930750000001,
                PAX = 1427.1930750000001,
                BUSD = 1427.1930750000001,
                USDC = 1427.1930750000001,
                XRP = 948.1309499292485,
                DOGE = 2960.981483402489,
                DAI = 1425.0934048139654,
                WBTC = 0.02495145570387066
            },
            ETH = new
            {
                USD = 4156.600660000002,
                EUR = 3418.000145000001,
                BTC = 0.07262515738127348,
                BCH = 2.904033102311154,
                GUSD = 4156.600660000002,
                PAX = 4156.600660000002,
                BUSD = 4156.600660000002,
                USDC = 4156.600660000002,
                XRP = 2761.365509177756,
                DOGE = 8623.652821576765,
                DAI = 4150.4855164823275,
                WBTC = 0.0726693809431983
            },
            GUSD = new
            {
                USD = 1,
                EUR = 0.822741,
                BTC = 0.000017472247954960737,
                BCH = 0.0006986557862672219,
                ETH = 0.00024046033726966907,
                PAX = 1,
                BUSD = 1,
                USDC = 1,
                XRP = 0.664332644641825,
                DOGE = 2.0746887966804977,
                DAI = 0.9985288113971301,
                WBTC = 0.000017482887312826024
            },
            PAX = new
            {
                USD = 1,
                EUR = 0.822741,
                BTC = 0.000017472247954960737,
                BCH = 0.0006986557862672219,
                ETH = 0.00024046033726966907,
                GUSD = 1,
                BUSD = 1,
                USDC = 1,
                XRP = 0.664332644641825,
                DOGE = 2.0746887966804977,
                DAI = 0.9985288113971301,
                WBTC = 0.000017482887312826024
            },
            BUSD = new
            {
                USD = 1,
                EUR = 0.822741,
                BTC = 0.000017472247954960737,
                BCH = 0.0006986557862672219,
                ETH = 0.00024046033726966907,
                GUSD = 1,
                PAX = 1,
                USDC = 1,
                XRP = 0.664332644641825,
                DOGE = 2.0746887966804977,
                DAI = 0.9985288113971301,
                WBTC = 0.000017482887312826024
            },
            USDC = new
            {
                USD = 1,
                EUR = 0.822741,
                BTC = 0.000017472247954960737,
                BCH = 0.0006986557862672219,
                ETH = 0.00024046033726966907,
                GUSD = 1,
                PAX = 1,
                BUSD = 1,
                XRP = 0.664332644641825,
                DOGE = 2.0746887966804977,
                DAI = 0.9985288113971301,
                WBTC = 0.000017482887312826024
            },
            XRP = new
            {
                USD = 1.4995417500000001,
                EUR = 1.2359324250000001,
                BTC = 0.00002620036527481575,
                BCH = 0.0010476635203867759,
                ETH = 0.0003605803149549498,
                GUSD = 1.4995417500000001,
                PAX = 1.4995417500000001,
                BUSD = 1.4995417500000001,
                USDC = 1.4995417500000001,
                DOGE = 3.111082468879668,
                DAI = 1.4973356412678729,
                WBTC = 0.000026216319436127937
            },
            DOGE = new
            {
                USD = 0.48138770000000003,
                EUR = 0.3960573976857,
                BTC = 0.000008410925256868251,
                BCH = 0.0003363243020428695,
                ETH = 0.00011575464869947028,
                GUSD = 0.48138770000000003,
                PAX = 0.48138770000000003,
                BUSD = 0.48138770000000003,
                USDC = 0.48138770000000003,
                XRP = 0.31980156383904546,
                DAI = 0.4806794879021982,
                WBTC = 0.0000084160469128805
            },
            DAI = new
            {
                USD = 1.001472,
                EUR = 0.8239520747519998,
                BTC = 0.00001749796710395044,
                BCH = 0.0006996842075846071,
                ETH = 0.00024081429488613,
                GUSD = 1.001472,
                PAX = 1.001472,
                BUSD = 1.001472,
                USDC = 1.001472,
                XRP = 0.6653105422947376,
                DOGE = 2.0777427385892113,
                WBTC = 0.000017508622122950502
            },
            WBTC = new
            {
                USD = 57151.869999999995,
                EUR = 47021.18667566999,
                BTC = 0.9985716437296819,
                BCH = 39.92948467149204,
                ETH = 13.74275793579228,
                GUSD = 57151.869999999995,
                PAX = 57151.869999999995,
                BUSD = 57151.869999999995,
                USDC = 57151.869999999995,
                XRP = 37967.85294332578,
                DOGE = 118572.34439834024,
                DAI = 57067.78882022329
            }
        },
        amountPaid = 700700,
        orderId = request.OrderId,
        transactionCurrency = cryptocurrency
    };
    var response = await WebhookRequest.Request(JsonSerializer.Serialize(sendRequest));
    return response;
})
.WithName("BitPay-PostWebhook");

#endregion


#region CoinPayments

app.MapPost("/coinpayments", ([FromQuery] string version,
                              [FromQuery] string key,
                              [FromQuery] string cmd,
                              [FromQuery] int amount,
                              [FromQuery] string currency1,
                              [FromQuery] string currency2,
                              [FromQuery] string buyer_email,
                              [FromQuery] string address,
                              [FromQuery] string buyer_name,
                              [FromQuery] string item_name,
                              [FromQuery] string item_number,
                              [FromQuery] string ipn_url) =>
{
    var id = Guid.NewGuid().ToString();
    object result = null;
    if (cmd == "create_transaction")
    {
        result = new
        {
            error = "ok",
            result = new
            {
                amount = amount,
                address = address,
                dest_tag = key,
                txn_id = id,
                confirms_needed = "10",
                timeout = 9000,
                checkout_url = $"https://www.coinpayments.net/index.php?cmd=checkout&id={id}&key={key}",
                status_url = $"https://www.coinpayments.net/index.php?cmd=status&id={id}&key={key}",
                qrcode_url = $"https://www.coinpayments.net/qrgen.php?id={id}&key={key}",
            }
        };
    }
    else if (cmd == "rates")
    {
        result = new
        {
            error = "ok",
            result = new
            {
                BTC = new
                {
                    is_fiat = 0,
                    rate_btc = "1.000000000000000000000000",
                    last_update = "1375473661",
                    tx_fee = "0.00100000",
                    status = "online",
                    image = "https://www.coinpayments.net/images/coins/BTC.png",
                    name = "Bitcoin",
                    confirms = "2",
                    capabilities = new List<string> { "payments", "wallet", "transfers", "convert" }
                },
                LTC = new
                {
                    is_fiat = 0,
                    rate_btc = "0.018343387500000000000000",
                    last_update = "1518463609",
                    tx_fee = "0.00100000",
                    status = "online",
                    name = "Litecoin",
                    confirms = "3",
                    capabilities = new List<string> { "payments", "wallet", "transfers", "convert" }
                },
                USD = new
                {
                    is_fiat = 1,
                    rate_btc = "0.000114884285404190000000",
                    last_update = "1518463609",
                    tx_fee = "0.00000000",
                    status = "online",
                    name = "United States Dollar",
                    confirms = "1",
                    capabilities = new List<string>()
                },
                CAD = new
                {
                    is_fiat = 1,
                    rate_btc = "0.000091601308947890000000",
                    last_update = "1518463609",
                    tx_fee = "0.00000000",
                    status = "online",
                    name = "Canadian Dollar",
                    confirms = "1",
                    capabilities = new List<string>()
                },
                MAID = new
                {
                    is_fiat = 0,
                    rate_btc = "0.000049810000000000000000",
                    last_update = "1518463609",
                    tx_fee = "0.00000000",
                    status = "online",
                    name = "MaidSafeCoin",
                    confirms = "2",
                    capabilities = new List<string> { "payments", "wallet" }
                },
                XMR = new
                {
                    is_fiat = 0,
                    rate_btc = "0.028198593333333000000000",
                    last_update = "1518463609",
                    tx_fee = "0.01000000",
                    status = "online",
                    name = "Monero",
                    confirms = "3",
                    capabilities = new List<string> { "payments", "wallet", "transfers", "dest_tag" }
                },
                LTCT = new
                {
                    is_fiat = 0,
                    rate_btc = "1.000000000000000000000000",
                    last_update = "1375473661",
                    tx_fee = "0.00100000",
                    status = "online",
                    name = "Litecoin Testnet",
                    confirms = "0",
                    capabilities = new List<string> { "payments", "wallet", "transfers" }
                }
            }
        };
    }
    return result;
})
.WithName("CoinPayments-PostTransaction");

// CoinPayments webhook https://www.coinpayments.net/merchant-tools-ipn
app.MapPost("/coinpayments/webhook", async (
                              [FromQuery] string txn_id,
                              [FromQuery] int amount,
                              [FromQuery] int amount2,
                              [FromQuery] string currency1,
                              [FromQuery] string currency2,
                              [FromQuery] string buyer_email,
                              [FromQuery] string address,
                              [FromQuery] string buyer_name,
                              [FromQuery] string item_name,
                              [FromQuery] string item_number) =>
{
    var sendRequest = new SortedList<string, string>()
    {
        { "ipn_version", "1.0" },
        { "ipn_type", "api" },
        { "ipn_mode", "hmac" },
        { "ipn_id", Guid.NewGuid().ToString() },
        { "merchant", "merchant ID" },
        { "txn_id", "" },
        { "item_name", item_name },
        { "item_number", item_number },
        { "amount1", amount.ToString() },
        { "amount2", amount2.ToString() },
        { "currency1", currency1 },
        { "currency2", currency2 },
        { "buyer_name", buyer_name },
        { "buyer_email", buyer_email },
        { "address", address },
        { "status", "100" },
        { "status_text", "Payment completed successfully" },
        { "received_amount", amount2.ToString() },
        { "received_confirms", "1" },
    };
    var response = await WebhookRequest.Request(JsonSerializer.Serialize(sendRequest));
    return response;
})
.WithName("CoinPayments-PostWebhook");

#endregion


#region Coinqvest

// This returns the convertion amount
app.MapPost("/coinqvest/checkout", (Coinqvest.Request request) =>
{
    var id = Guid.NewGuid().ToString();
    var result = new
    {
        id = id,
        paymentMethods = new List<object>(){
            new
            {
                assetCode = "BTC",
                blockchain = "Bitcoin",
                paymentAmount = "0.0003672",
                settlement = new
                {
                    assetId = "EURT:GAP5LETOV6YIE62YAM56STDANPRDO7ZFDBGSNHJQIYGGKSMOZAHOOS2S",
                    amount = "14.3394628",
                    netAmount = "14.3394628",
                    fee = "0.0000000"
                }
            } ,
            new
            {
                assetCode = "ETH",
                blockchain = "Ethereum",
                paymentAmount = "0.0050734",
                settlement = new
                {
                    assetId = "EURT:GAP5LETOV6YIE62YAM56STDANPRDO7ZFDBGSNHJQIYGGKSMOZAHOOS2S",
                    amount = "14.3502846",
                    netAmount = "14.3394628",
                    fee = "0.0000000"
                }
            } ,
            new
            {
                assetCode = "LTC",
                blockchain = "Litecoin",
                paymentAmount = "0.0996787",
                settlement = new
                {
                    assetId = "EURT:GAP5LETOV6YIE62YAM56STDANPRDO7ZFDBGSNHJQIYGGKSMOZAHOOS2S",
                    amount = "14.3387874",
                    netAmount = "14.3394628",
                    fee = "0.0000000"
                }
            } ,
            new
            {
                assetCode = "XLM",
                blockchain = "Stellar Network",
                paymentAmount = "50",
                settlement = new
                {
                    assetId = "EURT:GAP5LETOV6YIE62YAM56STDANPRDO7ZFDBGSNHJQIYGGKSMOZAHOOS2S",
                    amount = "14.3904448",
                    netAmount = "14.3394628",
                    fee = "0.0000000"
                }
            }
        }
    };
    return result;
})
.WithName("Coinqvest-PostCheckout");

app.MapPost("/coinqvest/checkout-commit", (Coinqvest.RequestComplete request) =>
{
    var id = Guid.NewGuid().ToString();
    var result = new
    {
        depositInstructions = new
        {
            blockchain = "Ethereum",
            assetCode = request.AssetCode,
            amount = "0.0056804",
            address = "0xc85eBb197c8212C09B7d0905C3ce9e3fCe324F0B"
        },
        settlement = new
        {
            assetId = "USD:GDUKMGUGDZQK6YHYA5Z6AY2G4XDSZPSZ3SW5UN3ARVMO6QSRDWP5YLEX",
            amount = "19.2514345"
        },
        expirationTime = "2020-02-26T18:49:33+00:00",
        checkoutId = request.CheckoutId,
    };
    return result;
})
.WithName("Coinqvest-PostCheckoutResolve");

app.MapPost("coinqvest/webhook", async ([FromQuery] string id, [FromQuery] string cryptocurrency, [FromBody] Coinqvest.Request request) =>
{
    var totalAmount = request.Charge.LineItems.Select(e => e.NetAmount).Sum();
    var sendRequest = new
    {
        eventType = "CHECKOUT_COMPLETED",
        data = new
        {
            checkout = new
            {
                id = id,
                timestamp = DateTime.Now,
                state = "COMPLETED",
                type = "HOSTED",
                origin = "API",
                settlementAssetId = $"{request.Charge.Currency}:GDUKMGUGDZQK6YHYA5Z6AY2G4XDSZPSZ3SW5UN3ARVMO6QSRDWP5YLEX",
                settlementAmountRequired = $"{totalAmount}",
                settlementAmountReceived = $"{totalAmount}",
                settlementAmountFeePaid = "0",
                sourceAssetId = $"{cryptocurrency}:GBDEVU63Y6NTHJQQZIKVTC23NWLQVP3WJ2RI2OTSJTNYOIGICST6DUXR",
                sourceAmountRequired = "0.0069518",
                sourceAmountReceived = "0.0069518",
                sourceBlockchain = "Ethereum",
                sourceBlockchainAssetCode = "ETH",
                blockchainTransactions = new List<object> {
                    new
                    {
                        type = "ORIGIN",
                        typeDescription = "Blockchain payment transaction initiated by customer.",
                        blockchain = $"network {cryptocurrency}",
                        blockchainAssetCode = $"{cryptocurrency}",
                        tx = $"{Guid.NewGuid()}",
                        amount = "0.0069518",
                        amountAssetCode = $"{cryptocurrency}"
                    },
                    new
                    {
                        type = "TRANSFER",
                        typeDescription = $"Asset issuer fund transfer from native blockchain to {cryptocurrency} Network.",
                        blockchain = $"network {cryptocurrency}",
                        blockchainAssetCode = $"blockchain asset {cryptocurrency}",
                        tx = $"{Guid.NewGuid()}",
                        amount = "0.0069518",
                        amountAssetCode = $"{cryptocurrency}"
                    },
                    new
                    {
                        type = "SETTLEMENT",
                        typeDescription = $"{cryptocurrency} transaction crediting your COINQVEST merchant account.",
                        blockchain = $"network {cryptocurrency}",
                        blockchainAssetCode = $"blockchain asset {cryptocurrency}",
                        tx = $"{Guid.NewGuid()}",
                        amount = $"{totalAmount}",
                        amountAssetCode = $"{request.Charge.Currency}"
                    }
                },
                payload = new
                {
                    charge = request.Charge
                }
            }
        }
    };
    var response = await WebhookRequest.Request(JsonSerializer.Serialize(sendRequest));
    return response;
})
.WithName("Coinqvest-PostWebhook");

#endregion



app.Run();


struct BitPay
{
    internal class Invoice
    {
        public int Price { get; set; }
        public string Currency { get; set; }
        public string OrderId { get; set; }
        public string NotificationURL { get; set; }
        public string RedirectURL { get; set; }
        public string Token { get; set; }
        public Buyer Buyer { get; set; }
    }

    internal class Buyer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
    }

}

struct Coinqvest
{
    internal class Request
    {
        public CoinqvestCharge Charge { get; set; }
        public string Webhook { get; set; }
    }

    internal class CoinqvestCharge
    {
        public string Currency { get; set; }
        public Item[] LineItems { get; set; }
    }

    internal class Item
    {
        public string Description { get; set; }
        public int NetAmount { get; set; }
        public int Quantity { get; set; }
    }

    internal class RequestComplete
    {
        public string CheckoutId { get; set; }
        public string AssetCode { get; set; }
    }
}

internal class WebhookRequest
{
    private static readonly HttpClient _Client = new HttpClient();
    private static readonly string pUrl = "https://webhook.site/c3343ce1-4f83-4b14-932a-d0d56f38d4bc";//TODO - Change to correct

    internal static async Task<HttpResponseMessage> Request(string pJsonContent)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(pUrl)
        };

        HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
        httpRequestMessage.Content = httpContent;

        return await _Client.SendAsync(httpRequestMessage);
    }

    internal static async Task<HttpResponseMessage> Request(SortedList<string, string> parms)
    {
        var pJsonContent = new StringBuilder();
        foreach (KeyValuePair<string, string> parm in parms)
        {
            if (pJsonContent.Length > 0)
            {
                pJsonContent.Append('&');
            }
            pJsonContent.Append($"{parm.Key}={Uri.EscapeDataString(parm.Value)}");
        }

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(pUrl + pJsonContent)
        };

        return await _Client.SendAsync(httpRequestMessage);
    }
}