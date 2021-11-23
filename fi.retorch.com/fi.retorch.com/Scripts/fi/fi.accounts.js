var fi = fi || {};

fi.accounts = {
    DefaultPositiveText: 'Positive',
    DefaultNegativeText: 'Negative',
    account: function (item) {
        // set properties
        this.accountId = item.accountId;

        this.markup = item.markup;
        this.displayedBalance = item.displayedBalance;
        this.container = item.container;
        this.objects = []; // rendered markup objects

        this.currentBalance = item.currentBalance; // up to date balance from database
        this.initialBalance = item.currentBalance; // calculated as balance before visible posted transactions
        this.postedBalance = item.currentBalance; // running balance after each posted transactions
        this.visibleBalance = item.currentBalance; // running balance after each item

        this.transactions = []; // data from ajax
        this.reminders = []; // data from ajax

        // methods
        this.isItemJustBeforeComparisonItem = function (newItem, comparisonItem) {
            // this function is for "first past the goal line"
            // as soon as a comparisonItem is found that's after the newItem, we return true

            // compare posted transactions to sort into posted order
            var comparePosted = comparisonItem.posted && newItem.posted;
            
            if (newItem.date.getTime() < comparisonItem.date.getTime()) {
                // true; insert new item before comparison item
                return true;
            } else if (comparisonItem.date.getTime() == newItem.date.getTime()) {
                // transactions before reminders
                if (newItem.type == 'T' && comparisonItem.type == 'R')
                    return true;

                // stop here, we can't insert yet
                if (newItem.type == 'R' && comparisonItem.type == 'T')
                    return false;

                // posted transactions before not
                if (newItem.posted && !comparisonItem.posted)
                    return true;
                else if (!newItem.posted && comparisonItem.posted)
                    return false;

                // once posted, keep that order
                if (comparePosted)
                    return (newItem.postedDate < comparisonItem.postedDate);

                // if all else matches, higher id is after
                if (newItem.dataId < comparisonItem.dataId)
                    return true;
            }

            // new item is after
            return false;
        };

        this.updateAllBalances = function () {
            // update all items after this one
            for (var i = 0; i < this.objects.length; i++) {
                var item = this.objects[i].item;

                // if interest, calculate amount now that balances are set
                if (item.interestRate != null) {
                    // the first item is calculated in fi.reminders.js createMarkup
                    // otherwise we do it here, but only if there is a lastStoredDate
                    // TODO: But what if there is no lastStoredDate?
                    if (item.lastStoredDate != null)
                        fi.reminders.calculateInterestAmount(this, this.objects[i]);
                }

                // calculate
                if (i == 0) {
                    this.visibleBalance = this.initialBalance + parseFloat(item.amount);
                    this.postedBalance = this.initialBalance + (item.posted ? parseFloat(item.amount) : 0);
                } else {
                    this.visibleBalance += parseFloat(item.amount);
                    this.postedBalance += (item.posted ? parseFloat(item.amount) : 0);
                }

                // apply to markup
                this.objects[i].updateBalance(this.postedBalance, this.visibleBalance);
            }

            this.displayedBalance.removeClass('negative');
            this.displayedBalance.text(this.postedBalance);
            if (parseFloat(this.postedBalance).toFixed(2) < 0) {
                //console.log("Negative: " + parseFloat(this.postedBalance).toFixed(2));
                this.displayedBalance.addClass('negative');
            }
        };

        this.placeItemInAccount = function (item, object) {
            var insertIndex = 0;
            var nextItem = null;
            //var prevItem = null;

            for (var i = 0; i < this.objects.length; i++) {
                if (this.isItemJustBeforeComparisonItem(item, this.objects[i].item)) {
                    nextItem = this.objects[i];
                    insertIndex = i;
                    break;
                } else {
                    //prevItem = this.objects[i];
                    insertIndex++;
                }
            }

            this.objects.splice(insertIndex, 0, object);

            // place on page
            if (nextItem) {
                $(nextItem.markup).before(object.markup);
            } else {
                object.markup.appendTo(this.container);
            }
        };

        this.removeObject = function (type, dataId) {
            this.objects = $.grep(this.objects, function (e) {
                return !(e.item.type == type && e.item.dataId == dataId);
            });
        };

        this.removeDateObject = function (type, dataId, date) {
            this.objects = $.grep(this.objects, function (e) {
                return !(e.item.type == type && e.item.dataId == dataId && e.item.date.getTime() == date.getTime());
            });
        };
        
        this.getTransaction = function (transactionId) {
            return $.grep(this.transactions, function (e) {
                return e.Id == transactionId;
            })[0];
        };

        this.removeTransaction = function (transactionId) {
            this.transactions = $.grep(this.transactions, function (e) {
                return e.Id != transactionId;
            });
        };

        this.removeReminder = function (reminderId) {
            this.reminders = $.grep(this.reminders, function (e) {
                return e.Id != reminderId;
            });
        };
    },
    setup: function () {
        $('.datepicker').datepicker({
            orientation: "bottom auto",
            autoclose: true
        });
    }
};