function MyProfileViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");

    self.Account = ko.observable(new AccountDataModel());
    self.AccountSkills = ko.observableArray([]);
    self.Skills = ko.observableArray([]);
    self.ShowSelectSkillError = ko.observable(false);

    self.GetAccount = function () {
        BaseModel.ServiceCall('/Account/GetAccount', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Account(new AccountDataModel(response.Account));
                response.AccountSkills.map(function (skill) {
                    self.AccountSkills.push(skill);
                });
                self.GetSkills();
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.GetSkills = function () {
        BaseModel.ServiceCall('/Account/GetSkills', "post", null, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                self.Skills([]);
                response.Skills.map(function (skill) {
                    var selected = self.AccountSkills().indexOf(skill.Id) >= 0;
                    self.Skills.push({ Id: skill.Id, Name: skill.Name, Selected: selected });
                });
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.SaveImage = function (image) {
        self.GetImageOrientation(image, function (callback) {
            self.ResetImageOrientation(image.dataURL, callback, function (resetImageSrc) {
                var resetImage = self.DataUrlToFile(resetImageSrc, image.name);
                console.log(resetImage);
                self.Storage = ko.observable(new StorageDataModel());
                self.Storage().AzureStorageId(BaseModel.Guid.New());
                self.Storage().FileName(image.name);
                self.Storage().MimeType(image.type);
                self.Storage().FileSizeInBytes(image.size);

                var blobService = AzureStorage.createBlobServiceWithSas(BaseModel.AzureStorageConfig.StorageAccountUri, BaseModel.AzureStorageConfig.SasToken);
                blobService.singleBlobPutThresholdInBytes = BaseModel.AzureStorageConfig.ChunkSize(image.size);

                var options = {};
                blobService.createBlockBlobFromBrowserFile(BaseModel.AzureStorageConfig.Container(), self.Storage().AzureStorageId(), resetImage, options, function (error, result, response) {
                    if (error) { BaseModel.LogError(error); }
                    else {
                        console.log("File finished", response, result);
                        var data = { Storage: ko.toJS(self.Storage()) };
                        BaseModel.ServiceCall('/Account/SaveAccountImage', "post", data, true, function (response) {
                            try {
                                if (!response.IsSuccess) { throw response.ErrorMessage; }
                                self.Account().ProfileImageStorageId(self.Storage().AzureStorageId());
                                self.Account().ProfileImageStorage(self.Storage());
                            }
                            catch (ex) {
                                BaseModel.LogError(ex);
                            }
                        });
                    }
                });
            });
        });
    };

    self.GetImageOrientation = function (image, callback) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var view = new DataView(e.target.result);
            if (view.getUint16(0, false) != 0xFFD8) {
                return callback(-2);
            }
            var length = view.byteLength, offset = 2;
            while (offset < length) {
                if (view.getUint16(offset + 2, false) <= 8) return callback(-1);
                var marker = view.getUint16(offset, false);
                offset += 2;
                if (marker == 0xFFE1) {
                    if (view.getUint32(offset += 2, false) != 0x45786966) {
                        return callback(-1);
                    }

                    var little = view.getUint16(offset += 6, false) == 0x4949;
                    offset += view.getUint32(offset + 4, little);
                    var tags = view.getUint16(offset, little);
                    offset += 2;
                    for (var i = 0; i < tags; i++) {
                        if (view.getUint16(offset + (i * 12), little) == 0x0112) {
                            return callback(view.getUint16(offset + (i * 12) + 8, little));
                        }
                    }
                }
                else if ((marker & 0xFF00) != 0xFF00) {
                    break;
                }
                else {
                    offset += view.getUint16(offset, false);
                }
            }
            return callback(-1);
        };
        reader.readAsArrayBuffer(image);
    };

    self.ResetImageOrientation = function (srcBase64, srcOrientation, callback) {
        var img = new Image();
        img.onload = function () {
            var width = img.width,
                height = img.height,
                canvas = document.createElement('canvas'),
                ctx = canvas.getContext("2d");

            // set proper canvas dimensions before transform & export
            if (4 < srcOrientation && srcOrientation < 9) {
                canvas.width = height;
                canvas.height = width;
            } else {
                canvas.width = width;
                canvas.height = height;
            }

            // transform context before drawing image
            switch (srcOrientation) {
                case 2: ctx.transform(-1, 0, 0, 1, width, 0); break;
                case 3: ctx.transform(-1, 0, 0, -1, width, height); break;
                case 4: ctx.transform(1, 0, 0, -1, 0, height); break;
                case 5: ctx.transform(0, 1, 1, 0, 0, 0); break;
                case 6: ctx.transform(0, 1, -1, 0, height, 0); break;
                case 7: ctx.transform(0, -1, -1, 0, height, width); break;
                case 8: ctx.transform(0, -1, 1, 0, 0, width); break;
                default: break;
            }

            // draw image
            ctx.drawImage(img, 0, 0);

            // export base64
            callback(canvas.toDataURL());
        };

        img.src = srcBase64;
    };

    self.DataUrlToFile = function (dataurl, filename) {
        var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
            bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }
        return new File([u8arr], filename, { type: mime });
    };

    self.RemoveImage = function () {
        self.Account().ProfileImageStorageId(null);
    };

    self.SaveAccount = function () {
        self.AccountSkills([]);
        self.Skills().map(function (skill) {
            if (skill.Selected) { self.AccountSkills.push(skill.Id); }
        });

        self.ShowSelectSkillError(false);
        if (!$("#my-profile-form").valid() || self.AccountSkills().length === 0) {
            if (self.AccountSkills().length === 0) { self.ShowSelectSkillError(true); }
            BaseModel.Message("Please Correct Errors.", BaseModel.MessageLevels.Error);
            return;
        }

        var data = { Account: ko.toJS(self.Account()), AccountSkills: ko.toJS(self.AccountSkills()) };
        BaseModel.ServiceCall('/Account/SaveAccount', "post", data, true, function (response) {
            try {
                if (!response.IsSuccess) { throw response.ErrorMessage; }
                BaseModel.Message("Your profile has been saved!", BaseModel.MessageLevels.Success);
            }
            catch (ex) {
                BaseModel.LogError(ex);
            }
        });
    };

    self.BindValidation = function () {
        $("#my-profile-form").validate({
            rules: {
                FirstName: "required",
                LastName: "required",
                Email: "required",
                City: "required",
                State: "required",
                ZipCode: "required"
            },
            messages: {
                FirstName: "First name is required.",
                LastName: "Last name is required.",
                Email: "Email is required.",
                City: "City is required",
                State: "State is required",
                ZipCode: "Zip code is required"
            }
        });
    };

    self.Load = function () {
        self.BindValidation();
        self.GetAccount();
    };
}