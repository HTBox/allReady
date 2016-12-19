"use strict";

define("Main", ["EditModule"], function (editModule) {

    editModule.addDeleteCampaignImageHandler();
    editModule.checkForMobileDeviceAndShowImageDeleteButton();
    editModule.addPopulatePreviewTableHandler();

});