function UserViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");

    self.Users = ko.observableArray([]);
    self.Skills = ko.observableArray([]);
    self.User = ko.observable(new AccountWithSkillsDataModel());
    self.UserMessage = ko.observable(new MessageDataModel());

    self.UsersFilter = ko.observable("");
    self.FilteredUsers = ko.computed(function () {
        var expr = new RegExp(self.UsersFilter(), "i");
        return $.map(self.Users(), function (user) {
            var searchfield = user.FirstName() + " " + user.LastName() + " " + user.Address().City() + " " + user.Address().State();
            user.Skills().map(function (skill) {
                searchfield += " " + skill.Name();
            });
            if (searchfield.match(expr)) return user;
        });
    });
    self.UsersPagination = ko.observable(new PaginationVM(self.FilteredUsers, 12));

    self.GetUsers = function () {
        BaseModel.ServiceCall('/User/GetUsers', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Users([]);
                response.Accounts.map(function (account) {
                    self.Users.push(new AccountWithSkillsDataModel(account));
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetUserDetail = function (user) {
        self.User(user);
        self.Show("detail");
    };

    self.ContactUser = function () {
        if (!$("#contact-user-form").valid()) { return; }

        self.UserMessage().ToAccountId(self.User().Id());
        var data = { Message: ko.toJS(self.UserMessage()) };
        BaseModel.ServiceCall('/User/ContactUser', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                BaseModel.Message("Your message has been sent!", BaseModel.MessageLevels.Success);
                self.UserMessage(new MessageDataModel());
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.BackToUserList = function () {
        self.Show("list");
    };

    self.Show = function (page) {
        $("#user-" + self.CurrentPage()).hide();
        $("#user-" + page).show();
        self.CurrentPage(page);
    };

    self.BindValidation = function () {
        $("#contact-user-form").validate({
            rules: {
                Subject: {
                    required: true,
                    maxlength: 100
                },
                Message: {
                    required: true,
                    maxlength: 2000
                }
            },
            messages: {
                Subject: {
                    required: "A subject is required",
                    maxlength: "Max subject length is 100 characters, please shorten your subject"
                },
                Message: {
                    required: "A message is required",
                    maxlength: "Max message length is 2000 characters, please shorten your message"
                }
            }
        });
    };

    self.Load = function () {
        self.BindValidation();
        self.GetUsers();
        self.Show("list");
    };
}