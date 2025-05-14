function ProjectViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");

    self.Projects = ko.observableArray([]);
    self.Skills = ko.observableArray([]);
    self.Project = ko.observable(new ProjectDataModel());
    self.ProjectOwner = ko.observable(new AccountDataModel());
    self.OwnerMessage = ko.observable(new MessageDataModel());
    self.IsAuthenticated = ko.observable(false);

    self.ProjectsFilter = ko.observable("");
    self.FilteredProjects = ko.computed(function () {
        var expr = new RegExp(self.ProjectsFilter(), "i");
        return $.map(self.Projects(), function (project) {
            var searchfield = project.Description() + " " + project.ProjectName() + " " + project.CompanyName() + " " + project.City() + " " + project.State();
            project.ProjectSkills().map(function (skill) {
                searchfield += " " + skill.Name();
            });
            if (searchfield.match(expr)) return project;
        });
    });
    self.ProjectsPagination = ko.observable(new PaginationVM(self.FilteredProjects, 5));

    self.GetProjects = function () {
        BaseModel.ServiceCall('/Project/GetProjects', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Projects([]);
                response.Projects.map(function (project) {
                    self.Projects.push(new ProjectDataModel(project));
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetProjectOwner = function (accountId) {
        var data = { AccountId: accountId };
        BaseModel.ServiceCall('/Project/GetProjectOwner', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.ProjectOwner(new AccountDataModel(response.Account));
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetProjectDetail = function (project) {
        if (!self.IsAuthenticated()) {
            window.location = "/Security";
            return;
        }

        self.Project(project);
        self.GetProjectOwner(project.AccountId);
        self.Show("detail");
    };

    self.ContactOwner = function () {
        if (!$("#contact-owner-form").valid()) { return; }

        self.OwnerMessage().ToAccountId(self.ProjectOwner().Id());
        var data = { Message: ko.toJS(self.OwnerMessage()) };
        BaseModel.ServiceCall('/Project/ContactOwner', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                BaseModel.Message("Your message has been sent!", BaseModel.MessageLevels.Success);
                self.OwnerMessage(new MessageDataModel());
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.BackToProjectList = function () {
        self.Show("list");
    };

    self.Show = function (page) {
        $("#project-" + self.CurrentPage()).hide();
        $("#project-" + page).show();
        self.CurrentPage(page);
    };

    self.BindValidation = function () {
        $("#contact-owner-form").validate({
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

    self.Load = function (isAuthenticated) {
        self.IsAuthenticated(isAuthenticated === "True");
        self.BindValidation();
        self.GetProjects();
        self.Show("list");
    };
}

function CreateProjectViewModel() {
    var self = this;

    self.Project = ko.observable(new ProjectDataModel());
    self.Skills = ko.observableArray([]);
    self.ShowSelectSkillError = ko.observable(false);

    self.GetSkills = function () {
        BaseModel.ServiceCall('/Project/GetSkills', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Skills([]);
                response.Skills.map(function (skill) {
                    self.Skills.push({ Id: skill.Id, Name: skill.Name, Selected: false });
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetSelectedSkills = function () {
        var selectedSkill = [];
        self.Skills().map(function (skill) {
            if (skill.Selected) {
                selectedSkill.push(skill.Id);
            }
        });
        return selectedSkill;
    };

    self.CreateProject = function () {
        var skills = self.GetSelectedSkills();
        self.ShowSelectSkillError(false);
        if (!$("#create-project-form").valid() || skills.length === 0) {
            if (skills.length === 0) { self.ShowSelectSkillError(true); }
            BaseModel.Message("Please Correct Errors.", BaseModel.MessageLevels.Error);
            return;
        }

        self.Project().IsActive(true);
        var data = { Project: ko.toJS(self.Project()), ProjectSkills: ko.toJS(skills) };
        BaseModel.ServiceCall('/Project/SaveProject', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                window.location = "/Project/MyProjects";
                BaseModel.Message("Your new project has been saved!", BaseModel.MessageLevels.Success);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.BindValidation = function () {
        $("#create-project-form").validate({
            rules: {
                ProjectName: "required",
                City: "required",
                State: "required",
                ZipCode: "required"
            },
            messages: {
                ProjectName: "Project name is required.",
                City: "City is required",
                State: "State is required",
                ZipCode: "Zip code is required"
            },
            errorPlacement: function (error, element) {
                if (element.attr("name") === "Skill") {
                    error.appendTo("#skill-validation-error");
                } else {
                    error.insertAfter(element);
                }
            }
        });
    };

    self.Load = function () {
        self.BindValidation();
        self.GetSkills();
    };
}

function MyProjectViewModel() {
    var self = this;

    self.Projects = ko.observableArray([]);
    self.Skills = ko.observableArray([]);
    self.ProjectEdit = ko.observable(new ProjectDataModel());
    self.ProjectEditSkills = ko.observableArray([]);
    self.IsEditProject = ko.observable(false);
    self.IsViewInterested = ko.observable(false);
    self.ShowSelectSkillError = ko.observable(false);

    self.ProjectFilter = ko.observable("");
    self.FilteredProjects = ko.computed(function () {
        var expr = new RegExp(self.ProjectFilter(), "i");
        return $.map(self.Projects(), function (project) {
            var searchfield = project.ProjectName() + " " + project.CompanyName() + " " + project.Description();
            if (searchfield.match(expr)) return project;
        });
    });
    self.OrdersPagination = ko.observable(new PaginationVM(self.FilteredProjects, 5));

    self.GetMyProjects = function () {
        BaseModel.ServiceCall('/Project/GetMyProjects', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Projects([]);
                response.Projects.map(function (project) {
                    self.Projects.push(new ProjectDataModel(project));
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetSkills = function () {
        BaseModel.ServiceCall('/Project/GetSkills', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Skills([]);
                response.Skills.map(function (skill) {
                    self.Skills.push({ Id: skill.Id, Name: skill.Name, Selected: false });
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.EditProject = function (project) {
        self.ProjectEdit(project);
        self.IsEditProject(true);
        self.IsViewInterested(false);
        self.ProjectEditSkills([]);
        self.Skills().map(function (skill) {
            var selected = self.ProjectEdit().ProjectSkills().findIndex(_ => _.Id() === skill.Id) >= 0;
            self.ProjectEditSkills.push({ Id: skill.Id, Name: skill.Name, Selected: selected });
        });
    };

    self.SaveProject = function () {
        var skills = self.GetSelectedSkills();
        self.ShowSelectSkillError(false);
        if (!$("#edit-project-form").valid() || skills.length === 0) {
            if (skills.length === 0) { self.ShowSelectSkillError(true); }
            return;
        }

        var data = { Project: ko.toJS(self.ProjectEdit()), ProjectSkills: ko.toJS(skills) };
        BaseModel.ServiceCall('/Project/SaveProject', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.IsEditProject(false);
                self.GetMyProjects();
                BaseModel.Message("Your project has been edited!", BaseModel.MessageLevels.Success);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetSelectedSkills = function () {
        var selectedSkill = [];
        self.ProjectEditSkills().map(function (skill) {
            if (skill.Selected) {
                selectedSkill.push(skill.Id);
            }
        });
        return selectedSkill;
    };

    self.DeleteProject = function (deletedProject) {
        var data = { ProjectId: deletedProject.Id() };
        BaseModel.ServiceCall('/Project/DeleteProject', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                BaseModel.Message("Your project has been deleted!", BaseModel.MessageLevels.Success);
                self.Projects.remove(deletedProject);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.ViewInterested = function () {
        self.IsEditProject(false);
        self.IsViewInterested(true);
    };

    self.BindValidation = function () {
        $("#edit-project-form").validate({
            rules: {
                ProjectName: "required",
                City: "required",
                State: "required",
                ZipCode: "required"
            },
            messages: {
                ProjectName: "Project name is required.",
                City: "City is required",
                State: "State is required",
                ZipCode: "Zip code is required"
            },
            errorPlacement: function (error, element) {
                if (element.attr("name") === "Skill") {
                    error.appendTo("#skill-validation-error");
                } else {
                    error.insertAfter(element);
                }
            }
        });
    };

    self.Load = function () {
        self.BindValidation();
        self.GetMyProjects();
        self.GetSkills();
    };
}