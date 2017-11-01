var fi = fi || {};
fi.networth = fi.networth || {};

fi.networth.instant = {
    settings: {
        valueDate: new Date()
    },
    markup: $('#report'),
    totalMarkup: $('#netWorthTotal'),
    accountTypesMarkup: $('#accountTypes'),
    totalNetWorth: 0.0,
    accountTypes: [],
    accounts: [],
    getAccountType: function (accountTypeId) {
        return $.grep(fi.networth.instant.accountTypes, function (e) {
            return e.data.Id == accountTypeId;
        })[0];
    },
    getAccount: function (accountId) {
        return $.grep(fi.networth.instant.accounts, function (e) {
            return e.data.Id == accountId;
        })[0];
    },
    getValueDate: function () {
        var date = fi.networth.instant.settings.valueDate;
        var dateString = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
        return dateString;
    },
    compareAccountTypes: function (a, b) {
        if (a.Account.TypeId < b.Account.TypeId)
            return -1;
        if (a.Account.TypeId > b.Account.TypeId)
            return 1;
        return 0;
    },
    formatReportCurrency: function (accountId) {
        $('#report').find('.currency').each(function () {
            fi.objects.formatCurrency(this);
        })
    },
    resetReport: function() {
        fi.networth.instant.totalMarkup.text('');
        fi.networth.instant.accountTypesMarkup.text('');

        fi.networth.instant.totalNetWorth = 0.0;
        fi.networth.instant.accountTypes = [];
        fi.networth.instant.accounts = [];
    },
    setDate: function(date) {
        fi.networth.instant.settings.valueDate = new Date(date);
    },
    loadAccountBalances: function () {
        $.getJSON('/Reports/NetWorth/GetAccountBalances/', {
            startDate: fi.networth.instant.getValueDate()
        }, function (result) {
            fi.networth.instant.setAccountBalances(result);
        });
    },
    setAccountBalances: function (result) {
        // order records by type
        result.sort(fi.networth.instant.compareAccountTypes);

        // create account type objects with totals
        $(result).each(function () {
            //console.log(this);

            // tally overall net worth
            var mult = this.Type.IsDebt ? -1.0 : 1.0;
            fi.networth.instant.totalNetWorth += (parseFloat(this.Account.CurrentBalance) * mult);

            // group each into Account Types
            var thisType = fi.networth.instant.getAccountType(this.Type.Id);
            if (thisType == null) {
                thisType = {
                    data: this.Type,
                    total: 0
                };
                fi.networth.instant.accountTypes.push(thisType);
            }

            thisType.total += this.Account.CurrentBalance;            
            
            // store accounts locally
            var object = {
                data: this.Account
            }
            fi.networth.instant.accounts.push(object);
        });

        // now that types/accounts are set up, adjust for recent transactions
        fi.networth.instant.loadTransactionTotals();
    },
    loadTransactionTotals: function () {
        $.getJSON('/Reports/NetWorth/GetTransactionTotals/', {
            startDate: fi.networth.instant.getValueDate()
        }, function (result) {
            fi.networth.instant.setTransactionTotals(result);
        });
    },
    setTransactionTotals: function (result) {
        $(result).each(function () {
            console.log(this);

            var object = {
                data: this
            };
            
            var mult = 1.0;
            var account = fi.networth.instant.getAccount(object.data.AccountId);
            // we'll only look at transactions in accounts that are open at the time of this
            if (account) {
                var accountType = fi.networth.instant.getAccountType(account.data.TypeId);
                accountType.total -= parseFloat(object.data.Total);
                mult = accountType.data.IsDebt ? -1.0 : 1.0;

            // remove transaction amounts since net worth target date
            fi.networth.instant.totalNetWorth -= (parseFloat(object.data.Total) * mult);
            };
        });

        fi.networth.instant.displayReport();
    },
    displayReport: function() {
        // display total with bold for each account type
        $(fi.networth.instant.accountTypes).each(function () {

            var isDebt = this.data.IsDebt;

            //console.log(this);
            var accountType = this;
            accountType.markup = document.createElement('div');
            $(accountType.markup).addClass('accountType');

            $(document.createElement('span'))
               .attr('class', 'typeName')
               .text(accountType.data.Name)
               .appendTo(accountType.markup);

            $(document.createElement('span'))
               .attr('class', 'typeTotal currency' + (isDebt ? ' negative' : ''))
               .text(accountType.total)
               .appendTo(accountType.markup);

            $(accountType.markup).appendTo($('#accountTypes'));

            // group extra information below each Type listing specific accounts
        });

        $(document.createElement('span'))
           .attr('class', 'typeName')
           .text('Net Worth')
           .appendTo($('#netWorthTotal'));

        $(document.createElement('span'))
           .attr('class', 'currency' + (fi.networth.instant.totalNetWorth < 0.0 ? ' negative' : ''))
           .text(fi.networth.instant.totalNetWorth)
           .appendTo($('#netWorthTotal'));

        // formatting
        fi.networth.instant.formatReportCurrency();

    },
    setup: function () {
        fi.networth.instant.setDate($('#date').val());
        fi.networth.instant.loadAccountBalances();

        $('.datepicker').datepicker({
            orientation: "bottom auto",
            autoclose: true
        });

        $('#date').change(function () {
            var date = $(this).val();

            if (moment(date, "MM/DD/YYYY", true).isValid() && fi.networth.instant.settings.valueDate.getTime() != new Date(date).getTime()) {
                fi.networth.instant.resetReport();
                fi.networth.instant.setDate(date);
                fi.networth.instant.loadAccountBalances();
            }
        });
    }
};