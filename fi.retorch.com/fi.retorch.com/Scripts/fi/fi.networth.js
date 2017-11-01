var fi = fi || {};
fi.networth = fi.networth || {};

fi.networth = {
    accountTypes: [],
    compareAccountTypes: function (a, b) {
        if (a.data.TypeId < b.data.TypeId)
            return -1;
        if (a.data.TypeId > b.data.TypeId)
            return 1;
        return 0;
    },
    findAccountType: function (accountTypeId) {
        return $.grep(fi.networth.accountTypes, function (e) {
            return e.data.TypeId == accountTypeId;
        })[0];
    },
    reset() {
        this.accountTypes = [];
    },
    formatReportCurrency: function () {
        $('#report').find('.currency').each(function () {
            fi.objects.formatCurrency(this, 0);
        })
    },
    parseBaseReportData: function(result, startDate, endDate) {
        // create account type objects with totals
        $(result.AccountTypes).each(function () {
            // group each into Account Types
            var thisType = fi.networth.findAccountType(this.AccountType.Id);
            if (thisType == null) {
                thisType = {
                    data: {
                        TypeId: this.AccountType.Id,
                        TypeName: this.AccountType.Name,
                        IsDebt: this.AccountType.IsDebt
                    },
                    RunningTotal: this.EndingBalance
                };
                fi.networth.accountTypes.push(thisType);
            } else {
                thisType.RunningTotal += parseFloat(this.EndingBalance);
            }
        });

        // order records by type
        fi.networth.accountTypes.sort(fi.networth.compareAccountTypes);
    },
    addTypeColumns: function (header) {        
        $(fi.networth.accountTypes).each(function () {
            var newCell = $(document.createElement('th'));
            newCell.text(this.data.TypeName);

            newCell.appendTo(header.find('tr'));
        });
    }
};