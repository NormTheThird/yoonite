function HeaderViewModel(isAuthenticated) {
    var self = this;
    self.UnreadMessageCount = ko.observable(0);

    self.GetUnreadMessageCount = function () {
        BaseModel.ServiceCall('/Home/GetUserUnreadMessageCount', "post", null, true, function (response) {
            if (response.IsSuccess) { self.UnreadMessageCount(response.Count); }
        });
    };

    self.Load = function () {
        if (isAuthenticated) {
            (function loopingFunction() {
                self.GetUnreadMessageCount();
                setTimeout(loopingFunction, 10000);
            })();
        }
    };

    self.Load(isAuthenticated);
}