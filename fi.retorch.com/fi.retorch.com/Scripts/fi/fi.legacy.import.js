var fi = fi || {};
fi.legacy = fi.legacy || {};

fi.legacy.import = {

    // legacy login
    login: function(form) {        
        //if ($(this).valid()) {
        $.ajax({
            url: form.action,
            type: form.method,
            data: $(form).serialize(),
            success: function (result) {
                if (result) {
                    $('form').addClass('hide');
                    $('#loginSuccess').removeClass();
                }
            }
        });
        //}
    },

    // get bookmarks
    importBookmarks: function () {
        $('#bookmarkStatus').removeClass('hide');
        $.ajax({
            url: '/Legacy/Import/Bookmarks',
            type: 'get',
            success: function (result) {
                $('#bookmarkStatus').text(result.length + ' bookmarks imported.');

                fi.legacy.import.importCategories();
            }
        });
    },
    
    // get categories
    importCategories: function() {
        $('#loginSuccess').addClass('hide');
        $('#categoryStatus').removeClass('hide');
        $.ajax({
            url: '/Legacy/Import/Categories',
            type: 'get',
            success: function (result) {
                categoryMappings = result;
                $('#categoryStatus').text(result.length + ' categories imported.');

                fi.legacy.import.importAccounts();
            }
        });
    },

    // get accounts
    importAccounts: function () {
        $('#accountStatus').removeClass('hide');
        $.ajax({
            url: '/Legacy/Import/Accounts',
            type: 'get',
            success: function (result) {
                $('#accountStatus').text(result.length + ' accounts imported.');

                fi.legacy.import.importAccountCategories();
            }
        });
    },

    // get account categories
    importAccountCategories: function () {
        $('#accountCategoryStatus').removeClass('hide');
        $.ajax({
            url: '/Legacy/Import/AccountCategories',
            type: 'get',
            success: function (result) {
                $('#accountCategoryStatus').text(result.length + ' account categories imported.');

                fi.legacy.import.importReminders();
            }
        });
    },

    // get reminders
    importReminders: function () {
        $('#reminderStatus').removeClass('hide');
        $.ajax({
            url: '/Legacy/Import/Reminders',
            type: 'get',
            success: function (result) {
                $('#reminderStatus').text(result.length + ' reminders imported.');

                fi.legacy.import.importTransactions();
            }
        });
    },

    // get transactions
    importTransactions: function () {
        $('#transactionStatus').removeClass('hide');
        $.ajax({
            url: '/Legacy/Import/Transactions',
            type: 'get',
            success: function (result) {
                $('#transactionStatus').text(result.length + ' transactions imported.');

                // Done
            }
        });
    },

    setup: function () {
        // login
        $('.frmLogin').submit(function (e) {
            e.preventDefault();

            fi.legacy.import.login(this);

            //return false;
        });

        var categoryMappings = [];

        // get categories
        $('#btnImport').click(function (e) {
            e.preventDefault();

            fi.legacy.import.importBookmarks();
        });

    }
};