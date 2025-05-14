function BaseDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.Id || BaseModel.Guid.Empty);
    self.DateCreated = ko.observable(BaseModel.ToDate(data.DateCreated, "mm/dd/yyyy") || false);
}

function ActiveDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new BaseDataModel(data));

    self.IsActive = ko.observable(data.IsActive || false);
}

function AccountListDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new ActiveDataModel(data));

    self.ProfileImageStorageId = ko.observable(data.ProfileImageStorageId || null);
    self.ProfileImageStorage = ko.observable(new StorageDataModel(data.ProfileImageStorage));
    self.FirstName = ko.observable(data.FirstName || "");
    self.LastName = ko.observable(data.LastName || "");
    self.Email = ko.observable(data.Email || "");
    self.PhoneNumber = ko.observable(data.PhoneNumber || "");

    self.FullName = ko.computed(function () {
        return self.FirstName() + " " + self.LastName();
    });

    self.Initials = ko.computed(function () {
        return self.FirstName().substring(0, 1) + self.LastName().substring(0, 1);
    });
}

function AccountDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new AccountListDataModel(data));

    self.AddressId = ko.observable(data.AddressId || BaseModel.Guid.Empty);
    self.Address = ko.observable(new AddressDataModel(data.Address));
    self.AltPhoneNumber = ko.observable(data.AltPhoneNumber || "");
    self.FacebookUrl = ko.observable(data.FacebookUrl || "");
    self.TwitterUrl = ko.observable(data.TwitterUrl || "");
    self.LinkedinUrl = ko.observable(data.LinkedinUrl || "");
    self.InstagramUrl = ko.observable(data.InstagramUrl || "");
    self.WebsiteUrl = ko.observable(data.WebsiteUrl || "");
    self.Bio = ko.observable(data.Bio || "");
    self.RefreshTokenLifeTimeMinutes = ko.observable(data.RefreshTokenLifeTimeMinutes || "");
    self.DateOfBirth = ko.observable(BaseModel.ToDate(data.DateOfBirth, "yyyy-dd-mm") || false);
}

function AccountWithSkillsDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new AccountDataModel(data));

    self.Skills = ko.observableArray(data.Skills ? data.Skills.map(function (skill) {
        if (skill === null) return;
        return new SkillDataModel(skill);
    }) : []);
}

function AccountRefreshTokenDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.Id || BaseModel.Guid.Empty);
    self.RefreshToken = ko.observable(data.RefreshToken || "");
    self.DateIssued = ko.observable(BaseModel.ToDate(data.DateIssued) || false);
    self.DateExpired = ko.observable(BaseModel.ToDate(data.DateExpired) || false);
}

function AddressDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new BaseDataModel(data));

    self.Address1 = ko.observable(data.Address1 || "");
    self.Address2 = ko.observable(data.Address2 || "");
    self.City = ko.observable(data.City || "");
    self.State = ko.observable(data.State || "");
    self.ZipCode = ko.observable(data.ZipCode || "");
    self.Latitude = ko.observable(data.Latitude || 0);
    self.Longitude = ko.observable(data.Longitude || 0);

    self.FullAddress = ko.computed(function () {
        var retval = (self.Address1() + " " + self.Address2()).trim();
        if (retval !== "") { retval += ", "; }
        retval += self.City() + ", " + self.State() + " " + self.ZipCode();
        if (self.City() === "") { retval = self.State() + " " + self.ZipCode(); }
        return retval.trim();
    });

    self.CityStateDisplay = ko.computed(function () {
        if (self.State() === undefined) { self.State(""); }
        var retval = self.City() + ", " + self.State();
        if (self.City() === "") { retval = self.State(); }
        return retval.trim();
    });

    self.CityStateZipDisplay = ko.computed(function () {
        if (self.State() === undefined) { self.State(""); }
        var retval = self.City() + ", " + self.State() + " " + self.ZipCode();
        if (self.City() === "") { retval = self.State() + " " + self.ZipCode(); }
        return retval.trim();
    });
}

function ErrorDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new BaseDataModel(data));

    self.Source = ko.observable(data.Source || "");
    self.ExceptionType = ko.observable(data.ExceptionType || "");
    self.ExceptionMessage = ko.observable(data.ExceptionMessage || "");
    self.InnerExceptionMessage = ko.observable(data.InnerExceptionMessage || "");
    self.StackTrace = ko.observable(data.StackTrace || "");
    self.IsReviewed = ko.observable(data.IsReviewed || false);
    self.ReviewedBy = ko.observable(data.ReviewedBy || "");
    self.DateReviewed = ko.observable(BaseModel.ToDate(data.DateReviewed) || "");
}

function MessageDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new ActiveDataModel(data));

    self.ParentId = ko.observable(data.ParentId || BaseModel.Guid.Empty);
    self.FromAccountId = ko.observable(data.FromAccountId || BaseModel.Guid.Empty);
    self.FromAccount = ko.observable(new AccountListDataModel(data.FromAccount));
    self.ToAccountId = ko.observable(data.ToAccountId || BaseModel.Guid.Empty);
    self.ToAccount = ko.observable(new AccountListDataModel(data.ToAccount));
    self.Subject = ko.observable(data.Subject || "");
    self.Message = ko.observable(data.Message || "");
    self.UnRead = ko.observable(data.UnRead || false);
    self.IsDeleted = ko.observable(data.IsDeleted || false);

    self.ShortSubject = ko.computed(function () {
        return self.Subject().length > 25 ? self.Subject().substring(0, 25) + "..." : self.Subject;
    });

    self.ShortMessage = ko.computed(function () {
        return self.Message().length > 50 ? self.Message().substring(0, 50) + "..." : self.Message;
    });

    self.MessageFrom = ko.computed(function () {
        if (self.FromAccountId() === BaseModel.Guid.Empty) return "";
        var user = BaseModel.GetSecurityModel();
        return self.FromAccountId().toLowerCase() === user.Id.toLowerCase() ? "You" : self.FromAccount().FullName();
    });

    self.MessageTo = ko.computed(function () {
        if (self.ToAccountId() === BaseModel.Guid.Empty) return "";
        var user = BaseModel.GetSecurityModel();
        return self.ToAccountId().toLowerCase() === user.Id.toLowerCase() ? "You" : self.ToAccount().FullName();
    });
}

function MessageWithChildrenDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new MessageDataModel(data));

    self.ChildMessages = ko.observableArray(data.ChildMessages ? data.ChildMessages.map(function (child) {
        if (child === null) return;
        return new MessageDataModel(child);
    }) : []);
}

function ProjectDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new ActiveDataModel(data));

    self.AccountId = ko.observable(data.AccountId || BaseModel.Guid.Empty);
    self.ProjectName = ko.observable(data.ProjectName || "");
    self.CompanyName = ko.observable(data.CompanyName || "");
    self.Description = ko.observable(data.Description || "");
    self.Url = ko.observable(data.Url || "");
    self.City = ko.observable(data.City || "");
    self.State = ko.observable(data.State || "");
    self.ZipCode = ko.observable(data.ZipCode || "");
    self.CanBeRemote = ko.observable(data.CanBeRemote || false);
    self.ProjectSkills = ko.observableArray(data.ProjectSkills ? data.ProjectSkills.map(function (skill) {
        if (skill === null) return;
        return new SkillDataModel(skill);
    }) : []);

    self.CityStateDisplay = ko.computed(function () {
        if (self.State() === undefined) { self.State(""); }
        var retval = self.City() + ", " + self.State();
        if (self.City() === "") { retval = self.State(); }
        return retval.trim();
    });

    self.Initials = ko.computed(function () {
        return "";
    });
}

function SkillDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new ActiveDataModel(data));

    self.Name = ko.observable(data.Name || "");
    self.Description = ko.observable(data.Description || "");
    self.IsEdit = ko.observable(false);
}

function StorageDataModel(data) {
    var self = this;
    if (!data) data = {};
    $.extend(self, new ActiveDataModel(data));

    self.AzureStorageId = ko.observable(data.AzureStorageId || BaseModel.Guid.Empty);
    self.FileName = ko.observable(data.FileName || "");
    self.MimeType = ko.observable(data.MimeType || "");
    self.FileSizeInBytes = ko.observable(data.FileSizeInBytes || 0);
    self.IsDeleted = ko.observable(data.IsDeleted || false);

    self.Image = ko.computed(function () {
        if (self.AzureStorageId() === BaseModel.Guid.Empty) { return null; }
        var url = BaseModel.AzureStorageConfig.StorageAccountUri + "/"
            + BaseModel.AzureStorageConfig.Container() + "/"
            + self.AzureStorageId() + BaseModel.AzureStorageConfig.SasToken;
        return url;
    });
}