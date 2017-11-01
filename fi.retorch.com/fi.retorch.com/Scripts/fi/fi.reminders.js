var fi = fi || {};

fi.reminders = {
    schedules: Object.freeze({
        Daily: 1,
        Weekly: 2,
        BiWeekly: 3,
        SemiMonthly: 4,
        Monthly: 5,
        Quarterly: 6,
        SemiAnnual: 7,
        Annual: 8
    }),
    reminder: function (item) {
        // set properties
        this.item = item; // contains original reminder object (fi.objects.item)
        this.resultBalance = 0.0;
        this.postedBalance = 0.0;
        this.markup = null;

        // methods
        this.createMarkup = function (account) {
            // creates element
            this.markup = $(document.createElement('div'));

            this.markup.attr('class', 'reminder')
                .attr('data-val', this.item.dataId);

            // date
            var date = this.item.date;
            var dateFormatted = fi.objects.getCleanDate(date);
            this.markup.attr('data-date', dateFormatted);

            // reminder
            var item = $(document.createElement('span'))
                .attr('class', 'item');
            $(document.createElement('span'))
                .attr('class', 'date')
                .text(dateFormatted)
                .appendTo(this.markup);

            var name = $(document.createElement('a'))
                .attr('class', 'edit')
                .text(this.item.label);
            name.attr('href', this.item.editLink);
            name.appendTo(item);

            // actions
            var actions = $(document.createElement('a'))
                .attr('class', 'actions go').attr('data-toggle', 'tooltip').attr('title', 'Log transaction')
                .text('Go');
            actions.attr('href', this.item.goLink + '&reminderDate=' + fi.objects.getCleanDate(this.item.date));
            actions.appendTo(item);

            item.appendTo(this.markup);

            // calculate amount from interest
            if (this.item.interestRate != null && this.item.previousBalance != null && this.item.lastStoredDate == null) {
                if (this.item.date.getDate() > 1) {
                    var oldDate = new moment(this.item.date).subtract(1, 'months');
                    var days = new moment(this.item.date).diff(oldDate, 'days');
                    this.item.amount = fi.reminders.calculateSimpleInterest(this.item.previousBalance, this.item.interestRate, this.item.scheduleId, days);
                } else {
                    // assume mortgage
                    this.item.amount = fi.reminders.calculateSimpleInterest(this.item.previousBalance, this.item.interestRate, this.item.scheduleId, 0);
                }
            }

            // amount
            $(document.createElement('span'))
                .attr('class', 'amount currency ' + (this.item.positive ? 'positive' : 'negative'))
                .text(this.item.amount)
                .appendTo(this.markup);

            // balance
            $(document.createElement('span'))
                .attr('class', 'balance currency')
                .appendTo(this.markup);

            return this.markup;
        };

        this.updateBalance = function (postedBalance, visibleBalance) {
            this.postedBalance = postedBalance;
            this.resultBalance = visibleBalance;

            var displayBalance = this.item.posted ? this.postedBalance : this.resultBalance;
            var divBalance = this.markup.find('.balance');
            divBalance.removeClass('positive').removeClass('negative').addClass(displayBalance.toFixed(2) >= 0 ? 'positive' : 'negative');
            divBalance.text(displayBalance.toFixed(2));
        };
    },
    convertDataToItem: function (data, d, lastStoredDate) {
        return new fi.objects.item({
            dataId: data.Id,
            accountId: data.AccountId,
            type: 'R',
            label: data.Name,
            scheduleId: data.ReminderScheduleId,
            //scheduleName: data.ReminderScheduleName,
            date: new Date(d),
            //lastDate: data.LastDate,
            amount: data.Amount,
            positive: data.IsCredit,
            interestRate: data.Rate,
            previousBalance: data.PreviousBalance,
            lastStoredDate: lastStoredDate == null ? null : new Date(lastStoredDate),
            posted: false,
            editLink: data.EditLink,
            goLink: data.GoLink
        });
    },
    calculateDailyInterest: function (balance, rate, days) {
        var total = parseFloat(balance);
        var interest = parseFloat(rate) / 36500;

        var accumulated = days * total * interest;
        return accumulated.toFixed(2);
    },
    calculateMonthlyInterest: function (balance, rate, months) {
        var total = parseFloat(balance);
        var interest = parseFloat(rate) / 1200;

        var accumulated = months * total * interest;
        return accumulated.toFixed(2);
    },
    calculateSimpleInterest: function (balance, rate, schedule, days) {
        var accumulated = 0;
        switch (schedule) {
            case fi.reminders.schedules.Daily: //'Daily':
                accumulated = fi.reminders.calculateDailyInterest(balance, rate, 1);
                break;
            case fi.reminders.schedules.Weekly: //'Weekly':
                accumulated = fi.reminders.calculateDailyInterest(balance, rate, 7);
                break;
            case fi.reminders.schedules.BiWeekly: //'Bi-weekly':
                accumulated = fi.reminders.calculateDailyInterest(balance, rate, 14);
                break;
            case fi.reminders.schedules.SemiMonthly: //'Semi-monthly':
                accumulated = fi.reminders.calculateMonthlyInterest(balance, rate, 1);
                accumulated = (accumulated / 2.0).toFixed(2);
                break;
            case fi.reminders.schedules.Monthly: //'Monthly':
                if (days == 0) {
                    accumulated = fi.reminders.calculateMonthlyInterest(balance, rate, 1);
                } else {
                    accumulated = fi.reminders.calculateDailyInterest(balance, rate, days);
                }
                break;
            case fi.reminders.schedules.Quarterly: //'Quarterly':
                accumulated = fi.reminders.calculateMonthlyInterest(balance, rate, 3);
                break;
            case fi.reminders.schedules.SemiAnnual: //'Semi-annual':
                accumulated = fi.reminders.calculateMonthlyInterest(balance, rate, 6);
                break;
            case fi.reminders.schedules.Annual: //'Annual':
                accumulated = fi.reminders.calculateMonthlyInterest(balance, rate, 12);
                break;
        }
        return accumulated;
    },
    calculateInterestAmount: function (account, reminder) {
        var item = reminder.item;

        // first item will use previous balance, all others calculate here
        if (item.lastStoredDate != null) {
            var oldBalance = 0;
            var itemBalance = 0;
            for (var j = 0; j < account.objects.length; j++) {
                if (account.objects[j].item.date > item.date)
                    break;

                // get first transaction since then
                if (account.objects[j].item.date >= item.lastStoredDate && oldBalance == 0) {
                    oldBalance = account.objects[j].resultBalance;

                    // remove transaction if between now and last reminder
                    if (account.objects[j].item.date > item.lastStoredDate)
                        oldBalance -= item.amount;
                }
            }

            if (oldBalance != 0) {
                item.previousBalance = oldBalance;
            }

            // finally calculate interest
            var days = 0;
            if (reminder.scheduleId == fi.reminders.schedules.Monthly && item.date.getDate() > 1) {
                var oldDate = new Date(item.date);
                oldDate.setMonth(oldDate.getMonth() - 1);
                days = fi.objects.dateDiffInDays(oldDate, item.date); // only used for monthly
            }
            item.amount = fi.reminders.calculateSimpleInterest(item.previousBalance, item.interestRate, item.scheduleId, days);

            var amount = reminder.markup.find('.amount');
            amount.text(item.amount).removeClass('negative');
            if (item.amount < 0)
                amount.addClass('negative');
        }
    },
    buildReminder: function (account, item) {
        // find spot to place it
        var insertIndex = 0;
        var nextItem = null;
        var prevItem = null;
        for (var i = 0; i < account.objects.length; i++) {
            if (account.isItemJustBeforeComparisonItem(item, account.objects[i].item)) {
                nextItem = account.objects[i];
                insertIndex = i;
                break;
            } else {
                prevItem = account.objects[i];
                insertIndex++;
            }
        }

        // create
        var objReminder = new fi.reminders.reminder(item);
        objReminder.createMarkup(account);
        account.objects.splice(insertIndex, 0, objReminder)

        // place on page
        if (nextItem) {
            $(nextItem.markup).before(objReminder.markup);
        } else {
            objReminder.markup.appendTo(account.container);
        }

        // set up transaction edit to open modal
        objReminder.markup.find('.edit').click(function () {
            fi.reminders.setupEdit(this, objReminder);

            return false;
        });

        // handle post
        objReminder.markup.find('.actions.go').click(function () {
            fi.reminders.setupGo(this, account, objReminder);

            return false;
        });
    },
    addReminder: function (account, reminder) {
        // calculate each occurrence of the reminder from it's next date until the forecast end date
        switch (reminder.ReminderScheduleId) {
            case fi.reminders.schedules.Daily:
                fi.reminders.displayDailyReminder(account, reminder, 1);
                break;
            case fi.reminders.schedules.Weekly:
                fi.reminders.displayDailyReminder(account, reminder, 7);
                break;
            case fi.reminders.schedules.BiWeekly:
                fi.reminders.displayDailyReminder(account, reminder, 14);
                break;
            case fi.reminders.schedules.SemiMonthly:
                fi.reminders.displaySemiMonthlyReminder(account, reminder);
                break;
            case fi.reminders.schedules.Monthly:
                fi.reminders.displayMonthlyReminder(account, reminder, 1);
                break;
            case fi.reminders.schedules.Quarterly:
                fi.reminders.displayMonthlyReminder(account, reminder, 3);
                break;
            case fi.reminders.schedules.SemiAnnual:
                fi.reminders.displayMonthlyReminder(account, reminder, 6);
                break;
            case fi.reminders.schedules.Annual:
                fi.reminders.displayMonthlyReminder(account, reminder, 12);
                break;
        }
    },
    displayDailyReminder: function (account, reminder, days) {
        // calculate dates
        var startDate = new Date(parseInt(reminder.NextDate.substr(6)));
        var endDate = new Date(fi.dashboard.settings.endDate);

        var lastStoredDate = null;
        for (var d = startDate; d <= endDate; d.setDate(d.getDate() + days)) {
            if (reminder.LastDate && fi.objects.convertDate(reminder.LastDate) < d)
                break;

            // copy item to item but with new date
            fi.reminders.buildReminder(account, fi.reminders.convertDataToItem(reminder, d, lastStoredDate));
            lastStoredDate = new Date(d);
        }
    },
    displaySemiMonthlyReminder: function (account, reminder) {
        // calculate dates
        var startDate = new Date(parseInt(reminder.NextDate.substr(6)));
        var endDate = new Date(fi.dashboard.settings.endDate);

        var getNextDay = function (previousDay) {
            var nextDay = new Date(previousDay);

            // special treatment for February
            if (previousDay.getDate() == 14 && previousDay.getMonth() == 1) {
                nextDay = new Date(previousDay.getFullYear(), previousDay.getMonth() + 1, 0);
            } else {
                switch (previousDay.getDate()) {
                    case 1:
                        nextDay.setDate(15);
                        break;
                    case 15:
                    case 16:
                        nextDay = new Date(previousDay.getFullYear(), previousDay.getMonth() + 1, 0);
                        break;
                    case 30:
                    case 31:
                        nextDay = new Date(previousDay.getFullYear(), previousDay.getMonth() + 1, 15);
                        break;
                    default:
                        nextDay.setDate(previousDay.getDate() + 15);
                        break;
                }
            }

            return nextDay;
        };

        var lastStoredDate = null;
        for (var d = startDate; d <= endDate; d = getNextDay(d)) {
            if (reminder.LastDate && fi.objects.convertDate(reminder.LastDate) < d)
                break;

            // copy item to item but with new date
            fi.reminders.buildReminder(account, fi.reminders.convertDataToItem(reminder, d, lastStoredDate));
            lastStoredDate = new Date(d);
        }
    },
    displayMonthlyReminder: function (account, reminder, increment) {
        // calculate dates
        var startDate = new Date(parseInt(reminder.NextDate.substr(6)));
        var endDate = new Date(fi.dashboard.settings.endDate);

        var getNextDay = function (previousDay, increment) {
            var nextDay = new Date(previousDay);

            // check if previousDay is last day of month
            var checkDay = new Date(previousDay);
            checkDay.setDate(checkDay.getDate() + 1);
            if (checkDay.getMonth() != previousDay.getMonth()) {
                // reset as last day of next month
                nextDay.setMonth(nextDay.getMonth() + 2);
                nextDay.setDate(0);
            } else {
                nextDay.setMonth(nextDay.getMonth() + increment);
            }

            return nextDay;
        };

        var lastStoredDate = null;
        for (var d = startDate; d <= endDate; d = getNextDay(d, increment)) {
            if (reminder.LastDate && fi.objects.convertDate(reminder.LastDate) < d)
                break;

            // copy item to item but with new date
            fi.reminders.buildReminder(account, fi.reminders.convertDataToItem(reminder, d, lastStoredDate));
            lastStoredDate = new Date(d);
        }
    },
    loadAccountCategories: function () {
        var accountId = $('#AccountId').val();
        var container = $('#divCategories');
        if (accountId.length > 0) {
            var ddl = $('#CategoryId');
            $.getJSON('/Dashboard/Accounts/GetAccountCategories/' + accountId, function (result) {
                var defaultOption = ddl.find('option').first();
                var selected = ddl.val();
                ddl.empty();
                if ($(result).length == 0) {
                    if (!container.hasClass('hide'))
                        container.addClass('hide');
                } else {
                    ddl.prepend(defaultOption);
                    $(result).each(function () {
                        $(document.createElement('option'))
                            .attr('value', this.Id)
                            .text(this.Value)
                            .appendTo(ddl);
                    });
                    ddl.val(selected);
                    container.removeClass('hide');
                }
            });

            $.getJSON('/Dashboard/Accounts/GetAccountType/' + accountId, function (result) {
                if (result.PositiveText.length > 0) {
                    $('#PositiveText').text(result.PositiveText);
                }
                if (result.NegativeText.length > 0) {
                    $('#NegativeText').text(result.NegativeText);
                }
            });
        } else {
            if (!container.hasClass('hide'))
                container.addClass('hide');

            $('#PositiveText').text(fi.accounts.DefaultPositiveText);
            $('#NegativeText').text(fi.accounts.DefaultNegativeText);
        }
    },
    //deleteReminder: function (account, reminder) {
    //    // remove
    //    account.removeTransaction(reminder.item.dataId);
    //    account.removeObject('R', reminder.item.dataId);
    //    $(account.markup).find('.transaction[data-val=' + transaction.item.dataId + ']').remove();
    //},
    setupGo: function (button, account, reminder) {
        // update amount in querystring
        var href = button.href;
        var indexOfAmount = button.href.indexOf('&amount=');
        if (indexOfAmount > -1)
            href = button.href.substr(0, indexOfAmount);

        // retrieve amount displayed (needed for calculated interest)
        var newAmount = reminder.markup.find('.amount').text().replace('$', '').replace(',', '');
        button.href = href + '&amount=' + newAmount;

        fi.dashboard.setupModal(button, 'frmTransactionsCreate', fi.transactions.setupAddEdit, function (result) {
            // send in a reminder, get a transaction back
            account.transactions.push(result);
            fi.transactions.dataToObject(account, result);

            // remove the instance of a reminder
            var cleanDate = fi.objects.getCleanDate(reminder.item.date);
            account.removeDateObject('R', reminder.item.dataId, reminder.item.date);
            $(account.markup).find('.reminder[data-val=' + reminder.item.dataId + '][data-date="' + cleanDate + '"]').remove();

            account.updateAllBalances();
            fi.dashboard.finalFormatting();
        });
    },
    setupAdd: function (button) {
        fi.dashboard.setupModal(button, 'frmRemindersCreate', fi.reminders.setup, function (result) {
            var account = fi.dashboard.getAccount(result.AccountId);
            account.reminders.push(result);
            fi.reminders.addReminder(account, result);

            account.updateAllBalances();
            fi.dashboard.finalFormatting();
        });
    },
    onCreate: function () {
        console.log("complete");
    },
    deleteReminder: function (account, reminder) {
        // remove from account
        account.removeReminder(reminder.item.dataId);
        account.removeObject('R', reminder.item.dataId);

        // delete all instances
        $(account.markup).find('.reminder[data-val=' + reminder.item.dataId + ']').remove();
    },
    editReminder: function (account, reminder, result) {
        // delete all instances
        fi.reminders.deleteReminder(account, reminder);

        // re-add all instances
        account.reminders.push(result);
        fi.reminders.addReminder(account, result);

        account.updateAllBalances();
        fi.dashboard.finalFormatting();
    },
    setupEdit: function (reminderLink, reminder) {
        fi.dashboard.setupModal(reminderLink, 'frmRemindersEdit',
            function () {
                // usual edit setup
                fi.reminders.setup();

                // capture delete button
                var deleteLink = $('#reusableModal-container').find('.deleteLink');

                deleteLink.click(function () {
                    fi.dashboard.setupModal(this, 'frmReminderDelete', function (){},
                        function (result) {
                            // handle return delete
                            if (result.deleted) {
                                var account = fi.dashboard.getAccount(reminder.item.accountId);
                                fi.reminders.deleteReminder(account, reminder);

                                account.updateAllBalances();
                                fi.dashboard.finalFormatting();
                            }
                        });

                    return false;
                });
            }, function (result) {
                var account = fi.dashboard.getAccount(result.AccountId);

                // remove reminder and update balances
                fi.reminders.editReminder(account, reminder, result);

                fi.dashboard.finalFormatting();
            });
    },
    setup: function () {
        $('.datepicker').datepicker({
            orientation: "bottom auto",
            autoclose: true
        });

        fi.reminders.loadAccountCategories();
        $('#AccountId').change(function () {
            fi.reminders.loadAccountCategories();
        });
    }
};