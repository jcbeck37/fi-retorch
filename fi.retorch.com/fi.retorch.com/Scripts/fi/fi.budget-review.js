var fi = fi || {};

fi.budget_review = {
    Title: 'Summary',
    categoryGroups: [],
    TimePeriods: [],
    colNames: [],
    settings: {
        startDate: new Date(),
        endDate: new Date(),
        markup: null,
        table: null,
        thead: null,
        tbody: null
    },
    findTimePeriod: function (y, m) {
        return $.grep(fi.budget_review.TimePeriods, function (e) {
            return e.Year == y && (m == undefined || e.Month == m);
        })[0];
    },
    findCategoryGroup: function (g) {
        return $.grep(fi.budget_review.categoryGroups, function (e) {
            return e.data.Id == g;
        })[0];
    },
    getStartDate: function () {
        return fi.objects.getCleanDate(fi.budget_review.settings.startDate);
    },
    getEndDate: function () {
        return fi.objects.getCleanDate(fi.budget_review.settings.endDate);
    },
    formatReportCurrency: function () {
        $('#report').find('.currency').each(function () {
            fi.objects.formatCurrency(this, 2);
        })
    },
    resetReport: function () {
        this.categoryGroups = [];
        this.TimePeriods = [];

        $('#summary').text('');
        $('#categoryGroups').text('');
    },
    loadBaseReport: function (result) {
        var totalRecords = 0;
        $(result).each(function () {
            var record = this;

            // create all Time Periods as needed
            if (fi.budget_review.findTimePeriod(record.Year, record.Month) == null)
            {
                var insertIndex = 0;
                $(fi.budget_review.TimePeriods).each(function () {
                    if (this.Year < record.Year) {
                        insertIndex++;
                    } else if (this.Year == record.Year && record.Month != undefined) {
                        if (this.Month < record.Month) {
                            insertIndex++;
                        }
                    }
                });

                var TimePeriod = {
                    Year: record.Year,
                    Month: record.Month,
                    ExpenseTotal: 0,
                    IncomeTotal: 0,
                    TransferTotal: 0,
                    RecordTotal: 0,
                    CategoryTotals: [],
                    Column: null
                };
                totalRecords++;

                fi.budget_review.TimePeriods.splice(insertIndex, 0, TimePeriod);
            }

            // create category group object which will contain sub-table
            var categoryGroup = fi.budget_review.findCategoryGroup(record.Category.GroupId);
            if (categoryGroup == null) {
                var categoryGroup = new fi.budget_review.category_group.instance({
                    Id: record.Category.GroupId,
                    Name: record.Category.GroupName,
                    TransferType: record.Category.TransferType
                });
                fi.budget_review.categoryGroups.push(categoryGroup);
                //categoryGroup = fi.budget_review.findCategoryGroup(record.Category.GroupId);
            }
        });
    },
    buildTransferTotal: function (target, type, summary) {
        var typeName = '';
        switch (type) {
            case 0:
                typeName = 'Expense';
                break;
            case 1:
                typeName = 'Income';
                break;
            case 2:
                typeName = 'Transfer';
                break;
        }

        var transferRow = $(document.createElement('tr')).attr('class', 'transfer-total');
        $(document.createElement('th')).text(typeName + ' Total:').appendTo(transferRow);

        $(summary.TimePeriods).each(function () {
            // transfer
            var typeTotal = 0;
            switch (type) {
                case 0:
                    typeTotal = this.ExpenseTotal;
                    break;
                case 1:
                    typeTotal = this.IncomeTotal;
                    break;
                case 2:
                    typeTotal = this.TransferTotal;
                    break;
            }

            var transferTotal = $(document.createElement('th'));
            var transferAmount = $(document.createElement('div')).text(typeTotal.toFixed(2)).addClass('currency');
            if (typeTotal.toFixed(2) < 0)
                transferAmount.addClass('negative');
            transferAmount.appendTo(transferTotal);

            transferTotal.appendTo(transferRow);
        });

        transferRow.appendTo(target);
    },
    buildGroupSummary: function () {
        var summary = this;

        // create table
        summary.markup = $(document.createElement('table')).attr('class', 'table table-bordered table-striped');

        // build header row
        summary.thead = $(document.createElement('thead')).appendTo(summary.markup);

        summary.reportName = $(document.createElement('tr')).appendTo(summary.thead);
        var label1 = $(document.createElement('th')).text(summary.Title).appendTo(summary.reportName);

        summary.colNames = $(document.createElement('tr')).appendTo(summary.thead);
        var label2 = $(document.createElement('th')).text('Group').attr('class', 'groupName').appendTo(summary.colNames);

        $(summary.TimePeriods).each(function () {
            var timePeriod = this;
            var colName = $(document.createElement('th'))
                .attr('data-val-year', timePeriod.Year)
                .attr('data-val-month', timePeriod.Month)
                .attr('class', 'header-currency')
                .text(fi.budget_review.Interval == 'Y' ? timePeriod.Year : (timePeriod.Month + '/' + timePeriod.Year));
            colName.appendTo(summary.colNames);
        });

        // build data rows
        var lastGroupTransferType = -1;
        summary.tbody = $(document.createElement('tbody')).appendTo(summary.markup);
        $(fi.budget_review.categoryGroups).each(function () {
            var group = this;

            // after each set of transfer type groups, show transfer type total
            if (group.data.TransferType != lastGroupTransferType && lastGroupTransferType > -1) {
                fi.budget_review.buildTransferTotal(summary.tbody, lastGroupTransferType, summary);
            }

            lastGroupTransferType = group.data.TransferType;

            // create summary row
            var groupRow = $(document.createElement('tr'));

            var colLabel = $(document.createElement('td'))
                .text(group.data.Name).attr('class', 'categoryName');
            colLabel.appendTo(groupRow);

            var tpIndex = 0;
            $(summary.TimePeriods).each(function () {
                var thisTotal = group.TimePeriodTotals[tpIndex];
                if (thisTotal == undefined)
                    thisTotal = 0;

                var colData = $(document.createElement('td'))
                    .addClass('currency')
                    .text(thisTotal.toFixed(2));
                if (thisTotal.toFixed(2) < 0)
                    colData.addClass('negative');
                colData.appendTo(groupRow);

                this.RecordTotal += thisTotal;
                switch (group.data.TransferType)
                {
                    case 0:
                        this.ExpenseTotal += thisTotal;
                        break;
                    case 1:
                        this.IncomeTotal += thisTotal;
                        break;
                    case 2:
                        this.TransferTotal += thisTotal;
                        break;
                }
                tpIndex++;
            });

            groupRow.appendTo(summary.tbody);
        });

        // display total rows
        summary.tfoot = $(document.createElement('tfoot')).appendTo(summary.markup);
        
        var totalRow = $(document.createElement('tr')).attr('class', 'overall-total');
        $(document.createElement('th')).text('Total:').appendTo(totalRow);
        totalRow.appendTo(summary.tfoot);

        // final transfer type
        if (lastGroupTransferType > -1) {
            fi.budget_review.buildTransferTotal(summary.tbody, lastGroupTransferType, summary);
        }

        $(summary.TimePeriods).each(function () {
            // overall
            var total = $(document.createElement('th'));
            var amount = $(document.createElement('div')).text(this.RecordTotal.toFixed(2)).addClass('currency');
            if (this.RecordTotal.toFixed(2) < 0)
                amount.addClass('negative');
            amount.appendTo(total);

            total.appendTo(totalRow);
        });

        // display
        summary.reportName.find('th').attr('colspan', summary.TimePeriods.length + 1);
        summary.markup.appendTo($('#summary'));
    }
};