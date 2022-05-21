namespace CryptocurrencyPaymentConfiguration.Repositories
{
    using CryptocurrencyPaymentConfiguration.Model;

    public class DatabaseContext
    {
        // BitPay - BTC, BCH, DOGE, ETH, GUSD, USDC, USDP, BUSD, DAI, WBTC, XRP
        // Coinbase - BTC, BCH, DAI, LTC, ETH, DOGE
        // CoinPayment - BTC, BCH, BCN, ETH, ETN, LTC, QTUM, VLX, APL, ASK, DASH, DIVI 
        // COINQVEST - BTC, LTC, ETH, XLM
        public List<MerchantConfiguration> MerchantConfigurations { get; set; } = new List<MerchantConfiguration>()
        {
            new MerchantConfiguration()
            {
                MerchantId = "merchantId-Test",
                CurrencyPaymentGateways = new List<CurrencyPaymentGateway>()
                {
                    { CreateBTC("EUR") },
                    { CreateBTC("USD") },
                    { CreateBTC("CAD") },

                    { CreateBCH("EUR") },
                    { CreateBCH("USD") },
                    { CreateBCH("CAD") },

                    { CreateDOGE("EUR") },
                    { CreateDOGE("USD") },
                    { CreateDOGE("CAD") },

                    { CreateETH("EUR") },
                    { CreateETH("USD") },
                    { CreateETH("CAD") },

                    { CreateGUSD("EUR") },
                    { CreateGUSD("USD") },
                    { CreateGUSD("CAD") },

                    { CreateUSDC("EUR") },
                    { CreateUSDC("USD") },
                    { CreateUSDC("CAD") },

                    { CreateUSDP("EUR") },
                    { CreateUSDP("USD") },
                    { CreateUSDP("CAD") },

                    { CreateBUSD("EUR") },
                    { CreateBUSD("USD") },
                    { CreateBUSD("CAD") },

                    { CreateDAI("EUR") },
                    { CreateDAI("USD") },
                    { CreateDAI("CAD") },

                    { CreateWBTC("EUR") },
                    { CreateWBTC("USD") },
                    { CreateWBTC("CAD") },

                    { CreateXRP("EUR") },
                    { CreateXRP("USD") },
                    { CreateXRP("CAD") },

                    { CreateLTC("EUR") },
                    { CreateLTC("USD") },
                    { CreateLTC("CAD") },

                    { CreateBCN("EUR") },
                    { CreateBCN("USD") },
                    { CreateBCN("CAD") },

                    { CreateETN("EUR") },
                    { CreateETN("USD") },
                    { CreateETN("CAD") },

                    { CreateQTUM("EUR") },
                    { CreateQTUM("USD") },
                    { CreateQTUM("CAD") },

                    { CreateVLX("EUR") },
                    { CreateVLX("USD") },
                    { CreateVLX("CAD") },

                    { CreateAPL("EUR") },
                    { CreateAPL("USD") },
                    { CreateAPL("CAD") },

                    { CreateASK("EUR") },
                    { CreateASK("USD") },
                    { CreateASK("CAD") },

                    { CreateDASH("EUR") },
                    { CreateDASH("USD") },
                    { CreateDASH("CAD") },

                    { CreateDIVI("EUR") },
                    { CreateDIVI("USD") },
                    { CreateDIVI("CAD") },

                    { CreateXLM("EUR") },
                    { CreateXLM("USD") },
                    { CreateXLM("CAD") },
                }
            }
        };

        private static CurrencyPaymentGateway CreateBTC(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "BTC",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.Coinbase,
                    PaymentGatewayName.Coinqvest,
                    PaymentGatewayName.BitPay,
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateBCH(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "BCH",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {

                    PaymentGatewayName.Coinbase,
                    PaymentGatewayName.BitPay,
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateDOGE(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "DOGE",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.Coinbase,
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateETH(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "ETH",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.Coinbase,
                    PaymentGatewayName.Coinqvest,
                    PaymentGatewayName.BitPay,
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateGUSD(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "GUSD",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateUSDC(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "USDC",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateUSDP(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "USDP",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateBUSD(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "BUSD",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateDAI(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "DAI",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.Coinbase,
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateWBTC(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "WBTC",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateXRP(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "XRP",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.BitPay
                }
            };
        }

        private static CurrencyPaymentGateway CreateLTC(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "LTC",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.Coinbase,
                    PaymentGatewayName.Coinqvest,
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateBCN(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "BCN",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateETN(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "ETN",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateQTUM(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "QTUM",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateVLX(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "VLX",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateAPL(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "APL",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateASK(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "ASK",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateDASH(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "DASH",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateDIVI(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "DIVI",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.CoinPayments
                }
            };
        }

        private static CurrencyPaymentGateway CreateXLM(string fiat)
        {
            return new CurrencyPaymentGateway()
            {
                CryptoCurrency = "XLM",
                FiatCurrency = fiat,
                PaymentGatewayNames = new List<PaymentGatewayName>()
                {
                    PaymentGatewayName.Coinqvest
                }
            };
        }

    }
}
