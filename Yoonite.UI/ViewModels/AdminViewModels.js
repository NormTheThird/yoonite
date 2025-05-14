function AdminAccountViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");

    self.Account = ko.observable(new AccountDataModel());
    self.Accounts = ko.observableArray([]);

    self.GetAccounts = function () {
        BaseModel.ServiceCall('/Admin/GetAccounts', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Accounts(ko.observableArray(response.Accounts ? response.Accounts.map(function (account) {
                    return new AccountDataModel(account);
                }) : []));
            }
            catch (ex) {
                self.ErrorMessage(ex);
                BaseModel.LogError(ex);
            }
        });
    };

    self.ChangeAccountStatus = function (account) {
        var data = { AccountId: account.Id() };
        BaseModel.ServiceCall('/Admin/ChangeAccountStatus', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; };
                account.IsActive(!account.IsActive());
                ko.utils.arrayFirst(self.Accounts(), function (findAccount) {
                    if (findAccount.id === account.id) {
                        findAccount.IsActive(!findAccount.IsActive());
                    }
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.EditAccount = function (account) {
        self.Account(account);
        self.Show("edit");
    };

    self.Show = function (page) {
        $("#admin-account-" + self.CurrentPage()).hide();
        $("#admin-account-" + page).show();
        self.CurrentPage(page);
    };

    self.CancelChanges = function () {
        self.Account(null);
        self.Show("list");
    };

    self.BindValidation = function () {
        $("#RegistrationInfo").validate({
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
        self.GetAccounts();
        self.Show("list");
    };
}

function AdminProjectViewModel() {
    var self = this;

    self.Projects = ko.observableArray([]);

    self.GetProjects = function () {
        BaseModel.ServiceCall('/Admin/GetProjects', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Projects(ko.observableArray(response.Projects ? response.Projects.map(function (project) {
                    return new ProjectDataModel(project);
                }) : []));
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.Load = function (resetId) {
        self.GetProjects();
    };
}

function AdminSkillViewModel() {
    var self = this;

    self.Skills = ko.observableArray([]);

    self.GetSkills = function () {
        BaseModel.ServiceCall('/Admin/GetSkills', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Skills(ko.observableArray(response.Skills ? response.Skills.map(function (skill) {
                    return new SkillDataModel(skill);
                }) : []));
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.EditSkill = function (skill) {
        skill.IsEdit(true);
    };

    self.NewSkill = function () {
        var newSkill = new SkillDataModel();
        newSkill.IsEdit(true);
        self.Skills.push(newSkill);
    };

    self.SaveSkill = function (skill) { 
        var data = { Skill: ko.toJS(skill) };
        BaseModel.ServiceCall('/Admin/SaveSkill', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                skill.Id(response.Skill.Id);
                skill.Name(response.Skill.Name);
                skill.Description(response.Skill.Description);
                skill.IsActive(response.Skill.IsActive);
                skill.DateCreated(BaseModel.ToDate(response.Skill.DateCreated, "mm/dd/yyyy"));
                skill.IsEdit(false);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.Load = function (resetId) {
        self.GetSkills();
    };
}