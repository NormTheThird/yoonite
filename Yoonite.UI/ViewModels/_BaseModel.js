var BaseModel = {
    ServiceCall: function (url, type, data, async, callback) {
        if (callback === undefined || url === undefined) {
            var badcall = { IsSuccess: false, ErrorMessage: "Call must provide callback and url!", IsTokenBad: false };
            callback(badcall);
        }
        if (type === undefined || type.length === 0) { type = "get"; }
        if (async === undefined || async.length === 0) { async = true; }
        if (data === undefined) { data = {}; }
        $.ajax({
            type: type,
            url: url,
            data: data,
            cache: false,
            async: async,
            success: function (response) {
                if (response.IsTokenBad) { window.location = "/Account"; };
                callback(response);
            },
            error: function (xhr2, status2, error2) {
                var message = status2 + " / " + error2;
                if (xhr2 !== undefined && xhr2.responseText !== undefined) {
                    var s = xhr2.responseText.indexOf("<title>");
                    var e = xhr2.responseText.indexOf("</title>");
                    if (s > 0 && e > s) {
                        var l = e - s;
                        message = xhr2.responseText.substr(s + 7, l - 7);
                    }
                }
                var response = { IsSuccess: false, ErrorMessage: "Error: " + message, IsTokenBad: false };

                callback(response);
            }
        });
    },
    PromiseCall: function (url, type, data) {
        return new Promise(function (resolve, reject) {
            Base.ServiceCall(url, type, data, true, function (response) {
                if (response.IsSuccess) { resolve(response); }
                else { reject(response); }
            });
        });
    },
    Message: function (message, level) {
        if (level.type === "success")
            toastr.success(message, "Success", { progressBar: false });
        else if (level.type === "info")
            toastr.info(message, "Information", { progressBar: false });
        else if (level.type === "warning")
            toastr.warning(message, "Warning", { timeOut: 0 });
        else if (level.type === "error")
            toastr.error(message, "Error", { timeOut: 0 });
    },
    MessageLevels: {
        Success: { type: "success", color: "green" },
        Info: { type: "info", color: "blue" },
        Warning: { type: "warning", color: "yellow" },
        Error: { type: "error", color: "red" }
    },
    LogError: function (exception) {
        console.error(exception);
        BaseModel.Message(exception, BaseModel.MessageLevels.Error);
    },
    Cookies: function (cookie, newVal) {
        var cookies = document.cookie.split(";");
        var allCookies = {};
        for (i in cookies) {
            var cookieParts = cookies[i].split('=');
            var key = cookieParts.shift();
            var value = cookieParts.join("=");
            if (key && value) allCookies[key.toLowerCase().trim()] = value.trim();
        }

        if (cookie) {
            if (newVal) {
                var newCookie = cookie + "=" + newVal + "; path=/";
                document.cookie = newCookie;
                return allCookies[cookie.toLowerCase()] = newVal;
            }
            else {
                return allCookies[cookie.toLowerCase()];
            }
        }
        return allCookies;
    },
    GetUrlQuery: function (query) {
        var args = window.location.href.split('?');
        var params = {};
        if (args.length <= 1) { return null; }
        var url = args[1].split('#')[0].split("&");
        for (i in url) {
            param = url[i].split("=");
            params[param[0].toLowerCase()] = param[1];
        }
        if (query) {
            return params[query];
        }
        return params;
    },
    Guid: {
        Empty: '00000000-0000-0000-0000-000000000000',
        New: function () {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
        }
    },
    ToDate: function (date, format) {
        try {
            if (date) {
                if (date instanceof Date) { return date; }
                var newDate = new Date(parseInt(date.match(/\d+/) || 0));
                if (format === "mm/dd/yyyy") return newDate.toLocaleDateString();
                if (format === "yyyy-dd-mm") return newDate.toISOString().split('T')[0];
                return newDate;
            }
        }
        catch (ex) {
            BaseModel.LogError("Date parse failed", date);
        }
    },
    ToTime: function (date) {
        try {
            if (date) {
                if (date instanceof Date) { return date; }
                var newDate = new Date(parseInt(date.match(/\d+/) || 0));
                var minutes = newDate.getMinutes();
                if (minutes < 10)
                    minutes = "0" + minutes;
                var hour = newDate.getHours();
                if (hour > 12)
                    return ((hour - 12) + ":" + minutes + " PM")
                else if (hour == 12)
                    return (hour + ":" + minutes + " PM")
                else
                    return (hour + ":" + minutes + " AM")
            }
        }
        catch (ex) {
            console.error("Date parse Failed", date);
        }
    },
    States: [
        { Name: "Alabama", ShortName: "AL" },
        { Name: "Alaska", ShortName: "AK" },
        { Name: "Arizona", ShortName: "AZ" },
        { Name: "Arkansas", ShortName: "AR" },
        { Name: "California", ShortName: "CA" },
        { Name: "Colorado", ShortName: "CO" },
        { Name: "Connecticut", ShortName: "CT" },
        { Name: "Delaware", ShortName: "DE" },
        { Name: "District of Columbia", ShortName: "DC" },
        { Name: "Florida", ShortName: "FL" },
        { Name: "Georgia", ShortName: "GA" },
        { Name: "Guam", ShortName: "GU" },
        { Name: "Hawaii", ShortName: "HI" },
        { Name: "Idaho", ShortName: "ID" },
        { Name: "Illinois", ShortName: "IL" },
        { Name: "Indiana", ShortName: "IN" },
        { Name: "Iowa", ShortName: "IA" },
        { Name: "Kansas", ShortName: "KS" },
        { Name: "Kentucky", ShortName: "KY" },
        { Name: "Louisiana", ShortName: "LA" },
        { Name: "Maine", ShortName: "ME" },
        { Name: "Maryland", ShortName: "MD" },
        { Name: "Massachusetts", ShortName: "MA" },
        { Name: "Michigan", ShortName: "MI" },
        { Name: "Minnesota", ShortName: "MN" },
        { Name: "Mississippi", ShortName: "MS" },
        { Name: "Missouri", ShortName: "MO" },
        { Name: "Montana", ShortName: "MT" },
        { Name: "Nebraska", ShortName: "NE" },
        { Name: "Nevada", ShortName: "NV" },
        { Name: "New Hampshire", ShortName: "NH" },
        { Name: "New Jersey", ShortName: "NJ" },
        { Name: "New Mexico", ShortName: "NM" },
        { Name: "New York", ShortName: "NY" },
        { Name: "North Carolina", ShortName: "NC" },
        { Name: "North Dakota", ShortName: "ND" },
        { Name: "Ohio", ShortName: "OH" },
        { Name: "Oklahoma", ShortName: "OK" },
        { Name: "Oregon", ShortName: "OR" },
        { Name: "Pennsylvania", ShortName: "PA" },
        { Name: "Rhode Island", ShortName: "RI" },
        { Name: "South Carolina", ShortName: "SC" },
        { Name: "South Dakota", ShortName: "SD" },
        { Name: "Tennessee", ShortName: "TN" },
        { Name: "Texas", ShortName: "TX" },
        { Name: "Utah", ShortName: "UT" },
        { Name: "Vermont", ShortName: "VT" },
        { Name: "Virginia", ShortName: "VA" },
        { Name: "Washington", ShortName: "WA" },
        { Name: "West Virginia", ShortName: "WV" },
        { Name: "Wisconsin", ShortName: "WI" },
        { Name: "Wyoming", ShortName: "WY" }
    ],
    Environment: function () {
        var baseUrl = window.location.hostname;
        if (baseUrl.match("tek-find.azurewebsites.net")) { return "PROD"; }
        else if (baseUrl.match("Yoonite.io")) { return "PROD"; }
        else if (baseUrl.match("tek-find-test.azurewebsites.net")) { return "TEST"; }
        else { return "DEV"; }
    },
    AzureStorageConfig: {
        StorageAccountUri: "https://yoonite.blob.core.windows.net",
        SasToken: '?sv=2018-03-28&ss=b&srt=sco&sp=rwdlac&se=2022-05-01T11:13:55Z&st=2019-08-28T03:13:55Z&spr=https&sig=3QTMxM1cgv8Q02spKIL1NSC%2Fq%2BV8MMzthGeSfka53Gk%3D',
        MaxChunkSize: 45000,
        ChunkSize: function (sizeInB) {
            var sizeInGb = Math.floor(sizeInB / 1024 / 1024 / 1024);
            return sizeInGb > 4 ? sizeInB / UploaderConfig.MaxChunkSize : 1 * 1024 * 1024
        },
        Container: function () {
            if (window.location.hostname.includes("yoonite.io") || window.location.hostname.includes("yoonite.azurewebsites.net")) { return "production"; }
            else if (window.location.hostname.includes("yoonite-test.azurewebsites.net")) { return "test"; }
            else { return "development"; }
        }
    },
    GetSecurityModel: function () {
        try {
            var rtn;
            $.ajax({
                type: "post",
                url: "Security/GetSecurityModel",
                data: null,
                cache: false,
                async: false,
                success: function (securityModel) {
                    rtn = securityModel;
                },
                error: function (err) {
                    BaseModel.LogError(err);
                }
            });
            return rtn;
        }
        catch (ex) {
            BaseModel.LogError("Get security model failed", date);
        }
    }
};

function MainViewModel(isAuthenticated) {
    var self = this;
    self.HeaderVM = ko.observable(new HeaderViewModel(isAuthenticated === "True"));
    self.CurrentVM = ko.observable();
}

ko.bindingHandlers.fadeVisible = {
    init: function (_elem, _value) {
        var value = _value();
        $(_elem).toggle(ko.unwrap(value));
    },
    update: function (_elem, _value) {
        var value = _value();
        ko.unwrap(value) ? $(_elem).slideDown() : $(_elem).slideUp();
    }
};

ko.bindingHandlers.delayVisible = {
    init: function (_elem, _value) {
        var value = _value();
        $(_elem).toggle(ko.unwrap(value));
    },
    update: function (_elem, _value) {
        var value = _value();
        ko.unwrap(value) ? $(_elem).show(1000) : $(_elem).hide(1000);
    }
};

ko.bindingHandlers.fadeTemplate = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        return ko.bindingHandlers['template']['init'](element, valueAccessor, allBindings);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        $(element).fadeOut(function () {
            ko.bindingHandlers['template']['update'](element, valueAccessor, allBindings, viewModel, bindingContext);
            $(this).fadeIn();
        });
    }
};

ko.bindingHandlers.slideTemplate = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        return ko.bindingHandlers['template']['init'](element, valueAccessor, allBindings);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        $(element).slideUp(function () {
            ko.bindingHandlers['template']['update'](element, valueAccessor, allBindings, viewModel, bindingContext);
            $(this).slideDown();
        });
    }
};

ko.bindingHandlers.dataTablesForEach = {
    page: 0,
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor());
        ko.unwrap(options.data);
        if (options.dataTableOptions.paging) {
            valueAccessor().data.subscribe(function (changes) {
                var table = $(element).closest("table").DataTable();
                ko.bindingHandlers.dataTablesForEach.page = table.page();
                table.destroy();
            }, null, "arrayChange");
        }
        var nodes = Array.prototype.slice.call(element.childNodes, 0);
        ko.utils.arrayForEach(nodes, function (node) {
            if (node && node.nodeType !== 1) {
                node.parentNode.removeChild(node);
            }
        });
        return ko.bindingHandlers.foreach.init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor()),
            key = "DataTablesForEach_Initialized";
        ko.unwrap(options.data);
        var table;
        if (!options.dataTableOptions.paging) {
            table = $(element).closest("table").DataTable();
            table.destroy();
        }
        ko.bindingHandlers.foreach.update(element, valueAccessor, allBindings, viewModel, bindingContext);
        table = $(element).closest("table").DataTable(options.dataTableOptions);
        if (options.dataTableOptions.paging) {
            if (table.page.info().pages - ko.bindingHandlers.dataTablesForEach.page == 0)
                table.page(--ko.bindingHandlers.dataTablesForEach.page).draw(false);
            else
                table.page(ko.bindingHandlers.dataTablesForEach.page).draw(false);
        }
        if (!ko.utils.domData.get(element, key) && (options.data || options.length))
            ko.utils.domData.set(element, key, true);
        return { controlsDescendantBindings: true };
    }
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        ko.utils.registerEventHandler(element, "change", function () {
            var value = valueAccessor();
            console.log(element.value);
            if (element.value === "") return;
            value(element.value);
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor();
        if (!value()) return;
        element.value = value();
    }
};

ko.subscribable.fn.PhoneMask = PhoneMask;
function PhoneMask() {
    return ko.computed({
        read: function () {
            var phoneNumber = this() ? this() : "";
            if (phoneNumber.length < 1) return;
            var out = "";
            out = out + "(" + phoneNumber.substring(0, 3);
            if (phoneNumber.length > 3) out = out + ") " + phoneNumber.substring(3, 6);
            if (phoneNumber.length > 6) out = out + "-" + phoneNumber.substring(6, 10);
            return out;
        },
        write: function (_phoneNumber) {
            var phoneNumber = _phoneNumber.replace(/[^\.\d]/g, "");
            this(phoneNumber);
            this.notifySubscribers();
        },
        owner: this,
    }).extend({ notify: 'always' });
}

ko.subscribable.fn.ToAmount = ToAmount;
function ToAmount() {
    return ko.computed({
        read: function () {
            var num = ko.unwrap(this);
            return this() ? "$" + num.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",") : "$0.00";
        },
        write: function (_amount) {
            var amount = (_amount.match(/\d+\.?/g) || [0]).join("");
            this(parseFloat(amount));
            this.notifySubscribers();
        },
        owner: this,
    }).extend({ notify: 'always' });
}

ko.subscribable.fn.ToShortAmount = ToShortAmount;
function ToShortAmount() {
    return ko.computed({
        read: function () {
            var num = ko.unwrap(this);
            return this() ? "$" + num.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ",") : "$0";
        },
        write: function (_amount) {
            this(_amount);
            this.notifySubscribers();
        },
        owner: this,
    }).extend({ notify: 'always' });
}

ko.subscribable.fn.OnlyPositive = OnlyPositive;
function OnlyPositive() {
    return ko.computed({
        read: function () {
            return this();
        },
        write: function (_amount) {
            this(Math.abs(_amount));
            this.notifySubscribers();
        },
        owner: this,
    }).extend({ notify: 'always' });
}

ko.subscribable.fn.ToShortDate = ToShortDate;
function ToShortDate() {
    return ko.computed({
        read: function () {
            if (this() instanceof Date) {
                return this().toLocaleDateString('en-US', { month: 'numeric', day: 'numeric', year: 'numeric' });
            }
        },
        write: function (date) {
            if (date) {
                this(new Date(date));
            }
            this.notifySubscribers();
        },
        owner: this,
    }).extend({ notify: 'always' });
}

ko.subscribable.fn.ToDaysAgo = ToDaysAgo;
function ToDaysAgo() {
    return ko.computed({
        read: function () {
            if (this() instanceof Date) {
                var today = new Date();
                return Math.floor((today - this()) / 1000 / 60 / 60 / 24) + " days ago.";
            }
        },
        write: function (date) {
            if (date) {
                this(new Date(date));
            }
            this.notifySubscribers();
        },
        owner: this,
    }).extend({ notify: 'always' });
}

Dropzone.autoDiscover = false;
var dropzone;
ko.bindingHandlers.dropzone = {
    init: function (element, valueAccessor) {
        var value = ko.unwrap(valueAccessor());
        var options = {
            maxFiles: 20,
            uploadMultiple: true,
            createImageThumbnails: true
        };

        $.extend(options, value);
        $(element).addClass("dropzone");
        dropzone = new Dropzone(element, options);
        dropzone.on("queuecomplete", function () {
            dropzone.removeAllFiles(true);
        });
    }
};