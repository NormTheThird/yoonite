function MessageViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");

    self.Messages = ko.observableArray([]);
    self.Message = ko.observable(new MessageWithChildrenDataModel());
    self.ResponseMessage = ko.observable(new MessageDataModel());
    self.MessageFilter = ko.observable("");
    self.FilteredMessages = ko.computed(function () {
        var expr = new RegExp(self.MessageFilter(), "i");
        return $.map(self.Messages(), function (message) {
            var searchfield = message.Subject() + " " + message.Message();
            if (searchfield.match(expr)) return message;
        });
    });
    self.MessagesPagination = ko.observable(new PaginationVM(self.FilteredMessages, 10));

    self.GetUserMessages = function () {
        BaseModel.ServiceCall('/Message/GetUserMessages', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Messages([]);
                response.Messages.map(function (message) {
                    self.Messages.push(new MessageWithChildrenDataModel(message));
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.MarkMessageAsRead = function (id) {
        var data = { ParentMessageId: id };
        BaseModel.ServiceCall('/Message/MarkMessageAsRead', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Message().UnRead(false);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.SendResponseMessage = function () {
        if (!$("#response-message-form").valid()) { return; }

        var data = { Message: ko.toJS(self.ResponseMessage()) };
        BaseModel.ServiceCall('/Message/SendResponseMessage', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                BaseModel.Message("Your message has been sent!", BaseModel.MessageLevels.Success);
                var message = new MessageDataModel(response.Message);
                self.Message().ChildMessages.push(message);
                self.SetNewResponseMessage(message);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.ViewMessageDetails = function (message) {
        self.Message(message);
        self.SetNewResponseMessage(message);
        self.MarkMessageAsRead(message.ParentId());
        self.Show("detail");
    };

    self.SetNewResponseMessage = function (message) {
        self.ResponseMessage(new MessageDataModel());
        self.ResponseMessage().ParentId(message.ParentId());
        self.ResponseMessage().Subject(message.Subject());
        self.ResponseMessage().ToAccountId(message.ToAccountId());
        var user = BaseModel.GetSecurityModel();
        if (message.ToAccountId() === user.Id.toLowerCase())
            self.ResponseMessage().ToAccountId(message.FromAccountId());
    };

    self.BackToMessageList = function () {
        self.Show("list");
    };

    self.DeleteMessage = function (message) {
        var data = { MessageId: ko.toJS(message.Id) };
        BaseModel.ServiceCall('/Message/DeleteMessage', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                BaseModel.Message("Your message has been deleted!", BaseModel.MessageLevels.Success);
                self.Messages.remove(message);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.Show = function (page) {
        $("#message-" + self.CurrentPage()).hide();
        $("#message-" + page).show();
        self.CurrentPage(page);
    };

    self.BindValidation = function () {
        $("#response-message-form").validate({
            rules: {
                Message: {
                    required: true,
                    maxlength: 2000
                }
            },
            messages: {
                Message: {
                    required: "A message is required",
                    maxlength: "Max message length is 2000 characters, please shorten your message"
                }
            }
        });
    };

    self.Load = function () {
        self.BindValidation();
        self.GetUserMessages();
        self.Show("list");
    };
}