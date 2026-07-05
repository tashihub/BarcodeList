using BarcodeList.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeList.Interface
{
    internal interface IBarcodeCreateService
    {
        public Task SaveBarcodeToHistory(string barcodeValue,int folderId);
        public (bool isValid, string error) IsValid(string value);

        public string Validate(string value);
    }
}
