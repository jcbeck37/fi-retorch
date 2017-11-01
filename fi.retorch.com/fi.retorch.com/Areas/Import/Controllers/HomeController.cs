using CsvHelper;
using fi.retorch.com.Areas.Dashboard.Code.QuerySettings;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Import.Code.Base;
using fi.retorch.com.Areas.Import.Code.Enums;
using fi.retorch.com.Areas.Import.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Import.Controllers
{
    public class HomeController : BaseController
    {
        const int TotalRowsToPreview = 5;
        private const string TempDataKeyImportColumns = "ImportColumns";
        private const string TempDataKeyImportRecords = "ImportRecords";

        // GET: Import/Home
        public ActionResult Index()
        {
            ImportConfigurationModel model = new ImportConfigurationModel();
            model.HasHeaderRow = true;

            ViewBag.Accounts = new SelectList(new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items, "Id", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Preview([Bind(Include = "AccountId,HasHeaderRow,ImportFile")] ImportConfigurationModel importModel)
        {
            ImportPreviewModel previewModel = new ImportPreviewModel();
            previewModel.ImportColumns = new List<ImportColumn>();
            previewModel.AccountId = importModel.AccountId;
            previewModel.CreditTotal = 0;
            previewModel.DebitTotal = 0;
            previewModel.TotalRecords = 0;
            previewModel.FailedRecords = 0;

            List<AccountModel> accounts = new AccountList(db, userKey, new AccountQuerySettings() { Sort = "Name" }).Items;
            AccountModel account = accounts.FirstOrDefault(a => a.Id == previewModel.AccountId);
            if (account != null)
            {
                AccountTypeModel accountType = AccountTypeModel.Get(db, userKey, account.TypeId);

                previewModel.AccountName = account.Name;
                previewModel.IsDebt = account.IsDebt;
                previewModel.CreditText = accountType.PositiveText;
                previewModel.DebitText = accountType.NegativeText;
            }

            StreamReader stream = new StreamReader(importModel.ImportFile.InputStream);
            var csv = new CsvReader(stream);

            // read first line
            if (importModel.HasHeaderRow)
            {
                //string firstLine = stream.ReadLine();
                //string[] values = firstLine.Split(',');

                csv.ReadHeader();
                string[] columns = csv.FieldHeaders;

                previewModel.ImportColumns = ImportPreviewModel.GenerateColumnsFromHeaders(columns);
            } else {
                //string firstLine = stream.ReadLine();
                //string[] values = firstLine.Split(',');

                csv.ReadHeader();
                string[] columns = csv.FieldHeaders;

                previewModel.ImportColumns = ImportPreviewModel.GenerateColumnsFromData(columns);

                // in case human error left checkbox unchecked
                if (previewModel.ImportColumns.Where(ic => ic.ColumnType == ColumnTypeEnum.Amount).ToList().Count == 0)
                {
                    // possibly all strings
                    List<ImportColumn> testColumns = ImportPreviewModel.GenerateColumnsFromHeaders(columns);
                    if (testColumns.Where(ic => ic.ColumnType == ColumnTypeEnum.Amount).ToList().Count > 0)
                    {
                        previewModel.ImportColumns = testColumns;
                        previewModel.HasHeaderRow = true;
                    }
                }

                if (!previewModel.HasHeaderRow)
                {
                    // return to beginning and include first line
                    importModel.ImportFile.InputStream.Position = 0;
                    stream.DiscardBufferedData();
                    csv = new CsvReader(stream);
                }
            }

            if (previewModel.IsDebt && previewModel.ImportColumns.Any(ic => ic.ColumnType == ColumnTypeEnum.AccountBalance))
            {
                previewModel.CalculateInterest = true;
            }

            List<ImportRecord> importRecords = new List<ImportRecord>();
            int recordCount = 0;
            //while (!stream.EndOfStream)
            //{
            //    string line = stream.ReadLine();
            //    string[] values = line.Split(',');
            while (csv.Read()) {
                string[] values = csv.CurrentRecord;

                // must have at least the import columns (extras can be ignored)
                if (values.Count() >= previewModel.ImportColumns.Count)
                {
                    ImportRecord record = new ImportRecord();
                    record.Sequence = recordCount;
                    record.Values = new List<ImportValue>();

                    int sequence = 0;
                    foreach (string value in values)
                    {
                        // only import columns within header definitions
                        if (sequence < previewModel.ImportColumns.Count)
                        {
                            ImportColumn colDefinition = previewModel.ImportColumns.First(s => s.Sequence == sequence);
                            // parse these and make them presentable
                            ImportValue cellValue = colDefinition.GetDataDisplay(value);

                            record.Values.Add(cellValue);
                        }
                        sequence++;
                    }

                    // if has transaction, date and amount (all valid)
                    if (record.HasColumnType(ColumnTypeEnum.Transaction) && record.HasColumnType(ColumnTypeEnum.TransactionDate) && record.HasColumnType(ColumnTypeEnum.Amount))
                    {
                        importRecords.Add(record);
                        previewModel.TotalRecords++;

                        ImportValue amount = record.GetColumnType(ColumnTypeEnum.Amount);
                        {
                            if ((!previewModel.IsDebt && amount.ValidAmount >= 0) || (previewModel.IsDebt && amount.ValidAmount < 0))
                                previewModel.CreditTotal += amount.ValidAmount;
                            else
                                previewModel.DebitTotal += amount.ValidAmount;
                        }
                    }
                    else
                    {
                        previewModel.FailedRecords++;
                    }
                } else
                {
                    previewModel.FailedRecords++;
                }

                recordCount++;
            }

            // secondary check for whether interest transactions are needed
            if (previewModel.CalculateInterest)
            {
                if (importRecords.Any(ir => ir.Values.Any(iv => iv.Column.ColumnType == ColumnTypeEnum.Transaction && iv.Data == "Interest")))
                {
                    previewModel.CalculateInterest = false;
                }
            }

            // the preview will show up to this many records
            previewModel.PreviewRecords = importRecords.Take(TotalRowsToPreview).ToList();
            
            if (account != null) {
                int mult = account.IsDebt ? -1 : 1;
                previewModel.CurrentAccountBalance = account.CurrentBalance.Value;
                previewModel.NewAccountBalance = previewModel.CurrentAccountBalance + (mult * previewModel.CreditTotal);
                previewModel.NewAccountBalance = previewModel.NewAccountBalance + (mult * previewModel.DebitTotal);
            }
            ViewBag.Accounts = new SelectList(accounts, "Id", "Name");

            //TempData[TempDataKeyImportColumns] = previewModel.ImportColumns;
            TempData[TempDataKeyImportRecords] = importRecords;

            return View(previewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process([Bind(Include = "AccountId,CalculateInterest,HasHeaderRow,HasInvertedAmounts")] ImportPreviewModel model)
        {
            AccountModel account = AccountModel.Get(db, userKey, model.AccountId);

            //if (TempData.ContainsKey(TempDataKeyImportColumns))
            //    model.ImportColumns = (List<ImportColumn>)TempData[TempDataKeyImportColumns];
            if (TempData.ContainsKey(TempDataKeyImportRecords))
                model.PreviewRecords = (List<ImportRecord>)TempData[TempDataKeyImportRecords];
            else
                return RedirectToAction("Index");

            int mult = model.HasInvertedAmounts ? -1 : 1;

            // create model to store results
            ImportProcessModel processModel = new ImportProcessModel();
            processModel.AccountName = account.Name;
            processModel.IsDebt = account.IsDebt;
            processModel.CurrentAccountBalance = account.CurrentBalance.Value;

            foreach (ImportRecord record in model.PreviewRecords.OrderBy(ir => ir.Values.First(iv => iv.ValidDate != null).ValidDate))
            {
                TransactionModel transaction = new TransactionModel();
                transaction.AccountId = model.AccountId;
                transaction.Name = record.Values.First(v => v.Column.ColumnType == ColumnTypeEnum.Transaction).Data;
                transaction.DisplayDate = record.Values.First(v => v.Column.ColumnType == ColumnTypeEnum.TransactionDate).ValidDate;

                ImportValue amount = record.Values.First(v => v.Column.ColumnType == ColumnTypeEnum.Amount && v.ValidAmount != 0);
                transaction.Amount = Math.Abs(amount.ValidAmount);
                // invert if needed
                amount.ValidAmount = mult * amount.ValidAmount;
                if (amount.Column.IsPositive.HasValue)
                {
                    if (account.IsDebt)
                        transaction.IsCredit = !amount.Column.IsPositive.Value;
                    else
                        transaction.IsCredit = amount.Column.IsPositive.Value;
                }
                else
                {
                    if (account.IsDebt)
                        transaction.IsCredit = amount.ValidAmount < 0;
                    else
                        transaction.IsCredit = amount.ValidAmount >= 0;
                }

                ImportValue postedDate = record.Values.FirstOrDefault(v => v.Column.ColumnType == ColumnTypeEnum.PostedDate);
                transaction.IsPosted = true;
                transaction.DatePosted = postedDate != null ? postedDate.ValidDate : transaction.DisplayDate;

                ImportValue category = record.Values.FirstOrDefault(v => v.Column.ColumnType == ColumnTypeEnum.Category);
                if (category != null)
                {
                    CategoryModel categoryModel = CategoryModel.FindByName(db, userKey, category.Data);
                    if (categoryModel == null)
                        categoryModel = CategoryModel.FindByName(db, userKey, transaction.Name);
                    if (categoryModel != null)
                        transaction.CategoryId = categoryModel.Id;
                }

                // TODO look for a transaction that matches on name, amount and transaction date; don't save duplicates
                //
                //
                // processModel.SkippedRecords++;

                TransactionModel.Save(transaction, db, userKey, true);

                // store results for screen

                if ((!account.IsDebt && transaction.IsCredit) || (account.IsDebt && !transaction.IsCredit))
                    processModel.CreditTotal += transaction.Amount.Value;
                else
                    processModel.DebitTotal -= transaction.Amount.Value;
                processModel.ImportedRecords++;

                if (model.CalculateInterest)
                {
                    // refresh from db
                    account = AccountModel.Get(db, userKey, model.AccountId);

                    ImportValue accountBalance = record.Values.First(v => v.Column.ColumnType == ColumnTypeEnum.AccountBalance);
                    decimal interestAmount = accountBalance.ValidAmount - account.CurrentBalance.Value;

                    // reuse transaction but as new (account, dates carry over)
                    transaction.Id = 0;
                    transaction.Name = "Interest";
                    transaction.Amount = Math.Abs(interestAmount);
                    transaction.IsCredit = true; // interest adds to debts

                    // find "interest" category
                    CategoryModel categoryModel = CategoryModel.FindByName(db, userKey, "Interest");
                    transaction.CategoryId = (categoryModel != null) ? categoryModel.Id : (int?)null;

                    TransactionModel.Save(transaction, db, userKey, true);

                    // store results for screen
                    processModel.CreditTotal += transaction.Amount.Value;
                    processModel.InterestRecords++;
                }
            }

            mult = account.IsDebt ? -1 : 1;
            processModel.NewAccountBalance = processModel.CurrentAccountBalance + (mult * processModel.CreditTotal);
            processModel.NewAccountBalance = processModel.NewAccountBalance + (mult * processModel.DebitTotal);

            return View(processModel);
        }
    }
}