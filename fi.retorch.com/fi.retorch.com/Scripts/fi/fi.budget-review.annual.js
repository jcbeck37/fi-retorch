var fi = fi || {};
fi.budget_review = fi.budget_review || {};

fi.budget_review.annual = {
    setStartDate: function (loadOptions) {
        var startDateValue = $('input[name=StartDate]').val();
        fi.budget_review.settings.startDate = new Date(startDateValue, 0, 1);

        if (loadOptions.getReport)
            fi.budget_review.annual.getReport();
    },
    setEndDate: function (loadOptions) {
        var endDateValue = $('input[name=EndDate]').val();
        fi.budget_review.settings.endDate = new Date(Number(endDateValue) + 1, 0, 0); // last day of year

        if (loadOptions.getReport)
            fi.budget_review.annual.getReport();
    },
    getReport: function () {
        $.getJSON('/Reports/BudgetReview/GetAnnualBudgetReview/', {
            startDate: fi.budget_review.getStartDate(),
            endDate: fi.budget_review.getEndDate()
        }, function (result) {
            fi.budget_review.resetReport();
            fi.budget_review.annual.loadReport(result);
        });
    },
    loadReport: function (result) {
        var records = [];
        
        // repair data as needed
        $(result).each(function () {
            var record = this;

            // null will be set to 0 => "Uncategorized"
            record.Category = (record.Category == null) ? { Id: 0, Name: "Uncategorized" } : record.Category;
            record.Category.GroupId = (record.Category.GroupId == null) ? 0 : record.Category.GroupId;
            record.Category.GroupName = (record.Category.GroupName == null) ? "Uncategorized" : record.Category.GroupName;
            record.Category.TransferType = (record.Category.TransferType == null) ? 0 : record.Category.TransferType;

            records.push(record);
        });

        // build list of time periods and category groups
        fi.budget_review.loadBaseReport(records);

        // group records into category groups
        $(records).each(function () {
            var record = this;

            var categoryGroup = fi.budget_review.findCategoryGroup(record.Category.GroupId);
            
            var categoryYearTotal = {
                Year: record.Year,
                CategoryId: record.Category.Id,
                GroupId: record.Category.GroupId,
                Name: record.Category.Name,
                Total: record.Total
            }

            categoryGroup.CategoryRecords.push(categoryYearTotal);
        });

        // this creates each categroy group sub-table
        fi.budget_review.report = fi.budget_review.annual;
        $(fi.budget_review.categoryGroups).each(function () {
            fi.budget_review.category_group.build(this, 'Y');
        });

        // create a summary table with category group line items
        fi.budget_review.buildGroupSummary();

        // format all currency
        fi.budget_review.formatReportCurrency();
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
            var date = $(this).val();

            if (moment(date, "YYYY", true).isValid() && fi.budget_review.settings.startDate.getTime() != new Date(date).getTime()) {
                fi.budget_review.annual.setStartDate({ getReport: true });
            }
        });

        $('input[name=EndDate]').change(function () {
            var date = $(this).val();

            if (moment(date, "YYYY", true).isValid() && fi.budget_review.settings.endDate.getTime() != new Date(date).getTime()) {
                fi.budget_review.annual.setEndDate({ getReport: true });
            }
        });

        fi.budget_review.annual.setStartDate({ getReport: false });
        fi.budget_review.annual.setEndDate({ getReport: true });
    }
};