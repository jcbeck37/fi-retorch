var fi = fi || {};

fi.objects = {
    item: function (values) {
        this.dataId = values.dataId;
        this.accountId = values.accountId;
        this.type = values.type;
        this.date = values.date;
        this.name = values.name;
        this.positive = values.positive;
        this.amount = values.amount * (this.positive ? 1 : -1);

        // action links
        this.editLink = values.editLink;
        this.postLink = values.postLink;
        this.goLink = values.goLink;

        // transactions
        this.posted = values.posted;
        this.order = values.order;
        if (this.posted)
            this.postedDate = values.postedDate;
        
        // reminders
        this.label = values.label;
        //this.reminderType = values.reminderType;
        this.scheduleId = values.scheduleId;
        //this.scheduleName = values.scheduleName;
        this.interestRate = values.interestRate;
        this.previousBalance = values.previousBalance;
        this.lastStoredDate = values.lastStoredDate;
        //this.lastDate = values.lastDate;
    },
    convertDate: function (string) {
        return new Date(parseInt(string.substr(6)));
    },
    getCleanDate: function (date) {
        return date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
    },
    compareDate: function (a, b) {
        if (a.date < b.date)
            return -1;
        if (a.date > b.date)
            return 1;
        if (a.type == 'T' && b.type == 'R')
            return 1;
        if (a.type == 'R' && b.type == 'T')
            return -1;
        if (a.posted && !b.posted)
            return 1;
        if (!a.posted && b.posted)
            return -1;
        return 0;
    },
    // a and b are javascript Date objects
    dateDiffInDays: function (a, b) {
        var _MS_PER_DAY = 1000 * 60 * 60 * 24;

        // Discard the time and time-zone information.
        var utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
        var utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());

        return Math.floor((utc2 - utc1) / _MS_PER_DAY);
    },
    daysInMonth: function(month, year) {
        return new Date(year, month+1, 0).getDate();
    },
    formatCurrency: function (selector, digits) {
        digits = (digits == null ? 2 : digits);
        var value = $(selector).text();
        value = value.replace('-', '');

        // so repeated formatting doesn't break
        value = value.replace('$', '');
        value = value.replace(',', '');

        if (digits == 2)
            $(selector).text('$' + parseFloat(value, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
        else
            $(selector).text('$' + parseFloat(value, 10).formatMoney(digits).toString());
    }
};