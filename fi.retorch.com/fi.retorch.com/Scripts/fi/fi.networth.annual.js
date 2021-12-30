var fi = fi || {};
fi.networth = fi.networth || {};

fi.networth.annual = {
    settings: {
        startDate: new Date(),
        endDate: new Date()
    },
    table: null,
    header: null,
    body: null,
    Accounts: [],
    Years: [],
    UpdateRecords: null,
    setStartDate: function () {
        var startDateValue = $('input[name=StartDate]').val();
        fi.networth.annual.settings.startDate = new Date(startDateValue, 0, 1);
        fi.networth.annual.settings.endDate = new Date(fi.networth.annual.settings.endDate.getFullYear(), 11, 31);

        fi.networth.annual.getReportData();
    },
    getStartDate: function () {
        return fi.objects.getCleanDate(fi.networth.annual.settings.startDate);
    },
    getEndDate: function () {
        return fi.objects.getCleanDate(fi.networth.annual.settings.endDate);
    },
    findYear: function (y) {
        return $.grep(fi.networth.annual.Years, function (e) {
            return e.Year == y;
        })[0];
    },
    findAccountsThatOpened: function (accountTypeId, y) {
        return $.grep(this.Accounts, function (e) {
            if (e.data.OpeningBalance == null)
                return false;

            var dateOpened = fi.objects.convertDate(e.data.DateOpened);
            return e.data.TypeId == accountTypeId
                && dateOpened.getFullYear() == y;
        });
    },
    findAccountsThatClosed: function (accountTypeId, y) {
        return $.grep(this.Accounts, function (e) {
            if (e.data.CurrentBalance == null || e.data.TypeId != accountTypeId || e.data.DateClosed == null || e.data.CurrentBalance == 0)
                return false;

            var dateClosed = fi.objects.convertDate(e.data.DateClosed);
            return dateClosed.getFullYear() == y;
        });
    },
    findMatchingUpdateRecords: function (accountTypeId, y) {
        return $.grep(this.UpdateRecords, function (e) {
            var dateOpened = fi.objects.convertDate(e.Account.DateOpened);
            if (dateOpened.getFullYear() > y)
                return false;
            if (e.Account.DateClosed != null) {
                var dateClosed = fi.objects.convertDate(e.Account.DateClosed);
                if (dateClosed.getFullYear() < y)
                    return false;
            }
            return e.Account.TypeId == accountTypeId && e.Year == y;
        });
    },
    resetReport() {
        fi.networth.reset();

        this.Accounts = [];
        this.Years = [];
        this.UpdateRecords = null;

        this.header.text('');
        this.body.text('');
        
        var headerRow = document.createElement('tr');
        $(document.createElement('th')).text('Year').appendTo(headerRow);
        $(document.createElement('th')).text('Total').appendTo(headerRow);
        $(headerRow).appendTo(this.header);
    },
    getReportData: function () {
        $.getJSON('/Reports/NetWorth/GetAnnualData/', {
            startDate: fi.networth.annual.getStartDate(),
            endDate: fi.networth.annual.getEndDate()
        }, function (result) {
            fi.networth.annual.resetReport();
            fi.networth.annual.parseReportData(result);
        });
    },
    parseReportData: function (result) {
        var thisReport = this;

        // base parsing gets ending balance for each account type
        fi.networth.parseBaseReportData(result);

        // store accounts locally
        $(result.Accounts).each(function () {
            var object = {
                data: this
            }
            thisReport.Accounts.push(object);

            // any accounts opened or closed during report period need reversed from Current Balance to initialize
            var accountType = fi.networth.findAccountType(this.TypeId);
            var accountOpening = fi.objects.convertDate(this.DateOpened);
            if (accountOpening >= thisReport.settings.startDate) {
                accountType.RunningTotal -= parseFloat(this.OpeningBalance) * (this.IsDebt ? -1 : 1);
            }

            if (this.DateClosed) {
                var accountClosing = fi.objects.convertDate(this.DateClosed);
                if (accountClosing <= thisReport.settings.endDate) {
                    accountType.RunningTotal += parseFloat(this.CurrentBalance) * (this.IsDebt ? -1 : 1);
                }
            }
        });

        // store transactions locally
        this.UpdateRecords = result.Items.sort((a, b) => (a.Year > b.Year ? 1 : -1));

        // parse years from records
        $(result.Items).each(function () {
            var record = this;
            var Year = thisReport.findYear(record.Year);
            if (Year == null) {
                Year = {
                    Year: record.Year
                };
                thisReport.Years.push(Year);
            }

            // any transactions occurring during the report need reversed from Current Balance to initialize
            var accountType = fi.networth.findAccountType(record.Type.Id);
            accountType.RunningTotal -= parseFloat(record.TransactionTotal);
        });

        // finally use data to build report
        fi.networth.addTypeColumns(this.header);
        fi.networth.annual.buildReport();
    },
    getNextYear: function (date) {
        var nextYear = date;
        nextYear.setYear(nextYear.getFullYear() + 1);
        return nextYear;
    },
    buildReport: function () {
        var thisReport = this;

        var start = new Date(thisReport.settings.startDate);
        var end = new Date(thisReport.settings.endDate);

        var failSafe = 0;
        for (var i = start; i <= end; this.getNextYear(i)) {
            var iYear = i.getFullYear();
            var newRow = $(document.createElement('tr')).attr('data-val-year', iYear);

            var newCell = $(document.createElement('td'));
            newCell.text(iYear);
            newCell.appendTo(newRow);

            var newCell = $(document.createElement('td'));
            newCell.attr('class', 'annualTotal');
            newCell.appendTo(newRow);

            var annualTotal = 0;
            $(fi.networth.accountTypes).each(function () {
                var type = this;

                // find any accounts that opened or closed
                var accountOpenings = thisReport.findAccountsThatOpened(type.data.TypeId, iYear);
                $(accountOpenings).each(function () {
                    type.RunningTotal += parseFloat(this.data.OpeningBalance) * (this.data.IsDebt ? -1 : 1);
                });

                var accountClosings = thisReport.findAccountsThatClosed(type.data.TypeId, iYear);
                $(accountClosings).each(function () {
                    type.RunningTotal -= parseFloat(this.data.CurrentBalance) * (this.data.IsDebt ? -1 : 1);
                });

                //var accountBalances = monthYear.findAccountBalances(type.data.Id);
                var updateRecords = thisReport.findMatchingUpdateRecords(type.data.TypeId, iYear);
                $(updateRecords).each(function () {
                    type.RunningTotal += parseFloat(this.TransactionTotal);
                });

                // start with starting balance (or balance after last years changes)
                var newCell = $(document.createElement('td'));
                newCell.text(type.RunningTotal.toFixed(2)).addClass('currency');
                if (type.RunningTotal < 0)
                    newCell.addClass('negative');
                newCell.appendTo(newRow);

                // save transaction adjustments for next years total
                annualTotal += parseFloat(type.RunningTotal);
            });

            newRow.appendTo(thisReport.body);

            var colTotal = $('tr[data-val-year=' + iYear + '] td[class="annualTotal"]');
            colTotal.text(annualTotal.toFixed(2)).addClass('currency');
            if (annualTotal < 0)
                colTotal.addClass('negative');

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
            format: "yyyy",
            startView: "years",
            minViewMode: "years"
        });

        $('input[name=StartDate]').change(function () {
            var newStartDate = $(this).val();

            if (moment(newStartDate, "YYYY", true).isValid()) {
                if (fi.networth.annual.settings.startDate.getTime() != new Date(newStartDate).getTime()) {
                    fi.networth.annual.setStartDate();
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