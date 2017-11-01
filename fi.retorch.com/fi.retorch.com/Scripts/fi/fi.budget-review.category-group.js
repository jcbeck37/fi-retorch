var fi = fi || {};
fi.budget_review = fi.budget_review || {};

fi.budget_review.category_group = {
    instance: function (values) {
        this.data = values;

        this.report = null;

        this.CategoryRecords = [];
        this.Interval = '';
        this.CategoryRows = [];
        this.TimePeriodTotals = [];
        this.markup = null;
        this.thead = null;
        this.tbody = null;
        this.tfoot = null;
        this.groupName = null;
        this.colNames = null;

        this.findYear = function (i, y) {
            return $.grep(i.Years, function (e) {
                return e.Year == y;
            })[0];
        };

        this.findMonthYear = function (i, m, y) {
            return $.grep(i.MonthYears, function (e) {
                return e.Month == m && e.Year == y;
            })[0];
        };

        this.findCategoryRow = function (i, c) {
            return $.grep(i.CategoryRows, function (e) {
                return e.Id == c;
            })[0];
        };
    },
    calculateChanges: function (instance, myIndex, totalRow, lastTotal, thisTotal) {
        var total = $(document.createElement('td'));
        var amount = $(document.createElement('div')).text(thisTotal.toFixed(2)).addClass('currency');
        if (thisTotal.toFixed(2) < 0)
            amount.addClass('negative');
        amount.appendTo(total);

        if (myIndex > 0 && thisTotal != 0) {
            var changeSpan = $(document.createElement('div')).addClass('change-text');

            var change = thisTotal - lastTotal;

            var changeText = '(' + (change > 0 ? '+' : '-') + Math.abs(change).toFixed(2);

            var pct = 0;
            if (lastTotal.toFixed(0) != 0) {
                pct = Math.abs(change) * 100 / Math.abs(lastTotal);
                changeText += ' ' + (change > 0 ? '+' : '-') + pct.toFixed(0) + '%';
            }
            changeText += ')'

            changeSpan.text(changeText);

            if (change.toFixed(2) > 0 && pct.toFixed(0) != 0)
                changeSpan.addClass('positive-change');
            else if (change.toFixed(2) < 0 && pct.toFixed(0) != 0)
                changeSpan.addClass('negative-change');

            changeSpan.appendTo(total);
        }

        total.appendTo(totalRow);
    },
    build: function (instance, interval) {
        // { 'Y', 'M' }
        fi.budget_review.Interval = interval;

        // create table
        instance.markup = $(document.createElement('table')).attr('data-val', instance.data.Id).attr('class', 'table table-bordered table-striped');

        // set up column headers
        instance.thead = $(document.createElement('thead')).appendTo(instance.markup);

        instance.groupName = $(document.createElement('tr')).appendTo(instance.thead);
        var label1 = $(document.createElement('th')).text(instance.data.Name).appendTo(instance.groupName);

        instance.colNames = $(document.createElement('tr')).appendTo(instance.thead);
        var label2 = $(document.createElement('th')).text('Category').attr('class', 'categoryName').appendTo(instance.colNames);

        // set up Time Period headers
        $(fi.budget_review.TimePeriods).each(function () {
            var timePeriod = this;

            // display on page
            var colName = $(document.createElement('th'))
                .attr('data-val-year', timePeriod.Year)
                .attr('data-val-month', timePeriod.Month)
                .attr('class', 'header-currency')
                .text(fi.budget_review.Interval == 'Y' ? timePeriod.Year : (timePeriod.Month + '/' + timePeriod.Year));
            colName.appendTo(instance.colNames);
        });

        // set up rows
        instance.tbody = $(document.createElement('tbody')).appendTo(instance.markup);

        // loop through category month totals; add month/year columns and category rows as needed

        var totalTimePeriods = fi.budget_review.TimePeriods.length;
        $(instance.CategoryRecords).each(function () {
            var record = this;
            var filler = 1;
  
            // fine time period
            var TimePeriod = fi.budget_review.findTimePeriod(record.Year, record.Month);
            var tpIndex = fi.budget_review.TimePeriods.indexOf(TimePeriod);

            // set category month total here
            if (instance.TimePeriodTotals[tpIndex] == null) {
                instance.TimePeriodTotals[tpIndex] = 0;
            }
            instance.TimePeriodTotals[tpIndex] += record.Total;

            // display on page
            var categoryRow = instance.findCategoryRow(instance, record.CategoryId);
            if (categoryRow == null) {
                var colRow = $(document.createElement('tr'))
                    .attr('data-val', record.CategoryId);

                var colLabel = $(document.createElement('td'))
                    .text(record.Name).attr('class', 'categoryName');
                colLabel.appendTo(colRow);

                // find location in markup
                var insertIndex = 0;
                if (instance.CategoryRows.length == 0) {
                    colRow.appendTo(instance.tbody);
                } else {
                    var target = null;
                    $(instance.CategoryRows).each(function () {
                        if (record.Name < this.Name)
                            target = target || this.Row;
                        else
                            insertIndex++;
                    });
                    if (insertIndex == instance.CategoryRows.length)
                        colRow.appendTo(instance.tbody);
                    else
                        colRow.insertBefore(target);
                }

                // metadata
                var newRow = {
                    Id: record.CategoryId,
                    Name: record.Name,
                    Row: colRow
                };
                instance.CategoryRows.splice(insertIndex, 0, newRow);

                // reference
                categoryRow = instance.findCategoryRow(instance, record.CategoryId);
            }

            //adjust for empty data
            var cellCount = categoryRow.Row.find('td').length;
            var thisTimePeriod = tpIndex + 1;
            while (cellCount < thisTimePeriod) {
                var colFiller = $(document.createElement('td'))
                    .html("&nbsp;");
                colFiller.appendTo(categoryRow.Row);

                cellCount++;
            }

            var colData = $(document.createElement('td'))
                .addClass('currency')
                .text(record.Total);
            if (record.Total.toFixed(2) < 0)
                colData.addClass('negative');
            colData.appendTo(categoryRow.Row);
        });

        // fill in gaps
        var columns = totalTimePeriods;
        instance.groupName.find('th').attr('colspan', columns + 1);
        $(instance.CategoryRows).each(function () {
            var cellCount = this.Row.find('td').length;

            while (cellCount < columns + 1) {
                var colFiller = $(document.createElement('td'))
                    .html("&nbsp;");
                colFiller.appendTo(this.Row);

                cellCount++;
            }
        });

        // display total row
        instance.tfoot = $(document.createElement('tfoot')).appendTo(instance.markup);
        var totalRow = $(document.createElement('tr'));
        totalRow.appendTo(instance.tfoot);

        $(document.createElement('td')).text('Total:').appendTo(totalRow);

        // calculate changes in total row
        var myIndex = 0;
        var lastTotal = 0;

        $(fi.budget_review.TimePeriods).each(function () {
            var thisTotal = instance.TimePeriodTotals[myIndex] || 0;
            fi.budget_review.category_group.calculateChanges(instance, myIndex, totalRow, lastTotal, thisTotal);
            lastTotal = Number(thisTotal);
            myIndex++;
        });

        // for now, just throw in Report div; sort this out later
        instance.markup.appendTo($('#categoryGroups'));
    }
};