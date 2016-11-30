SystemJS.config({
    baseURL: '/js/Admin/Campaign/Edit',
    defaultJSExtensions: true
});

SystemJS.import('EditModule').then(function (editModule) {

    editModule.addDeleteCampaignImageHandler();
    editModule.checkForMobileDeviceAndShowImageDeleteButton();
});

