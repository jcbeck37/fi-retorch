var fi = fi || {};

fi.dashboard = {
    settings: {
        startDate: new Date(),
        endDate: new Date(),
        bookmarksVisible: false
    },
    accounts: [], // list of account objects
    setStartDate: function (loadOptions) {
        var newDate = $('#startDate').val();
        fi.dashboard.settings.startDate = new Date(newDate);

        if (loadOptions.loadAccounts)
            fi.dashboard.loadAccounts(loadOptions.loadReminders);
    },
    setEndDate: function (loadOptions) {
        var newDate = $('#endDate').val();
        fi.dashboard.settings.endDate = new Date(newDate);

        if (loadOptions.loadAccounts)
            fi.dashboard.loadAccounts(loadOptions.loadReminders);
    },
    getStartDate: function () {
        return fi.objects.getCleanDate(fi.dashboard.settings.startDate);
    },
    getEndDate: function () {
        return fi.objects.getCleanDate(fi.dashboard.settings.endDate);
    },
    getAccount: function(accountId) {
        return $.grep(fi.dashboard.accounts, function (e) {
            return e.accountId == accountId;
        })[0];
    },
    formatAccountCurrency: function (accountId) {
        $('.account[data-val=' + accountId + ']').find('.currency').each(function() {
            fi.objects.formatCurrency(this);
        })
    },
    finalFormatting: function() {
        // finally add nice touches
        $(fi.dashboard.accounts).each(function () {
            fi.dashboard.formatAccountCurrency(this.accountId);
            $(this.container).fadeIn();
        });

        $('.grid').masonry({
            itemSelector: '.grid-item'
        })
    },
    loadAccounts: function (loadReminders) {
        if (loadReminders) {
            //initialize
            fi.dashboard.accounts = [];
            $('.account').each(function () {
                var container = $(this).find('.transactions');
                container.text(''); // clear
                var accountId = $(this).attr('data-val');
                var currentBalance = $(this).attr('data-bal');
                var account = new fi.accounts.account({
                    accountId: accountId,
                    container: container,
                    currentBalance: currentBalance,
                    displayedBalance: $(this).find('.accountBalance')
                });
                account.markup = this;
                fi.dashboard.accounts.push(account);
            });
        } else {
            //just remove visuals, all transactions and tracking objects        
            $(fi.dashboard.accounts).each(function () {
                // clear markup
                fi.dashboard.accountReset(this.accountId);

                // remove transactions
                this.transactions = [];
            });
        }

        $.getJSON('/Dashboard/Home/GetAccountTransactions/', {
            startDate: fi.dashboard.getStartDate(),
            endDate: fi.dashboard.getEndDate()
        }, function (result) {
            fi.dashboard.setAccountTransactions(loadReminders, result);
        });
    },
    accountReset: function(accountId) {
        var account = fi.dashboard.getAccount(accountId);

        $(account.container).text('');
        account.objects = [];
    },
    setAccountTransactions: function(loadReminders, transactions) {
        // assigns each transaction to corresponding account
        $(transactions).each(function () {
            var account = fi.dashboard.getAccount(this.AccountId);
            if (account)
                account.transactions.push(this);
            else
                console.log(this);
        });

        // for each account, display transactions
        $(fi.dashboard.accounts).each(function () {
            fi.dashboard.displayTransactions(this.accountId);
        });

        // now we're ready to load reminders
        if (loadReminders) {
            // initial load from the server
            $.getJSON('/Dashboard/Home/GetAccountReminders/', function (result) {
                if ($(result).length != 0) {
                    fi.dashboard.setAccountReminders(result);
                } else {
                    fi.dashboard.finalFormatting();
                }
            });
        } else {
            // for each account, display reminders
            $(fi.dashboard.accounts).each(function () {
                fi.dashboard.displayReminders(this.accountId);
            });

            fi.dashboard.finalFormatting();
        }
    },
    setAccountReminders: function (reminders) {
        // assigns each reminder to corresponding account
        $(reminders).each(function () {
            fi.dashboard.getAccount(this.AccountId).reminders.push(this);
        });

        // for each account, display reminders
        $(fi.dashboard.accounts).each(function () {
            fi.dashboard.displayReminders(this.accountId);
        });

        fi.dashboard.finalFormatting();
    },
    displayTransactions: function (accountId) {
        // current account balance
        var account = fi.dashboard.getAccount(accountId);
        account.visibleBalance = parseFloat(account.currentBalance);
        
        if (account.transactions) {
            // substract all posted transactions to start
            var postedTransactions = $.grep(account.transactions, function (t) {
                return t.IsPosted;
            });

            $(postedTransactions).each(function () {
                var amount = parseFloat(this.Amount) * (this.IsCredit ? 1 : -1);
                account.visibleBalance += -1.0 * amount;
            });

            // stored for reminders that get inserted at the beginning
            account.initialBalance = account.visibleBalance;

            // non-posted items don't affect "posted" balance => track separately
            account.postedBalance = account.visibleBalance;

            // loop through and display
            $(account.transactions).each(function () {
                fi.transactions.dataToObject(account, this);
            });
        }

        account.updateAllBalances();
    },
    displayReminders: function (accountId)
    {
        var account = fi.dashboard.getAccount(accountId);

        // prase reminders
        $(account.reminders).each(function () {
            fi.reminders.addReminder(account, this);
        });

        account.updateAllBalances();
    },
    setupModal: function (button, formName, loadCallback, successCallback) {
        var url = $(button).attr('modal-url');
        var url = (url == null) ? $(button).attr('href') : url;

        $.get(url, function (data) {
            var modal = $('#reusableModal');
            var modalMarkup = $('#reusableModal-container');

            // loads ajax form and initializes client-side validation
            modalMarkup.html(data);
            $.validator.unobtrusive.parse("#form0");

            // hide normal button and use footer
            $('.buttons').addClass('hide');

            // remove hide and make into a modal
            modal.modal('show').removeClass('hide');

            modal.find('.modal-header').removeClass('hide');
            modal.find('.modal-footer').removeClass('hide');

            $('form[name=' + formName + ']').submit(function (event) {
                event.preventDefault();

                var form = $('form[name=' + formName + ']');
                if ($(form).valid()) {
                    $.ajax({
                        url: form.attr('action'),
                        type: form.attr('method'),
                        data: $(form).serialize(),
                        success: function (result) {
                            modal.modal('hide');

                            successCallback(result);
                        }
                    });
                }

                return false;
            });

            loadCallback();
        });
    },
    setup: function () {
        var startDate = new Date();
        startDate.setDate(startDate.getDate() - 31);
        $('#startDate').datepicker({
            dateFormat: 'mm/dd/yyyy',
            startDate: startDate,
            endDate: new Date(),
            orientation: "bottom auto",
            autoclose: true
        });

        var endDate = new Date();
        endDate.setMonth(endDate.getMonth() + 24);
        $('#endDate').datepicker({
            dateFormat: 'mm/dd/yyyy',
            startDate: new Date(),
            endDate: endDate,
            orientation: "bottom auto",
            autoclose: true
        });

        $('.container.body-content > h2').fadeOut(500)

        $('#startDate').change(function () {
            var date = $(this).val();

            if (moment(date, "MM/DD/YYYY", true).isValid() && fi.dashboard.settings.startDate.getTime() != new Date(date).getTime()) {                
                fi.dashboard.setStartDate({ loadAccounts: true, loadReminders: false });
            }
        });
        fi.dashboard.setStartDate({ loadAccounts: false, loadReminders: false });

        $('#endDate').change(function () {
            var date = $(this).val();

            if (moment(date, "MM/DD/YYYY", true).isValid() && fi.dashboard.settings.endDate.getTime() != new Date(date).getTime()) {
                fi.dashboard.setEndDate({ loadAccounts: true, loadReminders: false });
            }
        });
        fi.dashboard.setEndDate({ loadAccounts: true, loadReminders: true });

        // set up add transaction link to open modal
        $('#add-transaction-button').click(function () {
            fi.transactions.setupAdd(this);

            return false;
        });

        // set up add reminder link to open modal
        $('#add-reminder-button').click(function () {
            fi.reminders.setupAdd(this);

            return false;
        });

        // set up bookmark button
        $('#show-bookmarks-button').click(function () {
            var bookmarkList = $('#bookmarkList');
            if (fi.dashboard.settings.bookmarksVisible) {
                bookmarkList.slideToggle();
                fi.dashboard.settings.bookmarksVisible = false;
            } else {
                bookmarkList.slideToggle();
                fi.dashboard.settings.bookmarksVisible = true;
            }

            return false;
        });

        // set up bookmarks
        $('.bookmark-link').each(function () {
            var bookmarkList = $('#bookmarkList');

            $(this).attr("target", "_blank");
            $(this).click(function () {
                // hide bookmarks
                bookmarkList.slideToggle();
                fi.dashboard.settings.bookmarksVisible = false;
            })
        })
    }
};