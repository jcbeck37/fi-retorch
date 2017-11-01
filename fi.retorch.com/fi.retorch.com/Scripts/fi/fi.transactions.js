var fi = fi || {};

fi.transactions = {
    maxVisibleName: 25,
    defaults: {
        AccountDDL: null,
        CategoryDDL: null
    },
    selectors: {
        TypeDDL: null,
        AccountDDL: null,
        CategoryDDL: null,
        PositiveTextTB: null,
        NegativeTextTB: null
    },
    transaction: function (item, data) {
        // set properties
        this.item = item; // contains original reminder data (fi.objects.item)
        //this.data = data;
        this.resultBalance = 0.0;
        this.postedBalance = 0.0;
        this.markup = null;

        // methods
        this.createMarkup = function (account) {
            // creates element
            this.markup = $(document.createElement('div'));

            var transactionClass = this.item.posted ? 'posted' : 'pending';
            this.markup.attr('class', 'transaction ' + transactionClass)
                .attr('data-val', this.item.dataId);

            // date
            var date = this.item.date;
            var dateFormatted = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
            $(document.createElement('span'))
                .attr('class', 'date')
                .text(dateFormatted)
                .appendTo(this.markup);

            // transaction
            var item = $(document.createElement('span'))
                .attr('class', 'item');

            // transaction edit
            var displayName = this.item.name;
            var displayTitle = displayName;
            if (displayName.length > fi.transactions.maxVisibleName) {
                displayName = displayName.substr(0, (fi.transactions.maxVisibleName - 3)) + '...';
                displayName = displayName.toLowerCase(); // use CSS to initial cap
            }
            var name = $(document.createElement('a'))
                .attr('class', 'edit' + (this.item.posted ? ' posted' : ''))
                .attr('title', displayTitle)
                .text(displayName);
            name.attr('href', this.item.editLink);
            name.appendTo(item);
            
            // transaction (quick) actions
            if (!this.item.posted) {
                var actions = $(document.createElement('a'))
                    .attr('class', 'actions');
                    actions.attr('href', this.item.postLink);
                    actions.addClass('post title').attr('data-toggle', 'tooltip').attr('title','Mark posted');
                    actions.text('Post');
                actions.appendTo(item);
            }

            item.appendTo(this.markup);

            // amount
            var amountClass = this.item.positive ? 'positive' : 'negative';
            $(document.createElement('span'))
                .attr('class', 'amount currency ' + amountClass)
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
    buildTransaction: function (account, item) {
        // create
        var objTransaction = new fi.transactions.transaction(item);
        objTransaction.createMarkup(account);

        // find spot to place it
        account.placeItemInAccount(item, objTransaction);

        // set up transaction edit to open modal
        objTransaction.markup.find('.edit').click(function () {
            fi.transactions.setupEdit(this, objTransaction);

            return false;
        });

        // handle post
        objTransaction.markup.find('.actions.post').click(function () {
            fi.transactions.postTransaction(this, account, objTransaction);

            return false;
        });
    },
    dataToObject: function (account, data) {
        // convert to account item for overall display order
        var item = new fi.objects.item({
            dataId: data.Id,
            accountId: data.AccountId,
            type: 'T',
            name: data.Name,
            date: new Date(parseInt(data.DisplayDate.substr(6))),
            posted: data.IsPosted,
            postedDate: data.IsPosted ? new Date(parseInt(data.DatePosted.substr(6))) : null,
            amount: data.Amount,
            positive: data.IsCredit,
            order: data.DisplayOrder,
            editLink: data.EditLink,
            postLink: data.PostLink
        });

        // copy data to item
        fi.transactions.buildTransaction(account, item);
    },
    postTransaction: function (button, account, transaction) {
        var url = $(button).attr('href');

        $.ajax({
            url: url,
            type: 'post',
            data: { Id: transaction.item.dataId },
            success: function (result) {
                fi.transactions.editTransaction(account, transaction, result);

                account.updateAllBalances();
                fi.dashboard.finalFormatting();
            }
        });
    },
    editTransaction: function (account, transaction, result) {
        fi.transactions.deleteTransaction(account, transaction);

        // re-add
        account.transactions.push(result);
        fi.transactions.dataToObject(account, result);

        account.updateAllBalances();
    },
    deleteTransaction: function (account, transaction) {
        // remove
        account.removeTransaction(transaction.item.dataId);
        account.removeObject('T', transaction.item.dataId);
        $(account.markup).find('.transaction[data-val=' + transaction.item.dataId + ']').remove();
    },
    loadAccounts: function () {
        var typeId = $(this.selectors.AccountTypeDDL).val();

        if (typeId > 0) {
            $.getJSON('/Dashboard/Accounts/GetAccountsByType/' + typeId, function (result) {
                var ddl = $(fi.transactions.selectors.AccountDDL);
                var defaultOption = ddl.find('option').first();
                var selected = ddl.val();
                ddl.empty();
                if ($(result).length != 0) {
                    ddl.prepend(defaultOption);
                    var hasOldValue = false;
                    $(result).each(function () {
                        $(document.createElement('option'))
                            .attr('value', this.Id)
                            .text(this.Name)
                            .appendTo(ddl);
                        if (selected == this.Id)
                            hasOldValue = true;
                    });
                    if (hasOldValue)
                        ddl.val(selected);
                    else
                        ddl.val('');
                }
            });
        } else {
            var ddl = $(this.selectors.AccountDDL);
            ddl.html(this.defaults.AccountDDL);
        }
    },
    loadAccountCategories: function () {
        var accountId = $(this.selectors.AccountDDL).val();

        var container = $(this.selectors.CategoryContainer);
        if (accountId.length > 0) {
            var ddl = $(fi.transactions.selectors.CategoryDDL);
            $.getJSON('/Dashboard/Accounts/GetAccountCategories/' + accountId, {
                CategoryId: ddl.val()
            }, function (result) {
                var defaultOption = ddl.find('option').first();
                var selected = ddl.val();
                ddl.empty();
                if ($(result).length == 0) {
                    if (!container.hasClass('hide'))
                        container.addClass('hide');
                } else {
                    ddl.prepend(defaultOption);
                    var hasOldValue = false;
                    $(result).each(function () {
                        $(document.createElement('option'))
                            .attr('value', this.Id)
                            .text(this.Value)
                            .appendTo(ddl);
                        if (selected == this.Id)
                            hasOldValue = true;
                    });
                    if (hasOldValue)
                        ddl.val(selected);
                    else
                        ddl.val('');
                    container.removeClass('hide');
                }
            });

            if (this.selectors.PositiveTextTB || this.selectors.NegativeTextTB) {
                // also update account type data on transaction edit
                $.getJSON('/Dashboard/Accounts/GetAccountType/' + accountId, function (result) {
                    if (result.PositiveText.length > 0) {
                        $(fi.transactions.selectors.PositiveTextTB).text(result.PositiveText);
                    }
                    if (result.NegativeText.length > 0) {
                        $(fi.transactions.selectors.NegativeTextTB).text(result.NegativeText);
                    }
                });
            }
        } else {
            if (this.selectors.CategoryContainer) {
                if (!container.hasClass('hide'))
                    container.addClass('hide');
            } else {
                var ddl = $(this.selectors.CategoryDDL);
                ddl.html(this.defaults.CategoryDDL);
            }

            if (this.selectors.PositiveTextTB)
                $(this.selectors.PositiveTextTB).text(fi.accounts.DefaultPositiveText);
            if (this.selectors.NegativeTextTB)
            $(this.selectors.NegativeTextTB).text(fi.accounts.DefaultNegativeText);
        }
    },
    onCreate: function () {
        //console.log("complete");
    },
    setupAdd: function (button) {
        fi.dashboard.setupModal(button, 'frmTransactionsCreate', fi.transactions.setupAddEdit, function (result) {
            var account = fi.dashboard.getAccount(result.AccountId);
            account.transactions.push(result);
            fi.transactions.dataToObject(account, result);

            account.updateAllBalances();
            fi.dashboard.finalFormatting();
        });
    },
    setupEdit: function (transactionLink, transaction) {
        fi.dashboard.setupModal(transactionLink, 'frmTransactionsEdit',
            function () {
                fi.transactions.setupAddEdit();

                // capture delete button
                var deleteLink = $('#reusableModal-container').find('.deleteLink');

                deleteLink.click(function () {
                    fi.dashboard.setupModal(this, 'frmTransactionDelete', function () { },
                        function (result) {
                            // handle return delete
                            if (result.deleted) {
                                var account = fi.dashboard.getAccount(transaction.item.accountId);
                                fi.transactions.deleteTransaction(account, transaction);

                                account.updateAllBalances();
                                fi.dashboard.finalFormatting();
                            }
                        });

                    return false;
                });
            }, function (result) {
            var account = fi.dashboard.getAccount(result.AccountId);

            // remove transaction and update balances
            fi.transactions.editTransaction(account, transaction, result);

            account.updateAllBalances();
            fi.dashboard.finalFormatting();
        });
    },
    onEdit: function () {
        //console.log("complete");
    },
    setupAddEdit: function () {
        var thisModule = fi.transactions;

        $('.datepicker').datepicker({
            orientation: "bottom auto",
            autoclose: true
        });

        // set view selectors
        thisModule.selectors.AccountDDL = '#AccountId';
        thisModule.selectors.CategoryDDL = '#CategoryId';
        thisModule.selectors.CategoryContainer = '#divCategories';

        thisModule.selectors.PositiveTextTB = '#PositiveText';
        thisModule.selectors.NegativeTextTB = '#NegativeText';

        thisModule.loadAccountCategories();
        $(thisModule.selectors.AccountDDL).change(function () {
            fi.transactions.loadAccountCategories();
        });
    },
    setupList: function () {
        $('.datepicker').datepicker({
            orientation: "bottom auto",
            autoclose: true
        });

        $('#reset').click(function () {
            window.location = window.location.href.split('?')[0] + '?reset=true';
        });

        // set view selectors
        this.selectors.AccountTypeDDL = '#Settings_AccountTypeId';
        this.selectors.AccountDDL = '#Settings_AccountId';
        this.selectors.CategoryDDL = '#Settings_CategoryId';

        // refresh accounts list
        this.defaults.AccountDDL = $(this.selectors.AccountDDL).html();
        this.loadAccounts();
        $(this.selectors.AccountTypeDDL).change(function () {
            fi.transactions.loadAccounts();
        });

        // refresh category list
        this.defaults.CategoryDDL = $(this.selectors.CategoryDDL).html();
        this.loadAccountCategories();
        $(this.selectors.AccountDDL).change(function () {
            fi.transactions.loadAccountCategories();
        });

    }
};