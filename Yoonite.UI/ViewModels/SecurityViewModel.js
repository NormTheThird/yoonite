function SecurityViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");

    self.FirstName = ko.observable("");
    self.LastName = ko.observable("");
    self.Email = ko.observable("");
    self.Password = ko.observable("");
    self.ConfirmPassword = ko.observable("");
    self.ResetId = ko.observable("");
    self.ErrorMessage = ko.observable("");
    self.Agrees = ko.observable(false);
    self.RememberMe = ko.observable(false);
    self.IsResetPassword = ko.observable(false);
    self.IsSubmitting = ko.observable(false);

    self.EnterSubmit = function (vm, evt) {
        if (evt.key === 'Enter') { self.Login(); }
    };
    self.EnterRegister = function (vm, evt) {
        if (evt.key === 'Enter') { self.RegisterAccount(); }
    };

    self.ShowRegisterAccount = function () {
        self.Show("register");
    };

    self.RegisterAccount = function () {
        if (!$("#registration-form").valid()) return false;
        self.IsSubmitting(true);
        var data = {
            FirstName: self.FirstName(),
            LastName: self.LastName(),
            Email: self.Email(),
            Password: self.Password()
        };  
        BaseModel.ServiceCall("/Security/Register", "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Show("login");
            }
            catch (ex) {
                self.ErrorMessage(ex);
                BaseModel.LogError(ex);
            }
            finally { self.IsSubmitting(false); }
        });
    };

    self.ShowLogin = function () {
        self.Show("login");
    };

    self.ValidateAccount = function () {
        self.ErrorMessage("");
        self.IsSubmitting(true);
        var data = { Email: self.Email(), Password: self.Password(), RememberMe: self.RememberMe() };
        BaseModel.ServiceCall("/Security/ValidateAccount", "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                window.location = "/Home";
            } catch (ex) {
                self.ErrorMessage(ex);
                self.IsSubmitting(false);
                BaseModel.LogError(ex);
            }
        });
    };

    self.ShowForgotPassword = function () {
        self.IsResetPassword(false);
        self.Show("forgot-password");
    };

    self.ForgotPassword = function () {
        self.ErrorMessage("");
        var data = { Email: self.Email(), BaseUrl: window.location.origin };
        BaseModel.ServiceCall("/Security/ForgotPassword", "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.IsResetPassword(true);
                self.Show("login");
                BaseModel.Message("A link has been sent to your email", BaseModel.MessageLevels.Success);
            }
            catch (ex) {
                self.ErrorMessage(ex);
                BaseModel.LogError(ex);
            }
        });
    };

    self.ResetPassword = function () {
        self.ErrorMessage("");
        if (self.Password() !== self.ConfirmPassword()) {
            self.ErrorMessage("* Passwords do not match!");
            return;
        }

        var data = { ResetId: self.ResetId(), NewPassword: self.Password() };
        BaseModel.ServiceCall("/Security/ResetPassword", "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
           
            }
            catch (ex) {
                self.ErrorMessage(ex);
                BaseModel.LogError(ex);
            }
        });
    };

    self.Show = function (page) {
        self.ErrorMessage("");
        $("#security-" + self.CurrentPage()).hide();
        $("#security-" + page).show();
        self.CurrentPage(page);
    };

    self.BindValidation = function () {
        $("#registration-form").validate({
            rules: {
                FirstName: "required",
                LastName: "required",
                Email: "required",
                UserName: "required",
                Password: "required",
                Agree: "required"
            },
            messages: {
                FirstName: "First name is required.",
                LastName: "Last name is required.",
                Email: "Email is required.",
                UserName: "User name is required.",
                Password: "Password is required.",
                Agree: "* Please agree to the terms to sign up."
            },
            errorPlacement: function (error, element) {
                if (element.attr("name") === "Agree") {
                    error.appendTo("#validationError");
                } else {
                    error.insertAfter(element);
                }
            }
        });
    };

    self.Load = function (resetId) {
        self.BindValidation();
        if (resetId !== "") {
            self.ResetId(resetId);
            self.Show("reset-password");
        }
        else {
            self.Show("login");
        }
    };
}