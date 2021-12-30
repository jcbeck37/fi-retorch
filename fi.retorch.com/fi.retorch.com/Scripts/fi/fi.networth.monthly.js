var fi = fi || {};
fi.networth = fi.networth || {};

fi.networth.monthly = {
    settings: {
        monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
        startDate: new Date(),
        endDate: new Date()
    },
    table: null,
    header: null,
    body: null,
    MonthYears: [],
    Accounts: [],
    UpdateRecords: null,
    setStartDate: function () {
        var startDateValue = $('input[name=StartDate]').val();
        fi.networth.monthly.settings.startDate = new Date(startDateValue.split('-')[1], startDateValue.split('-')[0] - 1, 1);

        fi.networth.monthly.getReportData();
    },
    getStartDate: function () {
        return fi.objects.getCleanDate(fi.networth.monthly.settings.startDate);
    },
    getEndDate: function () {
        var date = fi.networth.monthly.getNextMonth(fi.networth.monthly.settings.endDate);
        date.setDate(1);
        date.setDate(date.getDate() - 1);

        return fi.objects.getCleanDate(date);
        
    },
    findMonthYear: function (m, y) {
        return $.grep(fi.networth.monthly.MonthYears, function (e) {
            return e.Month == m && e.Year == y;
        })[0];
    },
    findAccountsThatOpened: function (accountTypeId, m, y) {
        return $.grep(this.Accounts, function (e) {
            if (e.data.OpeningBalance == null)
                return false;

            var dateOpened = fi.objects.convertDate(e.data.DateOpened);
            return e.data.TypeId == accountTypeId
                && dateOpened.getFullYear() == y && (dateOpened.getMonth() + 1) == m;
        });
    },
    findAccountsThatClosed: function (accountTypeId, m, y) {
        return $.grep(this.Accounts, function (e) {
            if (e.data.CurrentBalance == null || e.data.TypeId != accountTypeId || e.data.DateClosed == null || e.data.CurrentBalance == 0)
                return false;

            var dateClosed = fi.objects.convertDate(e.data.DateClosed);
            return dateClosed.getFullYear() == y && (dateClosed.getMonth() + 1) == m;
        });
    },
    findMatchingUpdateRecords: function (accountTypeId, m, y) {
        return $.grep(fi.networth.monthly.UpdateRecords, function (e) {
            //var dateOpened = fi.objects.convertDate(e.Account.DateOpened);
            //if (dateOpened.getFullYear() > y)
            //    return false;
            //if (dateOpened.getFullYear() == y && (dateOpened.getMonth() + 1) > m)
            //    return false;
            //if (e.Account.DateClosed != null) {
            //    var dateClosed = fi.objects.convertDate(e.Account.DateClosed);
            //    if (dateClosed.getFullYear() < y)
            //        return false;
            //    if (dateClosed.getFullYear() == y && (dateClosed.getMonth() + 1) < m)
            //        return false;
            //}
            return e.Account.TypeId == accountTypeId && e.Year == y && e.Month == m;
        });
    },
    resetReport() {
        fi.networth.reset();

        this.MonthYears = [];
        this.Accounts = [];
        this.UpdateRecords = null;

        this.header.text('');
        this.body.text('');
        
        var headerRow = document.createElement('tr');
        $(document.createElement('th')).text('Month').appendTo(headerRow);
        $(document.createElement('th')).text('Total').appendTo(headerRow);
        $(headerRow).appendTo(this.header);
    },
    getReportData: function () {
        $.getJSON('/Reports/NetWorth/GetMonthlyData/', {
            startDate: fi.networth.monthly.getStartDate(),
            endDate: fi.networth.monthly.getEndDate()
        }, function (result) {
            fi.networth.monthly.resetReport();
            fi.networth.monthly.parseReportData(result);
        });
    },
    parseReportData: function (result) {
        //var thisReport = this;

        // base parsing
        fi.networth.parseBaseReportData(result);

        // store accounts locally
        $(result.Accounts).each(function () {
            var object = {
                data: this
            }
            fi.networth.monthly.Accounts.push(object);

            // any accounts opened or closed during report period need reversed from Current Balance to initialize
            var accountType = fi.networth.findAccountType(this.TypeId);
            var accountOpening = fi.objects.convertDate(this.DateOpened);
            if (accountOpening >= fi.networth.monthly.settings.startDate) {
                accountType.RunningTotal -= parseFloat(this.OpeningBalance) * (this.IsDebt ? -1 : 1);
            }

            if (this.DateClosed) {
                var accountClosing = fi.objects.convertDate(this.DateClosed);
                if (accountClosing <= fi.networth.monthly.settings.endDate) {
                    accountType.RunningTotal += parseFloat(this.CurrentBalance) * (this.IsDebt ? -1 : 1);
                }
            }
        });

        // result will be { Month, Year, Account, AccountType, CurrentBalance, TransactionTotal, MonthStartBalance
        fi.networth.monthly.UpdateRecords = result.Items;

        // parse month years from records
        $(result.Items).each(function () {
            var record = this;
            var MonthYear = fi.networth.monthly.findMonthYear(record.Month, record.Year);
            if (MonthYear == null) {
                MonthYear = {
                    Year: record.Year,
                    Month: record.Month
                };
                fi.networth.monthly.MonthYears.push(MonthYear);
            }

            // any transactions occurring during the report need reversed from Current Balance to initialize
            var accountType = fi.networth.findAccountType(record.Type.Id);
            accountType.RunningTotal -= parseFloat(record.TransactionTotal);
        });

        // finally use data to build report
        fi.networth.addTypeColumns(this.header);
        fi.networth.monthly.buildReport();
    },
    getNextMonth: function (date) {
        var nextMonth = date;
        nextMonth.setMonth(nextMonth.getMonth() + 1);
        return nextMonth;
    },
    //getPreviousMonth: function (date) {
    //    var nextMonth = date;
    //    nextMonth.setMonth(nextMonth.getMonth() - 1);
    //    return nextMonth;
    //},
    buildReport: function () {
        var thisReport = this;

        var start = new Date(thisReport.settings.startDate);
        var end = new Date(thisReport.settings.endDate);
        
        var failSafe = 0;
        for (var i = start; i <= end; i = thisReport.getNextMonth(i)) {
            var iMonth = i.getMonth() + 1;
            var iYear = i.getFullYear();
            var newRow = $(document.createElement('tr')).attr('data-val-month', iMonth).attr('data-val-year', iYear);

            var newCell = $(document.createElement('td'));
            newCell.text(iMonth + '/' + iYear);
            newCell.appendTo(newRow);

            var newCell = $(document.createElement('td'));
            newCell.attr('class', 'monthTotal');
            newCell.appendTo(newRow);

            var monthTotal = 0;
            $(fi.networth.accountTypes).each(function () {
                var type = this;

                // find any accounts that opened or closed
                var accountOpenings = thisReport.findAccountsThatOpened(type.data.TypeId, iMonth, iYear);
                $(accountOpenings).each(function () {
                    type.RunningTotal += parseFloat(this.data.OpeningBalance) * (this.data.IsDebt ? -1 : 1);
                });

                var accountClosings = thisReport.findAccountsThatClosed(type.data.TypeId, iMonth, iYear);
                $(accountClosings).each(function () {
                    type.RunningTotal -= parseFloat(this.data.CurrentBalance) * (this.data.IsDebt ? -1 : 1);
                });

                var updateRecords = thisReport.findMatchingUpdateRecords(type.data.TypeId, iMonth, iYear);
                $(updateRecords).each(function () {
                    type.RunningTotal += parseFloat(this.TransactionTotal);
                });

                // start with starting balance (or balance after last months changes)
                var newCell = $(document.createElement('td'));
                newCell.text(type.RunningTotal.toFixed(2)).addClass('currency');
                if (type.RunningTotal.toFixed(2) < 0)
                    newCell.addClass('negative');
                newCell.appendTo(newRow);

                // save transaction adjustments for next months total
                monthTotal += parseFloat(type.RunningTotal);
            });

            newRow.appendTo(thisReport.body);

            var colTotal = $('tr[data-val-month=' + iMonth + '][data-val-year=' + iYear + '] td[class="monthTotal"]');
            colTotal.text(monthTotal.toFixed(2)).addClass('currency');
            if (monthTotal < 0)
                colTotal.addClass('negative');

            //console.log(i);
            failSafe++;
            if (failSafe > 999) {
                break;
            }
        }

        fi.networth.formatReportCurrency();
    },
    setup: function () {
        $('.datepicker').datepicker({
            orientation: "bottom auto",
            autoclose: true,
            format: "mm-yyyy",
            startView: "months",
            minViewMode: "months"
        });

        $('input[name=StartDate]').change(function () {
            var newStartDate = $(this).val();

            if (moment(newStartDate, "MM-YYYY", true).isValid()) {
                if (fi.networth.monthly.settings.startDate.getTime() != new Date(newStartDate).getTime()) {
                    fi.networth.monthly.setStartDate();
                }
            }
        });

        // set up markup
        this.table = $('#balances');
        this.header = this.table.find('thead');
        this.body = this.table.find('tbody');

        this.setStartDate();
    }
};