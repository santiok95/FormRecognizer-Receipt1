using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

//use your `key` and `endpoint` environment variables to create your `AzureKeyCredential` and `DocumentAnalysisClient` instances
string key = "3cefc52d84204f3dbc295d3a3cc9ed7f";
string endpoint = "https://demorecognizer15.cognitiveservices.azure.com/";
AzureKeyCredential credential = new AzureKeyCredential(key);
DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

// sample document document
var receiptUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/rest-api/receipt.png");

AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-receipt", receiptUri);

AnalyzeResult receipts = operation.Value;

foreach (AnalyzedDocument receipt in receipts.Documents)
{
    if (receipt.Fields.TryGetValue("MerchantName", out DocumentField merchantNameField))
    {
        if (merchantNameField.FieldType == DocumentFieldType.String)
        {
            string merchantName = merchantNameField.Value.AsString();

            Console.WriteLine($"Merchant Name: '{merchantName}', with confidence {merchantNameField.Confidence}");
        }
    }

    if (receipt.Fields.TryGetValue("TransactionDate", out DocumentField transactionDateField))
    {
        if (transactionDateField.FieldType == DocumentFieldType.Date)
        {
            DateTimeOffset transactionDate = transactionDateField.Value.AsDate();

            Console.WriteLine($"Transaction Date: '{transactionDate}', with confidence {transactionDateField.Confidence}");
        }
    }

    if (receipt.Fields.TryGetValue("Items", out DocumentField itemsField))
    {
        if (itemsField.FieldType == DocumentFieldType.List)
        {
            foreach (DocumentField itemField in itemsField.Value.AsList())
            {
                Console.WriteLine("Item:");

                if (itemField.FieldType == DocumentFieldType.Dictionary)
                {
                    IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                    if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                    {
                        if (itemDescriptionField.FieldType == DocumentFieldType.String)
                        {
                            string itemDescription = itemDescriptionField.Value.AsString();

                            Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                        }
                    }

                    if (itemFields.TryGetValue("TotalPrice", out DocumentField itemTotalPriceField))
                    {
                        if (itemTotalPriceField.FieldType == DocumentFieldType.Double)
                        {
                            double itemTotalPrice = itemTotalPriceField.Value.AsDouble();

                            Console.WriteLine($"  Total Price: '{itemTotalPrice}', with confidence {itemTotalPriceField.Confidence}");
                        }
                    }
                }
            }
        }
    }

    if (receipt.Fields.TryGetValue("Total", out DocumentField totalField))
    {
        if (totalField.FieldType == DocumentFieldType.Double)
        {
            double total = totalField.Value.AsDouble();

            Console.WriteLine($"Total: '{total}', with confidence '{totalField.Confidence}'");
        }
    }
}