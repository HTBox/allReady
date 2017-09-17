"use strict";

define("Main", ["EditModule"], function (editModule) {

    editModule.addDeleteEventImageHandler();
    editModule.checkForMobileDeviceAndShowImageDeleteButton();

});