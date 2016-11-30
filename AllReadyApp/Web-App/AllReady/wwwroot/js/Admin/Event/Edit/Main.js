SystemJS.config({
    baseURL: '/js/Admin/Event/Edit',
    defaultJSExtensions: true
});

SystemJS.import('EditModule').then(function (editModule) {

    editModule.addDeleteEventImageHandler();
    editModule.checkForMobileDeviceAndShowImageDeleteButton();
});