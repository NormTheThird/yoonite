function ContactUsDataModel() {
    var self = this;
    self.Name = ko.observable("");
    self.Subject = ko.observable("");
    self.Email = ko.observable("");
    self.Message = ko.observable("");
    self.IsMessageSent = ko.observable(false);
    self.MessageSentText = ko.observable("");

    self.MessageSent = function () {
        self.Name("");
        self.Subject("");
        self.Email("");
        self.Message("");
        self.IsMessageSent(true);
        self.MessageSentText("Thank you for your message.");
    };
}

function HomeViewModel() {
    var self = this;

    self.ContactUsDataModel = ko.observable(new ContactUsDataModel());

    self.ContactUs = function () {
        if (!$("#contact-form").valid()) { return; }

        var data = {
            FromName: self.ContactUsDataModel().Name(),
            FromEmail: self.ContactUsDataModel().Email(),
            Subject: self.ContactUsDataModel().Subject(),
            Message: self.ContactUsDataModel().Message()
        };
        BaseModel.ServiceCall('/Home/ContactUs', "post", data, true, function (response) {
            self.ContactUsDataModel().MessageSent();
        });
    };

    self.BindValidation = function () {
        $("#contact-form").validate({
            rules: {
                Name: "required",
                Subject: "required",
                Email: "required",
                Message: "required"
            },
            messages: {
                Name: "Your name is required.",
                Subject: "A subject is required",
                Email: "Your email is required",
                Message: "A message is required"
            }
        });
    };

    self.Load = function () {
        self.BindValidation();
    };
}